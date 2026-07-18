using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerSnap : MonoBehaviour
{
    [Header("Snapping")]
    [SerializeField] Animator hand_anim;
    [SerializeField] Camera cam;
    [SerializeField] LayerMask snap_layers;
    bool palming;
    [SerializeField] GameObject snap_indicator;
    [SerializeField] GameObject spark_prefab;
    [SerializeField] float max_snap_distance = 25f;
    [SerializeField] AudioSource spark_sfx;
    void Update()
    {
        hand_anim.SetBool("palming", palming);
        PositionSnapIndicator();
    }
    public void OnAttack(InputValue value)
    {
        hand_anim.SetTrigger("snap");

        GameObject spark = Instantiate(
            spark_prefab,
            snap_indicator.transform.position,
            Quaternion.identity
        );
        spark.GetComponent<Spark>().Ignite();
        
        spark_sfx.Play();
    }
    public void OnAltAttack(InputValue value)
    {
        palming = value.isPressed;
    }
    void PositionSnapIndicator()
    {
        RaycastHit raycast_hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out raycast_hit, max_snap_distance, snap_layers))
        {
            snap_indicator.gameObject.SetActive(true);
            snap_indicator.transform.position = raycast_hit.point + raycast_hit.normal * .05f;
        }
        else
        {
            snap_indicator.gameObject.SetActive(false);
        }
    }
}
