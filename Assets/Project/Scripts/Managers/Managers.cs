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

    static Managers Init() // �ٸ� ���� Awake���� �ν��Ͻ� Get ��û�ص� Initȣ�� �� ����
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