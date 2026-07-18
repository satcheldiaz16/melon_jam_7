using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    void Awake()
    {
        if(instance = null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(this);
    }
}
