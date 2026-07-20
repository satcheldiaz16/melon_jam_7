using UnityEngine;

public class Win : MonoBehaviour
{
    bool activated = false;
    [SerializeField] Animator fader;
    [SerializeField] GameObject win_screen;
    [SerializeField] AudioSource win_dialouge;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerController p) && !activated)
        {
            p.grabbed = true;
            activated = true;
            fader.SetTrigger("win");
            win_dialouge.Play();
            Invoke("Thing", 2);
        }
    }
    public void Thing() => win_screen.SetActive(true);
}
