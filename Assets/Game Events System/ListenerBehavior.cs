using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

namespace EventSystem
{
    public abstract class ListenerBehavior : MonoBehaviour
    {
        private SubscriptionList subscriptions = new SubscriptionList();
        protected abstract void RegisterSubscriptions(GameEvents events, ref SubscriptionList subscriptions);

        protected virtual void OnEnabled() { }
        protected virtual void OnDisabled() { }
        private void OnEnable()
        {
            RegisterSubscriptions(GameEvents.Instance, ref subscriptions);

            int count = 0;
            foreach (var subscription in subscriptions)
            {
                count++;
                subscription.Key.Add(subscription.Value);
            }
            print(count + " subscriptions happened");
            OnEnabled();
        }
        private void OnDisable()
        {
            foreach (var subscription in subscriptions)
            {
                subscription.Key.Remove(subscription.Value);
            }

            OnDisabled();
        }
    }

    public class SubscriptionList : IEnumerable<KeyValuePair<GameEventBase, Delegate>>
    {
        private Dictionary<GameEventBase, Delegate> subscriptions = new Dictionary<GameEventBase, Delegate>();
        public IEnumerator<KeyValuePair<GameEventBase, Delegate>> GetEnumerator()
        {
            return subscriptions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(GameEventBase _event, UnityAction action)
        {
            subscriptions[_event] = action;
        }
        public void Add<T>(GameEventBase _event, UnityAction<T> action)
        {
            subscriptions[_event] = action;
        }
    }
}