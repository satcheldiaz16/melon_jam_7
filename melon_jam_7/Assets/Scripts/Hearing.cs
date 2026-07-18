using UnityEngine;

public class Hearing : MonoBehaviour
{
    [SerializeField] float hearing_radius;
    [SerializeField] LayerMask target_mask;
    public event System.Action<Target> TargetHeard;
    void HearingCheck()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, hearing_radius, target_mask);

        foreach(Collider col in hits)
        {
            TargetHeard?.Invoke(col.gameObject.GetComponent<Target>());
        }
    }
}
