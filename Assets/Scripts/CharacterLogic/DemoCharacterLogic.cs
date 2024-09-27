using Cinemachine;
using StarterAssets;
using UnityEngine;

public class DemoCharacterLogic : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera playerAimCamera;
    [SerializeField] private float zoomOutSensitivity;
    [SerializeField] private float zoomInSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform aimDebugTransform;
    [SerializeField] private Transform preProjectile;
    [SerializeField] private Transform projectileSpawnPosition;

    private DemoCharacterInputs demoCharacterInputs;
    private DemoCharacterController demoCharacterController;
    private Animator animator;

    private void Awake()
    {
        demoCharacterInputs = GetComponent<DemoCharacterInputs>();
        demoCharacterController = GetComponent<DemoCharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        // ȭ�� �߾����� ���� ���
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2); // ȭ�� �߾� ��
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 9999f, aimColliderLayerMask))
        {
            aimDebugTransform.position = hitInfo.point;
            mouseWorldPosition = hitInfo.point;
        }

        // ���� ���� ����
        if (demoCharacterInputs.aim)
        {
            playerAimCamera.gameObject.SetActive(true);
            demoCharacterController.SetSensitivity(zoomInSensitivity);

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y; // ĳ���Ͷ� ���� ���߱�
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);

            demoCharacterController.SetRotateOnMove(false);

            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
        }
        else
        {
            playerAimCamera.gameObject.SetActive(false);
            demoCharacterController.SetSensitivity(zoomOutSensitivity);

            demoCharacterController.SetRotateOnMove(true);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        }

        /*if (demoCharacterInputs.shoot)
        {
            Vector3 vector3 = (mouseWorldPosition - projectileSpawnPosition.position).normalized;
            Instantiate(preProjectile, projectileSpawnPosition.position, Quaternion.LookRotation(vector3, Vector3.up));
            demoCharacterInputs.shoot = false; // ����
        }*/
    }
}
