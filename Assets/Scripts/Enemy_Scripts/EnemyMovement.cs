using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [Space(5)]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private FieldOfView fov;

    [Header("Stop Distance")]
    [Space(5)]
    [SerializeField] private float stopDistance;

    [Header("Enemy Rotate To Player Speed")]
    [Space(5)]
    [SerializeField] private float rotationSpeed;
    public bool lookAtPlayer;
    public float maxAngle = 30f;

    private float walkBlendTreePrameter;


    private void Start()
    {
         agent = GetComponent<NavMeshAgent>();
         animator = GetComponent<Animator>();
        fov = GetComponent<FieldOfView>();
    }

    private void Update()
    {
        if (agent.velocity.magnitude > 0)
        {
            DOTween.To(() => walkBlendTreePrameter, x =>
            {
                walkBlendTreePrameter = x;
                animator.SetFloat("Speed", walkBlendTreePrameter);
            }, 1f, 0.5f);
        }
        else if (agent.velocity.magnitude <=0)
        {
            DOTween.To(() => walkBlendTreePrameter, x =>
            {
                walkBlendTreePrameter = x;
                animator.SetFloat("Speed", walkBlendTreePrameter);
            }, 0f, 0.5f);
        }
    }
    
    public void AttackTarget(Transform target)
    {
        float targetDistance = Vector3.Distance(transform.position, target.position);

        Vector3 directionToPlayer = target.position - transform.position;
        directionToPlayer.y = 0f;
        Vector3 forwardDirection = transform.forward;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        float angle = Vector3.Angle(directionToPlayer, forwardDirection);

        if (targetDistance > stopDistance)
        {
            agent.isStopped = false;
            MoveToPlayer(target);
        }
        else
        {
            agent.isStopped = true;

            //Turn to the player


            if (directionToPlayer != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

                if (angle < maxAngle && fov.seesAPlayer)
                    lookAtPlayer = true;
                else
                    lookAtPlayer = false;
            }
        }
    }

    private void MoveToPlayer(Transform target)
    {
        agent.SetDestination(target.position);
    }
}
