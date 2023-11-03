using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillBar_Script : MonoBehaviour
{
    [SerializeField]
    private bool fill_decoImg_moveActive;

    private Image fillbar_in_image;
    private Image fillbar_deco_image;
    private float fCurrnetFill_value;

    private void Awake()
    {
        fillbar_in_image = transform.GetChild(0).GetComponent<Image>();
        fillbar_deco_image = fillbar_in_image.transform.GetChild(0).GetComponent<Image>();
    }

    public void FillAmountEnergyBar(float value)
    {
        // value를 [0, 1] 범위로 클램프하여 채우기
        fCurrnetFill_value = Mathf.Clamp01(value);
        fillbar_in_image.fillAmount = fCurrnetFill_value;

        if (fill_decoImg_moveActive)
        {
            // fillbar_deco_image의 위치를 채워진 이미지의 끝으로 이동
            fillbar_deco_image.rectTransform.anchoredPosition = new Vector2(
                fillbar_in_image.rectTransform.rect.width * fCurrnetFill_value, 0f);
        }
    }

    public float GetCurrentFill_Value() { return fCurrnetFill_value; }

    public void ResetFillingBar()
    {

    }
}
