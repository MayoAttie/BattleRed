using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HandlePauseTool;

public class MonsterAtkColliderMng : CombatMediator
{
    MonsterManager mobMng;
    Monster monster;
    Collider myCollider;

    // 공격력 보정치(곱연산 후 덧셈)
    public float ATK_offset = 0;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        mobMng = GetComponentInParent<MonsterManager>();
    }
    private void OnEnable()
    {
        // 게임매니저의 이벤트에 구독
        GameManager.OnPauseStateChanged += HandlePauseStateChanged;
    }
    private void OnDisable()
    {
        // 게임매니저의 이벤트 구독 해제
        GameManager.OnPauseStateChanged -= HandlePauseStateChanged;
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

            if (monster.GetMonsterType() == Monster.e_MonsterType.Boss)
                ATK_offset += 0.15f;

            if (ATK_offset != 0)
            {
                int curAtk = monster.GetMonsterAtkPower();
                int corredtedAtk = (int)(curAtk * ATK_offset);
                Mediator_MonsterAttack(monster, player, corredtedAtk);

            }
            else
            {
                Mediator_MonsterAttack(monster, player);
            }

            gameObject.SetActive(false);
        }
    }

    public Collider MyCollider
    {
        get { return myCollider; }
    }
}