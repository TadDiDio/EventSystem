using UnityEngine;

public class GameEvents : MonoBehaviour
{
    /// <summary>
    /// A singleton giving access to all game events.
    /// </summary>
    public static GameEvents Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    /*
     * The expected use would be to define classes below as containers, then instantiate a single 
     * instance as a public field in this GameEvents class so that it is accessible from the 
     * singleton instance.
     * 
     * public class InputEventsContainer()
     * {
     *     public GameEvent Jump = new();
     * }
     * 
     * public InputEventsContainer InputEvents { get; private set; } = new InputEvents();
     */
}
