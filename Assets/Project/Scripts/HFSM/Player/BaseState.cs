using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState : IState
{
    protected bool IsRootState { get; set; } = false;
    protected PlayerStateMachine StateMachine { get; private set; }
    protected PlayerStateProvider Provider { get; private set; }
    protected Player Player { get; private set; }
    protected BaseState _currentSuperState;
    protected BaseState _currentSubState;
    public BaseState(PlayerStateMachine stateMachine, PlayerStateProvider stateProvider)
    {
        StateMachine = stateMachine;
        Provider = stateProvider;
        Player = stateMachine.Player;
    }
    public virtual void EnterState() { } // TODO: virtual로 할지 abstract로 할지는 나중에..
    public virtual void UpdateState() { }
    public virtual void ExitState() { }
    public virtual void CheckSwitchState() { }
    public virtual void InitializeSubState() { }

    public void UpdateStates()
    {
        UpdateState();
        if (_currentSubState != null)
            _currentSubState.UpdateStates();
    }

    protected void SwitchState(BaseState newState)
    {
        ExitState();
        if (IsRootState || null != _currentSubState)//최상위상태 and 서브상태가 있음
        {
            _currentSubState?.ExitState(); // 서브상태 exit
            _currentSubState?._currentSubState?.ExitState(); // 서브서브상태 exit
        }

        newState.EnterState();

        if (IsRootState) // 루트라면(Grounded나 Fall이라면) 루트끼리 바뀌게
        {
            StateMachine.CurrentState = newState;
        }
        else if (_currentSuperState != null) // 상위상태가 있음 -> 나는 서브 or 서브서브
        {
            _currentSuperState.SetSubState(newState); // 상위 상태의 서브를 매개변수로 교체
        }
    }

    public void SetSuperState(BaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    public void SetSubState(BaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}