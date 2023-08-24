using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : Singleton<UI_Manager>
{
    [SerializeField]
    Image Hpbar;
    [SerializeField]
    GameObject monster_HPbar;


    Color fullHpColor = Color.green; // 100% 체력일 때의 색상
    Color midHpColor = Color.yellow; // 50% 체력일 때의 색상
    Color zeroHpColor = Color.red; // 0% 체력일 때의 색상

    private float changeRate = 1f; // 체력이 얼마나 빠르게 깎일지 조절하는 값
    float currentHP;


    #region 캐릭터 체력바

    public void HpBarFill_Init(float initialHp)
    {
        currentHP = initialHp;
    }

    public void HpBarFill_End(float maxHp, float targetHp, bool isRecovery)
    {
        // 체력 초기화
        if(currentHP == targetHp)
            UpdateHpBar(maxHp, targetHp);
        // 회복 코루틴 호출
        StartCoroutine(UpdateHpOverTime(maxHp, targetHp, isRecovery));
    }

    // 최대체력, 변경될 목표 체력, 회복인지 아닌지 판단
    private IEnumerator UpdateHpOverTime(float maxHp, float targetHp, bool isRecovery)
    {
        if(isRecovery)  // 회복일경우
        {
            while (currentHP < targetHp)
            {
                currentHP += changeRate;
                UpdateHpBar(maxHp, currentHP);
                yield return new WaitForSeconds(0.01f); // 프레임당 일정 시간 간격으로 체력 감소

                if (currentHP > targetHp)
                    currentHP = targetHp; // 목표 체력 이하로 내려가지 않도록 보정
            }
        }
        else    //데미지일경우
        {
            while (currentHP > targetHp)
            {
                currentHP -= changeRate;
                UpdateHpBar(maxHp, currentHP);
                yield return new WaitForSeconds(0.01f); // 프레임당 일정 시간 간격으로 체력 감소

                if (currentHP < targetHp)
                    currentHP = targetHp; // 목표 체력 이하로 내려가지 않도록 보정
            }
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
    #endregion

    #region 몬스터 체력바



    #endregion

}
