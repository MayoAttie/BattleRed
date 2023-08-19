using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICombatMediator
{
    void Mediator_CharacterAttack(CharacterClass character, Monster targetMonster);
    void Mediator_CharacterSkillAttack(CharacterClass character, CharacterManager characMng ,Monster targetMonster);
    bool Mediator_PlantObj(CharacterClass character, Monster targetMonster);
    bool Mediator_MonsterAttack(Monster monster, CharacterClass targetCharacter);

}