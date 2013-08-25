using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace EditorModel
{
    public abstract class Subject
    {
        List<IObserver> observers;
        List<Thread> threads;

        public Subject()
        {
            observers = new List<IObserver>();
            threads = new List<Thread>();
        }
    
        public void Notify()
        {
            for (int i = 0; i < threads.Count; i++)
            {
                observers[i].UpdateObserver();
                //if (threads[i].IsAlive)
                //{
                //    threads[i] = new Thread(() => observers[i].UpdateObserver());
                //}
                //threads[i].Start();
            }
        }

        public void Attach(IObserver o)
        {
            if (!observers.Contains(o))
            {
                observers.Add(o);
                threads.Add(new Thread(() => o.UpdateObserver()));
            }
        }

        public void Detach(IObserver o)
        {
            int i = 0;
            foreach (IObserver observer in observers)
            {
                if (observer == o)
                {
                    break;
                }
                i++;
            }
            observers.RemoveAt(i);
            threads.RemoveAt(i);
        }
    }
}
