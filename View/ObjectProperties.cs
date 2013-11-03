using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using EditorModel;
using EditorModel.PropertyModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.ComponentModel.Design;

namespace View
{
    public partial class ObjectProperties : UserControl, IObserver
    {
        BaseObject model;

        public BaseObject Model
        {
            get { return model; }
            set
            {
                model = value;
                propertyGrid1.SelectedObject = model;
            }
        }

        private MainUserControl mainUserControl;

        public MainUserControl MainUserControl
        {
            get { return mainUserControl; }
            set { mainUserControl = value; }
        }

        public CheckBox ViewBodyCheckBox
        {
            get
            {
                return viewBodyCheckBox;
            }
        }

        public ObjectProperties()
        {
            InitializeComponent();
            EditorModel.PropertyModel.ItemCollectionEditor.ItemsChanged += new System.EventHandler(propertyGrid_PropertyValueChanged);
        }

        public void UpdateObserver()
        {
            propertyGrid1.Refresh();
        }

        private void propertyGrid_PropertyValueChanged(object s, EventArgs e)
        {
            if (propertyGrid1.SelectedObject is BaseObject)
            {
                BaseObject bObj = propertyGrid1.SelectedObject as BaseObject;
                if (propertyGrid1.SelectedObject is DrawingObject)
                {
                    DrawingObject obj = bObj as DrawingObject;
                    if (e is PropertyValueChangedEventArgs)
                    {
                        PropertyValueChangedEventArgs pe = e as PropertyValueChangedEventArgs;
                        if (pe != null && pe.ChangedItem.Label == "SourceFile")
                        {
                            if (mainUserControl != null)
                                obj.DrawingModel = mainUserControl.Editor.OpenModel(pe.ChangedItem.Value.ToString());
                            else
                                obj.SourceFile = pe.OldValue.ToString();
                        }
                    }
                }
                bObj.Notify();
                mainUserControl.Editor.Camera.Notify();
            }
            propertyGrid1.Refresh();
        }

        private void propertyGrid1_DragEnter(object sender, DragEventArgs e)
        {
            if (propertyGrid1.SelectedObject != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void propertyGrid1_DragDrop(object sender, DragEventArgs e)
        {
            if (propertyGrid1.SelectedObject != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                Array files = (Array)e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    if (File.Exists(file) && Path.GetExtension(file) == ".cs")
                    {
                        try
                        {
                            string path = file;
                            if (mainUserControl.ApplicationObject != null)
                            {
                                path = mainUserControl._ClassManager.AddScript(path);
                            }
                            else
                            {
                                if (Path.GetDirectoryName(file) != mainUserControl.FileSystemWatcher.Path)
                                {
                                    path = Path.Combine(mainUserControl.FileSystemWatcher.Path, Path.GetFileName(file));
                                    File.Copy(file, path);
                                }
                            }
                            BaseObject obj = propertyGrid1.SelectedObject as BaseObject;
                            obj.Scripts.Add(new Script() { Name = Path.GetFileName(file), Path = path });
                            obj.Notify();
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private void viewBodyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            mainUserControl.Editor.ViewBody = viewBodyCheckBox.Checked;
            mainUserControl.Editor.Camera.Notify();
        }

    }
}
