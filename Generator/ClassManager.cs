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
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;

namespace XleGenerator
{
    internal abstract class EnvDTEConstants
    {
        public const string vsProjectItemKindPhysicalFile = "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}";
    }

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
        private static string xleModelProjectName = "XleModel";
        private static string jitterProjectName = "Jitter";
        private static string skinnedModelProjectName = "SkinnedModel";
        private static string skinnedModelPipelineProjectName = "SkinnedModelPipeline";
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
        private string textureAsset;
        private string gridMapAsset;
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

        public static List<string> ListProjects()
        {
            List<string> projects = new List<string>();
            if (applicationObject != null)
            {
                Solution2 solution = (Solution2)applicationObject.Solution;
                foreach (Project project in solution.Projects)
                {
                    if (project.Name != jitterProjectName &&
                        project.Name != xleModelProjectName &&
                        project.Name != skinnedModelProjectName &&
                        project.Name != skinnedModelPipelineProjectName &&
                        !project.FullName.Contains(".contentproj"))
                        projects.Add(project.Name);
                }
            }
            return projects;
        }

        public static List<string> ListClasses(string projectName)
        {
            if (applicationObject == null)
                return null;
            List<string> classes = new List<string>();
            Solution2 solution = (Solution2)applicationObject.Solution;
            foreach (Project project in solution.Projects)
            {
                if (project.Name == projectName)
                {
                    foreach (ProjectItem projectItem in project.ProjectItems)
                    {
                        if (projectItem.Kind == EnvDTEConstants.vsProjectItemKindPhysicalFile && projectItem.Name.Contains(".cs"))
                            classes.Add(projectItem.Name.Substring(0, projectItem.Name.Length - 3));
                    }
                    break;
                }
            }
            return classes;
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
                    scriptsFolder = currentProject.ProjectItems.AddFolder("Scripts");
                else
                    scriptsFolder = currentProject.ProjectItems.AddFromDirectory(scriptsFolderPath);
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

        public string AddScript(string file)
        {
            try
            {
                string scriptsFolderPath = Path.GetDirectoryName(currentProject.FullName) + "\\Scripts";
                if (Path.GetDirectoryName(file) != scriptsFolderPath)
                {
                    string newFile = Path.Combine(scriptsFolderPath, Path.GetFileName(file));
                    File.Copy(file, newFile);
                    file = newFile;
                }
                scriptsFolder.ProjectItems.AddFromFile(file);
                return file;
            }
            catch (Exception ex)
            {
                throw ex;
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

        public void AddHeightMapToContentProject(bool isOpen = false, string target = null)
        {
            mapModel.Terrain.SaveHeightMap();
            if (string.IsNullOrEmpty(target))
                return;
            if (!File.Exists(target))
            {
                File.Copy(mapModel.Terrain.HeightMapFile, target);
            }
            contentProject.ProjectItems.AddFromFile(target);
            mapModel.Terrain.HeightMapFile = target;
            mapModel.Terrain.SaveHeightMap();
            heightMapAsset = Path.GetFileNameWithoutExtension(mapModel.Terrain.HeightMapFile);

            textureAsset = Path.GetDirectoryName(contentProject.FullName) + "\\" + Path.GetFileName(mapModel.Terrain.TextureFile);
            if (!File.Exists(textureAsset))
            {
                File.Copy(mapModel.Terrain.TextureFile, textureAsset);
            }
            contentProject.ProjectItems.AddFromFile(textureAsset);
            textureAsset = Path.GetFileNameWithoutExtension(textureAsset);
        }

        public void AddGridMapToContentProject(bool isOpen = false, string target = null)
        {
            mapModel.Grid.ExportGridMap();
            if (string.IsNullOrEmpty(target))
                return;
            if (!File.Exists(target))
            {
                File.Copy(mapModel.Grid.GridMapFile, target);
            }
            contentProject.ProjectItems.AddFromFile(target);
            mapModel.Grid.GridMapFile = target;
            mapModel.Grid.ExportGridMap();
            gridMapAsset = Path.GetFileNameWithoutExtension(target);

            for (int i = 0; i < mapModel.Grid.RoadAssetFiles.Count; i++)
            {
                target = Path.GetDirectoryName(contentProject.FullName) + "\\" + Path.GetFileName(mapModel.Grid.RoadAssetFiles[i]);
                if (!File.Exists(target))
                {
                    File.Copy(mapModel.Grid.RoadAssetFiles[i], target);
                }
                if (Path.GetExtension(target) == ".fbx")
                    contentProject.ProjectItems.AddFromFile(target);
                mapModel.Grid.RoadAssetFiles[i] = target;
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
                       Append("Camera camera;\r\n").
                       Append("Terrain terrain;\r\n").
                       Append("Texture2D heightMap;\r\n").
                       Append("World world;\r\n").
                       Append("Grid grid;\r\n").
                       ToString();
            sb.Clear();
            Vector3 cameraPos = mapModel.MainCamera.Position;
            Vector3 cameraRot = mapModel.MainCamera.EulerRotation;
            sb.
                Append("CollisionSystem collisionSystem = new CollisionSystemPersistentSAP();\r\n").
                Append("world = new World(collisionSystem);\r\n").
                Append("world.AllowDeactivation = true;\r\n").
                Append("world.Gravity = new JVector(0, ").Append(mapModel.PhysicsWorld.Gravity).Append("f, 0);\r\n").
                Append("world.ContactSettings.MaterialCoefficientMixing = ContactSettings.MaterialCoefficientMixingType.").Append(mapModel.PhysicsWorld.MaterialCoefficientMixing.ToString()).Append(";\r\n").
                Append("collisionSystem.CollisionDetected += new CollisionDetectedHandler(collisionSystem_CollisionDetected);\r\n\r\n").
                Append("camera = new Camera(this);\r\n").
                Append("camera.Name = \"camera\";\r\n").
                Append("camera.Position = new Vector3(").Append(cameraPos.X).Append("f, ").Append(cameraPos.Y).Append("f, ").Append(cameraPos.Z).Append("f);\r\n").
                Append("camera.EulerRotation = new Vector3(").Append(cameraRot.X).Append("f, ").Append(cameraRot.Y).Append("f, ").Append(cameraRot.Z).Append("f);\r\n");
            foreach (EditorModel.PropertyModel.Script script in mapModel.MainCamera.Scripts)
            {
                sb.Append("camera.AddScript(new ").Append(System.IO.Path.GetFileNameWithoutExtension(script.Name)).Append("());\r\n");
            }
            sb.Append("Components.Add(camera);\r\n\r\n");
            string constructor = sb.ToString();
            sb.Clear();
            sb.
                Append("heightMap = Content.Load<Texture2D>(\"" + heightMapAsset + "\");\r\n").
                Append("terrain = new Terrain(GraphicsDevice, camera, heightMap, this, world);\r\n").
                Append("terrain.Texture = Content.Load<Texture2D>(\"" + textureAsset + "\");\r\n").
                Append("Components.Add(terrain);\r\n\r\n").
                Append("grid = new Grid(this, terrain, 8, camera, GraphicsDevice, new BasicEffect(GraphicsDevice), world);\r\n").
                Append("grid.RoadModel = Content.Load<Model>(\"jalan_raya\");\r\n").
                Append("grid.RoadModel_belok = Content.Load<Model>(\"jalan_raya_belok\");\r\n").
                Append("grid.GridMap = Content.Load<Texture2D>(\"gridmap_Game2\");\r\n").
                Append("grid.ImportGridMap();\r\n\r\n");
            string loadContent = sb.ToString();
            sb.Clear();
            string update = sb.
                       Append("float step = (float)gameTime.ElapsedGameTime.TotalSeconds;\r\n").
                       Append("if (step > 1.0f / 100.0f) step = 1.0f / 100.0f;\r\n").
                       Append("world.Step(step, true);\r\n").
                       ToString();
            sb.Clear();
            string draw = sb.
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

        public Dictionary<string, BaseObject> ReadCodeLines()
        {
            EditPoint editPoint = null;
            EditPoint movePoint = null;
            string startPattern = "#region XnaLevelEditor";
            string endPattern = "#endregion";

            #region var declaration regex
            /* reference http://stackoverflow.com/questions/585853/regex-for-variable-declaration-and-initialization-in-c-sharp
             * tested at http://rubular.com/
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
             * \s*(DrawingObject|Camera)\s+<variable>\s*
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
             * * set a DrawingObject's Scale value
             * \s*<variable>\s*\.\s*Scale\s*=<instantiationVector3>
             * set PhysicsEnabled
             * \s*<variable>\s*\.\s*PhysicsEnabled\s*=\s*(true|false)\s*
             * set IsActive
             * \s*<variable>\s*\.\s*PhysicsAdapter\s*.\s*Body\s*.\s*IsActive\s*=\s*(true|false)\s*
             * set IsStatic
             * \s*<variable>\s*\.\s*PhysicsAdapter\s*.\s*Body\s*.\s*IsStatic\s*=\s*(true|false)\s*
             * set CharacterControllerEnabled
             * \s*<variable>\s*\.\s*PhysicsAdapter\s*.\s*EnableCharacterController\s*\(\s*\)\s*
             * change PhysicsAdapter
             * \s*<variable>\s*\.\s*ChangePhysicsAdapter\s*\(\s*typeof\s*\(\s*[a-zA-Z_][a-zA-Z0-9_]*\s*\)\s*,\s*new\s*object\s*\[\s*\]\s*{[\s\w\,\(\)\-\d\.]*}\s*\)\s*
             * add script
             * \s*<variable>\s*\.AddScript\s*\(new\s*<variable>\s*\(\s*\)\s*\)\s*
             * 
             */
            #endregion
            const string variableRegex = "[a-zA-Z_][a-zA-Z0-9_]*";
            const string integerOrFloatRegex = "\\-?\\d+(.\\d+)?f?";
            const string variableIntegerFloatRegex = integerOrFloatRegex + "|" + variableRegex;
            const string declarationRegex = "^\\s*(DrawingObject|Camera)\\s+" + variableRegex + "\\s*$";
            const string instantiationRegex = "^\\s*" + variableRegex + "\\s*=\\s*new\\s+DrawingObject\\(\\s*this\\s*,\\s*camera\\s*,\\s*\\\"\\w+\\\"\\s*,\\s*world\\s*\\)\\s*$";
            //const string loadContentRegex = "^\\s*" + variableRegex + "\\s*.\\s*DrawingModel\\s*=\\s*Content\\s*.\\s*Load\\s*<\\s*Model\\s*>\\s*\\(\\s*\\\"\\w+\\\"\\s*\\)\\s*$";
            const string instantiationVector3Regex = "\\s*new\\s+Vector3\\s*\\((\\s*(" + variableIntegerFloatRegex + ")\\s*,\\s*(" + variableIntegerFloatRegex + ")\\s*,\\s*(" + variableIntegerFloatRegex + ")\\s*)?\\)\\s*";
            const string setPositionRegex = "^\\s*" + variableRegex + "\\s*\\.\\s*Position\\s*=" + instantiationVector3Regex + "$";
            const string setRotationRegex = "^\\s*" + variableRegex + "\\s*\\.\\s*EulerRotation\\s*=" + instantiationVector3Regex + "$";
            const string setScaleRegex = "^\\s*" + variableRegex + "\\s*\\.\\s*Scale\\s*=" + instantiationVector3Regex + "$";
            const string setPhysicsEnabledRegex = "^\\s*" + variableRegex + "\\s*\\.\\s*PhysicsEnabled\\s*=\\s*(true|false)\\s*$";
            const string setIsActiveRegex = "^\\s*" + variableRegex + "\\s*\\.\\s*PhysicsAdapter\\s*.\\s*Body\\s*.\\s*IsActive\\s*=\\s*(true|false)\\s*$";
            const string setIsStaticRegex = "^\\s*" + variableRegex + "\\s*\\.\\s*PhysicsAdapter\\s*.\\s*Body\\s*.\\s*IsStatic\\s*=\\s*(true|false)\\s*$";
            const string enableCharacterControllerRegex = "^\\s*" + variableRegex + "\\s*\\.\\s*PhysicsAdapter\\s*.\\s*EnableCharacterController\\s*\\(\\s*\\)\\s*$";
            const string changePhysicsAdapterRegex = "^\\s*" + variableRegex + "\\s*\\.\\s*ChangePhysicsAdapter\\s*\\(\\s*typeof\\s*\\(\\s*[a-zA-Z_][a-zA-Z0-9_]*\\s*\\)\\s*,\\s*new\\s*object\\s*\\[\\s*\\]\\s*{.*}\\s*\\)\\s*$";
            const string addScriptRegex = "^\\s*" + variableRegex + "\\s*\\.AddScript\\s*\\(new\\s*" + variableRegex + "\\s*\\(\\s*\\)\\s*\\)\\s*$";
            Dictionary<string, BaseObject> objects = new Dictionary<string, BaseObject>();

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
                        ParseDeclaration(objects, line);
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
                        ParseInstantiation(variableRegex, objects, line, contentProject);
                    }
                    if (Regex.IsMatch(line, setPositionRegex))
                    {
                        ParseSetPosition(variableRegex, objects, line);
                    }
                    if (Regex.IsMatch(line, setRotationRegex))
                    {
                        ParseSetRotation(variableRegex, objects, line);
                    }
                    if (Regex.IsMatch(line, setScaleRegex))
                    {
                        ParseSetScale(variableRegex, objects, line);
                    }
                    if (Regex.IsMatch(line, setPhysicsEnabledRegex))
                    {
                        ParseSetPhysicsEnabled(variableRegex, objects, line);
                    }
                    if (Regex.IsMatch(line, setIsActiveRegex))
                    {
                        ParseSetIsActiveRegex(variableRegex, objects, line);
                    }
                    if (Regex.IsMatch(line, setIsStaticRegex))
                    {
                        ParseSetIsStaticRegex(variableRegex, objects, line);
                    }
                    if (Regex.IsMatch(line, enableCharacterControllerRegex))
                    {
                        ParseEnableCharacterController(variableRegex, objects, line);
                    }
                    if (Regex.IsMatch(line, changePhysicsAdapterRegex))
                    {
                        ParseChangePhysicsAdapter(variableRegex, integerOrFloatRegex, instantiationVector3Regex, objects, line);
                    }
                    if (Regex.IsMatch(line, addScriptRegex))
                    {
                        string varName = Regex.Match(line, "^\\s*" + variableRegex).Value.Trim();
                        if (objects.Keys.Contains(varName))
                        {
                            string scriptName = Regex.Match(line, "new\\s*" + variableRegex).Value.Replace("new", "").Trim() + ".cs";
                            string scriptsFolderPath = Path.GetDirectoryName(currentProject.FullName) + "\\Scripts";
                            objects[varName].Scripts.Add(new EditorModel.PropertyModel.Script() { Name = scriptName, Path = scriptsFolderPath + "\\" + scriptName });
                        }
                    }
                }
            }
            return objects;
        }

        private static void ParseChangePhysicsAdapter(string variableRegex, string integerOrFloatRegex, string instantiationVector3Regex, Dictionary<string, BaseObject> objects, string line)
        {
            string varName = Regex.Match(line, "^\\s*" + variableRegex).Value.Trim();
            if (objects.Keys.Contains(varName))
            {
                string adapterType = Regex.Replace(Regex.Match(line, "typeof\\s*\\(\\s*[a-zA-Z_][a-zA-Z0-9_]*\\s*\\)").Value, "(typeof)|[\\s*\\(\\)]", "");
                string values = Regex.Replace(Regex.Match(line, "\\s*{.*}\\s*\\)$").Value, "[{}]", "");
                MatchCollection matches = Regex.Matches(values, instantiationVector3Regex);
                if (matches.Count > 0)
                {
                    string bodyPositionString = matches[0].Value;
                    string[] vectorValues = Regex.Replace(Regex.Match(bodyPositionString, "\\(.*\\)").Value, "[\\(\\)\\sf]", "").Split(',');
                    objects[varName].BodyPosition = new Vector3(float.Parse(vectorValues[0], CultureInfo.InvariantCulture), float.Parse(vectorValues[1], CultureInfo.InvariantCulture), float.Parse(vectorValues[2], CultureInfo.InvariantCulture));
                }
                if (adapterType == "BoxAdapter")
                {
                    objects[varName].PhysicsShapeKind = PhysicsShapeKind.BoxShape;
                    string sizeString = matches[1].Value;
                    string[] vectorValues = Regex.Replace(Regex.Match(sizeString, "\\(.*\\)").Value, "[\\(\\)\\sf]", "").Split(',');
                    ((BoxShape)objects[varName].Body.Shape).Size = new JVector(float.Parse(vectorValues[0], CultureInfo.InvariantCulture), float.Parse(vectorValues[1], CultureInfo.InvariantCulture), float.Parse(vectorValues[2], CultureInfo.InvariantCulture));
                }
                else if (adapterType == "CapsuleAdapter")
                {
                    objects[varName].PhysicsShapeKind = PhysicsShapeKind.CapsuleShape;
                    string withoutVector3 = Regex.Replace(values, instantiationVector3Regex + "|\\)", "");
                    string[] valuesArray = withoutVector3.Split(',');
                    ((CapsuleShape)objects[varName].Body.Shape).Length = float.Parse(valuesArray[4].Replace("f", ""), CultureInfo.InvariantCulture);
                    ((CapsuleShape)objects[varName].Body.Shape).Radius = float.Parse(valuesArray[5].Replace("f", ""), CultureInfo.InvariantCulture);
                }
                else if (adapterType == "ConvexHullAdapter")
                {
                    objects[varName].PhysicsShapeKind = PhysicsShapeKind.ConvexHullShape;
                }
            }
        }

        private static void ParseEnableCharacterController(string variableRegex, Dictionary<string, BaseObject> objects, string line)
        {
            string varName = Regex.Match(line, "^\\s*" + variableRegex).Value.Trim();
            if (objects.Keys.Contains(varName))
            {
                objects[varName].CharacterControllerEnabled = true;
            }
        }

        private static void ParseSetIsStaticRegex(string variableRegex,Dictionary<string,BaseObject> objects,string line)
        {
            string varName = Regex.Match(line, "^\\s*" + variableRegex).Value.Trim();
            if (objects.Keys.Contains(varName))
            {
                string value = line.Substring(line.LastIndexOf("=") + 1).Trim();
                if (value == "true")
                {
                    objects[varName].IsStatic = true;
                }
                else if (value == "false")
                {
                    objects[varName].IsStatic = false;
                }
            }
        }

        private static void ParseSetIsActiveRegex(string variableRegex, Dictionary<string, BaseObject> objects, string line)
        {
            string varName = Regex.Match(line, "^\\s*" + variableRegex).Value.Trim();
            if (objects.Keys.Contains(varName))
            {
                string value = line.Substring(line.LastIndexOf("=") + 1).Trim();
                if (value == "true")
                {
                    objects[varName].IsActive = true;
                }
                else if (value == "false")
                {
                    objects[varName].IsActive = false;
                }
            }
        }

        private static void ParseSetPhysicsEnabled(string variableRegex, Dictionary<string, BaseObject> objects, string line)
        {
            string varName = Regex.Match(line, "^\\s*" + variableRegex).Value.Trim();
            if (objects.Keys.Contains(varName))
            {
                string value = line.Substring(line.LastIndexOf("=") + 1).Trim();
                if (value == "true")
                {
                    objects[varName].PhysicsEnabled = true;
                }
                else if (value == "false")
                {
                    objects[varName].PhysicsEnabled = false;
                }
            }
        }

        private static void ParseSetScale(string variableRegex, Dictionary<string, BaseObject> objects, string line)
        {
            string varName = Regex.Match(line, "^\\s*" + variableRegex).Value.Trim();
            if (objects.Keys.Contains(varName))
            {
                string values = Regex.Replace(Regex.Match(line, "\\s*\\(.*\\)\\s*$").Value, "[\\(\\)\\sf]", "");

                if (values != "")
                {
                    string[] valuesArray = values.Split(',');
                    objects[varName].Scale = new Vector3(float.Parse(valuesArray[0], CultureInfo.InvariantCulture), float.Parse(valuesArray[1], CultureInfo.InvariantCulture), float.Parse(valuesArray[2], CultureInfo.InvariantCulture));
                }
                else
                {
                    objects[varName].Scale = Vector3.Zero;
                }
            }
        }

        private static void ParseSetRotation(string variableRegex, Dictionary<string, BaseObject> objects, string line)
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

        private static void ParseSetPosition(string variableRegex, Dictionary<string, BaseObject> objects, string line)
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

        private static void ParseInstantiation(string variableRegex, Dictionary<string, BaseObject> objects, string line, Project contentProject)
        {
            string varName = Regex.Match(line, "^\\s*" + variableRegex).Value.Trim();
            if (objects.Keys.Contains(varName) && objects[varName] is DrawingObject)
            {
                string contentWithQuotes = Regex.Match(line, "\\\"\\w+\\\"").Value;
                string contentName = Regex.Replace(contentWithQuotes.Trim(), "\\\"", "");
                try
                {
                    foreach (string file in Directory.EnumerateFiles(Path.GetDirectoryName(contentProject.FullName)))
                    {
                        if (Path.GetFileNameWithoutExtension(file) == contentName)
                        {
                            contentName = Path.GetFileName(file);
                            break;
                        }
                    }
                    ProjectItem item = contentProject.ProjectItems.Item(contentName);
                    (objects[varName] as DrawingObject).SourceFile = Path.GetDirectoryName(contentProject.FullName) + "\\" + contentName;
                }
                catch
                {
                    objects.Remove(varName);
                }
            }
        }

        private static void ParseDeclaration(Dictionary<string, BaseObject> objects, string line)
        {
            string objectType = Regex.Match(line, "(DrawingObject|Camera)").Value.Trim();
            string key = line.Replace(objectType, "").Trim();
            if (objectType == "DrawingObject")
                objects.Add(key, new DrawingObject());
            else
                objects.Add(key, new DrawingCamera());
        }

        public void ImportModelSourceToContentProject()
        {
            foreach (BaseObject bObj in mapModel.Objects)
            {
                if (!(bObj is DrawingObject))
                    continue;
                DrawingObject obj = bObj as DrawingObject;
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
