using UnityEngine;
using System.Collections;


public class BabyManager : MonoBehaviour
{
    #region Vars
    [Header("Fear")]
    public float fear = 0;
    public bool isAfraid = false;
    public float numToScare = 50;
    public float maxFear = 200f;
    public float fearLimit = 100f;
    public float fearGain = 10f;
    [Header("UI")]
    [SerializeField] FearMeter fearMeter;

    [Header("Comfort")]
    public bool comfort;
    private Coroutine comfortCoroutine = null;
    [SerializeField] float relaxPoints = 10f;

    [Header("Timers")]
    [SerializeField] float actionTime = 5f;
    public float fearGainInterval = 1f;
    [SerializeField] float cryInterval = 3f;
    [SerializeField] float comfortTimer = 1f;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource cryAudioSource;
    [SerializeField] AudioClip[] cryLines;
    [SerializeField] AudioClip[] chillLines;
    [SerializeField] AudioClip[] scaredLines;
    private bool crying = false;
    private Coroutine fearCoroutine = null;
    private Coroutine calmCoroutine = null;
    private Coroutine cryCoroutine = null;
    #endregion

    #region ChooseActions
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
    #endregion

    #region FearControls
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
    #endregion
    
    #region Crying
    public IEnumerator CryEnum()
    {
        cryAudioSource.clip = (GetRandomLine(cryLines));
        audioSource.loop = true;
        cryAudioSource.Play();
        yield return new WaitForSeconds(cryInterval);
    }
    public void CallCry(bool isCrying)
    {
        crying = isCrying;
        if (!isCrying)
        {
            if (cryCoroutine != null) StopCoroutine(CryEnum());
            audioSource.loop = false;
            cryCoroutine = null;
        }

        if (isCrying && cryCoroutine != null) return;
        else 
        {
            cryCoroutine = StartCoroutine(CryEnum());
        }
    }
    #endregion

    #region Comfort 
    public void SetComfort (bool isComforted)
    {
        if (isComforted) 
        {
            if (comfortCoroutine != null) return;
            else comfortCoroutine = StartCoroutine(ComfortLoop());
        }
        else
        {
            if (comfortCoroutine == null) return;
            else comfortCoroutine = null;
        }
    }

    public IEnumerator ComfortLoop ()
    {
        fear -= relaxPoints;
        fear = Mathf.Clamp(fear, 0f, maxFear);
        if (fearMeter != null) fearMeter.UpdateUI(fear);
        yield return new WaitForSeconds(comfortTimer);
    }
    #endregion
    
    public void Start()
    {
        if (fearMeter == null) fearMeter = Object.FindFirstObjectByType<FearMeter>();
        StartCoroutine(Action());
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

    // Fetch random clip from list
    public static AudioClip GetRandomLine(AudioClip[] getLine)
    {
        int index = Random.Range(0, getLine.Length);
        return getLine[index];
    }

}
