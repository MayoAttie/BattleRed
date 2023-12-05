using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSetSynergyMng : MonoBehaviour
{
    // isStartTime == 전투 데미지 계산 시작 페이즈에 참조되는 타입   EX : <JeonjaengGwang_4>
    // !isStartTime == 전투 데미지 마지막 단계에서 참조되는 타입  EX :  <JeonjaengGwang_4>
    public static void SetSynergyCheckStarterAtPlaying(bool isStartTime, CharacterClass userData, MonsterManager monsterData, int damage)
    {
        var character = GameManager.Instance.GetUserClass().GetUserCharacter();

        // 복사본을 만들어, 탐색.
        // 원본은 수정될 가능성이 있음.
        var synergyData = character.GetEquipSetAppliedDictionary();
        Dictionary<Tuple<string, int>, float> copiedSynergyData = new Dictionary<Tuple<string, int>, float>(synergyData);

        foreach (var data in copiedSynergyData)
        {
            if(isStartTime)
            {
                if (data.Key.Item1.Equals("전투광") && data.Key.Item2 == 4)
                {
                    JeonjaengGwang_4(userData, true,false);
                }
            }
            else
            {
                if (data.Key.Item1.Equals("행자의 마음") && data.Key.Item2 == 4)
                {
                    var mobCls = monsterData.GetMonsterClass();
                    monsterData.GetMonsterHPMng().HpBarFill_Init(mobCls.GetMonsterCurrentHp());                 // 체력바 감산
                    int applyDamage = Hangja_Heart_4(userData, mobCls, damage, true);
                    if(applyDamage != 0)
                    {
                        int tmp = mobCls.GetMonsterCurrentHp() - applyDamage;
                        mobCls.SetMonsterCurrentHP(tmp);
                        monsterData.GetMonsterHPMng().HpBarFill_End(mobCls.GetMonsterMaxHp(), tmp, false);          // 체력바 감산
                        Vector3 offset = new Vector3(0, 10, 0);
                        DamageTextManager.Instance.CreateFloatingText(applyDamage.ToString(), monsterData.GetMonsterHeadPosition() + offset, Color.white);

                    }
                }
            }
        }
    }

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
    public static int Hangja_Heart_4(CharacterClass userData, Monster monsterData, int damage, bool isActive)
    {
        if (isActive == false)  // isActive는 함수의 기능 구동 여부를 판단함.
        {
            userData.AddEquipSetApplied("행자의 마음", 4, 0);
            return 0;
        }

        int reviseDamage = 0;
        if(monsterData.GetMonsterCurrentHp()>=(monsterData.GetMonsterMaxHp()/2))
        {
            int tmp = (int)(damage * 0.3f); // 30% 추가 데미지
            reviseDamage = tmp;
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
    public static void JeonjaengGwang_4(CharacterClass userData, bool isActive, bool isDelete)
    {
        if(isDelete == true)    // 장비를 해제할 때, 치확 증가 상태인지 확인 후, 데이터 제거
        {
            if (userData.GetEquipSetApplied("전투광", 4) != -1)
            {
                float tmp = userData.GetEquipSetApplied("전투광", 4);
                float curValue = userData.GetCriticalPercentage();
                userData.SetCriticalPersentage(curValue - tmp);
                userData.AddEquipSetApplied("전투광", 4, -1);
            }
            return;
        }

        if (isActive == false)// isActive는 함수의 기능 구동 여부를 판단함.
        {
            userData.AddEquipSetApplied("전투광", 4, -1);
            return;
        }
        if (userData.GetCurrentHp() <= userData.GetMaxHp() * 0.7 && userData.GetEquipSetApplied("전투광", 4) == -1)   // 70% 미만일 시, 치확 증가
        {
            float curCriticalRate = userData.GetCriticalPercentage();
            float tmp = curCriticalRate * 0.24f;
            userData.SetCriticalPersentage(curCriticalRate + tmp);
            userData.AddEquipSetApplied("전투광", 4, tmp);
        }
        else            // 70% 이상 시, 증가된 치확 제거
        {
            if(userData.GetEquipSetApplied("전투광",4)!=-1)
            {
                float tmp = userData.GetEquipSetApplied("전투광", 4);
                float curValue = userData.GetCriticalPercentage();
                userData.SetCriticalPersentage(curValue-tmp);
                userData.AddEquipSetApplied("전투광", 4, -1);
            }
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
    public static void BloodKnightChivalry_4(CharacterClass userData, bool isActive)
    {
        float curIncreaseDamage = userData.GetIncreased_NormalAttackDamage();
        if (isActive == true)
        {
            int tmp = 35;
            userData.AddEquipSetApplied("피에 물든 기사도", 4, tmp);
            userData.SetIncreased_NormalAttackDamage(curIncreaseDamage + tmp);
        }
        else
        {
            // 장비 해제에 따른 값 감산
            int tmp = (int)userData.GetEquipSetApplied("피에 물든 기사도", 4);
            userData.SetIncreased_NormalAttackDamage(curIncreaseDamage - tmp);
        }

    }
}
