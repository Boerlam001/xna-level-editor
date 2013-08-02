using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using EditorModel;

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

        public static ProjectItem SelectClass(string projectName, string className)
        {
            if (applicationObject.Solution == null)
            {
                return null;
            }
            else
            {
                Solution2 solution = (Solution2)applicationObject.Solution;
                Project projectModel = null;
                foreach (Project project in solution.Projects)
                {
                    if (project.Name == projectName)
                        projectModel = project;
                }
                if (projectModel == null)
                    return null;
                ProjectItem projectItem = projectModel.ProjectItems.Item(className + ".cs");

                return projectItem;
            }
        }

        private ProjectItem AddClass(string className)
        {
            if (applicationObject == null)
            {
                return null;
            }
            else
            {
                if (currentProject == null)
                    return null;

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

        /*
         * 
         *  foreach (DrawingObject obj in trueModel.Objects)
            {
                try
                {
                    string filename = Path.GetFileName(obj.SourceFile);
                    File.Copy(obj.SourceFile, Path.GetDirectoryName(project.FullName) + "\\" + filename, true);
                    project.ProjectItems.AddFromFile(Path.GetDirectoryName(project.FullName) + "\\" + filename);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                    project.ProjectItems.AddFromFile(obj.SourceFile);
                }
            }
         */

    }
}
