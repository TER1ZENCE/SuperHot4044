using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [Space(5f)]
    [SerializeField] NavMeshAgent agent;
    public GunGrabbable gunGrabbable;
    [SerializeField] private RagdollManager ragdollManager;

    [Header("Enemy Throw")]
    [Space(5f)]
    [SerializeField] private float enemyThrowForce;
    [SerializeField] private Transform playerTransform;

    public bool isDeath;
    public bool hasAGun;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();   
        ragdollManager = GetComponent<RagdollManager>();
        CheckForWeapons();
        if (gunGrabbable != null)
        {
            gunGrabbable.Pickup(this.transform);
        }

        agent = GetComponent<NavMeshAgent>();
        ragdollManager.SetRigidbodyState(true, isDeath);
        ragdollManager.SetCollidersState(false);
    }
    public void Die()
    {
        isDeath = true;

        if (!agent.isStopped)
            agent.isStopped = true;

        CheckForWeapons();
        if (hasAGun)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            Vector3 directionToTarget = (playerTransform.position - transform.position).normalized;
            gunGrabbable.Drop(directionToTarget,enemyThrowForce);
        }

        hasAGun = false;
        GetComponent<FieldOfView>().enabled = false;
        GetComponent<Animator>().enabled = false;
        ragdollManager.SetRigidbodyState(false, isDeath);
        ragdollManager.SetCollidersState(true);
        Destroy(gameObject, 10f);
    }

    private void CheckForWeapons()
    {
        gunGrabbable = GetComponentInChildren<GunGrabbable>();

        if (gunGrabbable != null)
        {
            hasAGun = true;
        }
        else
        {
            hasAGun = false;
        }
    }
}
