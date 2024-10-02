using StarterAssets;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
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
    private float _verticalVelocity; // 수직 속도 -> 점프에서 활용
    private float _terminalVelocity = 53.0f;
    // 프로퍼티
    public bool IsAim { get; set; } = false;
    public bool IsReload { get; set; } = false;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    // animation IDs
    private int _animIDSpeed; // 블렌드 트리의 블렌드 값
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed; //블렌드 트리의 멀티플라이어 값으로 쓰임

    private PlayerInput _playerInput; //큰 의미X 애니 속도 체크
    private Animator _animator;
    private CharacterController _controller;
    private PlayerInputs _input;
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
    private void Awake()
    {
        if (_mainCamera == null)
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }
    void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputs>();
        _playerInput = GetComponent<PlayerInput>();

        AssignAnimationIDs();

        // 점프 관련 수치 리셋
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    void Update()
    {
        _hasAnimator = TryGetComponent(out _animator);

        JumpAndGravity(); // 수직 이동 값이 여기서 나와야 Move에서 사용할 수 있음.
        GroundedCheck();
        Move();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }    

    private void AssignAnimationIDs() // 애니메이션 id를 string에서int로
    {
        _animIDSpeed = Animator.StringToHash("Speed"); // 블렌드 트리에 쓰이는 값
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("Fall");
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

    private void Move()
    {
        // 달리는 중이면 RunSpeed, 아니면 MoveSpeed
        float targetSpeed = 0;

        // 조준 중일 때 타겟 스피드 movespeed
        if (IsAim || IsReload)
            targetSpeed = MoveSpeed;
        else
            targetSpeed = _input.run ? RunSpeed : MoveSpeed;

        // 입력 없음 목표 속도 == 0;
        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        // 수평 속도(벡터 길이) 계산
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.move.magnitude;

        // 지금 수평 속도가 목표속도-0.1~목표속도+0.1 벗어나면 
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // (목표속도*인풋크기)로 Lerp시킴
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            // 1000.123f => 1000.0f
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else // 지금 수평 속도가 목표속도-0.1~목표속도+0.1 안에 들어오면 속도 그냥 목표속도로 고정 
        {
            _speed = targetSpeed;
        }

        // 애니메이션 블렌드 값 조절
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f; // 블렌드 값은 아무리 높아도 상관 없음

        // 입력 방향 정규화
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        if (_input.move != Vector2.zero)
        {
            // x와 z방향 값으로 y축 회전 값을 구해냄 | 카메라 방향을 더해서 카메라 방향 기준 회전 방향 구함
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;
            // 부드러운 회전을 위한 값을 구함
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // 최종 회전
            if (!IsAim)
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        // 오일러 각도에 forward를 곱하면 방향이 나오는구나..?? 충격
        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // 수평 이동 값 + 수직 이동 값
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                         new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        // 블렌드 값으로 
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend); //TODO 블렌드는 그냥 blend라고 명시하는게 나을듯
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude); 
        }
    }

    private void JumpAndGravity()
    {
        if (Grounded)
        {
            // 낙하 상태 돌입까지의 시간 대입
            _fallTimeoutDelta = FallTimeout;

            // 점프, 낙하 애니메이션 비활성화
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }

            // 수직 속도 -2로 해서 땅에 붙어있게 함
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // 점프 로직
            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // 수직 속도 계산
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                // 점프 애니메이션 활성화
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                }
            }

            // 점프 타이머 활성화
            // 지면에 착지 후에 일정 시간 흐른 후에 점프가 가능하도록 함
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // 점프 타이머 재설정
            _jumpTimeoutDelta = JumpTimeout;

            // _fallTimeoutDelta이 0보다 작아지면 낙하 애니메이션 활성화
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }
            }

            // 공중에 있을 때 불필요한 점프 방지
            _input.jump = false;
        }

        // 수직 속도가 최대 속도보다 작은 경우
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime; // 수직 속도 프레임마다 중력만큼 감소 
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        // Mathf.Clamp로 안전빵 값을 정해줌. 사실 필요 없는데 그냥 넣어준 것 같음.
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }


    private void OnDrawGizmosSelected() // 땅에 있을 땐 초록 / 아니면 빨강
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
    }

    public void SetSensitivity(float newSensitivity) => Sensitivity = newSensitivity;
}
