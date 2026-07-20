using UnityEngine;

public class BabyCollectible : MonoBehaviour
{
    [SerializeField] AudioSource tree_sfx;
    [SerializeField] AudioSource tree_mono;
    [SerializeField] AudioSource baby_mono;
    [SerializeField] float baby_monolouge_length = 20f;
    [SerializeField] BabyManager baby;
    void OnTriggerEnter(Collider other)
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        tree_sfx.Stop();
        tree_mono.Stop();

        baby_mono.Play();
        baby.gameObject.SetActive(true);
        PlayerController.instance.GetComponent<PlayerSnap>().snap_acquired = true;
        Invoke("BeginFearLoss", baby_monolouge_length);
    }
    public void BeginFearLoss()
    {
        
    }
}
