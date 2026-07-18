using System.Collections.Generic;
using UnityEngine;



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
                TargetSpotted?.Invoke(col.gameObject.GetComponent<Target>());
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
}
