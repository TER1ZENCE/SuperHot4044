using System.Collections;
using UnityEngine;
using UnityEngine.AI; 

public class EnemyPatrol : MonoBehaviour 
{
    public NavMeshAgent agent;
    public Transform centrePoint;
    public float range;

    public float patrolCoolDownTime;
    public bool isOnTheWay;

    void Start()
    {
        centrePoint = gameObject.transform;
        agent = GetComponent<NavMeshAgent>();
    }


    void Update()
    {
        Debug.Log(patrolCoolDownTime);
        patrolCoolDownTime += Time.deltaTime;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
          StartCoroutine(FollowTheNextPosition());
        }
    }

    private void GoToTarget()
    {
        Vector3 point;
        if (RandomPoint(centrePoint.position, range, out point))
        {
            patrolCoolDownTime = 0;
            isOnTheWay = true;
            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
            agent.SetDestination(point);
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
