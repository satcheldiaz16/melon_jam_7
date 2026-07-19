
using System.Collections;
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
    [SerializeField] Transform camera_pos;
    [Header("Movement")]
    [SerializeField] CharacterController character_controller;
    [SerializeField] AudioSource jump_sfx;
    [SerializeField] AudioSource walk_sfx;
    [SerializeField] GameObject pause_menu;
    float walk_sfx_timer;
    [SerializeField] float move_speed = 5f;
    [SerializeField] float crouch_speed = 2f;
    [SerializeField] float sprint_speed = 8f;
    [SerializeField] float jump_height = 10f;
    [SerializeField] float normal_height = .6f;
    [SerializeField] float crouch_height = .3f;
    [SerializeField] float gravity = -9.81f;
    Vector2 input_movement;
    Vector3 move_dir;
    float vertical_vel;
    bool jump_input_pressed;
    bool sprint_input_pressed;
    bool crouch_input_pressed;
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
    void CalculateMovement()
    {
        if(character_controller.isGrounded && vertical_vel < 0)
        {
            vertical_vel = -2f;
        }

        move_dir = new Vector3(input_movement.x, 0f, input_movement.y).normalized;
        move_dir = transform.TransformDirection(move_dir);

        Vector3 final_vel = move_dir * GetMoveSpeed();

        if (jump_input_pressed && character_controller.isGrounded)
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
}
