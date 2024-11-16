using UnityEngine.UIElements;

public class TestSubscriber : EventListener
{
    private void Awake()
    {
        Destroy(this, 3);
    }
    protected override void RegisterSubscriptions(GameEvents events, ref SubscriptionList subscriptions)
    {
        subscriptions.Add<CuddleData>(events.Timeline.CutScene.Romance.Cuddle, OnCuddle);
        subscriptions.Add(events.Input.Jump, OnJump);
    }

    protected override void OnEnabled()
    {
        GameEvents.Instance.Input.Jump.Add(OnManualJump);
    }
    protected override void OnDisabled()
    {
        GameEvents.Instance.Input.Jump.Remove(OnManualJump);
    }

    private void OnCuddle(CuddleData data)
    {
        print(data.ToString());
    }
    private void OnJump()
    {
        print("Jumped from auto subscribe");
    }

    private void OnManualJump()
    {
        print("Jumped from manual subscribe");
    }
}
