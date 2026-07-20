using UnityEngine;

public class Log : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
        Debug.Log(LightSource.Active.Count);
    }
}
