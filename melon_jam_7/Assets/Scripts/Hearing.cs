using UnityEngine;
using System.Linq;
using System.Collections.Generic;
public class Hearing : MonoBehaviour
{
    [SerializeField] float hearing_radius;
    [SerializeField] LayerMask target_mask;
    public event System.Action<List<Target>> TargetHeard;
    void Update()
    {
        HearingCheck();
    }
    void HearingCheck()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, hearing_radius, target_mask);
        Target[] targets = new Target[hits.Length];

        for(int i = 0; i < hits.Length; i++)
        {
            targets[i] = hits[i].gameObject.GetComponentInParent<Target>();
        }

        List<Target> sorted = new List<Target>();
        if(targets.Length > 0)
        {
            sorted = targets.OrderBy(c => (c.transform.position - transform.position).sqrMagnitude).ToList();
        }
        
        TargetHeard?.Invoke(sorted);
    }
}
