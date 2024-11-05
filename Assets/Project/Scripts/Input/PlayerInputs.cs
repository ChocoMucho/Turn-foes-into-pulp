using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInputs : MonoBehaviour // �Է� ������ �޾Ƴ��� �ٱ��� ����
{
    [Header("ĳ���� ��ǲ ����")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool aim;
    public bool shoot;
    public bool dodge;
    public bool reload;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("���콺 Ŀ�� ����")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    // �б� ���ϰ� ���ٽ� ���

    // receipt value
    public void OnMove(InputValue value) => MoveInput(value.Get<Vector2>());
    public void OnLook(InputValue value)
    {
        if (cursorInputForLook) 
            LookInput(value.Get<Vector2>());
    }
    public void OnJump(InputValue value) => JumpInput(value.isPressed);
    public void OnAim(InputValue value) => AimInput(value.isPressed);
    public void OnShoot(InputValue value) => ShootInput(value.isPressed);
    public void OnReload(InputValue value) => ReloadInput(value.isPressed);
    public void OnDodge(InputValue value) => DodgeInput(value.isPressed);
    private void OnApplicationFocus(bool hasFocus)=> SetCursorState(cursorLocked);

    // restore value
    public void MoveInput(Vector2 moveDirection) => move = moveDirection;
    public void LookInput(Vector2 lookDirection) => look = lookDirection;
    private void JumpInput(bool isPressed) => jump = isPressed;
    private void AimInput(bool isPressed) => aim = isPressed;
    private void ShootInput(bool isPressed) => shoot = isPressed;
    private void ReloadInput(bool isPressed) => reload = isPressed;
    private void DodgeInput(bool isPressed) => dodge = isPressed;
    private void SetCursorState(bool newState) => Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
}
