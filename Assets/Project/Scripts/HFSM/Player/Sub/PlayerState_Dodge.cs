using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Dodge : BaseState
{
    public PlayerState_Dodge(PlayerStateMachine stateMachine, PlayerStateProvider stateProvider) : base(stateMachine, stateProvider)
    {
    }

    public override void EnterState() 
    {
        InitializeSubState();
        Debug.Log("���� ���� ����");
    }
    public override void UpdateState()
    {
        if(!StateMachine.Animator.GetBool(Player.AnimationHash.Dodge))
            StateMachine.Animator.SetBool(Player.AnimationHash.Dodge, true);
        CheckSwitchState();
    }
    public override void ExitState() 
    {
        Debug.Log("���� ���� Ż��");
        if(!Player.Controller.IsDodging) // ���� ���°� �ٲ� Exit ȣ��ż� �ʿ���
            StateMachine.Animator.SetBool(Player.AnimationHash.Dodge, false);
    }
    public override void CheckSwitchState() 
    {
        if (!Player.Controller.IsDodging)
            SwitchState(Provider.GetState(Define.PlayerStates.Move));
    }
    public override void InitializeSubState() { }
}
