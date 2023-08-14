using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICombatMediator
{
    void Mediator_CharacterAttack(CharacterClass character, Monster targetMonster);
    bool Mediator_MonsterAttack(Monster monster, CharacterClass targetCharacter);
}