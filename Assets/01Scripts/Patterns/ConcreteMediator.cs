using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Element_Interaction;
// ConcreteMediator 클래스
// 옵저버 패턴 활용을 위한 Subject 상속
public class CombatMediator : Subject ,ICombatMediator
{
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

        // 크리티컬 데미지 배율 적용
        float criticalDamageMultiplier = criticalDamage/100;
        int totalCriticalDamage = Mathf.FloorToInt(attackPower * criticalDamageMultiplier);

        // 크리티컬이 발생한 경우 크리티컬 데미지 추가
        int damage = damageWithoutCritical + (isCritical ? totalCriticalDamage : 0);


        //몬스터 피깎
        int tmp = mobCurrentHp - damage;
        targetMonster.SetMonsterCurrentHP(tmp);

    }

    public void Mediator_MonsterAttack(Monster monster, CharacterClass targetCharacter)
    {
        int atkPower = monster.GetMonsterAtkPower();
        int hp = targetCharacter.GetCurrentHp();
        Debug.Log("몬스터 공격!" + hp + " - " + atkPower);
        //Console.WriteLine($"{monster.Name} attacks {targetCharacter.Name}.");
        // 여기에서 몬스터의 공격 구현 및 캐릭터의 피격 처리 등을 수행
    }
}