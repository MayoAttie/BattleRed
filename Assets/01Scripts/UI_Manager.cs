using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : Singleton<UI_Manager>
{
    [SerializeField]
    Image Hpbar;

    Color fullHpColor = Color.green; // 100% 체력일 때의 색상
    Color midHpColor = Color.yellow; // 50% 체력일 때의 색상
    Color zeroHpColor = Color.red; // 0% 체력일 때의 색상

    private float changeRate = 0.02f; // 체력이 얼마나 빠르게 깎일지 조절하는 값

    float initialHp;

    public void HpBarFill_Init(float maxHp, float initialHp)
    {
        UpdateHpBar(maxHp, initialHp);
    }

    public void HpBarFill_End(float maxHp, float targetHp)
    {
        StartCoroutine(UpdateHpOverTime(maxHp, targetHp));
    }

    private IEnumerator UpdateHpOverTime(float maxHp, float targetHp)
    {
        while (initialHp > targetHp)
        {
            initialHp -= changeRate;
            UpdateHpBar(maxHp, initialHp);
            yield return new WaitForSeconds(0.1f); // 프레임당 일정 시간 간격으로 체력 감소

            if (initialHp < targetHp)
                initialHp = targetHp; // 목표 체력 이하로 내려가지 않도록 보정
        }
    }

    private void UpdateHpBar(float maxHp, float hp)
    {
        float fillAmount = hp / maxHp;
        Hpbar.fillAmount = fillAmount;

        Color lerpedColor;

        if (fillAmount >= 0.5f)
        {
            lerpedColor = Color.Lerp(midHpColor, fullHpColor, (fillAmount - 0.5f) * 2);
        }
        else
        {
            lerpedColor = Color.Lerp(zeroHpColor, midHpColor, fillAmount * 2);
        }

        Hpbar.color = lerpedColor;
    }
}
