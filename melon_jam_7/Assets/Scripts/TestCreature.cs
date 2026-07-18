using Unity.VisualScripting;
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
    [SerializeField] CreatureState state;
    [Header("Pursuit")]
    [SerializeField] float pursuit_speed;
    float pursuit_timer = 0;
    [SerializeField] float time_btwn_vision_checks = 2f;
    [Header("Vision")]
    [SerializeField] VisionCone vision;
    [SerializeField] VisualTarget current_visual_target;
    [Header("Wander")]
    [SerializeField] float wander_speed;
    [SerializeField] float wander_location_radius = 25f;
    [SerializeField] float min_idle_time = 1f;
    [SerializeField] float max_idle_time = 5f;
    float wander_timer = 0;
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
        if(current_visual_target!=null && !target.is_player) return;

        current_visual_target = target;

        SetPathToPosition(target.transform.position);
        nav_agent.speed = pursuit_speed;
        state = CreatureState.pursuit;
    }
    void Start()
    {
        
    }
    void EnterStunnedState()
    {
        state = CreatureState.stunned;
    }
    void Update()
    {
        switch (state)
        {
            case CreatureState.wander: UpdateWander(); break;
            case CreatureState.pursuit: UpdatePursuit(); break;
        }
    }
    void UpdatePursuit()
    {
        if (HasArrived())
        {
            pursuit_timer = time_btwn_vision_checks;
            state = CreatureState.wander;
            current_visual_target = null;
            return;
        }

        if(pursuit_timer > 0)
        {
            pursuit_timer -= Time.deltaTime;
            return;
        }

        if(current_visual_target != null && vision.CanSee(current_visual_target.transform))
        {
            SetPathToPosition(current_visual_target.transform.position);
            Debug.Log("Checking target again");
        }
        Debug.Log("End of pursuit loop");
        pursuit_timer = time_btwn_vision_checks;
    }
    void UpdateWander()
    {
        if(wander_timer > 0)
        {
            wander_timer -= Time.deltaTime;
            return;
        }

        if (HasArrived())
        {
            nav_agent.speed = wander_speed;
            int roll = Random.Range(0, 4);
            if(roll < 2) // stand around
            {
                wander_timer = Random.Range(min_idle_time, max_idle_time);
            }
            else //wander
            {
                Vector3 wander_pos = PickWanderLocation();
                SetPathToPosition(wander_pos);
            }
        }
    }
    void SetPathToPosition(Vector3 position)
    {
        NavMeshPath path = new NavMeshPath();
        nav_agent.CalculatePath(position, path);
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
