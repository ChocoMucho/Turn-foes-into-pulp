using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Air : BaseState
{

    public PlayerState_Air(PlayerStateMachine stateMachine, PlayerStateProvider stateProvider) : base(stateMachine, stateProvider)
    {
        IsRootState = true;
    }

    public override void EnterState() 
    {
        InitializeSubState();
    }
    public override void UpdateState() 
    {
        // _fallTimeoutDelta이 0보다 작아지면 낙하 애니메이션 활성화
        if (Player.Controller.FallTimeoutDelta >= 0.0f)
        {
            Player.Controller.FallTimeoutDelta -= Time.deltaTime;
        }
        else
        {
            StateMachine.Animator.SetBool(Player.AnimationHash.Fall, true);
        }

        CheckSwitchState();
    }
    public override void ExitState() 
    {
        // 점프, 낙하 애니메이션 비활성화
        StateMachine.Animator.SetBool(Player.AnimationHash.Jump, false);
        StateMachine.Animator.SetBool(Player.AnimationHash.Fall, false);
    }
    public override void CheckSwitchState() 
    {
        if (Player.Controller.IsGround)
            SwitchState(Provider.GetState(Define.PlayerStates.Ground));
    }
    public override void InitializeSubState() 
    {
        if (!Player.Controller.IsDodging)
            SetSubState(Provider.GetState(Define.PlayerStates.Move));
        else
            SetSubState(Provider.GetState(Define.PlayerStates.Dodge));
    }
}
