using System.Collections.Generic;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    [SerializeField] GameObject owner;
    [SerializeField] List<Collider> spotted = new List<Collider>();
    public List<VisualTarget> targets = new List<VisualTarget>();
    public event System.Action<VisualTarget> TargetSpotted;
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter");
        AttemptToAdd(other);
    }
    void OnTriggerStay(Collider other)
    {
        Debug.Log("Stay");
        AttemptToAdd(other);
    }
    void AttemptToAdd(Collider other)
    {
        if(owner && other.gameObject==owner) return;

        if (spotted.Contains(other)) return;

        spotted.Add(other);

        TargetSpotted?.Invoke(other.gameObject.GetComponent<VisualTarget>());
    }
    void OnTriggerExit(Collider other)
    {
        AttemptToRemove(other);
    }
    void AttemptToRemove(Collider other)
    {
        if (spotted.Contains(other))
        {
            spotted.Remove(other);
        }
    }
}
