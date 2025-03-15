#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameEvents
{
    /// <summary>
    /// A static middleware singleton container for events accessible from anywhere in the project.
    /// </summary>
    public class Events
    {
        private static Events _instance;
    
        /// <summary>
        /// A singleton property giving access to all game events.
        /// </summary>
        public static Events Instance => _instance ??= new Events();

#if UNITY_EDITOR
        /// <summary>
        /// Clears the singleton without relying on editor reloading the domain.
        /// </summary>
        static Events() => EditorApplication.playModeStateChanged += (state) =>
        {
            if (state == PlayModeStateChange.ExitingPlayMode) _instance = null;
        };
#endif

        /*
         * The expected use would be to define classes below as containers, then instantiate a single
         * instance as a public field in this GameEvents class so that it is accessible from the
         * singleton instance.
         *
         * public class InputEventsContainer
         * {
         *     public GameEvent<Vector2> Move = new();
         * }
         *
         * public InputEventsContainer InputEvents { get; private set; } = new InputEventsContainer();
         *
         * You can subscribe from anywhere in the project in the following ways:
         *
         * 1.    Events.Instance.InputEvents.Move.AddListener(OnMove);
         * 2.    Events.Instance.InputEvents.Move.Event += OnMove;
         */
    }
   
}