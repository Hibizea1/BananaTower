using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-10)]
public class EventMaster : Singleton<EventMaster>
{
    Dictionary<string, UnityEvent> _eventDictionary = new Dictionary<string, UnityEvent>();
    Dictionary<string, UnityEvent<int>> _eventDictionaryInt = new Dictionary<string, UnityEvent<int>>();

    public UnityEvent GetEvent(string name)
    {
        if (_eventDictionary.ContainsKey(name))
        {
            return _eventDictionary[name];
        }

        Debug.Log("This event doesn't exist : " + name);
        return null;
    }

    public UnityEvent<int> GetEventInt(string name)
    {
        if (_eventDictionaryInt.ContainsKey(name))
        {
            return _eventDictionaryInt[name];
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

    public void CreateNewEventInt(string name)
    {
        if (_eventDictionaryInt.ContainsKey(name))
        {
            Debug.Log("This event already exist !" + name);
        }
        else
        {
            UnityEvent<int> newEvent = new UnityEvent<int>();
            _eventDictionaryInt.Add(name, newEvent);
        }
    }

    public void InvokeEvent(string name)
    {
        if (_eventDictionary.ContainsKey(name))
        {
            _eventDictionary[name].Invoke();
        }
    }

    public void InvokeEventInt(string name, int arg)
    {
        if (_eventDictionaryInt.ContainsKey(name))
        {
            _eventDictionaryInt[name].Invoke(arg);
        }
    }
}