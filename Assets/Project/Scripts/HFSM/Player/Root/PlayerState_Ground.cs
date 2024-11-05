using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.Windows;

public class PlayerState_Ground : BaseState
{
    public PlayerState_Ground(PlayerStateMachine stateMachine, PlayerStateProvider stateProvider) : base(stateMachine, stateProvider)
    {
        IsRootState = true;
    }

    public override void EnterState() 
    {
        InitializeSubState();
        
        StateMachine.Animator.SetBool(Player.AnimationHash.Grounded, true);
    }
    
    public override void UpdateState() 
    {
        if (Player.Inputs.jump && Player.Controller.JumpTimeoutDelta <= 0.0f)
        {
            // 점프 애니메이션 활성화
            StateMachine.Animator.SetBool(Player.AnimationHash.Jump, true);
        }

        CheckSwitchState();
    }
    
    public override void ExitState() 
    {
        StateMachine.Animator.SetBool(Player.AnimationHash.Grounded, false);
    }
    
    public override void CheckSwitchState() 
    {
        if (!Player.Controller.IsGround)
            SwitchState(Provider.GetState(Define.PlayerStates.Air));
    }

    public override void InitializeSubState() 
    {
        if(!Player.Controller.IsDodging)
            SetSubState(Provider.GetState(Define.PlayerStates.Move));
        else
            SetSubState(Provider.GetState(Define.PlayerStates.Dodge));
    }
}
