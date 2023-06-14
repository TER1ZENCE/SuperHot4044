using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyShooterController : MonoBehaviour
{

    [Header("References")]
    [Space(5)]
    [SerializeField] private GunGrabbable gunGrabbable;
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private FieldOfView fieldOfView;
    [SerializeField] private NavMeshAgent agent;

    [Header("Shoot Delay")]
    [Space(5)]
    [SerializeField] private float enemyShootDelay;
    private float lastShootTime;

    [Header("canShoot")]
    [Space(5)]
    [SerializeField] private bool canShoot;

    private void Start()
    {
        enemyController = GetComponent<EnemyController>();      
        fieldOfView = GetComponent<FieldOfView>();
        agent = GetComponent<NavMeshAgent>();
        enemyMovement = GetComponent<EnemyMovement>();
    }

    private void Update()
    {
        if (Time.time - lastShootTime >= enemyShootDelay)
        {
            CheckShootState();
            if (canShoot)
            {
                player = GameObject.FindGameObjectWithTag("Player");
                playerController = player.GetComponent<PlayerController>();

                if (!playerController.isDeath)
                {
                    Vector3 directionToTarget = player.transform.position + new Vector3(0, 1.3f, 0);
                    gunGrabbable.Shoot(directionToTarget);
                    lastShootTime = Time.time;
                }
            }
            lastShootTime = Time.time;  
        }
    }

    private void CheckShootState()
    {
        gunGrabbable = GetComponentInChildren<GunGrabbable>();

        if (gunGrabbable != null)
        {
            gunGrabbable.CheckAmmoCount();

            if (enemyController.hasAGun && gunGrabbable.hasAmmo && fieldOfView.isInAimState && agent.isStopped && enemyMovement.lookAtPlayer)
                EnableShooting();
            else
                DisableShooting();
        }

    }

    private void SetAimState()
    {
        if (!fieldOfView.isInAimState)
        {
            fieldOfView.isInAimState = true;
        }
    }

    public bool CanShoot
    {
        get { return canShoot; }
        set { canShoot = value; }
    }

    public void EnableShooting()
    {
        CanShoot = true;
    }

    public void DisableShooting()
    {
        CanShoot = false;
    }
}
