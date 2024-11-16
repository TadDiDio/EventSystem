using UnityEngine;

public class RomanceManager : MonoBehaviour
{
    private void Awake()
    {
        Invoke("TestLatest", 2);
    }

    private void Update()
    {
        CuddleData cuddleData = new CuddleData
        {
            duration = Time.time
        };
        GameEvents.Instance.Timeline.CutScene.Romance.Cuddle.Invoke(cuddleData);
    }
    private void TestLatest()
    {
        print($"Latest value of duration was {GameEvents.Instance.Timeline.CutScene.Romance.Cuddle.Latest.duration}");
    }
}

public struct CuddleData
{
    public float duration;
}