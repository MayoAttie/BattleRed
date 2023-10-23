using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICombatMediator
{
    void Mediator_CharacterAttack(CharacterClass character, CharacterManager characMng, MonsterManager targetMonster);
    void Mediator_CharacterSkillAttack(CharacterClass character, CharacterManager characMng, MonsterManager mobManager);
    bool Mediator_PlantObj(CharacterClass character, MonsterManager targetMonster);
    bool Mediator_MonsterAttack(Monster monster, CharacterClass targetCharacter);

}