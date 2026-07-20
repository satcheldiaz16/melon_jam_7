using Unity.Mathematics;
using UnityEngine;

public class Combustable : MonoBehaviour
{
    [SerializeField] GameObject combustion_effect_prefab;
    GameObject combustion_effect;
    [SerializeField] float time_until_destroyed = 15f;
    bool combusted;
    public void BeginCombustion()
    {
        if(combusted) return;

        combusted = true;
        combustion_effect = Instantiate(combustion_effect_prefab, transform.position, quaternion.identity);
        Invoke("RemoveCombustable", time_until_destroyed);
    }
    public void RemoveCombustable()
    {
        Destroy(combustion_effect);
        Destroy(gameObject);
    }
}
