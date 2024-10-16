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
    [SerializeField] private GameObject _mob; // �̰� ������ ��

    private float shootTimeoutDelta;

    private PlayerInputs _inputs;
    private PlayerController _controller;
    private Animator _animator;
    private Vector3 targetPosition;

    // ����
    private bool _isDead = false;
    public bool IsDead { get { return _isDead; } }

    // ������
    private float _currentHp; // max���� ���߿� �����ϱ� ������.
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
        Debug.Log($"ü�� : {_data.Hp}");
        Debug.Log($"���ݷ� : {_data.Damage}");
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
            if (_controller.IsReload) //TODO: ������������ üũ�ϴ� �޼���� ����
                return;

            AimControll(false);
            //rigging
            SetRigWeight(0);
            _animator.SetLayerWeight(1, 1); //TODO: �ε巴�� ���̾� �ٲٴ� ��..
            _animator.SetTrigger("Reload");
            _controller.IsReload = true;
        }

        if (_controller.IsReload)
            return;

        // ���� ���� ����
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
            targetAim.y = transform.position.y; // ���� �� Ÿ�ٰ� ĳ���Ͷ� ���� ���߱�
            Vector3 aimDirection = (targetAim - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);

            _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

            //rigging
            SetRigWeight(1);

            // TODO �̷� �ڵ�� ��Ʈ�ѷ� �κ����� ��������, ���⼭�� �Է� Ȯ�ΰ� �޼��� ȣ�� ���ַ�
            if (_inputs.shoot) // �߻� ���ΰ� 
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

    private void AimControll(bool isAim) //TODO �굵 ��Ʈ�ѷ��� ����
    {
        _controller.IsAim = isAim;
        playerAimCamera.gameObject.SetActive(isAim);
        crosshair.SetActive(isAim);
    }

    public void Reload() //���ε� �ִϸ��̼� ������ �� ȣ���� ���� �Լ�
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
        _mob.GetComponent<NormalMobContoller>().OnDamage(_data.Damage); // �������̽� ��¼���� �޾ƿ��� ��� ���� �ʾҳ�..
    }

    public void OnDamage(float damage)
    {
        _currentHp -= damage; 
        Debug.Log($"�÷��̾� ü�� : {_currentHp}");

        GameManager.instance.TakeDamageEffect();
    }
}
