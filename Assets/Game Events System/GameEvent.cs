using System;
using UnityEngine.Events;

namespace EventSystem
{
    public interface GameEventBase
    {
        public void Add(Delegate action);
        public void Remove(Delegate action);
        public void Invoke();
    }
    /* 
     * Wrapper class for a default UnityAction event to
     * provide a unified interface for the generic version
     */
    public class GameEvent : GameEventBase
    {
        private UnityAction Event;
        public void Add(UnityAction action)
        {
            Event += action;
        }
        public void Add(Delegate action)
        {
            if (action is UnityAction unityAction)
                Event += unityAction;
        }
        public void Remove(UnityAction action)
        {
            Event -= action;
        }
        public void Remove(Delegate action)
        {
            if (action is UnityAction unityAction)
            {
                Event -= unityAction;
            }
        }

        public void Invoke()
        {
            Event?.Invoke();
        }
    }

    /*
     * We purposefully only provide an interface with a single 
     * generic type to encourage clients to define custom data
     * types whose intent is more clear than that of their 
     * primative counterparts.
     * 
     * Example: Which is immediately more clear?
     * 
     * 1. CuddleStarted?.Invoke(true, 8, "Tomorrow"); 
     * 
     *         -- or --
     * 
     * 2. struct CuddleData
     *    {
     *       public bool holdHands;
     *       public float duration;
     *       public string subtitle;
     *    }
     * 
     *    CuddleStarted?.Invoke(cuddleData);
     *    
     * Clearly, option 2 gives much more detail as to the 
     * purpose of the parameters. This is exactly the same for 
     * all number of parameters INCLUDING 1.
     */
    public class GameEvent<T> : GameEventBase
    {
        private UnityAction<T> Event;
        public T Latest;

        public void Add(UnityAction<T> action)
        {
            Event += action;
        }
        public void Add(Delegate action)
        {
            if (action is UnityAction<T> unityAction)
            {
                Event += unityAction;
            }
        }
        public void Remove(UnityAction<T> action)
        {
            Event -= action;
        }
        public void Remove(Delegate action)
        {
            if (action is UnityAction<T> unityAction)
            {
                Event -= unityAction;
            }
        }
        /// <summary>
        /// Invoke while passing the value of arg and update latest to reflect this value
        /// </summary>
        public void Invoke(T arg)
        {
            Latest = arg;
            Event?.Invoke(arg);
        }

        /// <summary>
        /// Invoke using the most recent value passed to the event.
        /// </summary>
        public void Invoke()
        {
            if (Latest == null)
            {
                throw new Exception("You cannot call Invoke() before Invoke(arg). Did you mean to pass data with this event?");
            }
            Invoke(Latest);
        }
    }
}


