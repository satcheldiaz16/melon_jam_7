using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public PlayerInput input;
    public static float Sensitivity = .5f;
    public static float Volume = 1;
    [SerializeField] Slider volume_slider;
    [SerializeField] Slider sens_slider;
    [SerializeField] AudioMixer mixer;
    void Start()
    {
        sens_slider.value = Sensitivity;
        volume_slider.value = Volume;
    }
    public void OnEnable()
    {
        input.actions.FindActionMap("Player").Disable();
        input.actions.FindActionMap("UI").Enable();
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true; 
    }
    public void SetVolume()
    {
        Volume = volume_slider.value;              // now ranges 0.001 .. 10
        float dB = Mathf.Log10(Volume) * 20f;      // log10(10)*20 = +20 at the top
        mixer.SetFloat("MasterVolume", dB);
    }
    public void SetSens()
    {
        Sensitivity = sens_slider.value;
    }
    public void OnDisable()
    {
        input.actions.FindActionMap("Player").Enable();
        input.actions.FindActionMap("UI").Disable();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false; 
    }
    public void Quit()
    {
        Application.Quit();
    }
}
