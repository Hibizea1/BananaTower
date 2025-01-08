using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-11)]
public class EventMasterT<T> : MonoBehaviour 
{
    Dictionary<string, UnityEvent<T>> _eventDictionary = new Dictionary<string, UnityEvent<T>>();

    public static EventMasterT<T> Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public UnityEvent<T> GetEvent(string name)
    {
        if (_eventDictionary.ContainsKey(name))
        {
            return _eventDictionary[name];
        }

        Debug.Log("This event doesn't exist: " + name);
        return null;
    }

    public void CreateNewEvent(string name)
    {
        if (_eventDictionary.ContainsKey(name))
        {
            Debug.Log("This event already exists: " + name);
        }
        else
        {
            UnityEvent<T> newEvent = new UnityEvent<T>();
            _eventDictionary.Add(name, newEvent);
        }
    }

    public void InvokeEvent(string name, T arg)
    {
        if (_eventDictionary.ContainsKey(name))
        {
            _eventDictionary[name].Invoke(arg);
        }
    }
}