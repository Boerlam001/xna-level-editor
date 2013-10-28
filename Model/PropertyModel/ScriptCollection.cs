using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace EditorModel.PropertyModel
{
    public class ScriptCollectionPropertyDescriptor : PropertyDescriptor
    {
        ScriptCollection scripts;
        int index;
        public ScriptCollectionPropertyDescriptor(ScriptCollection scripts, int index)
            : base("#" + index, null)
        {
            this.scripts = scripts;
            this.index = index;
        }

        public override string DisplayName
        {
            get
            {
                return scripts[index].Name;
            }
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override Type ComponentType
        {
            get { return scripts.GetType(); }
        }

        public override object GetValue(object component)
        {
            return scripts[index];
        }

        public override bool IsReadOnly
        {
            get { return true; }
        }

        public override Type PropertyType
        {
            get { return scripts[index].GetType(); }
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            //scripts[index] = value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }
    }

    public class ScriptCollection : CollectionBase, ICustomTypeDescriptor
    {
        public Script this[int index]
        {
            get
            {
                if (index < List.Count)
                    return (Script)List[index];
                return null;
            }
        }

        public void Add(Script script)
        {
            List.Add(script);
        }

        public void Remove(Script script)
        {
            List.Remove(script);
        }

        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            // Create a new collection object PropertyDescriptorCollection
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            // Iterate the list of employees
            for (int i = 0; i < this.List.Count; i++)
            {
                // For each employee create a property descriptor 
                // and add it to the 
                // PropertyDescriptorCollection instance
                PropertyDescriptor pd = new ScriptCollectionPropertyDescriptor(this, i);
                pds.Add(pd);
            }
            return pds;
        }
    }
}
