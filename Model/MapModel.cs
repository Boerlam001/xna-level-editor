using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EditorModel
{
    public class MapModel : Subject
    {
        ObservableCollection<DrawingObject> objects;
        //List<DrawingObject> objects;

        public ObservableCollection<DrawingObject> Objects
        {
            get { return objects; }
            set { objects = value; }
        }

        public MapModel()
        {
            objects = new ObservableCollection<DrawingObject>();
            objects.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(objects_CollectionChanged);
        }

        void objects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Notify();
        }

        public DrawingObject getObjectByName(string name)
        {
            foreach (DrawingObject obj in objects)
            {
                if (obj.Name == name)
                    return obj;
            }
            return null;
        }

        public bool NameExists(string name)
        {
            return getObjectByName(name) != null;
        }

        /*
        public void AddObject(string file, string name, Vector3 position)
        {
            DrawingObject obj = new DrawingObject();

            string originalName = name;

            for (int i = 1; ; ++i)
            {
                if (!mapModel.NameExists(name))
                    break;
                name = originalName + "_" + i;
            }

            obj.DrawingModel = OpenModel(file);
            obj.Name = name;
            obj.Position = position;
            obj.SourceFile = file;
            obj.Attach(this);
            mapModel.Objects.Add(obj);
            Generator.CodeLines codeLines = new Generator.CodeLines();
            codeLines.Model = obj;
            mainUserControl._ClassManager.CodeLinesList.Add(codeLines);
            obj.Notify();
        }

        public Model OpenModel(string path)
        {
            bool isAdded = false;
            foreach (Microsoft.Build.Evaluation.ProjectItem item in contentBuilder.ProjectItems)
            {
                foreach (Microsoft.Build.Evaluation.ProjectMetadata metadata in item.Metadata)
                {
                    if (metadata.Name == "Link" && metadata.EvaluatedValue == System.IO.Path.GetFileName(path))
                    {
                        isAdded = true;
                    }
                }
            }

            string name = System.IO.Path.GetFileNameWithoutExtension(path);

            if (!isAdded)
            {
                contentBuilder.Add(path, name, null, "ModelProcessor");
                string errorBuild = contentBuilder.Build();
                if (string.IsNullOrEmpty(errorBuild))
                {
                    Model model = contentManager.Load<Model>(name);
                    graphicsDeviceControl1.Invalidate();
                    return model;
                }
            }
            else
            {
                Model model = contentManager.Load<Model>(name);
                graphicsDeviceControl1.Invalidate();
                return model;
            }
            return null;
        }
         */
    }
}
