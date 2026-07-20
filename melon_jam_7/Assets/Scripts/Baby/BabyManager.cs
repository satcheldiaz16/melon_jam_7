using UnityEngine;
using System.Collections;


public class BabyManager : MonoBehaviour
{ 
    [Header("Fear")]
    public float fear = 0;
    public bool isAfraid = false;
    public float numToScare = 50;
    [SerializeField] public float maxFear = 200f;
    [SerializeField] public float fearLimit = 100f;
    [SerializeField] float fearGain = 5f;
    [Header("UI")]
    [SerializeField] private FearMeter fearMeter;

    [Header("Timers")]
    [SerializeField] float actionTime = 5f;
    [SerializeField] float fearGainInterval = 1f;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] cryLines;
    [SerializeField] AudioClip[] chillLines;
    [SerializeField] AudioClip[] scaredLines;
    
    private Coroutine fearCoroutine;
    private Coroutine calmCoroutine;


    public void Start()
    {
        if (fearMeter == null) fearMeter = Object.FindFirstObjectByType<FearMeter>();
        StartCoroutine(Action());
    }

    public IEnumerator Action()
    {
        while (this.gameObject.activeSelf)
        {
            //Debug.Log("Deciding Action");
            DecideAction();
            yield return new WaitForSeconds(actionTime);
        }
    }

    public void DecideAction()
    {
        Debug.Log("Action Called, Afraid status is " + isAfraid);
        if (isAfraid)
        {
            if (fear >= fearLimit)
            {
                // baby is scared and crying
                Debug.Log("Deciding Afraid + crying");
                audioSource.clip = (GetRandomLine(cryLines));
                audioSource.Play();
            }
            else if (fear >= numToScare)
            {
                // baby is afraid but not crying
                Debug.Log("Deciding Afraid but not crying");
                audioSource.clip = (GetRandomLine(scaredLines));
                audioSource.Play();
            }
        }
        else
        {
            // chill
            Debug.Log("Deciding chilling");
            audioSource.clip = (GetRandomLine(chillLines));
            audioSource.Play();
        }
    }

    // Increase fear when scared
    public IEnumerator GetScared()
    {
        while (isAfraid)
        {
            if (fear < maxFear)
            {
                fear += fearGain;
                fear = Mathf.Clamp(fear, 0f, maxFear);
                if (fearMeter != null) fearMeter.UpdateUI(fear);
            }
            yield return new WaitForSeconds(fearGainInterval);
        }
        fearCoroutine = null;

    }

    // Decrease fear when calm
    public IEnumerator GetCalm()
    {
        while (!isAfraid)
        {
            fear -= fearGain;
            fear = Mathf.Clamp(fear, 0f, maxFear);
            if (fearMeter != null) fearMeter.UpdateUI(fear);
            yield return new WaitForSeconds(fearGainInterval);
        }
        calmCoroutine = null;
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
