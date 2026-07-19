using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Torch : MonoBehaviour
{
    [SerializeField] ParticleSystem particles;
    [SerializeField] GameObject _light;
    [SerializeField] AudioSource sfx;
    [SerializeField] GameObject target;
    [SerializeField] float lit_duration = 20f;
    float put_out_timer = 0;
    bool lit;
    public void Ignite()
    {
        particles.Play();
        _light.SetActive(true);
        put_out_timer = lit_duration;
        lit = true;
        target.SetActive(true);
        sfx.Play();
    }
    void Update()
    {
        if(put_out_timer > 0)
        {
            put_out_timer -= Time.deltaTime;
            return;
        }

        if(lit) SnuffOut();
    }
    void SnuffOut()
    {
        particles.Stop();
        _light.SetActive(false);
        lit = false;
        target.SetActive(false);
        sfx.Stop();
    }
}
