using UnityEngine;
using UnityEngine.AI;

public class TestCreature : MonoBehaviour
{
    [SerializeField] NavMeshAgent nav_agent;
    [SerializeField] PlayerController player;
    [SerializeField] float wander_location_radius = 25f;
    void Start()
    {
        TestPathToPlayer();
    }
    void Update()
    {
        if (HasArrived())
        {
            
        }
    }
    void TestPathToPlayer()
    {
        NavMeshPath path = new NavMeshPath();
        nav_agent.CalculatePath(player.transform.position, path);
        nav_agent.SetPath(path);
    }
    bool HasArrived()
    {
        if (!nav_agent.pathPending)
        {
            if (nav_agent.remainingDistance <= nav_agent.stoppingDistance)
            {
                if (!nav_agent.hasPath || nav_agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }
    void PickWanderLocation()
    {
        Vector2 xz_pos = new Vector2(
            transform.position.x,
            transform.position.z
        );
        Vector2 rand_offset = new Vector2(
            Random.Range(-wander_location_radius, wander_location_radius),
            Random.Range(-wander_location_radius, wander_location_radius)
        );

        Vector2 check_pos = xz_pos+rand_offset;
        
        NavMeshHit hit = new NavMeshHit();
        NavMesh.SamplePosition(check_pos, out hit, 10, NavMesh.AllAreas);
    }
}
