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
    private PlayerController controller;
    private Animator animator;
    private Vector3 targetPosition;

    // ������
    private float _currentHp; // max���� ���߿� �����ϱ� ������.
    private float _currentDamage;

    public float Damage { get { return _currentDamage; } }

    GameManager _gameManager;

    private void Awake()
    {
        _inputs = GetComponent<PlayerInputs>();
        controller = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();

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
        AimCheck();

        shootTimeoutDelta += Time.deltaTime;

        /*if (demoCharacterInputs.shoot)
        {
            Vector3 vector3 = (mouseWorldPosition - projectileSpawnPosition.position).normalized;
            Instantiate(preProjectile, projectileSpawnPosition.position, Quaternion.LookRotation(vector3, Vector3.up));
            demoCharacterInputs.shoot = false; // ����
        }*/
    }

    private void AimCheck()
    {             
        if(_inputs.reload)
        {
            _inputs.reload = false;
            if (controller.IsReload) //TODO: ������������ üũ�ϴ� �޼���� ����
                return;

            AimControll(false);
            //rigging
            SetRigWeight(0);
            animator.SetLayerWeight(1, 1); //TODO: �ε巴�� ���̾� �ٲٴ� ��..
            animator.SetTrigger("Reload");
            controller.IsReload = true;
        }

        if (controller.IsReload)
            return;

        // ���� ���� ����
        if (_inputs.aim) // Yes Aiming
        {
            AimControll(true);
            controller.SetSensitivity(zoomInSensitivity);

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

            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

            //rigging
            SetRigWeight(1);

            // TODO �̷� �ڵ�� ��Ʈ�ѷ� �κ����� ��������, ���⼭�� �Է� Ȯ�ΰ� �޼��� ȣ�� ���ַ�
            if (_inputs.shoot) // �߻� ���ΰ� 
            {
                if(shootTimeout < shootTimeoutDelta)
                {
                    shootTimeoutDelta = 0f;
                    animator.SetTrigger("Shoot");
                }
                
            }            
        }
        else // No Aiming
        {
            AimControll(false);
            controller.SetSensitivity(zoomOutSensitivity);

            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));

            animator.SetBool("Shoot", false);

            //rigging
            SetRigWeight(0);
        }
    }

    private void AimControll(bool isAim) //TODO �굵 ��Ʈ�ѷ��� ����
    {
        controller.IsAim = isAim;
        playerAimCamera.gameObject.SetActive(isAim);
        crosshair.SetActive(isAim);
    }

    public void Reload() //���ε� �ִϸ��̼� ������ �� ȣ���� ���� �Լ�
    {
        controller.IsReload = false;
        animator.SetLayerWeight(1, 0);
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
    }
}
