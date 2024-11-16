using UnityEngine;

public class ShitHead : MonoBehaviour
{
    private void Awake()
    {
        /* 
         * Events cache their most recent values so there
         * is no need to wait for a new event or obtain a 
         * reference to another object.
         */
        int timesShitWasEaten = GameEvents.Instance.Enemy.Shithead.AteShit.Latest;

        print($"Unfortunately, the shithead ate shit {timesShitWasEaten} times");
    }
}
