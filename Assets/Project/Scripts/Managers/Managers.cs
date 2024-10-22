using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers _instance;
    public static Managers Instance { get { return Init(); } }

    private SceneManagerExtend _scene = new SceneManagerExtend();

    public static SceneManagerExtend Scene { get { return Instance._scene; } }

    void Start() 
    {
        Init();
    }

    void Update()
    {
        
    }

    static Managers Init() // 다른 곳의 Awake에서 인스턴스 Get 요청해도 Init호출 될 것임
    {
        if(null == _instance)
        {
            GameObject go = GameObject.Find("@Managers");
            if(null == go)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go);
            _instance = go.GetComponent<Managers>();
        }

        return _instance;
    }
}