using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EquipmentSetSynergyMng : MonoBehaviour
{
    // 공격력+18%
    public static void Hangja_Heart_2(CharacterClass userData, bool isActive)
    {
        int curAtk = userData.GetAttack();
        if(isActive == true)
        {
            int tmp = (int)(curAtk * 0.18f);
            userData.AddEquipSetApplied("행자의 마음", 2, tmp);
            userData.SetAttack(curAtk+tmp);
        }
        else
        {
            // 장비 해제에 따른 값 감산
            int tmp = (int)userData.GetEquipSetApplied("행자의 마음", 2);
            userData.SetAttack(curAtk - tmp);
        }
    }

    // HP가 50%를 초과하는 적에게 가하는 피해가 30% 증가한다
    public static int Hangja_Heart_4(CharacterClass userData, Monster monsterData, int damage)
    {
        int reviseDamage = 0;
        if(monsterData.GetMonsterCurrentHp()>=(monsterData.GetMonsterMaxHp()/2))
        {
            int tmp = (int)(damage * 0.3f);
            reviseDamage = damage + tmp;
            userData.AddEquipSetApplied("행자의 마음",4,tmp);   
        }

        return reviseDamage;
    }

    // 치명타 확률+12%
    public static void JeonjaengGwang_2(CharacterClass userData, bool isActive)
    {
        float curCiriticalRate = userData.GetCriticalPercentage();
        if(isActive == true)
        {
            float tmp = curCiriticalRate * 0.12f;
            userData.AddEquipSetApplied("전투광", 2, tmp);
            userData.SetCriticalPersentage(curCiriticalRate + tmp);
        }
        else
        {
            // 장비 해제에 따른 값 감산
            float tmp = userData.GetEquipSetApplied("전투광", 2);
            userData.SetCriticalPersentage(curCiriticalRate-tmp);
        }

    }

    // HP 70% 미만 시 치명타 확률이 추가로 24% 증가한다
    public static void JeonjaengGwang_4(CharacterClass userData)
    {
        if (userData.GetCurrentHp() <= userData.GetMaxHp() * 0.7)
        {
            float curCriticalRate = userData.GetCriticalPercentage();
            userData.AddEquipSetApplied("전투광", 4, curCriticalRate);
        }
    }

    // 공격력+18%
    public static void BloodKnightChivalry_2(CharacterClass userData, bool isActive)
    {
        int curAtk = userData.GetAttack();
        if(isActive == true)
        {
            int tmp = (int)(curAtk * 0.18f);
            userData.AddEquipSetApplied("피에 물든 기사도", 2, tmp);
            userData.SetAttack(curAtk + tmp);
        }
        else
        {
            // 장비 해제에 따른 값 감산
            int tmp = (int)userData.GetEquipSetApplied("피에 물든 기사도", 2);
            userData.SetAttack(curAtk - tmp);
        }

    }

    // 해당 성유물 세트를 장착한 캐릭터가 한손검, 양손검, 장병기를 사용 시 캐릭터의 일반 공격으로 가하는 피해가 35% 증가한다
    public static void BloodKnightChivalry_4(CharacterClass userData)
    {

    }
}
