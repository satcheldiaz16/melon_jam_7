
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] CharacterController character_controller;
    [SerializeField] float move_speed = 5f;
    [SerializeField] float crouch_speed = 2f;
    [SerializeField] float sprint_speed = 8f;
    [SerializeField] float jump_height = 10f;
    [SerializeField] float gravity = -9.81f;
    Vector2 input_movement;
    Vector3 move_dir;
    float vertical_vel;
    bool jump_input_pressed;
    bool sprint_input_pressed;
    bool crouch_input_pressed;
    [Header("Camera Rotation")]
    [SerializeField] Camera cam;
    [SerializeField] Transform camera_pos;
    [SerializeField] float up_tilt_limit = -85f;
    [SerializeField] float down_tilt_limit = 85f;
    [SerializeField] float normal_height = .6f;
    [SerializeField] float crouch_height = .3f;
    float pitch;
    Vector2 look_vec;
    [Range(0,2f)]
    [SerializeField] float sensitivity;
    [Header("Snapping")]
    [SerializeField] Animator hand_anim;
    bool palming;
    [SerializeField] GameObject snap_indicator;
    [SerializeField] float max_snap_distance = 25f;
    void Start()
    {
        character_controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false; 
    }
    void Update()
    {
        CalculateLook();
        CalculateMovement();
        hand_anim.SetBool("palming", palming);
        PositionSnapIndicator();
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
    public void OnLook(InputValue value)
    {
        look_vec = value.Get<Vector2>();
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
        }
        else
        {
            vertical_vel += gravity * Time.deltaTime;
        }

        final_vel.y = vertical_vel;

        character_controller.Move(final_vel * Time.deltaTime);
    }
    void CalculateLook()
    {
        Vector2 final_rot = look_vec * sensitivity * Time.deltaTime * 18;

        transform.Rotate(Vector3.up * final_rot.x);
        
        pitch = Mathf.Clamp(pitch - final_rot.y, up_tilt_limit, down_tilt_limit);
        camera_pos.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        camera_pos.localPosition = new Vector3(0, GetCameraHeight(), 0);
    }
    public void OnAttack(InputValue value)
    {
        hand_anim.SetTrigger("snap");
    }
    public void OnAltAttack(InputValue value)
    {
        palming = value.isPressed;
    }
    void PositionSnapIndicator()
    {
        RaycastHit raycast_hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out raycast_hit, max_snap_distance))
        {
            snap_indicator.gameObject.SetActive(true);
            snap_indicator.transform.position = raycast_hit.point;
        }
        else
        {
            snap_indicator.gameObject.SetActive(false);
        }
    }
}
