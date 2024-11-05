using Cinemachine;
using StarterAssets;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Windows;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : MonoBehaviour, IBattle
{
    [SerializeField] private CinemachineVirtualCamera playerAimCamera;
    [SerializeField] private float zoomOutSensitivity;
    [SerializeField] private float zoomInSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform aimObj;
    [SerializeField] private float aimObjDistance = 20;
    [SerializeField] private Transform preProjectile;
    [SerializeField] private Transform projectileSpawnPosition;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private Rig rigHandLayer;
    [SerializeField] private Rig rigSpineLayer;
    [SerializeField] float shootTimeout = 0.3f;
    [SerializeField] private EntityData _data;
    [SerializeField] private GameObject _mob; // 이거 지워야 됨

    private float shootTimeoutDelta;

    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerInputs Inputs { get; private set; }
    public PlayerController Controller { get; private set; }
    public Animator Animator { get; private set; }
    public PlayerAnimationHash AnimationHash { get; private set; }
    private Vector3 targetPosition;

    // 상태
    private bool _isDead = false;
    public bool IsDead { get { return _isDead; } }

    // 데이터
    private float _currentHp; // max값은 나중에 생각하기 귀찮음.
    private float _currentDamage;

    public float Damage { get { return _currentDamage; } }


    private void Awake()
    {
        Inputs = gameObject.GetOrAddComponent<PlayerInputs>();
        Controller = gameObject.GetOrAddComponent<PlayerController>();
        Animator = GetComponent<Animator>();
        AnimationHash = new PlayerAnimationHash();
        StateMachine = gameObject.GetOrAddComponent<PlayerStateMachine>();
    }

    private void Start()
    {
        shootTimeoutDelta = shootTimeout;
        _currentHp = _data.Hp;
        _currentDamage = _data.Damage;
        Debug.Log($"체력 : {_data.Hp}");
        Debug.Log($"공격력 : {_data.Damage}");
    }

    void Update()
    {
        if (DeathCheck())
            return;
        AimCheck();

        shootTimeoutDelta += Time.deltaTime;
    }

    private bool DeathCheck()
    {
        if(_isDead) return true;

        if (_currentHp <= 0f)
        {
            _isDead = true;
            Animator.SetBool("Death", true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        return _isDead;
    }

    private void AimCheck()
    {             
        if(Inputs.reload)
        {
            Inputs.reload = false;
            if (Controller.IsReload) //TODO: 장전가능한지 체크하는 메서드로 묶기
                return;

            AimControll(false);
            //rigging
            SetRigWeight(0);
            Animator.SetLayerWeight(1, 1); //TODO: 부드럽게 레이어 바꾸는 법..
            Animator.SetTrigger("Reload");
            Controller.IsReload = true;
        }

        if (Controller.IsReload)
            return;

        // 조준 간단 구현
        if (Inputs.aim) // Yes Aiming
        {
            AimControll(true);
            Controller.SetSensitivity(zoomInSensitivity);

            Transform camTransform = Camera.main.transform;
            RaycastHit hit;
            if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, Mathf.Infinity, aimColliderLayerMask))
            {
                targetPosition = hit.point;
                aimObj.position = hit.point;
            }
            else
            {
                targetPosition = camTransform.position + camTransform.forward * aimObjDistance;
                aimObj.position = targetPosition;
            }


            Vector3 targetAim = targetPosition;
            targetAim.y = transform.position.y; // 조준 된 타겟과 캐릭터랑 높이 맞추기
            Vector3 aimDirection = (targetAim - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);

            Animator.SetLayerWeight(1, Mathf.Lerp(Animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

            //rigging
            SetRigWeight(1);

            // TODO 이런 코드들 컨트롤러 부분으로 빼버리기, 여기서는 입력 확인과 메서드 호출 위주로
            if (Inputs.shoot) // 발사 중인가 
            {
                if(shootTimeout < shootTimeoutDelta)
                {
                    shootTimeoutDelta = 0f;
                    Animator.SetTrigger("Shoot");
                }
                
            }            
        }
        else // No Aiming
        {
            AimControll(false);
            Controller.SetSensitivity(zoomOutSensitivity);

            Animator.SetLayerWeight(1, Mathf.Lerp(Animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));

            Animator.SetBool("Shoot", false);

            //rigging
            SetRigWeight(0);
        }
    }

    private void AimControll(bool isAim) //TODO 얘도 컨트롤러로 빼기
    {
        Controller.IsAim = isAim;
        playerAimCamera.gameObject.SetActive(isAim);
        crosshair.SetActive(isAim);
    }

    public void Reload() //리로드 애니메이션 끝나갈 때 호출할 리셋 함수
    {
        Controller.IsReload = false;
        Animator.SetLayerWeight(1, 0);
        Debug.Log("Reload");

        //rigging
        SetRigWeight(1);
    }

    private void SetRigWeight(float value)
    {
        rigHandLayer.weight = value;
        rigSpineLayer.weight = value;
    }

    public void OnFire()
    {
        Debug.Log("Shoot Event");
        GameManager.instance.Shoot(aimObj.position);
    }

    public void OnAttack()
    {
        _mob.GetComponent<NormalMobContoller>().OnDamage(_data.Damage); // 인터페이스 어쩌구로 받아오는 방법 있지 않았나..
    }

    public void OnDamage(float damage)
    {
        _currentHp -= damage; 
        Debug.Log($"플레이어 체력 : {_currentHp}");

        GameManager.instance.TakeDamageEffect();
    }
}
