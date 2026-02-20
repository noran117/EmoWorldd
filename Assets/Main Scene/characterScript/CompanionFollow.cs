using UnityEngine;
using UnityEngine.AI;

public class CompanionFollow : MonoBehaviour
{
    public Transform player;
    public float followDistance = 2f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        transform.LookAt(player);
        if (player == null) return;

        Vector3 distance = player.position - transform.position;
       if (distance.magnitude > followDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;
            agent.SetDestination(transform.position);
        }          


    }
}
