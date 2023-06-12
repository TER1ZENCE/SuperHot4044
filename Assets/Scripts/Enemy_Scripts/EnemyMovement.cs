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

        if (targetDistance > stopDistance)
        {
            agent.isStopped = false;
            MoveToPlayer(target);
        }
        else
        {
            agent.isStopped = true;

            //Turn to the player
            Vector3 direction = target.position - transform.position;
            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    private void MoveToPlayer(Transform target)
    {
        agent.SetDestination(target.position);
    }
}
