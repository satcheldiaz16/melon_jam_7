using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NoiseController : MonoBehaviour
{
    [Header("Audio tracking")]
    public float playerNoise = 0f;
    
    [SerializeField] string[] trackedTags = new string[] { "NoiseSource", "Player", "Baby"};
    
    [SerializeField] AudioSource[] audioSources = new AudioSource[0];

    [Header("UI Elements")]
    public Slider volumeBar;
    public TextMeshProUGUI decibelText;
    public string[] StatusLines = new string[] { "Hidden", "Spotted" };

    [Header("Settings")]
    public float decaySpeed = 5f; // How fast noise fades back down to zero
    private float[] audioSamples = new float[64]; 

    [Header("Status Info")]
    public string curStatus;
    public bool discovered = false;
    private int enemiesChasing = 0;

    void Awake()
    {
        RefreshAudioSources();

        if (volumeBar == null) volumeBar = GetComponentInChildren<Slider>();
        if (decibelText == null) decibelText = GetComponentInChildren<TextMeshProUGUI>();

        if (StatusLines != null && StatusLines.Length > 0) curStatus = StatusLines[0];
    }

    [ContextMenu("Refresh Audio Sources")]
    public void RefreshAudioSources()
    {
        AudioSource[] allSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        List<AudioSource> validSources = new List<AudioSource>();

        foreach (AudioSource src in allSources)
        {
            if (src == null) continue;

            // Check if object or root has any of the allowed tags
            bool isTracked = false;
            foreach (string t in trackedTags)
            {
                try
                {
                    if (src.CompareTag(t) || src.transform.root.CompareTag(t))
                    {
                        isTracked = true;
                        break;
                    }
                }
                catch { /* Tag doesn't exist in Tag Manager, ignore */ }
            }

            if (isTracked)
            {
                validSources.Add(src);
            }
        }

        audioSources = validSources.ToArray();
    }

    void Update()
    {
        int index = 0;

        if (GameManager.instance != null && GameManager.instance.creatures_pursuing_player != null)
        {
            if (GameManager.instance.creatures_pursuing_player.Count != enemiesChasing) 
                index = changeString(GameManager.instance.creatures_pursuing_player.Count);
        }

        

        if (StatusLines != null && StatusLines.Length > 1) 
            curStatus = discovered ? StatusLines[0] : StatusLines[1];

        float highestRms = 0f;

        if (audioSources != null)
        {
            foreach (AudioSource source in audioSources)
            {
                if (source == null) continue;
                if (!source.isPlaying || source.volume <= 0) continue;

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
        }
        if (highestRms < 0.0001f) highestRms = 0.0001f;
        float dB = 20 * Mathf.Log10(highestRms);
        float targetLoudness = Mathf.InverseLerp(-60f, 0f, dB);
        if (targetLoudness > playerNoise)
        {
            playerNoise = targetLoudness;
        }
        else
        {
            playerNoise = Mathf.Lerp(playerNoise, targetLoudness, Time.deltaTime * decaySpeed);
        }

        if (volumeBar != null)
        {
            volumeBar.value = playerNoise;
        }

        if (decibelText != null)
        {
            float displayDb = Mathf.Lerp(30f, 120f, playerNoise);
            decibelText.text = $"{Mathf.RoundToInt(displayDb)} dB";
        }
    }

    
    // Call this from any script to make noise
    public void TriggerImpulseNoise(float amount)
    {
        playerNoise = Mathf.Clamp01(Mathf.Max(playerNoise, amount));
    }

    public int changeString(int enemies)
    {
        enemiesChasing = enemies;
        GameObject instance = GameObject.Find("Mother");
        if (instance) return 3;
        return (enemies < 0) ? 0 : (enemies < 5) ? 1 : 2;
    }
}