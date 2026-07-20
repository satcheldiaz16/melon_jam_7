using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using UnityEditor.Experimental.GraphView;


public class BabyManager : MonoBehaviour
{ 
    [Header("Fear")]
    public float fear = 0;
    public bool isAfraid = false;
    public float numToScare = 50;
    [SerializeField] public float maxFear = 200f;
    [SerializeField] public float fearLimit = 100f;
    [SerializeField] float fearGain = 10f;
    [Header("UI")]
    [SerializeField] private FearMeter fearMeter;
    public bool comfort;

    [Header("Timers")]
    [SerializeField] float actionTime = 5f;
    [SerializeField] float fearGainInterval = 1f;
    [SerializeField] float cryInterval = 3f;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource cryAudioSource;
    [SerializeField] AudioClip[] cryLines;
    [SerializeField] AudioClip[] chillLines;
    [SerializeField] AudioClip[] scaredLines;
    private bool crying = false;
    
    private Coroutine fearCoroutine;
    private Coroutine calmCoroutine;
    private Coroutine cryCoroutine;


    public void Start()
    {
        if (fearMeter == null) fearMeter = Object.FindFirstObjectByType<FearMeter>();
        StartCoroutine(Action());
    }

    public IEnumerator Action()
    {
        while (this.gameObject.activeSelf)
        {
            yield return new WaitForSeconds(actionTime);
            if (crying) continue;
            DecideAction();
        }
    }

    public void DecideAction()
    {
        if (isAfraid && fear > numToScare)
        { 
            // baby is afraid but not crying
            audioSource.clip = (GetRandomLine(scaredLines));
            audioSource.Play();  
        }
        else
        {
            // chill
            audioSource.clip = (GetRandomLine(chillLines));
            audioSource.Play();
        }
    }

    // Increase fear when scared
    public IEnumerator GetScared()
    {
        while (isAfraid)
        {
            bool checkCry = fear > fearLimit;
            CallCry(checkCry);

            fear += fearGain;
            fear = Mathf.Clamp(fear, 0f, maxFear);
            if (fearMeter != null) fearMeter.UpdateUI(fear);
            yield return new WaitForSeconds(fearGainInterval);
        }
        fearCoroutine = null;
    }

    // Decrease fear when calm
    public IEnumerator GetCalm()
    {
        while (!isAfraid)
        {
            bool checkCry = fear > fearLimit;
            CallCry(checkCry);

            fear -= fearGain;
            fear = Mathf.Clamp(fear, 0f, maxFear);
            if (fearMeter != null) fearMeter.UpdateUI(fear);
            yield return new WaitForSeconds(fearGainInterval);
        }
        calmCoroutine = null;
    }

    public void CallCry(bool isCrying)
    {
        crying = isCrying;
        if (isCrying && cryCoroutine != null) return;

        if (isCrying && cryCoroutine == null)
        {
            cryCoroutine = StartCoroutine(CryEnum());
        }
        else 
        {
            if (cryCoroutine != null) StopCoroutine(CryEnum());
            cryCoroutine = null;
        }
    }

    public IEnumerator CryEnum()
    {
        cryAudioSource.clip = (GetRandomLine(cryLines));
        cryAudioSource.Play();
        yield return new WaitForSeconds(cryInterval);
    }

    // Afraid setter called by FearCon 
    public void CheckFear(bool value)
    {
        isAfraid = value;
        if (isAfraid && fearCoroutine == null) 
        {
            if (calmCoroutine != null)
            {
                StopCoroutine(calmCoroutine);
                calmCoroutine = null;
            }

            fearCoroutine = StartCoroutine(GetScared());
        }
        else if (!isAfraid && calmCoroutine == null)
        {
            if (fearCoroutine != null)
            {
                StopCoroutine(fearCoroutine);
                fearCoroutine = null;
            }
            calmCoroutine = StartCoroutine(GetCalm());
        }
    }
    public static AudioClip GetRandomLine(AudioClip[] getLine)
    {
        int index = Random.Range(0, getLine.Length);
        return getLine[index];
    }
   
}
