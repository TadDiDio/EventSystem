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
        #region SINGLETON
        private static Events _instance;

        // Delete the constructor to prevent duplicates
        private Events() { }

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
        #endregion
        
        #region INSTANCES
        
        /*
         * Add all instances of container classes here to be accessed throughout the project.
         * It's probably best to declare containers, types, and events in their own files within their
         * namespaces.
         */
        
        #endregion
    }
}