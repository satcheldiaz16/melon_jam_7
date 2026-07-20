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
    [SerializeField] Image BackgroundBar;
    [SerializeField] RectTransform FearBarRect;
    [SerializeField] RectTransform BackgroundBarRect;
    [SerializeField] Image[] skeletonParts;
    [SerializeField] Color endSkeletonColor;
    [SerializeField] Sprite startSkeleton;
    [SerializeField] Sprite endSkeleton;
    [SerializeField] Image skeleton;

    [Header("Text Settings")]
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float fillMoveSpeed = .05f;
    private float targetFill = 0f;
    private float displayedFill = 0f;
    [SerializeField] float FlashInterval = .3f;
    [SerializeField] Color FlashColor = Color.red;
    [SerializeField] Color StartingColor = Color.white;
    private Coroutine flashCoroutine;
    private bool isFlashing = false;
    



    public void Update()
    {
        displayedFill = Mathf.MoveTowards(displayedFill, targetFill, fillMoveSpeed * Time.deltaTime);
        FearBar.fillAmount = displayedFill;

        float filledPixels = displayedFill * FearBarRect.rect.height;
        float backgroundFraction = filledPixels / BackgroundBarRect.rect.height;
        BackgroundBar.fillAmount = Mathf.Clamp01(backgroundFraction);
    }

    public void UpdateUI(float fear)
    {
        float currentFear = baby.fear;
        if (currentFear >= baby.fearLimit)
        {
            skeleton.sprite = endSkeleton;
            BackgroundBar.color = new Color(BackgroundBar.color.r, BackgroundBar.color.g, BackgroundBar.color.b, 0f);
        }
        else
        {
            skeleton.sprite = startSkeleton;
            BackgroundBar.color = new Color(BackgroundBar.color.r, BackgroundBar.color.g, BackgroundBar.color.b, 1f);
        }
        float fill = currentFear / baby.maxFear;
        targetFill = fill;
        if (baby.fear >= baby.fearLimit) SetFlashingState(true);
        else SetFlashingState(false);
        UpdateSkeleton(fill);
    }

    public void UpdateSkeleton(float percent)
    {
         Color lerped = Color.Lerp(StartingColor, endSkeletonColor, percent);
        lerped.a = 1f; 
        foreach (Image img in skeletonParts)
        {
            img.color = lerped;
        }
    }

    private void Awake()
    {
        fillMoveSpeed = (baby.fearGain / baby.maxFear) / baby.fearGainInterval;
        text.color = StartingColor;
        foreach (Image img in skeletonParts)
        {
            img.color = StartingColor;
        }
    }

    public void SetFlashingState(bool shouldFlash)
    {
        if (shouldFlash == isFlashing) return;
        isFlashing = shouldFlash;

        if (isFlashing) 
        {
            flashCoroutine = StartCoroutine(FlashRoutine());
            text.color = FlashColor;
        }
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
