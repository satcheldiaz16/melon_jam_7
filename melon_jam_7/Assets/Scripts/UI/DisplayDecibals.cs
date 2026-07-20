using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayDecibals : MonoBehaviour
{
    [SerializeField] NoiseController noiseController;
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponentInParent<TextMeshProUGUI>();
    }

    public void Update()
    {
        float displayDb = Mathf.Lerp(0f, 120f, noiseController.playerNoise);
        text.text = $"{Mathf.RoundToInt(displayDb)} dB";
    }
}
