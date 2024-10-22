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

    private void SetEventSystem() // 강의에서는 기존 EventSystem을 프리팹으로 만들어서 생성만 함.
    {
        GameObject eventSystem = new GameObject("@EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }
}
