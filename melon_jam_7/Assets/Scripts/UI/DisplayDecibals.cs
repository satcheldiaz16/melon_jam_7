using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DisplayDecibals : MonoBehaviour
{
    private Text text;
    private float decibal;
    [SerializeField] NoiseController noiseController;

    private void Awake()
    {
        text = GetComponentInParent<Text>();
    }

    public void Update()
    {
        decibal = noiseController.playerNoise;
        text.text = decibal.ToString();
    }
}
