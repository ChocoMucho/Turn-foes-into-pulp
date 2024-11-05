using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Move : BaseState
{
    public PlayerState_Move(PlayerStateMachine stateMachine, PlayerStateProvider stateProvider) : base(stateMachine, stateProvider)
    {
    }

    public override void EnterState() 
    {
        InitializeSubState();
        Debug.Log("무브 상태 진입");
    }
    public override void UpdateState() 
    {
        CheckSwitchState();
    }
    public override void ExitState() { }
    public override void CheckSwitchState() 
    {
        if(Player.Controller.IsDodging)
            SwitchState(Provider.GetState(Define.PlayerStates.Dodge));
    }
    public override void InitializeSubState() { }    
}
