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

        public ClassManager(MapModel mapModel)
        {
            this.mapModel = mapModel;
            codeLinesList = new List<CodeLines>();
        }

        public ClassManager(MapModel mapModel, string projectName, string className)
        {
            this.mapModel = mapModel;
            codeLinesList = new List<CodeLines>();
            classFile = AddClass(projectName, className);
            TraverseCodeElements();
            if (codeNamespace != null)
                codeNamespace.Name = projectName;
            if (codeClass != null)
                codeClass.Name = className;
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

        private ProjectItem AddClass(string projectName, string className)
        {
            if (applicationObject == null)
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

                if (File.Exists(Path.GetDirectoryName(projectModel.FullName) + "\\" + className + ".cs"))
                {
                    File.Delete(Path.GetDirectoryName(projectModel.FullName) + "\\" + className + ".cs");
                }

                projectModel.ProjectItems.AddFromTemplate(AssemblyDirectory + "\\templates\\Game1.cs", className + ".cs");
                ProjectItem projectItem = projectModel.ProjectItems.Item(className + ".cs");

                return projectItem;
            }
        }

        private void TraverseCodeElements()
        {
            foreach (CodeElement codeElement in classFile.FileCodeModel.CodeElements)
            {
                ExpandSubCodeElement(codeElement);
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
            foreach (CodeElement child in codeElement.Children)
            {
                ExpandSubCodeElement(child);
            }
        }

        public void GenerateClass()
        {
            string variable = "", loadContent = "", draw = "";
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

        public bool LoadClasses()
        {
            
            if (applicationObject.Solution == null)
            {
                return false;
            }
            else
            {
                Solution2 solution = (Solution2) applicationObject.Solution;
                
                output = "Solution: " + solution.FullName + "\r\n";
                foreach (Project project in solution.Projects)
                {
                    output += "ProjectKind: " + project.Kind + "\r\n";
                    output += "Project: " + project.FullName + "\r\n";
                    
                    foreach (ProjectItem projectItem in project.ProjectItems)
                    {
                        output += projectItem.Name + "\r\n";
                        if (projectItem.FileCodeModel == null)
                        {
                            continue;
                        }
                        foreach (CodeElement codeElement in projectItem.FileCodeModel.CodeElements)
                        {
                            ExamineCodeElement(codeElement);
                        }
                    }
                }
            }
            return true;
        }

        public void ExamineCodeElement(CodeElement element)
        {
            string fullname = "";
            try
            {
                fullname = element.FullName;
            }
            catch (Exception e)
            {
                output += e.Message;
            }
            output += element.Kind.ToString() + " " + fullname + "\r\n";

            if (element.Kind == vsCMElement.vsCMElementFunction)
            {
                CodeFunction2 codeFunction = (CodeFunction2)element;
                TextPoint startPoint = codeFunction.GetStartPoint(vsCMPart.vsCMPartBody);
                TextPoint endPoint = codeFunction.GetEndPoint(vsCMPart.vsCMPartBody);
                EditPoint editPoint = startPoint.CreateEditPoint();
                try
                {
                    output += editPoint.GetText(endPoint) + "\r\n";
                }
                catch
                {
                }
            }

            foreach (CodeElement elementChild in element.Children)
            {
                ExamineCodeElement(elementChild);
            }
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return System.IO.Path.GetDirectoryName(path);
            }
        }

        /*
         * CodeClass2 codeClass = null;
         *foreach (CodeElement codeElement in projectItem.FileCodeModel.CodeElements)
         *{
         *    if (codeElement.Kind == vsCMElement.vsCMElementClass && codeElement.Name == className)
         *        codeClass = (CodeClass2)codeElement;
         *}
         *
         *return codeClass;
         */

        /*
         * if (project.FullName.Contains(".contentproj"))
                    {
                        foreach (DrawingObject obj in trueModel.Objects)
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
                    }
                    else
                    {
                        //String csItemTemplatePath = ((Solution2)applicationObject.Solution).GetProjectItemTemplate("CodeFile", "CSharp");
                        project.ProjectItems.AddFromTemplate(AssemblyDirectory + "\\templates\\Game1.cs", "MyCode.cs");
                        ProjectItem projectItem = project.ProjectItems.Item("MyCode.cs");
                        CodeNamespace windowsGame1 = ((FileCodeModel2)projectItem.FileCodeModel).AddNamespace("WindowsGame1", -1);
                        CodeClass2 chess = (CodeClass2)windowsGame1.AddClass("Chess", -1, null, null, vsCMAccess.vsCMAccessDefault);
                        CodeFunction2 cf = (CodeFunction2)chess.AddFunction("Move", vsCMFunction.vsCMFunctionFunction, "int", -1, vsCMAccess.vsCMAccessDefault, null);
                        cf.AddParameter("isOk", "bool", -1);
                        TextPoint tp = cf.GetStartPoint(vsCMPart.vsCMPartBody);
                        EditPoint ep = tp.CreateEditPoint();
                        ep.Insert("string test = \"Hello World!\";");
                    }
         */
    }
}
