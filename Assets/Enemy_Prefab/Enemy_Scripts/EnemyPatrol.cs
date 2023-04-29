using UnityEngine;
using UnityEngine.AI; 

public class EnemyPatrol : MonoBehaviour 
{
    public NavMeshAgent agent;
    public float range;

    public float patrolCoolDownTime;
    public float patrolTimer;

    public Transform centrePoint ;

    void Start()
    {
        centrePoint = gameObject.transform;
        agent = GetComponent<NavMeshAgent>();
    }


    void Update()
    {
        patrolTimer += Time.deltaTime;
        if (agent.remainingDistance <= agent.stoppingDistance && patrolTimer >= patrolCoolDownTime)
        {
            patrolTimer = 0;
            GoToTarget();
        }

    }

    private void GoToTarget()
    {
        Vector3 point;
        if (RandomPoint(centrePoint.position, range, out point))
        {
            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
            agent.SetDestination(point);
        }
    }
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 5.0f, NavMesh.AllAreas))
        {
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }


}
