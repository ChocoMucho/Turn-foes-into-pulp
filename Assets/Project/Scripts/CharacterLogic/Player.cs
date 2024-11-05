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
    [SerializeField] private GameObject _mob; // �̰� ������ ��

    private float shootTimeoutDelta;

    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerInputs Inputs { get; private set; }
    public PlayerController Controller { get; private set; }
    public Animator Animator { get; private set; }
    public PlayerAnimationHash AnimationHash { get; private set; }
    private Vector3 targetPosition;

    // ����
    private bool _isDead = false;
    public bool IsDead { get { return _isDead; } }

    // ������
    private float _currentHp; // max���� ���߿� �����ϱ� ������.
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
            if (Controller.IsReload) //TODO: ������������ üũ�ϴ� �޼���� ����
                return;

            AimControll(false);
            //rigging
            SetRigWeight(0);
            Animator.SetLayerWeight(1, 1); //TODO: �ε巴�� ���̾� �ٲٴ� ��..
            Animator.SetTrigger("Reload");
            Controller.IsReload = true;
        }

        if (Controller.IsReload)
            return;

        // ���� ���� ����
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
            targetAim.y = transform.position.y; // ���� �� Ÿ�ٰ� ĳ���Ͷ� ���� ���߱�
            Vector3 aimDirection = (targetAim - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);

            Animator.SetLayerWeight(1, Mathf.Lerp(Animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

            //rigging
            SetRigWeight(1);

            // TODO �̷� �ڵ�� ��Ʈ�ѷ� �κ����� ��������, ���⼭�� �Է� Ȯ�ΰ� �޼��� ȣ�� ���ַ�
            if (Inputs.shoot) // �߻� ���ΰ� 
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

    private void AimControll(bool isAim) //TODO �굵 ��Ʈ�ѷ��� ����
    {
        Controller.IsAim = isAim;
        playerAimCamera.gameObject.SetActive(isAim);
        crosshair.SetActive(isAim);
    }

    public void Reload() //���ε� �ִϸ��̼� ������ �� ȣ���� ���� �Լ�
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
        _mob.GetComponent<NormalMobContoller>().OnDamage(_data.Damage); // �������̽� ��¼���� �޾ƿ��� ��� ���� �ʾҳ�..
    }

    public void OnDamage(float damage)
    {
        _currentHp -= damage; 
        Debug.Log($"�÷��̾� ü�� : {_currentHp}");

        GameManager.instance.TakeDamageEffect();
    }
}
