using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody rigidbody;
    float speed = 10;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rigidbody.velocity = transform.forward * speed;
    }

    private void Update()
    {
        //Move(); 
    }

    private void Move()
    {
        rigidbody.velocity = transform.forward * speed; // TODO: �ٶ������� ����. ���� �ʿ�
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Target>() != null)
            Debug.Log("Ÿ�ٿ� ����");
        else
            Debug.Log("������");
        //Destroy(gameObject);
    }
}
