using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NoiseController : MonoBehaviour
{
    [Header("Audio sources monsters can track")]
    AudioSource[] audioSources;

    [Header("UI Elements")]
    public Slider volumeBar;
    public TextMeshProUGUI decibelText;
    public String[] EnemyPursuitLines;
    public String[] ThreatLevels;
    public String[] StatusLines;


   [Header("Adjust Settings")]
    public float smoothness = 15f; 
    private float[] audioSamples = new float[64]; 

    [Header("References")]
    [SerializeField] GameManager gameManager;

    void Update()
    {
        float highestRms = 0f;
        foreach (AudioSource source in audioSources)
        {
            if (source == null || !source.isPlaying || source.volume <= 0) 
                continue;
            source.GetOutputData(audioSamples, 0);
            float sum = 0;
            foreach (float sample in audioSamples)
            {
                sum += sample * sample;
            }
            float rmsValue = Mathf.Sqrt(sum / audioSamples.Length);
            rmsValue *= source.volume;
            if (rmsValue > highestRms)
            {
                highestRms = rmsValue;
            }
        }

        if (highestRms < 0.0001f) highestRms = 0.0001f;

        float dB = 20 * Mathf.Log10(highestRms);
        float normalizedLoudness = Mathf.InverseLerp(-60f, 0f, dB);
        volumeBar.value = Mathf.Lerp(volumeBar.value, normalizedLoudness, Time.deltaTime * smoothness);
        float displayDb = Mathf.Lerp(30f, 120f, normalizedLoudness);
        decibelText.text = $"{Mathf.RoundToInt(displayDb)} dB";
    }
}
