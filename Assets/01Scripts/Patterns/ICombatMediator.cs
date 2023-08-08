using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICombatMediator
{
    void CharacterAttack(CharacterClass character, Monster targetMonster);
    void MonsterAttack(Monster monster, CharacterClass targetCharacter);
}