using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using EnvDTE80;
using System.IO;

namespace XleGenerator
{
    public class SolutionManager
    {
        private static string assemblyDirectory = null;
        
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

        private DTE applicationObject;
        private Project currentProject, contentProject, xleModelProject;
        private const string xleModelProjectName = "XleModel";

        public SolutionManager(string projectName)
        {
            TraverseProjects(projectName);
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
    }
}
