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
        //Console.WriteLine($"{character.Name} attacks {targetMonster.Name}.");
        // 여기에서 캐릭터의 공격 구현 및 몬스터의 피격 처리 등을 수행
    }

    public void Mediator_MonsterAttack(Monster monster, CharacterClass targetCharacter)
    {
        //Console.WriteLine($"{monster.Name} attacks {targetCharacter.Name}.");
        // 여기에서 몬스터의 공격 구현 및 캐릭터의 피격 처리 등을 수행
    }
}