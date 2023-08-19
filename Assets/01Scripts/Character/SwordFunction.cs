using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordFunction : CombatMediator
{
    CharacterClass character;
    

    private void OnTriggerEnter(Collider other)
    {
        
        // 충돌한 객체의 레이어를 확인하여 몬스터 레이어인지 체크
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            character = CharacterManager.Instance.GetCharacterClass();
            CharacterAttackMng.e_AttackLevel attackLevel = CharacterManager.Instance.GetCharacterAtkLevel();

            // 평타 공격인지 체크
            if (attackLevel == CharacterAttackMng.e_AttackLevel.Attack1 ||
               attackLevel==CharacterAttackMng.e_AttackLevel.Attack2 ||
               attackLevel == CharacterAttackMng.e_AttackLevel.Attack3)
            {
                Monster mob = other.gameObject.GetComponent<MonsterManager>().GetMonsterClass();
                Mediator_CharacterAttack(character, mob);
                EffectManager.Instance.EffectCreate(this.gameObject.transform, 0);

                
            }
            else if(attackLevel == CharacterAttackMng.e_AttackLevel.AtkSkill)
            {
                character.GetCurrnetElement().SetIsActive(true);
                Monster mob = other.gameObject.GetComponent<MonsterManager>().GetMonsterClass();
                Mediator_CharacterSkillAttack(character, CharacterManager.Instance, mob);

            }


        }
        
        gameObject.SetActive(false);
    }



}
