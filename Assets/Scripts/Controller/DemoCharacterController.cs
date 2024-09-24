using StarterAssets;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class DemoCharacterController : MonoBehaviour
{
    [Header("플레이어 움직임 관련 값")]
    [Tooltip("이동 속도 m/s")]
    public float MoveSpeed = 2.0f;

    [Tooltip("뛰는 속도 m/s")]
    public float RunSpeed = 6.0f;

    [Tooltip("회전 속도: 클수록 방향 전환 시 더 느리게 회전함")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("속도가 빨라지는 정도")]
    public float SpeedChangeRate = 10.0f;

    [Tooltip("카메라 움직임 민감도")]
    public float Sensitivity = 1.0f;


    [Space(10)]
    [Tooltip("점프하는 높이")]
    public float JumpHeight = 1.2f;

    [Tooltip("캐릭터만의 고유 중력. 엔진 기본 값 -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("다시 점프할 수 있기까지 소요되는 시간")]
    public float JumpTimeout = 0.50f;

    [Tooltip("낙하 상태로 들어가기까지 소요되는 시간")]
    public float FallTimeout = 0.15f;

    [Header("플레이어 지면")]
    [Tooltip("캐릭터가 땅에 닿아 있는지. CharacterController와는 무관")]
    public bool Grounded = true;

    [Tooltip("거친 지형에 유용")]
    public float GroundedOffset = -0.14f;

    [Tooltip("접지 확인의 반지름, CharacterController의 반지름과 일치해야함")]
    public float GroundedRadius = 0.28f;

    [Tooltip("지면으로 사용할 레이어")]
    public LayerMask GroundLayers;

    [Header("시네머신")]
    [Tooltip("시네머신 가상 카메라에 설정된 추적 대상")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("카메라를 위로 올릴 수 있는 각도")]
    public float TopClamp = 70.0f;

    [Tooltip("카메라를 아래로 내릴 수 있는 각도")]
    public float BottomClamp = -30.0f;

    [Tooltip("카메라를 오버라이드하는 추가 각도. 잠길 때 카메라 위치를 미세 조정하는 데 유용합니다")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("모든 축에서 카메라 위치를 고정하기 위한 설정")]
    public bool LockCameraPosition = false;

    // cinemachine 
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    private bool _rotateOnMove = true;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    private PlayerInput _playerInput; //큰 의미X 애니 속도 체크
    private Animator _animator;
    private CharacterController _controller;
    private DemoCharacterInputs _input;
    private GameObject _mainCamera;

    private const float _threshold = 0.01f;

    private bool _hasAnimator;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }

    void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<DemoCharacterInputs>();
        _playerInput = GetComponent<PlayerInput>();

        AssignAnimationIDs();

        // 점프 관련 수치 리셋
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    void Update()
    {
        _hasAnimator = TryGetComponent(out _animator);

        JumpAndGravity();
        GroundedCheck();
        Move();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }    

    private void AssignAnimationIDs() // 애니메이션 id를 string에서int로
    {
        _animIDSpeed = Animator.StringToHash("Speed"); 
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void JumpAndGravity()
    {
        throw new NotImplementedException();
    }

    private void GroundedCheck()
    {
        // 캐릭터의 발 끝 위치를 [접지 체크 구체]의 중심으로 잡는다.
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        // [접지 체크 구체]가 땅에 닿는지 체크한다.
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        // 착지하는 애니메이션을 재생한다.
        if (_hasAnimator)
            _animator.SetBool(_animIDGrounded, Grounded);
    }

    private void CameraRotation()
    {
        // 카메라 이동 값이 _threshold 이상 and LockCameraPosition이 false일 때
        // => 카메라 이동중이며 안잠겨있을 때
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            // 마우스면 1로 고정, 아니면 델타타임 값
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            //yaw(좌우 회전) pitch(상하 회전)
            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * Sensitivity;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * Sensitivity;
        }

        // -360 ~ +360의 범위를 벗어나지 않게 조정해줌
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // 시네머신 목표 물체의 회전
        // x회전 값은 pitch, y회전 값은 yaw
        CinemachineCameraTarget.transform.rotation = 
            Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private void Move() //TODO
    {
        throw new NotImplementedException();
    }


    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        // Mathf.Clamp로 안전빵 값을 정해줌. 사실 필요 없는데 그냥 넣어준 것 같음.
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
