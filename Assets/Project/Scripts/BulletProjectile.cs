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
        rigidbody.velocity = transform.forward * speed; // TODO: 바람직하지 않음. 수정 필요
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Target>() != null)
            Debug.Log("타겟에 명중");
        else
            Debug.Log("감나빗");
        //Destroy(gameObject);
    }
}
