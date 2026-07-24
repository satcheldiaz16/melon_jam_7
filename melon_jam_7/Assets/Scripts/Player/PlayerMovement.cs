using UnityEngine;
using UnityEngine.InputSystem;
public enum PlayerMovementState
{
    walk,
    crouch,
    sprint
}
public class PlayerMovement : MonoBehaviour, givePerception
{
    [SerializeField] Transform camera_pos;
    [SerializeField] SphereCollider audio_target;
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
    [SerializeField] float walk_sfx_time = .5f;
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
    [SerializeField] float crouch_sfx_time = .8f;
    [Header("Sprint")]
    [SerializeField] float sprint_speed = 8f;
    [SerializeField] float sprint_sound_radius = 25f;
    [SerializeField] float sprint_sfx_time = .2f;
    [Header("Perceptibles")]
    public float curLight = 0;
    public float curVolume = 0;
    void Start()
    {
        character_controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false; 
    }
    void Update()
    {
        CalculateMovement();
        HandleWalkSFX();
        SetAudioTarget();
    }
    
    #region Input
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
    #endregion
    #region Perception
    public Perceivables givePerception()
    {
        Perceivables player = new Perceivables(curVolume, curLight);
        return player;
    }
    #endregion
    #region Audio Targetting
    float GetAudioTargetRadius()
    {
        if(input_movement==Vector2.zero){ return .01f;}
        if(crouch_input_pressed) return crouch_sound_radius;
        else if (sprint_input_pressed) return sprint_sound_radius;
        else return walk_sound_radius;
    }
    void SetAudioTarget()
    {
        audio_target.radius = GetAudioTargetRadius();
        audio_target.enabled = audio_target.radius >= .02f;
    }
    #endregion
    #region Movement
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
    #endregion
    #region SFX
    float GetWalkSFXTime()
    {
        if(crouch_input_pressed) return crouch_sfx_time;
        else if (sprint_input_pressed) return sprint_sfx_time;
        else return walk_sfx_time;
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
    #endregion
}
