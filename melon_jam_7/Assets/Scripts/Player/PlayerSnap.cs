using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerSnap : MonoBehaviour
{
    public BabyManager baby;
    [Header("Noise")]
    public NoiseController noiseController; 
    public float shootVol = .9f;
    [Header("UI")]
    [SerializeField] Animator hand_anim;
    [SerializeField] FingerPain ui;
    [SerializeField] GameObject snap_indicator;
    [SerializeField] Camera cam;
    [SerializeField] LayerMask snap_layers;
    [Header("Audio")]
    [SerializeField] AudioSource spark_sfx;
    [SerializeField] AudioSource ouch_sfx;
    [Header("Config")]
    [SerializeField] GameObject spark_prefab;
    [SerializeField] bool infinite_snaps = false;
    public bool snap_acquired = false;
    [SerializeField] float max_snap_distance = 25f;
    [SerializeField] float snap_recover_time = 2.5f;
    [SerializeField] float full_snap_recover_time = 10f;
    [SerializeField] int snaps_accumulated;
    [SerializeField] bool worn_out = false;
    float snap_recovery_timer;
    bool palming;
    void Update()
    {
        hand_anim.SetBool("palming", palming);
        PositionSnapIndicator();

        if(snap_recovery_timer > 0)
        {
            snap_recovery_timer -= Time.deltaTime;
            if(snap_recovery_timer <= 0)
            {
                if(snaps_accumulated > 0)
                {
                    hand_anim.SetBool("in_pain", false);
                    snaps_accumulated--;
                    snap_recovery_timer = snap_recover_time;
                }
                ui.SetPainLevel(snaps_accumulated);
                worn_out = false;
            }
        }
    }
    public void OnAttack(InputValue value)
    {
        if(worn_out || !snap_acquired) return;

        hand_anim.SetTrigger("snap");

        GameObject spark = Instantiate(
            spark_prefab,
            snap_indicator.transform.position,
            Quaternion.identity
        );
        spark.GetComponent<Spark>().Ignite();
        
        spark_sfx.Play();
        noiseController.TriggerImpulseNoise(shootVol);        
        
        if(infinite_snaps) return;

        snaps_accumulated++;
        ui.SetPainLevel(snaps_accumulated);
        snap_recovery_timer = snap_recover_time;
        if (snaps_accumulated == 3)
        {
            hand_anim.SetBool("in_pain", true);
            hand_anim.SetTrigger("pain");
            worn_out = true;
            snap_recovery_timer = full_snap_recover_time;
            ouch_sfx.Play();
        }
    }
    public void OnAltAttack(InputValue value)
    {
        if(!snap_acquired) return;
        
        palming = value.isPressed;
        baby.SetComfort(palming);
    }
    void PositionSnapIndicator()
    {
        if (worn_out || !snap_acquired)
        {
            snap_indicator.gameObject.SetActive(false);
            return;
        }

        RaycastHit raycast_hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out raycast_hit, max_snap_distance, snap_layers))
        {
            snap_indicator.gameObject.SetActive(true);
            if(raycast_hit.collider.gameObject.TryGetComponent(out SparkTarget t))
            {
                snap_indicator.transform.position = t.transform.position;
            }
            else
            {
                snap_indicator.transform.position = raycast_hit.point + raycast_hit.normal * .05f;
            }  
        }
        else
        {
            snap_indicator.gameObject.SetActive(false);
        }
    }
}
