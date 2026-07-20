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
        
        List<Target> targets = new List<Target>();
        Target player = null;

        foreach(Collider col in hits)
        {
            Target temp = col.gameObject.GetComponentInParent<Target>();

            if (temp.is_player)
            {
                player = temp;
            }
            else
            {
                targets.Add(temp);
            }
        }

        List<Target> sorted = targets.OrderBy(c => (c.transform.position - transform.position).sqrMagnitude).ToList();

        if(player!=null) sorted.Insert(0, player);
        
        TargetHeard?.Invoke(sorted);
    }
}