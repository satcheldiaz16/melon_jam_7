using System.Collections;
using UnityEngine;

public class Spark : MonoBehaviour
{
    [SerializeField] ParticleSystem effect;
    [SerializeField] Animator light_anim;
    [SerializeField] float spark_radius;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
    public void Ignite()
    {
        light_anim.SetTrigger("spark");
        effect.Play();
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            spark_radius
        );

        foreach(Collider col in hits)
        {
            if(col.gameObject.TryGetComponent(out SparkTarget t))
            {
                t.ignition_callback.Invoke();
            }
        }
    }
}
