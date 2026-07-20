using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class NoiseController : MonoBehaviour
{
    [Header("Audio sources monsters can track")]
    public float playerNoise = 0f;
    [SerializeField] AudioSource[] audioSources;

    [Header("UI Elements")]
    public Slider volumeBar;
    public TextMeshProUGUI decibelText;
    public String[] ThreatLevels;
    public String[] StatusLines;

   [Header("Adjust Settings")]
    public float smoothness = 15f; 
    private float[] audioSamples = new float[64]; 

    [Header("LinesForText")]
    public String curStatus;
    public String curThreat;
    public int curEnemies;
    public bool discovered = false;
    private int enemiesChasing = 0;

    void Awake()
    {
        curStatus = StatusLines[0];
        curEnemies = enemiesChasing;
        curThreat = ThreatLevels[0];
    }

    void Update()
    {
        int index = 0;
        if (GameManager.instance.creatures_pursuing_player.Count != enemiesChasing) index = changeString(GameManager.instance.creatures_pursuing_player.Count);
        curThreat = ThreatLevels[index];
        curEnemies = enemiesChasing;
        curStatus = discovered ? StatusLines[0] : StatusLines[1];

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

    public int changeString(int enemies)
    {
        enemiesChasing = enemies;
        GameObject instance = GameObject.Find("Mother");
        if (instance) return 3;
        return (enemies < 0) ? 0 : (enemies < 5) ? 1 : 2;
        
    }
}
