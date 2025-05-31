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

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogWarning("Multiple MessageBus instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        foreach (EventType type in Enum.GetValues(typeof(EventType)))
        {
            callbacks.Add(type, null);
        }
    }

    public enum EventType
    {
        CameraMove,
        CameraClick,
        MouseMove,
        ClickUnlock,
        CharacterDeath
    }

    public class Event
    {
        private object payload;

        public Event(object payload)
        {
            this.payload = payload;
        }

        public bool ReadPayload<T>(out T value)
        {
            if (payload == null || payload is not T)
            {
                value = default;
                return false;
            }

            value = (T)payload;
            return true;
        }

        public bool IsPayloadNull()
        {
            return payload is null;
        }
    }

    private readonly Dictionary<EventType, Action<Event>> callbacks = new();

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
