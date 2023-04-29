using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private bool isHunting;

    private void Start()
    {
         agent = GetComponent<NavMeshAgent>();
         animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (agent.velocity.magnitude > 0)
        {
            animator.SetBool("IsWalking", true);
        }
        else if (agent.velocity.magnitude <=0)
        {
            animator.SetBool("IsWalking", false);
        }
    }


}
