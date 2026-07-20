using UnityEngine;

public class KillPlayerOnCollision : MonoBehaviour
{
    [SerializeField] string cause_of_death;
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out PlayerController player))
        {
            if(!player.dead) player.Die(cause_of_death);
        }
    }
}
