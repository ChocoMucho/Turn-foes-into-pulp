using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    protected bool _isDestroyOnLoad = false;
    protected static T _instance;
    
    public static T Instance 
    {
        get 
        { 
            if (_instance == null)
            {
                _instance = (T)FindAnyObjectByType<T>();

                if(_instance == null)
                {
                    GameObject SingletonObject = new GameObject();
                    _instance = SingletonObject.AddComponent<T>();
                    SingletonObject.name = "@" + typeof(T).Name;
                    DontDestroyOnLoad(SingletonObject);
                }
            }
            return _instance; 
        } 
    }

    public virtual void Init()
    {
    }

    protected virtual void OnDestroy()
    {
        if (_isDestroyOnLoad)
            Dispose();
    }

    protected virtual void Dispose()
    {
        _instance = null;
    }
}
