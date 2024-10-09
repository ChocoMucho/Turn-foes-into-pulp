using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    protected bool _isDestroyOnLoad = false;
    protected static T _instance;
    
    public static T Instance {  get { return _instance; } }

    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        if(_instance == null)
        {
            _instance = (T)this; // ĳ������ �ּ��ΰ� ����� �غ��� ��..
            
            if (!_isDestroyOnLoad) 
                DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        Dispose();
    }

    protected virtual void Dispose()
    {
        _instance = null;
    }
}
