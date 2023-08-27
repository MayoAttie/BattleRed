using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Monster;

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

    private void FixedUpdate()
    {
        
    }

    #region 몬스터 스폰
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
    #endregion
    #region 몬스터 제거
    // 일정 범위 내의 몬스터를 제외한 나머지 몬스터를 오브젝트 풀에 리턴.
    public void MonsterReturnToPool(Collider[] detectedMonster)
    {
        List<MonsterManager> cactusList = CactusPool.GetPoolList();
        List<MonsterManager> AngryMushList = MushroomAngryPool.GetPoolList();

        // 오브젝트 풀을 순회.
        if (cactusList.Count > 0)
        {
            // Cactus을 저장한 리스트를 순회하며, 활성화 여부 체크, 및 detectedMonster 내의 객체 체크
            for (int i = 0; i < cactusList.Count; i++)
            {
                if (!cactusList[i].gameObject.activeSelf)
                    continue;
                bool isDetected = false;
                MonsterManager compareMob = cactusList[i];
                for (int j = 0; j < detectedMonster.Length; j++)
                {
                    MonsterManager detectedMob = detectedMonster[j].GetComponent<MonsterManager>(); 
                    if (compareMob == detectedMob)
                    {
                        isDetected = true;
                        break;
                    }
                }
                if (!isDetected)
                {
                    // 탐지된 적이 아닐 경우엔 다시 오브젝트풀에 리턴.
                    MonsterHpBarPool.ReturnToPool(compareMob.GetMonsterHPMng());
                    CactusPool.ReturnToPool(compareMob);
                }
            }
        }
        if (AngryMushList.Count > 0)
        {
            for (int i = 0; i < AngryMushList.Count; i++)
            {
                if (!AngryMushList[i].gameObject.activeSelf)
                    continue;
                bool isDetected = false;
                MonsterManager compareMob = AngryMushList[i];
                for (int j = 0; j < detectedMonster.Length; j++)
                {
                    MonsterManager detectedMob = detectedMonster[j].GetComponent<MonsterManager>(); // 수정된 부분
                    if (compareMob == detectedMob)
                    {
                        isDetected = true;
                        break;
                    }
                }
                if (!isDetected)
                {
                    MonsterHpBarPool.ReturnToPool(compareMob.GetMonsterHPMng());
                    MushroomAngryPool.ReturnToPool(compareMob);
                }
            }
        }
    }
    #endregion

}
