using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAtkColliderMng : CombatMediator
{

    MonsterManager mobMng;
    Monster monster;

    private void Awake()
    {
        mobMng = GetComponentInParent<MonsterManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 객체의 레이어를 확인하여 플레이어 레이어인지 체크
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            monster = mobMng.GetMonsterClass();
            var player = other.gameObject.GetComponent<CharacterManager>().GetCharacterClass();

            if (player.GetState() == CharacterClass.eCharactgerState.e_AVOID)
                return;

            Mediator_MonsterAttack(monster, player);

            EffectManager.Instance.EffectCreate(transform, 1);
            gameObject.SetActive(false);
        }
    }
}