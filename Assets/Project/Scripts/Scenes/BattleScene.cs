using System.Collections;
using System.Collections.Generic;
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

        _lobbybutton.onClick.AddListener(() =>
        {
            if (SceneManagerExtend.Instance != null)
                SceneManagerExtend.Instance.LoadScene(Define.Scene.Lobby);
            else
                Debug.LogError("�� �Ŵ���(Ȯ��)�� �������.");
        } );
        _battlebutton.onClick.AddListener(() => SceneManagerExtend.Instance.LoadScene(Define.Scene.Battle));
    }

    public override void Clear()
    {
    }
}
