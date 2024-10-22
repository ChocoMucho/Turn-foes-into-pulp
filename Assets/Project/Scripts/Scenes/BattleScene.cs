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

        _lobbybutton.onClick.AddListener(() => Managers.Scene.LoadScene(Define.Scene.Lobby));
        _battlebutton.onClick.AddListener(() => Managers.Scene.LoadScene(Define.Scene.Battle));
    }

    public override void Clear()
    {
    }
}
