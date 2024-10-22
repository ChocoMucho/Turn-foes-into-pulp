using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    protected Managers manager;

    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;    

    private void Start()
    {
        manager = Managers.Instance;
        Init();
    }

    protected virtual void Init()
    {
        if(null == GameObject.FindObjectOfType(typeof(EventSystem)))
            SetEventSystem();
    }

    public abstract void Clear();

    private void SetEventSystem() // ���ǿ����� ���� EventSystem�� ���������� ���� ������ ��.
    {
        GameObject eventSystem = new GameObject("@EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }
}
