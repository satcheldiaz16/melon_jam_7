using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Light))]
public class LightSource : MonoBehaviour
{
    public Light lightComp;
    public static readonly List<LightSource> Active = new List<LightSource>();

    void Awake()
    {
        if (lightComp == null) lightComp = GetComponent<Light>();
    }

    void OnEnable()
    {
        Active.Add(this);
    }

    void OnDisable()
    {
        Active.Remove(this);
    }

    void Update()
    {
        //Debug.Log(LightSource.Active.Count);
    }
}