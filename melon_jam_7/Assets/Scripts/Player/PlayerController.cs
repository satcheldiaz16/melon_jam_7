
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    public PlayerInput input;
    public static PlayerController instance;
    [Header("Noise")]
    public NoiseController noiseController; 
    public float jumpVol = .5f;
    public float runVol = .7f;
    public float walkVol = .3f;
    public float crouchVol = .1f;
    [Header("UI")]
    [SerializeField] GameObject pause_menu;
    [SerializeField] Animator fader;
    [Header("Monster Interaction")]
    [SerializeField] SphereCollider audio_target;
    [SerializeField] Target target;
    [SerializeField] DeathMenu death_menu;
    public bool grabbed = false;
    public bool dead = false;
    [SerializeField] AudioSource death_sfx;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        
    }
    void Update()
    {

    }
    public void OnMenu(InputValue value)
    {
        pause_menu.SetActive(true);
    }
    public void EnterGrabbedState()
    {
        grabbed = true;
        input.actions.FindActionMap("Player").Disable();
        target.gameObject.SetActive(false);
        death_sfx.Play();
        GetComponent<PlayerSnap>().snap_acquired = false;
    }
    public void Die(string cause = "You Died")
    {
        dead = true;
        fader.SetTrigger("die");
        death_menu.gameObject.SetActive(true);
        death_menu.SetCauseText(cause);
        //GetComponent<PlayerInput>().actions.FindActionMap("Player").Disable();
        if(!grabbed) death_sfx.Play();
    }
}
