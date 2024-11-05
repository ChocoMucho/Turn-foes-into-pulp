using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody rigidbody;
    float speed = 30;

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
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            IBattle battle = other.GetComponent<IBattle>();
            if(battle != null)
            {
                battle.OnDamage(GameManager.instance.Player.GetComponent<Player>().Damage); // 진짜 너무 더럽다.
            }
        }
        Destroy(gameObject);
    }
}
