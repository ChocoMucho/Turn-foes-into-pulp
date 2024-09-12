using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera playerAimCamera;
    [SerializeField] private float zoomOutSensitivity;
    [SerializeField] private float zoomInSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform aimDebugTransform;
    [SerializeField] private Transform preProjectile;
    [SerializeField] private Transform projectileSpawnPosition;

    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
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
        if (starterAssetsInputs.aim)
        {
            playerAimCamera.gameObject.SetActive(true);            
            thirdPersonController.SetSensitivity(zoomInSensitivity);

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y; // ĳ���Ͷ� ���� ���߱�
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);

            thirdPersonController.SetRotateOnMove(false);
        }
        else
        {
            playerAimCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(zoomOutSensitivity);

            thirdPersonController.SetRotateOnMove(true);
        }

        if(starterAssetsInputs.fire)
        {
            Vector3 vector3 = (mouseWorldPosition - projectileSpawnPosition.position).normalized;
            Instantiate(preProjectile, projectileSpawnPosition.position, Quaternion.LookRotation(vector3, Vector3.up));
            starterAssetsInputs.fire = false; // ����
        }
    }
}
