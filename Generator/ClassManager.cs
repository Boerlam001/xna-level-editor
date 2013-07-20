using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                            
                            project.ProjectItems.AddFromFile(obj.SourceFile);
                        }
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
    }
}
