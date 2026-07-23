using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Camera Rotation")]
    [SerializeField] Camera cam;
    [SerializeField] Transform camera_pos;
    [SerializeField] float up_tilt_limit = -85f;
    [SerializeField] float down_tilt_limit = 85f;
    float pitch;
    Vector2 look_vec;
    void Update()
    {
        CalculateLook();
    }
    public void OnLook(InputValue value)
    {
        look_vec = value.Get<Vector2>();
    }
    void CalculateLook()
    {
        Vector2 final_rot = look_vec * PauseMenu.Sensitivity * .5f;

        transform.Rotate(Vector3.up * final_rot.x);
        
        pitch = Mathf.Clamp(pitch - final_rot.y, up_tilt_limit, down_tilt_limit);
        camera_pos.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
