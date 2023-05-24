using System.Collections;
using UnityEngine;
using UnityEngine.AI; 

public class EnemyPatrol : MonoBehaviour 
{
    public NavMeshAgent agent;
    public Transform centrePoint;
    public float range;
    private Vector3 originalPoint;

    public float patrolCoolDownTime;
    public bool isOnTheWay;

    void Start()
    {
        originalPoint = transform.position;
        centrePoint = gameObject.transform;
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(FollowTheNextPosition());
    }


    void Update()
    {
        if (isOnTheWay && agent.remainingDistance <= 0)
        {
            isOnTheWay = false;
          StartCoroutine(FollowTheNextPosition());
        }
    }

    private void GoToTarget()
    {
        Debug.Log(originalPoint);
        Vector3 point;
        if (RandomPoint(centrePoint.position, range, out point))
        {
            originalPoint = transform.position;
            isOnTheWay = true;
            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
            agent.SetDestination(point);
        }
        else
        {
            agent.SetDestination(originalPoint);
        }
    }

    private IEnumerator FollowTheNextPosition()
    {
            yield return new WaitForSeconds(patrolCoolDownTime);
            GoToTarget();      
    }
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 5.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }


}
