using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerStateProvider
{
    private PlayerStateMachine stateMachine;

    private Dictionary<Enum, BaseState> _states = new Dictionary<Enum, BaseState>();

    public PlayerStateProvider(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;

        // TODO: ���µ� �����ϱ�
        SetState(PlayerStates.Ground, new PlayerState_Ground(stateMachine, this));
        SetState(PlayerStates.Air, new PlayerState_Air(stateMachine, this));

        SetState(PlayerStates.Move, new PlayerState_Move(stateMachine, this));
        SetState(PlayerStates.Dodge, new PlayerState_Dodge(stateMachine, this));

        SetState(PlayerStates.NonCombat, new PlayerState_NonCombat(stateMachine, this));
        SetState(PlayerStates.Shoot, new PlayerState_Shoot(stateMachine, this));
    }

    public BaseState GetState<PlayerStates>(PlayerStates stateEnum) where PlayerStates : Enum
    {
        if (_states.ContainsKey(stateEnum))
            return _states[stateEnum];
        else
            throw new InvalidOperationException("�߸��� ���� �䱸");
    }

    public void SetState<PlayerStates>(PlayerStates stateEnum, BaseState state) where PlayerStates : Enum
    {
        if (_states.ContainsKey(stateEnum))
            return;

        _states[stateEnum] = state;
    }
}
