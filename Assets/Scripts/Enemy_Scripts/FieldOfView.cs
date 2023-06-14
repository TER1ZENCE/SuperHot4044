using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius;
    [Range(0,360)]
    public float angle;


     public GameObject playerRef;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstructionMask;
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private EnemyPatrol enemyPatrol;
    [SerializeField] private Animator animator;


    [SerializeField] private int baseMovementLayerIndex;
    [SerializeField] private int aimingLayerIndex;

    public bool seesAPlayer;
    public bool waitBeforeLosingTarget;
    public bool isInAimState;

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        enemyMovement = GetComponent<EnemyMovement>();
        enemyPatrol = GetComponent<EnemyPatrol>();
        animator = GetComponent<Animator>();
        baseMovementLayerIndex = animator.GetLayerIndex("Base Movement");
        aimingLayerIndex = animator.GetLayerIndex("Aim Movement");
        ;    }

    private void Update()
    {
        FieldOfViewCheck();
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    animator.SetLayerWeight(aimingLayerIndex, Mathf.Lerp(animator.GetLayerWeight(aimingLayerIndex), 1f, Time.deltaTime * 10f));
                    animator.SetLayerWeight(baseMovementLayerIndex, Mathf.Lerp(animator.GetLayerWeight(baseMovementLayerIndex), 0f, Time.deltaTime * 1f));

                    seesAPlayer = true;
                    enemyPatrol.canPatroling = false;
                    enemyMovement.AttackTarget(playerRef.transform);
                }
                else
                {
                    animator.SetLayerWeight(aimingLayerIndex, Mathf.Lerp(animator.GetLayerWeight(aimingLayerIndex), 0f, Time.deltaTime * 10f));
                    animator.SetLayerWeight(baseMovementLayerIndex, Mathf.Lerp(animator.GetLayerWeight(baseMovementLayerIndex), 1f, Time.deltaTime * 200f));
                    seesAPlayer = false;
                    enemyPatrol.canPatroling = true;
                    isInAimState = false;
                }
            }
            else
            {
                animator.SetLayerWeight(aimingLayerIndex, Mathf.Lerp(animator.GetLayerWeight(aimingLayerIndex), 0f, Time.deltaTime * 10f));
                animator.SetLayerWeight(baseMovementLayerIndex, Mathf.Lerp(animator.GetLayerWeight(baseMovementLayerIndex), 1f, Time.deltaTime * 200f));
                seesAPlayer = false;
                enemyPatrol.canPatroling = true;
                isInAimState = false;
                enemyMovement.lookAtPlayer = false;
            }
        }
        else if (seesAPlayer)
        {
            enemyMovement.AttackTarget(playerRef.transform);
            waitBeforeLosingTarget = true;
            enemyMovement.lookAtPlayer = false;
            if (waitBeforeLosingTarget)
                StartCoroutine(WaitBeforeLosing());
        }

    }

    private IEnumerator WaitBeforeLosing()
    {
        seesAPlayer = false;
        yield return new WaitForSeconds(5f);
        waitBeforeLosingTarget = false;
        enemyPatrol.canPatroling = true;
        isInAimState = false;
    }
}
