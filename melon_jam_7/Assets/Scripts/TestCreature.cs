using UnityEngine;
using UnityEngine.AI;

public enum CreatureState
{
    idle, wander, pursuit, stunned
}
public class TestCreature : MonoBehaviour
{
    [SerializeField] NavMeshAgent nav_agent;
    [SerializeField] PlayerController player;
    [SerializeField] float wander_location_radius = 25f;
    [SerializeField] CreatureState state;
    [SerializeField] float wander_speed;
    [SerializeField] float pursuit_speed;
    [SerializeField] VisionCone vision;
    void OnEnable()
    {
        if (vision)
        {
            vision.TargetSpotted += SpotTarget;
        }
    }
    void OnDisable()
    {
        if (vision)
        {
            vision.TargetSpotted -= SpotTarget;
        }
    }
    void SpotTarget(VisualTarget target)
    {
        SetPathToPosition(target.transform.position);
    }
    void Start()
    {
        TestPathToPlayer();
    }
    void EnterStunnedState()
    {
        state = CreatureState.stunned;


    }
    void Update()
    {
        /*
        if (HasArrived())
        {
            NavMeshPath path = new NavMeshPath();
            nav_agent.CalculatePath(PickWanderLocation(), path);
            nav_agent.SetPath(path);
        }
        */
    }
    void SetPathToPosition(Vector3 position)
    {
        NavMeshPath path = new NavMeshPath();
        nav_agent.CalculatePath(position, path);
        nav_agent.SetPath(path);
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
    Vector3 PickWanderLocation()
    {
        for (int i = 0; i < 25; i++)
        {
            Vector2 rand_offset = Random.insideUnitCircle * wander_location_radius;

            Vector3 check_pos = transform.position + new Vector3(rand_offset.x, 0f, rand_offset.y);

            if (NavMesh.SamplePosition(check_pos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
                return hit.position;
        }

        return transform.position; // fallback: no valid spot found
    }
}
