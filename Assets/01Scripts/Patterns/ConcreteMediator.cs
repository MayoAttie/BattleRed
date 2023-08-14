using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using UnityEngine;
using static Element_Interaction;
// ConcreteMediator 클래스
// 옵저버 패턴 활용을 위한 Subject 상속
public class CombatMediator : Subject ,ICombatMediator
{


    #region 캐릭터
    // 캐릭터 공격 판정 함수
    public void Mediator_CharacterAttack(CharacterClass character, Monster targetMonster)
    {
        float criticalDamage = character.GetCriticalDamage();
        float criticalPercentage = character.GetCriticalPercentage();
        int attackPower = character.GetAttack();

        int mobDef = targetMonster.GetMonsterDef();
        int mobCurrentHp = targetMonster.GetMonsterCurrentHp();
        int mobMaxHp = targetMonster.GetMonsterMaxHp();

        // 크리티컬 확률을 기반으로 크리티컬 결정
        float randomValue = UnityEngine.Random.Range(0f, 1f);
        bool isCritical = randomValue < (criticalPercentage / 100);

        // 공격력 - 방어력
        int damageWithoutCritical = attackPower - mobDef;
        if (damageWithoutCritical < 0) return;

        // 크리티컬 데미지 배율 적용
        float criticalDamageMultiplier = criticalDamage/100;
        int totalCriticalDamage = Mathf.FloorToInt(attackPower * criticalDamageMultiplier);

        // 크리티컬이 발생한 경우 크리티컬 데미지 추가
        int damage = damageWithoutCritical + (isCritical ? totalCriticalDamage : 0);


        //몬스터 피깎
        int tmp = mobCurrentHp - damage;
        targetMonster.SetMonsterCurrentHP(tmp);

    }
    #endregion

    #region 몬스터

    // 몬스터 공격 판정함수
    public bool Mediator_MonsterAttack(Monster monster, CharacterClass targetCharacter)
    {

        bool isElementAttackActive = false;
        int atkPower = monster.GetMonsterAtkPower();
        int characHp = targetCharacter.GetCurrentHp();
        int characDef = targetCharacter.GetDeffense();
        Element elementMng =  monster.GetMonsterHaveElement();
        int damage = 0;

        if (elementMng.GetElement() == Element.e_Element.None || elementMng.GetIsActive() == false)
        {
            // 공격력 - 방어력
            damage = atkPower - characDef;
            if (damage < 0) // 방어력 감쇄로 데미지가 음수 값일 경우, 1보정.
                damage = 1;

            //캐릭터 피깎
            int tmp = characHp - damage;
            Debug.Log(damage);
            targetCharacter.SetCurrentHp(tmp);
        }
        else
        {
            // 캐릭터의 부착용 원소 객체를 인스턴스화
            Element characElement = targetCharacter.GetEncountElement();

            // 캐릭터의 현재 원소 적용 상태에 따라서 분기
            switch (characElement.GetElement())
            {
                case Element.e_Element.None:
                    // 캐릭터에게 원소 상태를 부착.
                    characElement.SetElement(elementMng.GetElement());
                    characElement.SetIsActive(true);
                    break;
                case Element.e_Element.Fire:   // 플레이어 == 불 타입 + N원소
                    if (elementMng.GetElement() == Element.e_Element.Water)
                    {
                        damage = m_FireToWater(monster, targetCharacter);
                    }
                    else if(elementMng.GetElement() == Element.e_Element.Plant)
                    {
                        damage = m_FireToPlant(monster, targetCharacter);
                    }
                    else if (elementMng.GetElement() == Element.e_Element.Lightning)
                    {
                        damage = m_FireToLightning(monster, targetCharacter);
                    }
                    break;
                case Element.e_Element.Water:   // 플레이어 == 물 타입 + N원소
                    if (elementMng.GetElement() == Element.e_Element.Fire)
                    {
                        damage = m_WaterToFire(monster, targetCharacter);
                    }
                    else if (elementMng.GetElement() == Element.e_Element.Plant)
                    {
                        damage = m_WaterToPlant(monster, targetCharacter);
                    }
                    else if (elementMng.GetElement() == Element.e_Element.Lightning)
                    {
                        damage = m_WaterToLightning(monster, targetCharacter);
                    }
                    break;
                case Element.e_Element.Plant:   // 플레이어 == 풀 타입 + N원소
                    if (elementMng.GetElement() == Element.e_Element.Water)
                    {
                        damage = m_PlantToWater(monster, targetCharacter);

                    }
                    else if (elementMng.GetElement() == Element.e_Element.Fire)
                    {
                        damage = m_PlantToFire(monster, targetCharacter);
                    }
                    else if (elementMng.GetElement() == Element.e_Element.Lightning)
                    {
                        damage = m_PlantToLightning(monster, targetCharacter);
                    }
                    break;
                case Element.e_Element.Lightning:   // 플레이어 == 라이트닝 타입 + N원소
                    if (elementMng.GetElement() == Element.e_Element.Water)
                    {
                        damage = m_LightningToWater(monster, targetCharacter);
                    }
                    else if (elementMng.GetElement() == Element.e_Element.Plant)
                    {
                        damage = m_LightningToPlant(monster, targetCharacter);
                    }
                    else if (elementMng.GetElement() == Element.e_Element.Fire)
                    {
                        damage = m_LightningToFire(monster, targetCharacter);
                    }
                    break;
                case Element.e_Element.Wind:   // 플레이어 == 바람 타입 + N원소
                    if (elementMng.GetElement() == Element.e_Element.Water)
                    {
                        m_WindToWater(monster, targetCharacter);
                    }
                    else if (elementMng.GetElement() == Element.e_Element.Plant)
                    {
                        m_WindToPlant(monster, targetCharacter);
                    }
                    else if (elementMng.GetElement() == Element.e_Element.Fire)
                    {
                        m_WindToFire(monster, targetCharacter);
                    }
                    else if(elementMng.GetElement() == Element.e_Element.Lightning)
                    {
                        m_WindToLightning(monster, targetCharacter);
                    }
                    break;
            }
            //캐릭터 피깎
            int tmp = characHp - damage;
            targetCharacter.SetCurrentHp(tmp);

            // 원소 공격 유무 액티브 활성화
            isElementAttackActive = true;
        }

        Debug.Log("몬스터 어택 성공~!!");
        return isElementAttackActive;
    }
    #endregion

}