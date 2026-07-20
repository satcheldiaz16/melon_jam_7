using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Unity.VisualScripting;

public enum CreatureState
{
    idle, wander, pursuit, stunned
}
public class TestCreature : MonoBehaviour
{
    [SerializeField] NavMeshAgent nav_agent;
    [SerializeField] CreatureState state;
    [Header("Stunned")]
    [SerializeField] bool can_be_stunned = true;
    [SerializeField] float stunned_time = 5f;
    float stunned_timer = 0;
    [SerializeField] AudioSource stunned_sfx;
    List<Target> disinterested = new List<Target>();
    [Header("Pursuit")]
    [SerializeField] float time_to_remain_disinterested = 30f;
    [SerializeField] float pursuit_speed;
    float pursuit_timer = 0;
    [SerializeField] float time_btwn_target_checks = 2f;
    [SerializeField] int checks_before_target_loss = 3;
    int current_num_of_checks = 0;
    [SerializeField] AudioSource target_cue;
    [SerializeField] AudioSource pursuit_sfx;
    [SerializeField] UnityEvent begin_pursuit_callback;
    [SerializeField] UnityEvent end_pursuit_callback;
    [Header("Vision")]
    [SerializeField] VisionCone vision;
    Target current_visual_target;
    [Header("Hearing")]
    [SerializeField] Hearing hearing;
    Target current_audio_target;
    List<Target> targets_currently_heard = new List<Target>();
    [Header("Wander")]
    [SerializeField] float wander_speed;
    [SerializeField] float wander_location_radius = 25f;
    [SerializeField] float min_idle_time = 1f;
    [SerializeField] float max_idle_time = 5f;
    float wander_timer = 0;
    bool pursuing_visually => current_visual_target != null;
    bool pursuing_audibly => current_audio_target != null && !pursuing_visually;
    [Header("Death")]
    [SerializeField] bool can_kill;
    [SerializeField] string death_msg = "Consumed";
    [SerializeField] float dist_to_kill = 2f;
    [SerializeField] AudioSource kill_sfx;
    [SerializeField] Transform kc_point;
    [SerializeField] Animator kc_anim;
    void OnEnable()
    {
        if (vision)
        {
            vision.TargetSpotted += See;
        }
        if (hearing)
        {
            hearing.TargetHeard += Hear;
        }
    }
    void OnDisable()
    {
        if (vision)
        {
            vision.TargetSpotted -= See;
        }
        if (hearing)
        {
            hearing.TargetHeard -= Hear;
        }
    }
    void SetPathToPosition(Vector3 position)
    {
        NavMeshPath path = new NavMeshPath();
        if(!nav_agent.CalculatePath(position, path))
        {
            if(
                NavMesh.SamplePosition(
                    position,
                    out NavMeshHit hit,
                    5f,
                    NavMesh.AllAreas
                )
            )
            {
                nav_agent.CalculatePath(hit.position, path);
            }
            else
            {
                Debug.LogError("Pathfinding failed on agent " + gameObject.name);
                return;
            }
        }
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
    void See(List<Target> targets)
    {
        if(targets.Count < 1 || state == CreatureState.stunned) return;

        if (targets[0].is_player && (!pursuing_visually || !current_visual_target.is_player))
        {
            current_visual_target = targets[0];
            BeginPursuit(targets[0]);
            return;
        }

        if(pursuing_visually) return;

        foreach(Target target in targets)
        {
            if(!target.is_player && disinterested.Contains(target))
            {
                continue;
            }

            current_visual_target = target;
            BeginPursuit(target);
            break;
        }
    }
    void Hear(List<Target> targets)
    {
        if(state == CreatureState.stunned) return;

        targets_currently_heard = targets;

        if(targets.Count < 1 || pursuing_visually) return;

        if (targets[0].is_player && (!pursuing_audibly || !current_audio_target.is_player))
        {
            current_audio_target = targets[0];
            BeginPursuit(targets[0]);
            return;
        }

        foreach(Target target in targets)
        {
            if(!target.is_player && disinterested.Contains(target))
            {
                continue;
            }

            current_audio_target = target;
            BeginPursuit(target);
            break;
        }
    }
    void BeginPursuit(Target target)
    {
        SetPathToPosition(target.transform.position);
        nav_agent.speed = pursuit_speed;
        state = CreatureState.pursuit;
        target_cue.Play();

        pursuit_sfx.Play();

        if(
            can_kill && 
            Vector3.Distance(PlayerController.instance.transform.position, transform.position) < dist_to_kill &&
            target.is_player
        )
        {
            GrabPlayer();
        }

        begin_pursuit_callback.Invoke();
    }
    void EndPursuit()
    {
        if (pursuing_visually)
        {
            AddToDisinterests(current_visual_target);
        } 
        else if (pursuing_audibly)
        {
            AddToDisinterests(current_audio_target);
        } 

        pursuit_timer = time_btwn_target_checks;

        current_visual_target = null;
        current_audio_target = null;

        state = CreatureState.wander;

        pursuit_sfx.Stop();

        end_pursuit_callback.Invoke();
    }
    void UpdatePursuit()
    {
        if (HasArrived())
        {
            EndPursuit();
            return;
        }

        if(pursuit_timer > 0)
        {
            pursuit_timer -= Time.deltaTime;
            return;
        }

        if (pursuing_visually)
        {
            if(!vision.CanSee(current_visual_target.transform))
            {
                current_num_of_checks++;
                Debug.Log("lost target");
            }
            else
            {
                SetPathToPosition(current_visual_target.transform.position);
                current_num_of_checks = 0;

                if(
                    can_kill && 
                    Vector3.Distance(PlayerController.instance.transform.position, transform.position) < dist_to_kill &&
                    current_visual_target.is_player
                )
                {
                    GrabPlayer();
                }
            }
        }
        else if (pursuing_audibly)
        {
            if(!targets_currently_heard.Contains(current_audio_target))
            {
                current_num_of_checks++;
                Debug.Log("cant hear target");
            }
            else
            {
                SetPathToPosition(current_audio_target.transform.position);
                current_num_of_checks = 0;
            }

            if(
                can_kill && 
                Vector3.Distance(PlayerController.instance.transform.position, transform.position) < dist_to_kill &&
                current_audio_target.is_player
            )
            {
                GrabPlayer();
            }
        }

        if(current_num_of_checks > checks_before_target_loss)
        {
            if(pursuing_visually) current_visual_target = null;
            else if(pursuing_audibly) current_audio_target = null;

            current_num_of_checks = 0;
        }

        pursuit_timer = time_btwn_target_checks;
    }
   void AddToDisinterests(Target target)
    {
        if (target == null || target.is_player || target.is_creature) return;

        if(!disinterested.Contains(target)) disinterested.Add(target);
        StartCoroutine(RemoveAfterDelay(target, time_to_remain_disinterested));
    }
    IEnumerator RemoveAfterDelay(Target target, float delay)
    {
        yield return new WaitForSeconds(delay);
        for(int i = disinterested.Count-1; i>=0; i--)
        {
            if(disinterested[i]==null) disinterested.RemoveAt(i);
        }
        if (target == null) yield return null;
        if (disinterested.Contains(target)) disinterested.Remove(target);
    }
    public void EnterStunnedState()
    {
        if(!can_be_stunned) return;

        if(state == CreatureState.pursuit) EndPursuit();

        state = CreatureState.stunned;
        current_visual_target = null;
        current_audio_target = null;
        nav_agent.ResetPath();
        nav_agent.velocity = Vector3.zero;
        stunned_timer = stunned_time;

        stunned_sfx.Play();
    }
    void Update()
    {
        switch (state)
        {
            case CreatureState.wander: UpdateWander(); break;
            case CreatureState.pursuit: UpdatePursuit(); break;
            case CreatureState.stunned: UpdateStunned(); break;
        }
        HandleWalkSFX();
    }
    void UpdateStunned()
    {
        if(stunned_timer > 0)
        {
            stunned_timer-=Time.deltaTime;
            return;
        }

        state = CreatureState.wander;
    }
    float walk_sfx_timer;
    [SerializeField] AudioSource walk_sfx;
    [SerializeField] float pursuit_sfx_time = .5f;
    [SerializeField] float wander_sfx_time = 1f;
    float GetWalkSFXTime()
    {
        if(state == CreatureState.pursuit) return pursuit_sfx_time;
        else return wander_sfx_time;
    }
    void HandleWalkSFX()
    {
        if(walk_sfx_timer > 0)
        {
            walk_sfx_timer-=Time.deltaTime;
        }
        else if(nav_agent.velocity.sqrMagnitude > 0.01f)
        {
            walk_sfx.Play();
            walk_sfx_timer = GetWalkSFXTime();
        }
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
    public void GrabPlayer()
    {
        if(PlayerController.instance.grabbed) return;

        PlayerController.instance.EnterGrabbedState();

        Camera cam = Camera.main;
        cam.transform.position = kc_point.position;
        cam.transform.rotation = kc_point.rotation;
        cam.transform.parent = kc_point;

        kc_anim.SetTrigger("die");

        kill_sfx?.Play();

        //tell player controller to enter grab state
        //capture main camera, position at and parent to grab point
        //play grab point animation
    }
    public void KillPlayer()
    {
        PlayerController.instance.Die(death_msg);
    }
}
