using UnityEngine;
using System.Collections;

public class BabyFearCon : MonoBehaviour
{
    private BabyManager baby;
    private bool setAfraid;
    [SerializeField] float checkFearInterval = .15f;
    [SerializeField] float litThreshold = 2f;
    public LayerMask ObstacleMask;
    
    public void Start()
    {
       baby = GetComponent<BabyManager>();
       StartCoroutine(CheckFear());
    }

    public IEnumerator CheckFear()
    {
        while (this.gameObject.activeSelf)
        {
            CheckIn();
            yield return new WaitForSeconds(checkFearInterval);
        }
    }

    public void CheckIn()
    {
        //Debug.Log("Is baby afraid? " + !IsLit());
        baby.CheckFear(!IsLit());
    }

      bool IsLit()
    {
        float illumination = 0f;

        foreach (var source in LightSource.Active)
        {
            if (source == null || source.lightComp == null) continue;

            Light light = source.lightComp;
            if (!light.enabled || light.intensity <= 0f) continue;

            Vector3 toPlayer = transform.position - light.transform.position;
            float dist = toPlayer.magnitude;
            if (dist > light.range) continue;

            if (light.type == LightType.Spot)
            {
                float angle = Vector3.Angle(light.transform.forward, toPlayer);
                if (angle > light.spotAngle * 0.5f) continue;
            }

            if (Physics.Linecast(light.transform.position, transform.position, ObstacleMask))
                continue;

            // calculate total light on baby based on distance and intensity #splish
            float atten = 1f - Mathf.Clamp01(dist / light.range);
            illumination += light.intensity * atten;
        }

        return illumination >= litThreshold;
    }

}
