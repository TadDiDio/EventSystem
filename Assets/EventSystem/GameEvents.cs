using UnityEngine;

public class GameEvents : MonoBehaviour
{
    /// <summary>
    /// A singleton giving access to all game events.
    /// </summary>
    public static GameEvents Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

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
     * 1.    GameEvents.Instance.InputEvents.Move.AddListener(OnMove);
     * 2.    GameEvents.Instance.InputEvents.Move.Event += OnMove;
     */
}
