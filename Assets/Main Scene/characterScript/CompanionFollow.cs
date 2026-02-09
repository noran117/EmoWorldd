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
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
       if (distance > followDistance)
        {
            agent.isStopped = false;
            //agent.SetDestination(player.position);
            agent.destination=player.position;
        }
        else
        {
            agent.isStopped = true;
        }            

    }
}
