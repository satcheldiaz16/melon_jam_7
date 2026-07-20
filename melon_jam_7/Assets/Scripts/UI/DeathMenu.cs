using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class DeathMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI cause_of_text;
    public void SetCauseText(string cause){cause_of_text.text = cause;} 
    public void TryAgain()
    {
        GetComponent<Animator>().SetTrigger("Disappear");
        Invoke("ReloadScene", 4f);
    }
    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true; 
    }
    public void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
