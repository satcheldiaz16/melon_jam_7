using System.Collections;
using UnityEngine;

public class Spark : MonoBehaviour
{
    [SerializeField] ParticleSystem effect;
    [SerializeField] Animator light_anim;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
    public void Ignite()
    {
        light_anim.SetTrigger("spark");
        effect.Play();
    }
}
