using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class FearMeter : MonoBehaviour
{
    [Header("Baby Ref")]
    [SerializeField] BabyManager baby;

    [Header("UI Reference")]
    [SerializeField] Image FearBar;
    [SerializeField] Image HeadSpine;
    [SerializeField] Image Ribs;
    [SerializeField] Color endSkeletonColor;

    [Header("Text Settings")]
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float FlashInterval = .3f;
    [SerializeField] Color FlashColor = Color.red;
    [SerializeField] Color StartingColor = Color.white;
    private Coroutine flashCoroutine;
    private bool isFlashing = false;
    


    public void UpdateUI(float fear)
    {
        float currentFear = baby.fear;
        float fill = currentFear / baby.maxFear;
        FearBar.fillAmount = fill;
        if (baby.fear >= baby.fearLimit) SetFlashingState(true);
        else SetFlashingState(false);
        UpdateSkeleton(fill);
    }

    public void UpdateSkeleton(float percent)
    {
        HeadSpine.color = Color.Lerp(StartingColor, endSkeletonColor, percent);
        Ribs.color = Color.Lerp(StartingColor, endSkeletonColor, percent);
    }

    private void Awake()
    {
        text.color = StartingColor;
        HeadSpine.color = StartingColor;
        Ribs.color = StartingColor;
    }

    public void SetFlashingState(bool shouldFlash)
    {
        if (shouldFlash == isFlashing) return;
        isFlashing = shouldFlash;

        if (isFlashing) flashCoroutine = StartCoroutine(FlashRoutine());
        else
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            text.color = StartingColor;
            text.enabled = true; 
        }
    }

    private IEnumerator FlashRoutine()
    {
        while (isFlashing)
        {
            text.enabled = !text.enabled;
            yield return new WaitForSeconds(FlashInterval);
        }
    }

}
