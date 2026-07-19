using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class VisionCone : MonoBehaviour
{
    [SerializeField] float view_radius = 15f;
    [SerializeField] float view_angle = 60f;   // full cone angle
    [SerializeField] LayerMask target_mask;
    [SerializeField] LayerMask obstacle_mask;
    public event System.Action<Target> TargetSpotted;
    void Start() => InvokeRepeating(nameof(VisionCheck), Random.Range(0f, 0.2f), 0.2f);
    void VisionCheck()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, view_radius, target_mask);

        foreach(Collider col in hits)
        {
            if (CanSee(col.gameObject.transform))
            {
                TargetSpotted?.Invoke(col.gameObject.GetComponentInParent<Target>());
            }
        }
    }
    public bool CanSee(Transform target)
    {
        Vector3 to_target = target.position - transform.position;

        if (to_target.sqrMagnitude > view_radius * view_radius) return false;
        if (Vector3.Angle(transform.forward, to_target) > view_angle * 0.5f) return false;

        // occlusion
        RaycastHit hit;
        bool occluded = !Physics.Raycast(transform.position, to_target.normalized, out hit, to_target.magnitude, obstacle_mask);
        //Debug.Log(hit.collider.gameObject.name);
        return occluded;
    }
    #if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // range sphere
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, view_radius);

        // cone edges
        Gizmos.color = Color.yellow;
        Vector3 left  = Quaternion.Euler(0f, -view_angle * 0.5f, 0f) * transform.forward;
        Vector3 right = Quaternion.Euler(0f,  view_angle * 0.5f, 0f) * transform.forward;

        Gizmos.DrawRay(transform.position, left  * view_radius);
        Gizmos.DrawRay(transform.position, right * view_radius);

        // arc
        Handles.color = new Color(1f, 1f, 0f, 0.1f);
        Handles.DrawSolidArc(transform.position, Vector3.up, left, view_angle, view_radius);
    }
    #endif
}