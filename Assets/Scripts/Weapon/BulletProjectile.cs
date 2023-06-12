using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody bulletRigidBody;
    [SerializeField] private float speed = 40f;
    PlayerController playerController;
    EnemyController enemyController;

    private void Awake()
    {
        bulletRigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        bulletRigidBody.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.GetComponent<PlayerController>())
            {
                playerController = collision.gameObject.GetComponent<PlayerController>();
                playerController.PlayerDie();
            }

            else if (collision.gameObject.GetComponent<EnemyController>())
            {
                enemyController = collision.gameObject.GetComponent<EnemyController>();
                enemyController.Die();
            }
        }

        Destroy(gameObject);
    }
}
