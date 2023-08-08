using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ConcreteMediator 클래스
public class CombatMediator : ICombatMediator
{
    public void CharacterAttack(CharacterClass character, Monster targetMonster)
    {
        //Console.WriteLine($"{character.Name} attacks {targetMonster.Name}.");
        // 여기에서 캐릭터의 공격 구현 및 몬스터의 피격 처리 등을 수행
    }

    public void MonsterAttack(Monster monster, CharacterClass targetCharacter)
    {
        //Console.WriteLine($"{monster.Name} attacks {targetCharacter.Name}.");
        // 여기에서 몬스터의 공격 구현 및 캐릭터의 피격 처리 등을 수행
    }
}