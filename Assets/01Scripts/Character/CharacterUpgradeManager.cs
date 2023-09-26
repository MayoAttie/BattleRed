using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUpgradeManager
{
    // 돌파 상태 도달 시, true 반환
    public static bool WeaponExpUp_Upgrade(WeaponAndEquipCls weapon, int nWeaponUpgradeExp)
    {
        int curExp = weapon.GetCurrentExp();
        int nextExp = curExp + nWeaponUpgradeExp;
        bool isLimit = false;
        weapon.SetCurrentExp(nextExp);

        while(weapon.GetCurrentExp() >= weapon.GetMaxExp())
        {
            if(weapon.GetLimitLevel()>weapon.GetLevel())
            {
                int curLevel = weapon.GetLevel();
                weapon.SetLevel(curLevel+1);
                weapon.SetMaxExp((int)(weapon.GetMaxExp()*1.6f));
            }
            else // 돌파단계 도달
            {
                weapon.SetCurrentExp(weapon.GetMaxExp());
                isLimit = true; ;
                break;
            }
        }
        // 장비의 메인 및 서브 스탯 데이터 수정
        GameManager.Instance.WeaponAndEquipItemStatusSet(weapon);
        return isLimit;
    }

    // 돌파 시, 다음 능력치 리턴(0-메인스탯, 1-서브스탯)
    public static float[] WeapomLimitBreakStateUp(WeaponAndEquipCls weapon)
    {
        float[] result = new float[2];

        int nextLevel = weapon.GetLimitLevel() + 1;
        var nextDataList = GameManager.Instance.GetList_WeaponAndEquipDataBase();
        var fullData =  nextDataList.Find(finder => finder.Item1.Equals(weapon.GetName()));
        var nextData = fullData.Item2.Find(tmp => tmp.LEVEL.Equals(nextLevel));
        result[0] = nextData.MAIN_STAT;
        result[1] = nextData.SUB_STAT;

        return result;
    }
}
