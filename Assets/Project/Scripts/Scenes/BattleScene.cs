using UnityEngine;
using UnityEngine.UI;

public class BattleScene : BaseScene
{
    [SerializeField] private Button _lobbybutton;
    [SerializeField] private Button _battlebutton;

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Battle;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _lobbybutton.onClick.AddListener(() => SceneManagerExtend.Instance.LoadScene(Define.Scene.Lobby));
        _battlebutton.onClick.AddListener(() => SceneManagerExtend.Instance.LoadScene(Define.Scene.Battle));
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public override void Clear()
    {
    }
}
