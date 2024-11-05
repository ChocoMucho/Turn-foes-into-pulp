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
    public virtual void EnterState() { } // TODO: virtual�� ���� abstract�� ������ ���߿�..
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
        if (IsRootState || null != _currentSubState)//�ֻ������� and ������°� ����
        {
            _currentSubState?.ExitState(); // ������� exit
            _currentSubState?._currentSubState?.ExitState(); // ���꼭����� exit
        }

        newState.EnterState();

        if (IsRootState) // ��Ʈ���(Grounded�� Fall�̶��) ��Ʈ���� �ٲ��
        {
            StateMachine.CurrentState = newState;
        }
        else if (_currentSuperState != null) // �������°� ���� -> ���� ���� or ���꼭��
        {
            _currentSuperState.SetSubState(newState); // ���� ������ ���긦 �Ű������� ��ü
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