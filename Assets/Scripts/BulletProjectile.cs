using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody rigidbody;
    float speed = 100f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rigidbody.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Target>() != null)
            Debug.Log("Å¸°Ù¿¡ ¸íÁß");
        else
            Debug.Log("°¨³ªºø");
        Destroy(gameObject);
    }
}
