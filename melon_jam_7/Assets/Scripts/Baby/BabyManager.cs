using UnityEngine;
using System.Collections;
using System.Security;
using System.Diagnostics.Contracts;

public class BabyManager : MonoBehaviour
{ 
    [Header("Fear")]
    public float fear = 0;
    public bool isAfraid = false;
    [SerializeField] float maxFear;
    [SerializeField] float fearLimit;
    [SerializeField] float fearGain;

    [Header("Timers")]
    [SerializeField] float actionTime;
    [SerializeField] float fearGainInterval;
    [SerializeField] float RandEventTimer;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] cryLines;
    [SerializeField] AudioClip[] chillLines;
    [SerializeField] AudioClip[] scaredLines;
    
    private Coroutine fearCoroutine;
    private Coroutine calmCoroutine;


    public void Start()
    {
        StartCoroutine(Action());
    }

    public IEnumerator Action()
    {
        while (this.gameObject.activeSelf)
        {
            DecideAction();
            yield return new WaitForSeconds(actionTime);
        }
    }

    public void DecideAction()
    {
        if (isAfraid)
        {
            if (fear >= fearLimit)
            {
                // baby is scared and crying
                audioSource.clip = (GetRandomLine(cryLines));
                audioSource.Play();
                while(isAfraid && fear >= fearLimit)
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource.clip = (GetRandomLine(cryLines));
                        audioSource.Play();
                    }
                }
            }
            else
            {
                // baby is afraid but not crying
                audioSource.clip = (GetRandomLine(scaredLines));
                audioSource.Play();
            }
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
            if (fear < maxFear)
            {
                fear += fearGain;
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

            StartCoroutine(GetScared());

        }
        else if (!isAfraid && calmCoroutine == null)
        {
            if (fearCoroutine != null)
            {
                StopCoroutine(fearCoroutine);
                fearCoroutine = null;
            }
            StartCoroutine(GetCalm());
        }
    }

    public static AudioClip GetRandomLine(AudioClip[] randomLines)
    {
        return randomLines[Random.Range(0, randomLines.Length)];
    }

   
}
