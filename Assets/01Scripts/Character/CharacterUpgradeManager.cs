using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUpgradeManager
{
    public static void WeaponExpUp_Upgrade(WeaponAndEquipCls weapon, int nWeaponUpgradeExp)
    {
        int curExp = weapon.GetCurrentExp();
        int nextExp = curExp + nWeaponUpgradeExp;

        weapon.SetCurrentExp(nextExp);

        while(weapon.GetCurrentExp() >= weapon.GetMaxExp())
        {
            if(weapon.GetLimitLevel()>weapon.GetLevel())
            {
                int curLevel = weapon.GetLevel();
                weapon.SetLevel(curLevel+1);
                weapon.SetMaxExp((int)(weapon.GetMaxExp()*1.6f));
            }
            else
            {

            }

        }
    }
}
