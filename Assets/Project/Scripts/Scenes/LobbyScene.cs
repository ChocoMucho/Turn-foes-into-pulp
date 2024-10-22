using UnityEngine;
using UnityEngine.UI;

public class LobbyScene : BaseScene
{
    [SerializeField] private Button _battlebutton;

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Lobby;
        _battlebutton.onClick.AddListener(() => Managers.Scene.LoadScene(Define.Scene.Battle));
    }
    public override void Clear()
    {
    }
}
