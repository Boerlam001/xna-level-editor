using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using EnvDTE;
using EnvDTE80;
using EditorModel;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Generator
{
    public class ClassManager
    {
        public static DTE2 applicationObject;

        ContentBuilder contentBuilder;

        public ContentBuilder ContentBuilder
        {
            get { return contentBuilder; }
            set { contentBuilder = value; }
        }

        string output;

        public string Output
        {
            get { return output; }
            set { output = value; }
        }

        MapModel mapModel;

        public MapModel MapModel
        {
            get { return mapModel; }
            set { mapModel = value; }
        }

        List<CodeLines> codeLinesList;

        public List<CodeLines> CodeLinesList
        {
            get { return codeLinesList; }
            set { codeLinesList = value; }
        }

        private ProjectItem classFile;
        private CodeNamespace codeNamespace;
        private CodeClass2 codeClass;
        private CodeFunction2 loadContentFunction;
        private CodeFunction2 drawFunction;
        private Project currentProject, contentProject, xleModelProject;
        private const string xleModelProjectName = "XleModel";

        private bool xleModelImportStmtExist;

        public ClassManager(MapModel mapModel)
        {
            this.mapModel = mapModel;
            codeLinesList = new List<CodeLines>();
        }

        public ClassManager(MapModel mapModel, string projectName, string className)
        {
            this.mapModel = mapModel;
            codeLinesList = new List<CodeLines>();
            TraverseProjects(projectName);
            classFile = AddClass(className);
            TraverseCodeElements();
            if (codeNamespace != null)
                codeNamespace.Name = projectName;
            if (codeClass != null)
                codeClass.Name = className;
            classFile.Save();
        }

        public ClassManager(MapModel mapModel, string projectName, string className, bool open)
        {
            this.mapModel = mapModel;
            codeLinesList = new List<CodeLines>();
            TraverseProjects(projectName);
            if (!open)
            {
                classFile = AddClass(className);
                classFile.Save();
                TraverseCodeElements();
                if (codeNamespace != null)
                    codeNamespace.Name = projectName;
                if (codeClass != null)
                    codeClass.Name = className;
            }
            else
            {
                classFile = SelectClass(className);
                TraverseCodeElements();
                if (codeNamespace != null)
                    codeNamespace.Name = projectName;
                if (codeClass != null)
                    codeClass.Name = className;
            }
        }

        public ProjectItem SelectClass(string className)
        {
            if (applicationObject == null || currentProject == null)
            {
                return null;
            }
            else
            {
                try
                {
                    return currentProject.ProjectItems.Item(className + ".cs");
                }
                catch
                {
                    return null;
                }
            }
        }

        private ProjectItem AddClass(string className)
        {
            if (applicationObject == null || currentProject == null)
            {
                return null;
            }
            else
            {
                if (File.Exists(Path.GetDirectoryName(currentProject.FullName) + "\\" + className + ".cs"))
                {
                    File.Delete(Path.GetDirectoryName(currentProject.FullName) + "\\" + className + ".cs");
                }

                currentProject.ProjectItems.AddFromTemplate(AssemblyDirectory + "\\templates\\Game1.cs", className + ".cs");
                ProjectItem projectItem = currentProject.ProjectItems.Item(className + ".cs");

                return projectItem;
            }
        }

        private void TraverseProjects(string projectName)
        {
            Solution2 solution = (Solution2)applicationObject.Solution;
            foreach (Project project in solution.Projects)
            {
                if (project.Name == projectName)
                    currentProject = project;
                if (project.Name == xleModelProjectName)
                    xleModelProject = project;
                if (project.FullName.Contains(".contentproj"))
                    contentProject = project;
            }

            if (xleModelProject == null)
            {
                AddXleModelProject();
            }
        }

        private void AddXleModelProject()
        {
            if (currentProject == null)
            {
                return;
            }

            Solution2 solution = (Solution2)applicationObject.Solution;

            string dirPath = Path.GetDirectoryName(solution.FullName) + "\\XleModel";
            FileHelper.DirectoryCopy(AssemblyDirectory + "\\templates\\XleModel", dirPath, true);
            xleModelProject = solution.AddFromFile(dirPath + "\\XleModel.csproj");

            VSLangProj.VSProject project = (VSLangProj.VSProject)currentProject.Object;
            project.References.AddProject(xleModelProject);
        }

        private void TraverseCodeElements()
        {
            xleModelImportStmtExist = false;
            foreach (CodeElement codeElement in classFile.FileCodeModel.CodeElements)
            {
                ExpandSubCodeElement(codeElement);
            }
            if (!xleModelImportStmtExist)
            {
                EditPoint editPoint = lastImportStatement.EndPoint.CreateEditPoint();
                editPoint.Insert("\r\nusing XleModel;");
            }
        }

        private void ExpandSubCodeElement(CodeElement codeElement)
        {
            if (codeElement.Kind == vsCMElement.vsCMElementNamespace)
            {
                codeNamespace = (CodeNamespace)codeElement;
            }
            if (codeElement.Kind == vsCMElement.vsCMElementClass)
            {
                codeClass = (CodeClass2)codeElement;
            }
            if (codeElement.Kind == vsCMElement.vsCMElementFunction)
            {
                CodeFunction2 codeFunction = (CodeFunction2)codeElement;
                switch (codeFunction.Name)
                {
                    case "LoadContent":
                        loadContentFunction = codeFunction;
                        break;
                    case "Draw":
                        drawFunction = codeFunction;
                        break;
                }
            }
            if (codeElement.Kind == vsCMElement.vsCMElementImportStmt)
            {
                lastImportStatement = (CodeImport)codeElement;
                if (((CodeImport)codeElement).Namespace == "XleModel")
                    xleModelImportStmtExist = true;
            }
            foreach (CodeElement child in codeElement.Children)
            {
                ExpandSubCodeElement(child);
            }
        }

        public void GenerateClass()
        {
            if (classFile == null)
            {
                return;
            }

            ImportModelSourceToContentProject();

            StringBuilder sb = new StringBuilder();
            string variable = sb.
                       Append("Camera camera;\r\n").ToString();
            sb.Clear();
            string loadContent = sb.
                       Append("camera = new Camera();\r\n").
                       Append("camera.Position = new Vector3(-4, 8, -25);\r\n").
                       Append("camera.AspectRatio = GraphicsDevice.Viewport.AspectRatio;\r\n").
                       Append("camera.Rotate(20, 55, 0);\r\n").ToString();
            sb.Clear();
            string draw = "";
            foreach (CodeLines codeLines in codeLinesList)
            {
                variable += codeLines.Code[CodeLines.CodePosition.Variable] + "\r\n";
                loadContent += codeLines.Code[CodeLines.CodePosition.LoadContent] + "\r\n";
                draw += codeLines.Code[CodeLines.CodePosition.Draw] + "\r\n";
            }

            EditPoint editPoint = null;
            EditPoint movePoint = null;
            string startPattern = "#region XnaLevelEditor";
            string endPattern = "#endregion";

            if (codeClass != null)
            {
                editPoint = codeClass.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
                editPoint.FindPattern(startPattern, (int)vsFindOptions.vsFindOptionsNone, ref editPoint);
                movePoint = editPoint.CreateEditPoint();
                movePoint.FindPattern(endPattern, (int)vsFindOptions.vsFindOptionsNone);
                editPoint.ReplaceText(movePoint, "\r\n" + variable, (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
                movePoint.SmartFormat(movePoint);
            }

            if (loadContentFunction != null)
            {
                editPoint = loadContentFunction.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
                editPoint.FindPattern(startPattern, (int)vsFindOptions.vsFindOptionsNone, ref editPoint);
                movePoint = editPoint.CreateEditPoint();
                movePoint.FindPattern(endPattern, (int)vsFindOptions.vsFindOptionsNone);
                editPoint.ReplaceText(movePoint, "\r\n" + loadContent, (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
                movePoint.SmartFormat(movePoint);
            }
            
            if (drawFunction != null)
            {
                editPoint = drawFunction.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
                editPoint.FindPattern(startPattern, (int)vsFindOptions.vsFindOptionsNone, ref editPoint);
                movePoint = editPoint.CreateEditPoint();
                movePoint.FindPattern(endPattern, (int)vsFindOptions.vsFindOptionsNone);
                editPoint.ReplaceText(movePoint, "\r\n" + draw, (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
                movePoint.SmartFormat(movePoint);
            }

            classFile.Save();
        }

        public Dictionary<string, DrawingObject> ReadCodeLines()
        {
            EditPoint editPoint = null;
            EditPoint movePoint = null;
            string startPattern = "#region XnaLevelEditor";
            string endPattern = "#endregion";

            #region var declaration regex
            /* reference http://stackoverflow.com/questions/585853/regex-for-variable-declaration-and-initialization-in-c-sharp
             * 
             * (a line can start with some spaces) followed by,
               (Type) followed by
               (at least one space)
               (variable_1)
               (optionally
                  (comma // next var
                   |
                   '='number // initialization
                   ) ...`
             * 
             * ^      \s*    \w+           \s+        \w+         ?          (','    |  '=' \d+   ) ...
               line  some    type          at least  var          optionally   more  or init some
               start spaces  (some chars)  one space (some chars)              vars     val  digits
             * 
             * 
             * variable <variable>
             * [a-zA-Z_][a-zA-Z0-9_]*
             * simple declaration of DrawingObject
             * \s*DrawingObject\s+<variable>\s*
             * instantiation DrawingObject
             * \s*<variable>\s*=\s*new\s+DrawingObject\(\)\s*
             * declaration (and instantiationon) of DrawingObject
             * \s*DrawingObject\s+<variable>(\s*=\s*new\s+DrawingObject\(\))?(,\s*<variable>\s*(=\s*new\s+DrawingObject\(\))))*
             * integer or float <integerFloat>
             * \-\d+(\.\d+f?)?
             * variable, integer, or float <variableIntegerFloat>
             * (<integerFloat>|<variable>)
             * Content load
             * ^\s*<variable>\s*\.\s*DrawingModel\s*=\s*Content\s*.\s*Load\s*<\s*Model\s*>\s*\(\s*\"\w+\"\s*\)\s*$
             * instantiation Vector3 <instantiationVector3>
             * \s*new\s+Vector3\s*\((\s*(<variableIntegerFloat>)\s*,\s*(<variableIntegerFloat>)\s*,\s*(<variableIntegerFloat>)\s*)?\)\s*
             * set a DrawingObject's Position value
             * \s*<variable>\s*\.\s*Position\s*=<instantiationVector3>
             * set a DrawingObject's Rotation value
             * \s*<variable>\s*\.\s*Rotation\s*=<instantiationVector3>
             * 
             */
            #endregion
            const string variableRegex = "[a-zA-Z_][a-zA-Z0-9_]*";
            const string integerOrFloatRegex = "\\-?\\d+(.\\d+f?)?";
            const string variableIntegerFloatRegex = integerOrFloatRegex + "|" + variableRegex;
            const string declarationRegex = "^\\s*DrawingObject\\s+" + variableRegex + "\\s*$";
            const string loadContentRegex = "^\\s*" + variableRegex + "\\s*.\\s*DrawingModel\\s*=\\s*Content\\s*.\\s*Load\\s*<\\s*Model\\s*>\\s*\\(\\s*\\\"\\w+\\\"\\s*\\)\\s*$";
            const string instantiationVector3 = "\\s*new\\s+Vector3\\s*\\((\\s*(" + variableIntegerFloatRegex + ")\\s*,\\s*(" + variableIntegerFloatRegex + ")\\s*,\\s*(" + variableIntegerFloatRegex + ")\\s*)?\\)\\s*";
            const string setPositionRegex = "^\\s*" + variableRegex + "\\s*\\.\\s*Position\\s*=" + instantiationVector3 + "$";
            const string setRotationRegex = "^\\s*" + variableRegex + "\\s*\\.\\s*EulerRotation\\s*=" + instantiationVector3 + "$";
            Dictionary<string, DrawingObject> objects = new Dictionary<string,DrawingObject>();

            if (codeClass != null)
            {
                editPoint = codeClass.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
                editPoint.FindPattern(startPattern, (int)vsFindOptions.vsFindOptionsNone, ref editPoint);
                movePoint = editPoint.CreateEditPoint();
                movePoint.FindPattern(endPattern, (int)vsFindOptions.vsFindOptionsNone);
                string lines = editPoint.GetText(movePoint);
                foreach (string line in lines.Split(';'))
                {
                    if (Regex.IsMatch(line, declarationRegex))
                    {
                        string key = line.Replace("DrawingObject", "").Trim();
                        objects.Add(key, new DrawingObject());
                    }
                }
            }

            if (loadContentFunction != null)
            {
                editPoint = loadContentFunction.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
                editPoint.FindPattern(startPattern, (int)vsFindOptions.vsFindOptionsNone, ref editPoint);
                movePoint = editPoint.CreateEditPoint();
                movePoint.FindPattern(endPattern, (int)vsFindOptions.vsFindOptionsNone);
                string lines = editPoint.GetText(movePoint);
                foreach (string line in lines.Split(';'))
                {
                    if (Regex.IsMatch(line, loadContentRegex))
                    {
                        string varName = Regex.Match(line, "^\\s*" + variableRegex).Value.Trim();
                        if (objects.Keys.Contains(varName))
                        {
                            string contentName = Regex.Replace(Regex.Match(line, "\\s*\\(.*\\)\\s*$").Value.Trim(), "(^\\(\\s*\\\")|(\\\"\\s*\\)$)", "");
                            try
                            {
                                ProjectItem item = contentProject.ProjectItems.Item(contentName + ".fbx");
                                objects[varName].SourceFile = Path.GetDirectoryName(contentProject.FullName) + "\\" + contentName + ".fbx";
                            }
                            catch
                            {
                                objects.Remove(varName);
                            }
                        }
                    }
                    if (Regex.IsMatch(line, setPositionRegex))
                    {
                        string varName = Regex.Match(line, "^\\s*" + variableRegex).Value.Trim();
                        if (objects.Keys.Contains(varName))
                        {
                            string values = Regex.Replace(Regex.Match(line, "\\s*\\(.*\\)\\s*$").Value, "[\\(\\)\\sf]", "");

                            if (values != "")
                            {
                                string[] valuesArray = values.Split(',');
                                objects[varName].Position = new Vector3(float.Parse(valuesArray[0], CultureInfo.InvariantCulture), float.Parse(valuesArray[1], CultureInfo.InvariantCulture), float.Parse(valuesArray[2], CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                objects[varName].Position = Vector3.Zero;
                            }
                        }
                    }
                    if (Regex.IsMatch(line, setRotationRegex))
                    {
                        string varName = Regex.Match(line, "^\\s*" + variableRegex).Value.Trim();
                        if (objects.Keys.Contains(varName))
                        {
                            string values = Regex.Replace(Regex.Match(line, "\\s*\\(.*\\)\\s*$").Value, "[\\(\\)\\sf]", "");

                            if (values != "")
                            {
                                string[] valuesArray = values.Split(',');
                                objects[varName].EulerRotation = new Vector3(float.Parse(valuesArray[0], CultureInfo.InvariantCulture), float.Parse(valuesArray[1], CultureInfo.InvariantCulture), float.Parse(valuesArray[2], CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                objects[varName].EulerRotation = Vector3.Zero;
                            }
                        }
                    }
                }
            }
            return objects;
        }

        public void ImportModelSourceToContentProject()
        {
            foreach (DrawingObject obj in mapModel.Objects)
            {
                string filename = Path.GetFileName(obj.SourceFile);
                string target = Path.GetDirectoryName(contentProject.FullName) + "\\" + filename;
                if (!File.Exists(target))
                {
                    File.Copy(obj.SourceFile, target);
                }
                try
                {
                    ProjectItem item = currentProject.ProjectItems.Item(filename);
                    if (item == null)
                    {
                        contentProject.ProjectItems.AddFromFile(target);
                    }
                }
                catch
                {
                    contentProject.ProjectItems.AddFromFile(target);
                }
            }
        }

        private static string assemblyDirectory = null;
        private CodeImport lastImportStatement;

        public static string AssemblyDirectory
        {
            get
            {
                if (assemblyDirectory == null)
                {
                    string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                    UriBuilder uri = new UriBuilder(codeBase);
                    string path = Uri.UnescapeDataString(uri.Path);
                    assemblyDirectory = System.IO.Path.GetDirectoryName(path);
                }
                return assemblyDirectory;
            }
        }
    }
}
