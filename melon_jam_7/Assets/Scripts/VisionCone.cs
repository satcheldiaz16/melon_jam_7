using System.Collections.Generic;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    [SerializeField] List<Collider> spotted = new List<Collider>();
    void OnTriggerEnter(Collider other)
    {
        if (!spotted.Contains(other))
        {
            spotted.Add(other);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (!spotted.Contains(other))
        {
            spotted.Add(other);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (spotted.Contains(other))
        {
            spotted.Remove(other);
        }
    }
}
