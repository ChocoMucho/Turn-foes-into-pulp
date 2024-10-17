using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFootCollider : MonoBehaviour
{
    GolemController golemController;

    private void Awake()
    {
        golemController = GetComponentInParent<GolemController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("�÷��̾ ������ �߿� ��ҽ��ϴ�!");
            golemController.OnAttack();
        }
    }
}
