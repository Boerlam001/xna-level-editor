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

namespace XleGenerator
{
    public class ClassManager
    {
        #region attributes
        private string name;
        public static DTE2 applicationObject;
        ContentBuilder contentBuilder;
        string output;
        List<CodeLines> codeLinesList;
        private ProjectItem classFile;
        private MapModel mapModel;
        private CodeNamespace codeNamespace;
        private CodeClass2 codeClass;
        private CodeFunction2 loadContentFunction;
        private CodeFunction2 drawFunction;
        private Project currentProject, contentProject, xleModelProject, jitterProject, skinnedModelProject, skinnedModelPipelineProject;
        private const string xleModelProjectName = "XleModel";
        private const string jitterProjectName = "Jitter";
        private const string skinnedModelProjectName = "SkinnedModel";
        private const string skinnedModelPipelineProjectName = "SkinnedModelPipeline";
        private bool xleModelImportStmtExist;
        private static string assemblyDirectory = null;
        private CodeImport lastImportStatement;
        private string heightMapAsset;
        private bool jitterImportStmtExist;
        private bool jitterCollisionImportStmtExist;
        private CodeFunction2 updateFunction;
        private CodeFunction2 constructorFunction;
        private bool open;
        private ProjectItem scriptsFolder;
        #endregion

        #region properties
        public List<CodeLines> CodeLinesList
        {
            get { return codeLinesList; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public ContentBuilder ContentBuilder
        {
            get { return contentBuilder; }
            set { contentBuilder = value; }
        }

        public string Output
        {
            get { return output; }
            set { output = value; }
        }

        public ProjectItem ClassFile
        {
            get { return classFile; }
            set { classFile = value; }
        }

        public MapModel MapModel
        {
            get { return mapModel; }
            set { mapModel = value; }
        }

        public Project ContentProject
        {
            get { return contentProject; }
            set { contentProject = value; }
        }

        public Project CurrentProject
        {
            get { return currentProject; }
            set { currentProject = value; }
        }

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
        #endregion

        #region Constructors
        public ClassManager()
        {
            codeLinesList = new List<CodeLines>();
            name = "asd";
        }

        public ClassManager(string projectName, string className, bool open = false)
        {
            this.name = className;
            this.open = open;
            codeLinesList = new List<CodeLines>();
            TraverseProjects(projectName);
            classFile = AddClass("MouseAction", "MouseAction.cs");
            TraverseCodeElements();
            if (!classFile.IsOpen)
                classFile.Open();
            classFile.Save();
            if (codeNamespace != null)
                codeNamespace.Name = projectName;
            if (!open)
            {
                classFile = AddClass(className, "Game1.cs");
                if (!classFile.IsOpen)
                    classFile.Open();
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
        #endregion Constructors

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

        private ProjectItem AddClass(string className, string templateName)
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
                
                currentProject.ProjectItems.AddFromTemplate(AssemblyDirectory + "\\templates\\" + templateName, className + ".cs");
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
                if (project.Name == jitterProjectName)
                    jitterProject = project;
                if (project.Name == xleModelProjectName)
                    xleModelProject = project;
                if (project.Name == skinnedModelProjectName)
                    skinnedModelProject = project;
                if (project.Name == skinnedModelPipelineProjectName)
                    skinnedModelPipelineProject = project;
                if (project.FullName.Contains(".contentproj"))
                    contentProject = project;
            }

            bool scriptsFolderExists = false;
            foreach (ProjectItem item in currentProject.ProjectItems)
            {
                if (item.Name == "Scripts")
                    scriptsFolderExists = true;
            }

            if (scriptsFolderExists)
                scriptsFolder = currentProject.ProjectItems.Item("Scripts");
            else
            {
                string scriptsFolderPath = Path.GetDirectoryName(currentProject.FullName) + "\\Scripts";
                if (!Directory.Exists(scriptsFolderPath))
                    currentProject.ProjectItems.AddFolder("Scripts");
                else
                    currentProject.ProjectItems.AddFromDirectory(scriptsFolderPath);
            }
            if (jitterProject == null)
            {
                AddJitterProject();
            }
            if (skinnedModelProject == null)
            {
                AddSkinnedModelAndPipeLineProject();
            }
            if (xleModelProject == null)
            {
                AddXleModelProject();
            }
        }

        private void AddJitterProject()
        {
            if (currentProject == null)
            {
                return;
            }

            Solution2 solution = (Solution2)applicationObject.Solution;

            string dirPath = Path.GetDirectoryName(solution.FullName) + "\\Jitter";
            FileHelper.DirectoryCopy(AssemblyDirectory + "\\templates\\Jitter", dirPath, true);
            jitterProject = solution.AddFromFile(dirPath + "\\Jitter.csproj");

            VSLangProj.VSProject project;

            project = (VSLangProj.VSProject)currentProject.Object;
            project.References.AddProject(jitterProject);
        }

        private void AddSkinnedModelAndPipeLineProject()
        {
            if (currentProject == null)
            {
                return;
            }

            Solution2 solution = (Solution2)applicationObject.Solution;

            string dirPath = Path.GetDirectoryName(solution.FullName) + "\\SkinnedModel";
            FileHelper.DirectoryCopy(AssemblyDirectory + "\\templates\\SkinnedModel", dirPath, true);
            skinnedModelProject = solution.AddFromFile(dirPath + "\\SkinnedModel.csproj");

            VSLangProj.VSProject project;

            project = (VSLangProj.VSProject)currentProject.Object;
            project.References.AddProject(skinnedModelProject);


            dirPath = Path.GetDirectoryName(solution.FullName) + "\\SkinnedModelPipeline";
            FileHelper.DirectoryCopy(AssemblyDirectory + "\\templates\\SkinnedModelPipeline", dirPath, true);
            skinnedModelPipelineProject = solution.AddFromFile(dirPath + "\\SkinnedModelPipeline.csproj");

            project = (VSLangProj.VSProject)skinnedModelPipelineProject.Object;
            project.References.AddProject(skinnedModelProject);

            project = (VSLangProj.VSProject)contentProject.Object;
            project.References.AddProject(skinnedModelPipelineProject);
        }

        private void AddXleModelProject()
        {
            if (currentProject == null || jitterProject == null || skinnedModelProject == null)
            {
                return;
            }

            Solution2 solution = (Solution2)applicationObject.Solution;

            string dirPath = Path.GetDirectoryName(solution.FullName) + "\\XleModel";
            FileHelper.DirectoryCopy(AssemblyDirectory + "\\templates\\XleModel", dirPath, true);
            xleModelProject = solution.AddFromFile(dirPath + "\\XleModel.csproj");

            VSLangProj.VSProject project;

            project = (VSLangProj.VSProject)xleModelProject.Object;
            project.References.AddProject(jitterProject);
            project.References.AddProject(skinnedModelProject);

            project = (VSLangProj.VSProject)currentProject.Object;
            project.References.AddProject(xleModelProject);
        }

        private void TraverseCodeElements()
        {
            xleModelImportStmtExist = false;
            jitterImportStmtExist = false;
            jitterCollisionImportStmtExist = false;
            foreach (CodeElement codeElement in classFile.FileCodeModel.CodeElements)
            {
                ExpandSubCodeElement(codeElement);
            }
            EditPoint editPoint = lastImportStatement.EndPoint.CreateEditPoint();
            if (!xleModelImportStmtExist)
            {
                editPoint.Insert("\r\nusing XleModel;");
            }
            if (!jitterImportStmtExist)
            {
                editPoint.Insert("\r\nusing Jitter;");
            }
            if (!jitterCollisionImportStmtExist)
            {
                editPoint.Insert("\r\nusing Jitter.Collision;");
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
                if (codeFunction.Name == "LoadContent")
                {
                    loadContentFunction = codeFunction;
                }
                else if ((!open && codeFunction.Name == "Game1") /*ngikut template*/ || (open && codeFunction.Name == name))
                {
                    constructorFunction = codeFunction;
                }
                else if (codeFunction.Name == "Draw")
                {
                    drawFunction = codeFunction;
                }
                else if (codeFunction.Name == "Update")
                {
                    updateFunction = codeFunction;
                }
            }
            if (codeElement.Kind == vsCMElement.vsCMElementImportStmt)
            {
                lastImportStatement = (CodeImport)codeElement;
                if (((CodeImport)codeElement).Namespace == "XleModel")
                    xleModelImportStmtExist = true;
                if (((CodeImport)codeElement).Namespace == "Jitter")
                    jitterImportStmtExist = true;
                if (((CodeImport)codeElement).Namespace == "Jitter.Collision")
                    jitterCollisionImportStmtExist = true;
            }
            foreach (CodeElement child in codeElement.Children)
            {
                ExpandSubCodeElement(child);
            }
        }

        public void AddHeightMapToContentProject(Terrain terrain, bool isOpen = false, string target = "")
        {            
            terrain.SaveHeightMap();
            if (target == "")
                return;
            if (!File.Exists(target))
            {
                File.Copy(terrain.HeightMapFile, target);
            }
            contentProject.ProjectItems.AddFromFile(target);
            terrain.HeightMapFile = target;
            heightMapAsset = Path.GetFileNameWithoutExtension(terrain.HeightMapFile);

            target = Path.GetDirectoryName(contentProject.FullName) + "\\" + Path.GetFileName(terrain.EffectFile);
            if (!File.Exists(target))
            {
                File.Copy(terrain.EffectFile, target);
            }
            contentProject.ProjectItems.AddFromFile(target);
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
                       Append("Camera camera;\r\n").
                       Append("Terrain terrain;\r\n").
                       Append("Effect terrainEffect;\r\n").
                       Append("Texture2D heightMap;\r\n").
                       Append("World world;\r\n").
                       ToString();
            sb.Clear();
            string constructor = sb.
                       Append("CollisionSystem collisionSystem = new CollisionSystemPersistentSAP();\r\n").
                       Append("world = new World(collisionSystem);\r\n").
                       Append("world.AllowDeactivation = true;\r\n\r\n").
                       Append("camera = new Camera(this);\r\n").
                       Append("camera.Position = new Vector3(0, 50, 0);\r\n").
                       Append("camera.Rotate(20, 45, 0);\r\n").
                       Append("Components.Add(camera);\r\n\r\n").
                       ToString();
            sb.Clear();
            string loadContent = sb.
                       Append("terrainEffect = Content.Load<Effect>(\"effects\");\r\n").
                       Append("heightMap = Content.Load<Texture2D>(\"" + heightMapAsset + "\");\r\n").
                       Append("terrain = new Terrain(GraphicsDevice, camera, heightMap, this, world);\r\n").
                       Append("terrain.Effect = terrainEffect;\r\n").
                       Append("Components.Add(terrain);\r\n\r\n").
                       ToString();
            sb.Clear();
            string update = sb.
                       Append("float step = (float)gameTime.ElapsedGameTime.TotalSeconds;\r\n").
                       Append("if (step > 1.0f / 100.0f) step = 1.0f / 100.0f;\r\n").
                       Append("world.Step(step, true);\r\n").
                       //Append("foreach (GameComponent component in Components)\r\n").
                       //Append("{\r\n").
                       //Append("    component.Update(gameTime);\r\n").
                       //Append("}\r\n").
                       ToString();
            sb.Clear();
            string draw = sb.
                       //Append("terrain.Draw(terrainEffect);\r\n").
                       ToString();
            sb.Clear();
            foreach (CodeLines codeLines in codeLinesList)
            {
                variable += codeLines.Code[CodeLines.CodePosition.Variable] + ((codeLines.Code[CodeLines.CodePosition.Variable] != "") ? "\r\n" : "");
                constructor += codeLines.Code[CodeLines.CodePosition.Constructor] + ((codeLines.Code[CodeLines.CodePosition.Constructor] != "") ? "\r\n" : "");
                loadContent += codeLines.Code[CodeLines.CodePosition.LoadContent] + ((codeLines.Code[CodeLines.CodePosition.LoadContent] != "") ? "\r\n" : "");
                draw += codeLines.Code[CodeLines.CodePosition.Draw] + ((codeLines.Code[CodeLines.CodePosition.Draw] != "") ? "\r\n" : "");
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

            if (constructorFunction != null)
            {
                editPoint = constructorFunction.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
                editPoint.FindPattern(startPattern, (int)vsFindOptions.vsFindOptionsNone, ref editPoint);
                movePoint = editPoint.CreateEditPoint();
                movePoint.FindPattern(endPattern, (int)vsFindOptions.vsFindOptionsNone);
                editPoint.ReplaceText(movePoint, "\r\n" + constructor, (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
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

            if (updateFunction != null)
            {
                editPoint = updateFunction.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
                editPoint.FindPattern(startPattern, (int)vsFindOptions.vsFindOptionsNone, ref editPoint);
                movePoint = editPoint.CreateEditPoint();
                movePoint.FindPattern(endPattern, (int)vsFindOptions.vsFindOptionsNone);
                editPoint.ReplaceText(movePoint, "\r\n" + update, (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
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
             * \s*<variable>\s*=\s*new\s+DrawingObject\(\s*this\s*,\s*camera\s*,\s*\"\w+\"\s*,\s*world\s*\)\s*
             * declaration (and instantiationon) of DrawingObject
             * \s*DrawingObject\s+<variable>(\s*=\s*new\s+DrawingObject\(\))?(,\s*<variable>\s*(=\s*new\s+DrawingObject\(\))))*
             * integer or float <integerFloat>
             * \-\d+(\.\d+)?f?
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
            const string integerOrFloatRegex = "\\-?\\d+(.\\d+)?f?";
            const string variableIntegerFloatRegex = integerOrFloatRegex + "|" + variableRegex;
            const string declarationRegex = "^\\s*DrawingObject\\s+" + variableRegex + "\\s*$";
            const string instantiationRegex = "^\\s*" + variableRegex + "\\s*=\\s*new\\s+DrawingObject\\(\\s*this\\s*,\\s*camera\\s*,\\s*\\\"\\w+\\\"\\s*,\\s*world\\s*\\)\\s*$";
            //const string loadContentRegex = "^\\s*" + variableRegex + "\\s*.\\s*DrawingModel\\s*=\\s*Content\\s*.\\s*Load\\s*<\\s*Model\\s*>\\s*\\(\\s*\\\"\\w+\\\"\\s*\\)\\s*$";
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

            if (constructorFunction != null)
            {
                editPoint = constructorFunction.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
                editPoint.FindPattern(startPattern, (int)vsFindOptions.vsFindOptionsNone, ref editPoint);
                movePoint = editPoint.CreateEditPoint();
                movePoint.FindPattern(endPattern, (int)vsFindOptions.vsFindOptionsNone);
                string lines = editPoint.GetText(movePoint);
                foreach (string line in lines.Split(';'))
                {
                    if (Regex.IsMatch(line, instantiationRegex))
                    {
                        string varName = Regex.Match(line, "^\\s*" + variableRegex).Value.Trim();
                        if (objects.Keys.Contains(varName))
                        {
                            string contentWithQuotes = Regex.Match(line, "\\\"\\w+\\\"").Value;
                            string contentName = Regex.Replace(contentWithQuotes.Trim(), "\\\"", "");
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
                    //if (Regex.IsMatch(line, loadContentRegex))
                    //{
                    //    string varName = Regex.Match(line, "^\\s*" + variableRegex).Value.Trim();
                    //    if (objects.Keys.Contains(varName))
                    //    {
                    //        string contentName = Regex.Replace(Regex.Match(line, "\\s*\\(.*\\)\\s*$").Value.Trim(), "(^\\(\\s*\\\")|(\\\"\\s*\\)$)", "");
                    //        try
                    //        {
                    //            ProjectItem item = contentProject.ProjectItems.Item(contentName + ".fbx");
                    //            objects[varName].SourceFile = Path.GetDirectoryName(contentProject.FullName) + "\\" + contentName + ".fbx";
                    //        }
                    //        catch
                    //        {
                    //            objects.Remove(varName);
                    //        }
                    //    }
                    //}
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
    }
}
