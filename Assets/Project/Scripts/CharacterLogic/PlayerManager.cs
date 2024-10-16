using Cinemachine;
using StarterAssets;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Windows;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerManager : MonoBehaviour, IBattle
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

    private PlayerInputs _inputs;
    private PlayerController _controller;
    private Animator _animator;
    private Vector3 targetPosition;

    // 상태
    private bool _isDead = false;
    public bool IsDead { get { return _isDead; } }

    // 데이터
    private float _currentHp; // max값은 나중에 생각하기 귀찮음.
    private float _currentDamage;

    public float Damage { get { return _currentDamage; } }

    GameManager _gameManager;

    private void Awake()
    {
        _inputs = GetComponent<PlayerInputs>();
        _controller = GetComponent<PlayerController>();
        _controller.playerManager = this;
        _animator = GetComponent<Animator>();

        _gameManager = GameManager.instance;
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
            _animator.SetBool("Death", true);
        }

        return _isDead;
    }

    private void AimCheck()
    {             
        if(_inputs.reload)
        {
            _inputs.reload = false;
            if (_controller.IsReload) //TODO: 장전가능한지 체크하는 메서드로 묶기
                return;

            AimControll(false);
            //rigging
            SetRigWeight(0);
            _animator.SetLayerWeight(1, 1); //TODO: 부드럽게 레이어 바꾸는 법..
            _animator.SetTrigger("Reload");
            _controller.IsReload = true;
        }

        if (_controller.IsReload)
            return;

        // 조준 간단 구현
        if (_inputs.aim) // Yes Aiming
        {
            AimControll(true);
            _controller.SetSensitivity(zoomInSensitivity);

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

            _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

            //rigging
            SetRigWeight(1);

            // TODO 이런 코드들 컨트롤러 부분으로 빼버리기, 여기서는 입력 확인과 메서드 호출 위주로
            if (_inputs.shoot) // 발사 중인가 
            {
                if(shootTimeout < shootTimeoutDelta)
                {
                    shootTimeoutDelta = 0f;
                    _animator.SetTrigger("Shoot");
                }
                
            }            
        }
        else // No Aiming
        {
            AimControll(false);
            _controller.SetSensitivity(zoomOutSensitivity);

            _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));

            _animator.SetBool("Shoot", false);

            //rigging
            SetRigWeight(0);
        }
    }

    private void AimControll(bool isAim) //TODO 얘도 컨트롤러로 빼기
    {
        _controller.IsAim = isAim;
        playerAimCamera.gameObject.SetActive(isAim);
        crosshair.SetActive(isAim);
    }

    public void Reload() //리로드 애니메이션 끝나갈 때 호출할 리셋 함수
    {
        _controller.IsReload = false;
        _animator.SetLayerWeight(1, 0);
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
