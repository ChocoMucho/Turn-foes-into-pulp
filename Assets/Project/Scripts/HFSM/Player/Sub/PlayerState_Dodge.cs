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
        Debug.Log("닷지 상태 진입");
    }
    public override void UpdateState()
    {
        if(!StateMachine.Animator.GetBool(Player.AnimationHash.Dodge))
            StateMachine.Animator.SetBool(Player.AnimationHash.Dodge, true);
        CheckSwitchState();
    }
    public override void ExitState() 
    {
        Debug.Log("닷지 상태 탈출");
        if(!Player.Controller.IsDodging) // 상위 상태가 바뀌어도 Exit 호출돼서 필요함
            StateMachine.Animator.SetBool(Player.AnimationHash.Dodge, false);
    }
    public override void CheckSwitchState() 
    {
        if (!Player.Controller.IsDodging)
            SwitchState(Provider.GetState(Define.PlayerStates.Move));
    }
    public override void InitializeSubState() { }
}
