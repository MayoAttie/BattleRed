using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarManager : MonoBehaviour
{
    [SerializeField]
    protected Image Hpbar;


    protected Color fullHpColor = Color.green; // 100% 체력일 때의 색상
    protected Color midHpColor = Color.yellow; // 50% 체력일 때의 색상
    protected Color zeroHpColor = Color.red; // 0% 체력일 때의 색상

    protected private float changeRate = 1f; // 체력이 얼마나 빠르게 깎일지 조절하는 값
    protected float currentHP;


    #region 캐릭터 체력바

    public void HpBarFill_Init(float initialHp)
    {
        currentHP = initialHp;
    }

    public void HpBarFill_End(float maxHp, float targetHp, bool isRecovery)
    {
        // 체력 초기화
        if (currentHP == targetHp)
            UpdateHpBar(maxHp, targetHp);
        if (gameObject.activeSelf)
        {
            changeRate = maxHp * 0.05f;
            // 회복 코루틴 호출
            StartCoroutine(UpdateHpOverTime(maxHp, targetHp, isRecovery));
        }
    }

    // 최대체력, 변경될 목표 체력, 회복인지 아닌지 판단
    private IEnumerator UpdateHpOverTime(float maxHp, float targetHp, bool isRecovery)
    {
        if (isRecovery)
        {
            while (currentHP < targetHp)
            {
                currentHP += changeRate;
                UpdateHpBar(maxHp, currentHP);

                if (!gameObject.activeSelf) // 게임 오브젝트가 비활성화되었다면
                    yield break; // 즉시 코루틴 종료

                yield return new WaitForSeconds(0.01f);

                if (currentHP > targetHp)
                    currentHP = targetHp;
            }
        }
        else
        {
            while (currentHP > targetHp)
            {
                currentHP -= changeRate;
                UpdateHpBar(maxHp, currentHP);

                if (!gameObject.activeSelf) // 게임 오브젝트가 비활성화되었다면
                    yield break; // 즉시 코루틴 종료

                yield return new WaitForSeconds(0.01f);

                if (currentHP < targetHp)
                    currentHP = targetHp;
            }
        }
    }

    protected void UpdateHpBar(float maxHp, float hp)
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
}
