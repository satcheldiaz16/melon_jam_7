using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FingerPain : MonoBehaviour
{
    [SerializeField] Image kid_named;
    [SerializeField] Sprite[] sprites;
    [SerializeField] Color[] colors;
    [SerializeField] TextMeshProUGUI pain_text;
    void Start()
    {
        SetPainLevel(0);
    }
    public void SetPainLevel(int idx)
    {
        kid_named.sprite = sprites[idx];
        pain_text.color = colors[idx];
        pain_text.text = idx == 3 ? "In Pain" : "Pain";
    }

}
