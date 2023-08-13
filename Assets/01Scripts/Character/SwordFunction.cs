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

            Monster mob = other.gameObject.GetComponent<MonsterManager>().GetMonsterClass();
            Mediator_CharacterAttack(character, mob);
            EffectManager.Instance.EffectCreate(this.gameObject.transform, 0);

            gameObject.SetActive(false);
        }
    }



}
