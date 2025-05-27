using System;
using System.Collections.Generic;
using UnityEngine;

public class MessageBus : MonoBehaviour
{
    private static MessageBus _instance;
    public static MessageBus Instance
    {
        get
        {
            if (_instance == null) Debug.LogError("Message Bus was not created in the scene.");
            return _instance;
        }
    }

    public enum EventType
    {
        CameraRaycastHitTilemapIdle,
    }

    public class Event
    {
        public object payload;

        public Event(object payload)
        {
            this.payload = payload;
        }
    }

    private readonly Dictionary<EventType, Action<Event>> callbacks = new();

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogWarning("Multiple MessageBus instances detected. Destroying duplicate.");
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            foreach (EventType type in Enum.GetValues(typeof(EventType)))
            {
                callbacks.Add(type, null);
            }
        }
    }

    public void Subscribe(EventType type, Action<Event> action)
    {
        callbacks[type] += action;
    }

    public void Publish(EventType type, object payload) 
    {
        if (!callbacks.TryGetValue(type, out var callback)) return;
        if (callback == null) return;
        callback.Invoke(new Event(payload));
    }
}
