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
        DTE2 applicationObject;

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

        TrueModel trueModel;

        public TrueModel TrueModel
        {
            get { return trueModel; }
            set { trueModel = value; }
        }

        List<CodeClass2> classModels;

        public ClassManager(DTE2 applicationObject)
        {
            this.applicationObject = applicationObject;
        }

        public bool LoadClasses()
        {
            
            if (applicationObject.Solution == null)
            {
                return false;
            }
            else
            {
                output = "Solution: " + applicationObject.Solution.FullName + "\r\n";
                foreach (Project project in applicationObject.Solution.Projects)
                {
                    output += "ProjectKind: " + project.Kind + "\r\n";
                    output += "Project: " + project.FullName + "\r\n";

                    if (project.FullName.Contains(".contentproj"))
                    {
                        //project.ProjectItems = contentBuilder.ProjectItems;
                        //foreach (Microsoft.Build.Evaluation.ProjectItem item in contentBuilder.ProjectItems)
                        //{
                        //    foreach (Microsoft.Build.Evaluation.ProjectMetadata metadata in item.Metadata)
                        //    {
                        //        if (metadata.Name == "Link")
                        //        {
                        //            project.ProjectItems.AddFromFile(metadata.Project.DirectoryPath + metadata.EvaluatedValue);
                        //        }
                        //    }
                        //    //project.ProjectItems.AddFromFile(
                        //}
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
                        String csItemTemplatePath;
                        ProjectItem projectItem;
                        csItemTemplatePath = ((Solution2)applicationObject.Solution).GetProjectItemTemplate("CodeFile", "CSharp");
                        project.ProjectItems.AddFromTemplate(csItemTemplatePath, "MyCode.cs");
                        projectItem = project.ProjectItems.Item("MyCode.cs");
                        CodeNamespace windowsGame1 = ((FileCodeModel2)projectItem.FileCodeModel).AddNamespace("WindowsGame1", -1);
                        CodeClass2 chess = (CodeClass2)windowsGame1.AddClass("Chess", -1, null, null, vsCMAccess.vsCMAccessDefault);
                        CodeFunction2 cf = (CodeFunction2)chess.AddFunction("Move", vsCMFunction.vsCMFunctionFunction, "int", -1, vsCMAccess.vsCMAccessDefault, null);
                        cf.AddParameter("isOk", "bool", -1);
                        TextPoint tp = cf.GetStartPoint(vsCMPart.vsCMPartBody);
                        EditPoint ep = tp.CreateEditPoint();
                        ep.Indent();
                        ep.Indent();
                        ep.Indent();
                        ep.Insert("string test = \"Hello World!\";");
                    }
                    
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
            foreach (CodeElement elementChild in element.Children)
            {
                ExamineCodeElement(elementChild);
            }
        }

        public void CreateClass()
        {

        }
    }
}
