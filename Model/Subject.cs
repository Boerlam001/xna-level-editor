using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModel
{
    public abstract class Subject
    {
        List<IObserver> observers;

        public Subject()
        {
            observers = new List<IObserver>();
        }
    
        public void Notify()
        {
            foreach (IObserver o in observers)
                o.Update();
        }

        public void Attach(IObserver o)
        {
            if (!observers.Contains(o))
                observers.Add(o);
        }

        public void Detach(IObserver o)
        {
            observers.Remove(o);
        }
    }
}
