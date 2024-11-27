#region

using UnityEngine;

#endregion

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    static T instance;

    protected static bool DontDestroy = true;

    static bool m_applicationIsQuitting;

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            if (DontDestroy) DontDestroyOnLoad(gameObject);
        }
        else if (instance != this as T)
        {
            Destroy(gameObject);
        }
        else if (DontDestroy)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnApplicationQuit()
    {
        m_applicationIsQuitting = true;
    }

    public static T GetInstance()
    {
        if (m_applicationIsQuitting) return null;

        if (instance == null)
        {
            instance = FindObjectOfType<T>();
            if (instance == null)
            {
                var obj = new GameObject();
                obj.name = typeof(T).Name;
                instance = obj.AddComponent<T>();
            }
        }

        return instance;
    }
}