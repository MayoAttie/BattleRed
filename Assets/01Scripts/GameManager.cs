using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : Singleton<GameManager>
{
    public CharacterClass characterCls;
    public GameObject[] Monsters;
    public GameObject MonsterHpBar;

    public ObjectPool<MonsterManager> CactusPool;
    public ObjectPool<MonsterManager> MushroomAngryPool;
    public ObjectPool<MonsterHp> MonsterHpBarPool;

    public Canvas BottomCanvas;
    public Camera HpCamera;

    private void Awake()
    {
        characterCls = new CharacterClass(300, 300, 0, 100, 50, 15, 1, 3.0f, CharacterClass.eCharactgerState.e_NONE,50,120,50,"플레이어","Knight",0,true);
        CactusPool = new ObjectPool<MonsterManager>(Monsters[0],10);
        MushroomAngryPool = new ObjectPool<MonsterManager>(Monsters[1],10);
        MonsterHpBarPool = new ObjectPool<MonsterHp>(MonsterHpBar, 15);

    }

    void Start()
    {


    }

    void Update()
    {
    }

    public void MonsterSpawn(Transform point)
    {
        Collider[] colliders = Physics.OverlapSphere(point.position, 12, 1 << LayerMask.NameToLayer("Monster"));

        if (colliders.Length > 0)
        {
            // 이미 해당 위치 인근에 몬스터가 있는 경우, 생성하지 않고 바로 반환
            return;
        }

        // 오브젝트 풀을 이용해 몬스터 생성
        foreach (var mob in Monsters)
        {
            if (mob.name == "Cactus")
            {
                Vector3 spawnPosition = point.position + new Vector3(1 * 2, 0, 0);
                int extraHealth = characterCls.GetLeveL()*100;
                int extraAttack = 0;
                Monster monsterCls = new Monster("몬스터", mob.name, 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Precedence, 200 + extraHealth, 200 + extraHealth, 10 + extraAttack, 15, 15, 1.8f, 100f, Element.e_Element.None, 1.5f);
                // 오브젝트 풀로 Hp와 몬스터 객체 생성
                MonsterManager monsterManager = CactusPool.GetFromPool(spawnPosition, Quaternion.identity);
                MonsterHp monsterHpMng = MonsterHpBarPool.GetFromPool(spawnPosition, Quaternion.identity,BottomCanvas.transform);
                monsterManager.SetMonsterHPMng(monsterHpMng);
                monsterManager.SetMonsterClass(monsterCls);
                //hp FillAmount 초기화
                monsterHpMng.HpBarFill_Init(monsterCls.GetMonsterCurrentHp());
                monsterHpMng.HpBarFill_End(monsterCls.GetMonsterMaxHp(), monsterCls.GetMonsterCurrentHp(), false);
            }
            else if (mob.name == "MushroomAngry")
            {
                Vector3 spawnPosition = point.position + new Vector3(2 * 2, 0, 0);
                int extraHealth = 0;
                int extraAttack = characterCls.GetLeveL() * 10;
                Monster monsterCls = new Monster("몬스터", mob.name, 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Precedence, 200 + extraHealth, 200 + extraHealth, 10 + extraAttack, 15, 15, 1.8f, 100f, Element.e_Element.None, 1.5f);
                // 오브젝트 풀로 Hp와 몬스터 객체 생성
                MonsterManager monsterManager = MushroomAngryPool.GetFromPool(spawnPosition, Quaternion.identity);
                MonsterHp monsterHpMng = MonsterHpBarPool.GetFromPool(spawnPosition, Quaternion.identity, BottomCanvas.transform);
                monsterManager.SetMonsterHPMng(monsterHpMng);
                monsterManager.SetMonsterClass(monsterCls);
                //hp FillAmount 초기화
                monsterHpMng.HpBarFill_Init(monsterCls.GetMonsterCurrentHp());
                monsterHpMng.HpBarFill_End(monsterCls.GetMonsterMaxHp(), monsterCls.GetMonsterCurrentHp(), false);
            }
        }
    }

}
