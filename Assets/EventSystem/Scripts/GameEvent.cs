using System;

namespace GameEvents
{
    // Inherit from this to make testing and custom Roslyn analyzer easier
    public interface IGameEvent { }

    /// <summary>
    /// Game event with no parameters.
    /// </summary>
    public class GameEvent : IGameEvent
    {
        private Action _event;

        /// <summary>
        /// Adds a delegate to the event list.
        /// </summary>
        /// <param name="action">The delegate to add.</param>
        public void Subscribe(Action action)
        {
            _event += action;
        }

        /// <summary>
        /// Removes a delegate from the event list.
        /// </summary>
        /// <param name="action">The delegate to remove.</param>
        public void Unsubscribe(Action action)
        {
            if (_event == null) return;
            _event -= action;
        }

        /// <summary>
        /// Invokes the event.
        /// </summary>
        public void Invoke()
        {
            _event?.Invoke();
        }
        
        /// <summary>
        /// Overload of the + operator for convenient subscription.
        /// </summary>
        /// <param name="gameEvent">'this' GameEvent</param>
        /// <param name="action">The action to subscribe to it.</param>
        /// <returns>'this' GameEvent with the subscribed action in its call list.</returns>
        public static GameEvent operator +(GameEvent gameEvent, Action action)
        {
            gameEvent.Subscribe(action);
            return gameEvent;
        }

        /// <summary>
        /// Overload of the - operator for convenient unsubscription.
        /// </summary>
        /// <param name="gameEvent">'this' GameEvent</param>
        /// <param name="action">The action to unsubscribe from it.</param>
        /// <returns>'this' GameEvent with the unsubscribed action removed from its call list.</returns>
        public static GameEvent operator -(GameEvent gameEvent, Action action)
        {
            gameEvent.Unsubscribe(action);
            return gameEvent;
        }
    }

    /*
     * We purposefully only provide an interface with a single 
     * generic type to encourage clients to define custom data
     * types whose intent is more clear than that of their 
     * primitive counterparts.
     * 
     * Example: Which is immediately more clear for the following event?
     * 
     * 1. GameEvent<uint, int, bool, string> PlayerConnected;
     * 
     *         -- or --
     * 
     * 2. struct PlayerConnectionData
     *    {
     *       public uint clientId,
     *       public int serverTick;
     *       public bool asHost;
     *       public string username;
     *    }
     *
     *    GameEvent<PlayerConnectionData> PlayerConnected;
     *
     * Clearly, option 2 gives much more detail as to the 
     * purpose of the parameters. This is exactly the same for 
     * all number of parameters INCLUDING 1.
     */

    /// <summary>
    /// Game event with one parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter to pass in.</typeparam>
    public class GameEvent<T>: IGameEvent
    {
        private Action<T> _event;
        
        /// <summary>
        /// The value of the most recent parameter this event was invoked with.
        /// </summary>
        public T Latest { get; private set; }

        /// <summary>
        /// Adds a delegate to event list.
        /// </summary>
        /// <param name="action">The delegate to add.</param>
        public void Subscribe(Action<T> action)
        {
            _event += action;
        }

        /// <summary>
        /// Removes a delegate from the event list.
        /// </summary>
        /// <param name="action">The delegate to remove.</param>
        public void Unsubscribe(Action<T> action)
        {
            if (_event == null) return;
            _event -= action;
        }

        /// <summary>
        /// Invokes the event
        /// </summary>
        /// <param name="arg">The arg to pass to event listeners.</param>
        public void Invoke(T arg)
        {
            Latest = arg;
            _event?.Invoke(arg);
        }
        
        /// <summary>
        /// Overload of the + operator for convenient subscription.
        /// </summary>
        /// <param name="gameEvent">'this' GameEvent</param>
        /// <param name="action">The action to subscribe to it.</param>
        /// <returns>'this' GameEvent with the subscribed action in its call list.</returns>
        public static GameEvent<T> operator +(GameEvent<T> gameEvent, Action<T> action)
        {
            gameEvent.Subscribe(action);
            return gameEvent;
        }

        /// <summary>
        /// Overload of the - operator for convenient unsubscription.
        /// </summary>
        /// <param name="gameEvent">'this' GameEvent</param>
        /// <param name="action">The action to unsubscribe from it.</param>
        /// <returns>'this' GameEvent with the unsubscribed action removed from its call list.</returns>
        public static GameEvent<T> operator -(GameEvent<T> gameEvent, Action<T> action)
        {
            gameEvent.Unsubscribe(action);
            return gameEvent;
        }
    }
}


