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

    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
    }

    void Update()
    {
        // ���� ���� ����
        if(starterAssetsInputs.aim)
        {
            playerAimCamera.gameObject.SetActive(true);            
            thirdPersonController.SetSensitivity(zoomInSensitivity);
        }
        else
        {
            playerAimCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(zoomOutSensitivity);
        }

        // ȭ�� �߾����� ���� ���
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2); // ȭ�� �߾� ��
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if(Physics.Raycast(ray, out RaycastHit hitInfo, 9999f, aimColliderLayerMask))
        {
            aimDebugTransform.position = hitInfo.point;
        }
    }
}
