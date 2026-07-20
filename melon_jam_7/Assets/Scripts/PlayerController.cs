
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public enum PlayerMovementState
{
    walk,
    crouch,
    sprint
}
public class PlayerController : MonoBehaviour
{
    public PlayerInput input;
    public static PlayerController instance;
    [SerializeField] SphereCollider audio_target;
    [SerializeField] Target target;
    [Header("UI")]
    [SerializeField] Transform camera_pos;
    [SerializeField] GameObject pause_menu;
    [SerializeField] Animator fader;
    [Header("Movement")]
    [SerializeField] CharacterController character_controller;
    [SerializeField] float move_speed = 5f;
    [SerializeField] float walk_sound_radius = 10;
    Vector2 input_movement;
    Vector3 move_dir;
    float vertical_vel;
    bool jump_input_pressed;
    bool sprint_input_pressed;
    bool crouch_input_pressed;
    float walk_sfx_timer;
    [SerializeField] float normal_height = .6f;
    [SerializeField] AudioSource walk_sfx;
    [Header("Jump")]
    [SerializeField] float jump_height = 10f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jump_slope_limit = 55f;
    [SerializeField] LayerMask ground_mask;
    [SerializeField] AudioSource jump_sfx;
    [Header("Crouch")]
    [SerializeField] float crouch_speed = 2f;
    [SerializeField] float crouch_height = .3f;
    [SerializeField] float crouch_sound_radius = 2.5f;
    [Header("Sprint")]
    [SerializeField] float sprint_speed = 8f;
    [SerializeField] float sprint_sound_radius = 25f;
    [Header("Death")]
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
        character_controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false; 
    }
    void Update()
    {
        if(dead) return;

        CalculateMovement();
        HandleWalkSFX();
        audio_target.radius = GetAudioTargetRadius();
    }
    public void OnMove(InputValue value)
    {
        input_movement = value.Get<Vector2>();
    }
    public void OnJump(InputValue value)
    {
        jump_input_pressed = value.isPressed;
    }
    public void OnSprint(InputValue value)
    {
        sprint_input_pressed = value.isPressed;
    }
    public void OnCrouch(InputValue value)
    {
        crouch_input_pressed = value.isPressed;
    }
    public void OnMenu(InputValue value)
    {
        pause_menu.SetActive(true);
    }
    float GetCameraHeight()
    {
        if(crouch_input_pressed) return crouch_height;
        return normal_height;
    }
    float GetMoveSpeed()
    {
        if(crouch_input_pressed) return crouch_speed;
        else if (sprint_input_pressed) return sprint_speed;
        return move_speed;
    }
    float GetWalkSFXTime()
    {
        if(crouch_input_pressed) return .8f;
        else if (sprint_input_pressed) return .2f;
        else return .5f;
    }
    float GetAudioTargetRadius()
    {
        if(input_movement==Vector2.zero) return 0;
        if(crouch_input_pressed) return crouch_sound_radius;
        else if (sprint_input_pressed) return sprint_sound_radius;
        else return walk_sound_radius;
    }
    void CalculateMovement()
    {
        if(character_controller.isGrounded && vertical_vel < 0)
        {
            vertical_vel = -2f;
        }

        move_dir = new Vector3(input_movement.x, 0f, input_movement.y).normalized;
        move_dir = transform.TransformDirection(move_dir);

        Vector3 final_vel = move_dir * GetMoveSpeed();

        if (jump_input_pressed && character_controller.isGrounded && IsGrounded())
        {
            vertical_vel = Mathf.Sqrt(jump_height * -2.0f * gravity);
            jump_sfx.Play();
        }
        else
        {
            vertical_vel += gravity * Time.deltaTime;
        }

        final_vel.y = vertical_vel;

        character_controller.Move(final_vel * Time.deltaTime);

        camera_pos.localPosition = new Vector3(0, GetCameraHeight(), 0);
    }
    bool IsGrounded()
    {
        float radius = character_controller.radius * 0.9f;
        Vector3 origin = transform.position + character_controller.center;
        float dist = (character_controller.height / 2f) - character_controller.radius + character_controller.skinWidth + 0.1f;

        if (Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit hit, dist, ground_mask, QueryTriggerInteraction.Ignore))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle <= jump_slope_limit;
        }
        return false;
    }
    void HandleWalkSFX()
    {
        if(walk_sfx_timer > 0)
        {
            walk_sfx_timer-=Time.deltaTime;
        }
        else if(input_movement != Vector2.zero && character_controller.isGrounded)
        {
            walk_sfx.Play();
            walk_sfx_timer = GetWalkSFXTime();
        }
    }
    public void EnterGrabbedState()
    {
        grabbed = true;
        input.actions.FindActionMap("Player").Disable();
        target.gameObject.SetActive(false);
    }
    public void Die(string cause = "You Died")
    {
        dead = true;
        fader.SetTrigger("die");
        death_menu.gameObject.SetActive(true);
        death_menu.SetCauseText(cause);
        GetComponent<PlayerInput>().enabled = false;
        death_sfx.Play();
    }
}
