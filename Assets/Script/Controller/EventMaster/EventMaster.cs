using System;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-10)]
public class EventMaster : Singleton<EventMaster>
{
    Dictionary<string, UnityEvent> _eventDictionary = new Dictionary<string, UnityEvent>();

    public UnityEvent GetEvent(string name)
    {
        if (_eventDictionary.ContainsKey(name))
        {
            return _eventDictionary[name];
        }

        Debug.Log("This event doesn't exist : " + name);
        return null;
    }

    public void CreateNewEvent(string name)
    {
        if (_eventDictionary.ContainsKey(name))
        {
            Debug.Log("This event already exist !" + name);
        }
        else
        {
            UnityEvent newEvent = new UnityEvent();
            _eventDictionary.Add(name, newEvent);
        }

    }

    public void InvokeEvent(string name)
    {

        if (_eventDictionary.ContainsKey(name))
        {
            _eventDictionary[name].Invoke();
        }

    }
}
[DefaultExecutionOrder(-11)]
public class EventMaster<T> : MonoBehaviour
{

    Dictionary<string, UnityEvent<T>> _eventDictionary = new Dictionary<string, UnityEvent<T>>();


    public static EventMaster<T> Instance { get; private set; }
    void Awake()
    {
        if (Instance != null)
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