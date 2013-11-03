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
        ObservableCollection<BaseObject> objects;
        DrawingCamera mainCamera;
        PhysicsWorld physicsWorld;
        private BaseObject selected;
        Terrain terrain;
        Grid grid;

        public ObservableCollection<BaseObject> Objects
        {
            get { return objects; }
            set { objects = value; }
        }

        public DrawingCamera MainCamera
        {
            get { return mainCamera; }
        }

        public PhysicsWorld PhysicsWorld
        {
            get { return physicsWorld; }
        }

        public BaseObject Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        public Terrain Terrain
        {
            get { return terrain; }
            set { terrain = value; }
        }

        public Grid Grid
        {
            get { return grid; }
            set { grid = value; }
        }

        public MapModel()
        {
            objects = new ObservableCollection<BaseObject>();
            objects.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(objects_CollectionChanged);
            mainCamera = new DrawingCamera();
            mainCamera.Name = "Main Camera";
            mainCamera.Position = new Vector3(64, 5, 64);
            mainCamera.MapModel = this;
            objects.Add(mainCamera);

            physicsWorld = new PhysicsWorld();
        }

        void objects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Notify();
        }

        public BaseObject getObjectByName(string name)
        {
            foreach (BaseObject obj in objects)
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
    }

    public enum MaterialCoefficientMixing
    {
        TakeMaximum = 0,
        TakeMinimum = 1,
        UseAverage = 2
    }

    public class PhysicsWorld : Subject
    {
        float gravity;
        MaterialCoefficientMixing materialCoefficientMixing;

        public float Gravity
        {
            get { return gravity; }
            set { gravity = value; }
        }

        public MaterialCoefficientMixing MaterialCoefficientMixing
        {
            get { return materialCoefficientMixing; }
            set { materialCoefficientMixing = value; }
        }

        public PhysicsWorld()
        {
            gravity = -30;
            materialCoefficientMixing = EditorModel.MaterialCoefficientMixing.TakeMaximum;
        }
    }
}
