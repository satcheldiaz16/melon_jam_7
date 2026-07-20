using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Hearing : MonoBehaviour
{
    [SerializeField] float hearing_radius = 15f;
    [SerializeField] LayerMask target_mask;

    [Header("Noise Detection")]
    [SerializeField] float detectableNoiseAmnt = .03f;
    [SerializeField] float curSoundHeard;

    [Header("Debug Visuals")]
    [SerializeField] bool showHearingSphere = true;
    [SerializeField] Color sphereColor = new Color(1f, 0.92f, 0.016f, 0.25f);

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
        curSoundHeard = 0f;

        foreach (Collider col in hits)
        {
            // 🛑 FILTER 1: Ignore self (don't hear this monster's own body/colliders)
            if (col.transform.IsChildOf(transform.root)) continue;

            // 🛑 FILTER 2: Ignore other enemies in the scene
            if (col.CompareTag("Enemy") || col.transform.root.CompareTag("Enemy")) continue;

            Target temp = col.gameObject.GetComponentInParent<Target>();
            if (temp == null) continue;

            // (Optional) If your Target.cs script has a bool field like 'is_enemy', uncomment this:
            // if (temp.is_enemy) continue;

            float perceivedVolume = GetPerceivedVolume(temp);

            if (perceivedVolume > detectableNoiseAmnt)
            {
                if (perceivedVolume > curSoundHeard)
                {
                    curSoundHeard = perceivedVolume;
                }   

                if (temp.is_player) player = temp;
                else targets.Add(temp); // Adds baby, burning trees/bushes, thrown props, etc.
            }
        }

        List<Target> sorted = targets.OrderBy(c => (c.transform.position - transform.position).sqrMagnitude).ToList();
        
        if (player != null)
        {
            sorted.Insert(0, player);
        }

        TargetHeard?.Invoke(sorted);
    }

    float GetPerceivedVolume(Target target)
    {
        NoiseController noiseController = target.GetComponentInParent<NoiseController>(); 
        if (noiseController == null)
        {
            noiseController = FindFirstObjectByType<NoiseController>();
        }
        if (noiseController == null) return 0f; 

        float distance = Vector3.Distance(transform.position, target.transform.position);
        float percentAway = Mathf.Clamp01(distance / hearing_radius);
        float hearingStrength = 1f - percentAway; 
        return hearingStrength * noiseController.playerNoise;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showHearingSphere) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearing_radius);

        Gizmos.color = sphereColor;
        Gizmos.DrawSphere(transform.position, hearing_radius);
    }
}