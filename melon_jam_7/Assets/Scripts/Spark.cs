using UnityEngine;

public class Spark : MonoBehaviour
{
    [SerializeField] ParticleSystem effect;
    [SerializeField] Animator light_anim;
    public void Ignite(Vector3 position)
    {
        transform.position = position;

        light_anim.SetTrigger("spark");
        effect.Play();
    }
}
