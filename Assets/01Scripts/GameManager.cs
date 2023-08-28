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
        // 스폰 포인트의 이름을 파싱하여, 번호를 분류.
        GameObject spawnObj = point.gameObject;
        string objName = spawnObj.name;
        string remainingCharacters = objName.Substring(12);

        // 탐색한 콜라이더 내의 몬스터 이름들을 저장하는 리스트 생성.
        Collider[] colliders = Physics.OverlapSphere(point.position, 15, 1 << LayerMask.NameToLayer("Monster"));
        List<string> names = new List<string>();

        foreach (Collider collider in colliders)
        {
            string colliderName = collider.GetComponent<MonsterManager>().GetMonsterClass().GetName();
            names.Add(colliderName);
        }
        string cactusName;

        // 분류한 번호에 따라, 분기하여 해당하는 몬스터를 생성.
        switch (remainingCharacters)
        {
            case "1":
            case "2":
            case "3":
                cactusName = "Cactus";
                for (int i = 0; i < 2; i++)
                {
                    Vector3 spawnPosition = point.position + new Vector3(i * 2, 0, 0);
                    int extraHealth = characterCls.GetLeveL() * 100;
                    int extraAttack = 0;
                    SpawnMonster(names, cactusName, spawnPosition, extraHealth, extraAttack);
                }
                break;
            case "4":
            case "5":
                cactusName = "MushroomAngry";
                for (int i = 0; i < 2; i++)
                {
                    Vector3 spawnPosition = point.position + new Vector3(i * 2, 0, 0);
                    int extraHealth = 0;
                    int extraAttack = characterCls.GetLeveL() * 10; ;
                    SpawnMonster(names, cactusName, spawnPosition, extraHealth, extraAttack);
                }
                break;
        }

        #region 레거시
        //// 오브젝트 풀을 이용해 몬스터 생성
        //foreach (var mob in Monsters)
        //{
        //    if (mob.name == "Cactus")
        //    {
        //        Vector3 spawnPosition = point.position + new Vector3(1 * 2, 0, 0);
        //        int extraHealth = characterCls.GetLeveL()*100;
        //        int extraAttack = 0;
        //        Monster monsterCls = new Monster("몬스터", mob.name, 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Precedence, 200 + extraHealth, 200 + extraHealth, 10 + extraAttack, 15, 15, 1.8f, 100f, Element.e_Element.None, 1.5f);
        //        // 오브젝트 풀로 Hp와 몬스터 객체 생성
        //        MonsterManager monsterManager = CactusPool.GetFromPool(spawnPosition, Quaternion.identity);
        //        MonsterHp monsterHpMng = MonsterHpBarPool.GetFromPool(spawnPosition, Quaternion.identity,BottomCanvas.transform);
        //        monsterManager.SetMonsterHPMng(monsterHpMng);
        //        monsterManager.SetMonsterClass(monsterCls);
        //        //hp FillAmount 초기화
        //        monsterHpMng.HpBarFill_Init(monsterCls.GetMonsterCurrentHp());
        //        monsterHpMng.HpBarFill_End(monsterCls.GetMonsterMaxHp(), monsterCls.GetMonsterCurrentHp(), false);
        //    }
        //    else if (mob.name == "MushroomAngry")
        //    {
        //        Vector3 spawnPosition = point.position + new Vector3(2 * 2, 0, 0);
        //        int extraHealth = 0;
        //        int extraAttack = characterCls.GetLeveL() * 10;
        //        Monster monsterCls = new Monster("몬스터", mob.name, 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Precedence, 200 + extraHealth, 200 + extraHealth, 10 + extraAttack, 15, 15, 1.8f, 100f, Element.e_Element.None, 1.5f);
        //        // 오브젝트 풀로 Hp와 몬스터 객체 생성
        //        MonsterManager monsterManager = MushroomAngryPool.GetFromPool(spawnPosition, Quaternion.identity);
        //        MonsterHp monsterHpMng = MonsterHpBarPool.GetFromPool(spawnPosition, Quaternion.identity, BottomCanvas.transform);
        //        monsterManager.SetMonsterHPMng(monsterHpMng);
        //        monsterManager.SetMonsterClass(monsterCls);
        //        //hp FillAmount 초기화
        //        monsterHpMng.HpBarFill_Init(monsterCls.GetMonsterCurrentHp());
        //        monsterHpMng.HpBarFill_End(monsterCls.GetMonsterMaxHp(), monsterCls.GetMonsterCurrentHp(), false);
        //    }
        //}
        #endregion
    }

    private void SpawnMonster(List<string> names,string monsterName, Vector3 spawnPosition, int extraHealth, int extraAttack)
    {
        string foundMonsterName = names.Find(name => name == monsterName);
        if (foundMonsterName != null)
            return;   // 이미 몬스터가 존재한다면 리턴

        Monster monsterCls = new Monster("몬스터", monsterName, 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Precedence, 200 + extraHealth, 200 + extraHealth, 10 + extraAttack, 15, 15, 1.8f, 100f, Element.e_Element.None, 1.5f);

        MonsterManager monsterManager = null;

        //오브젝트 풀로 몬스터 생성
        switch (monsterName)
        {
            case "Cactus":
                monsterManager = CactusPool.GetFromPool(spawnPosition, Quaternion.identity);
                break;
            case "MushroomAngry":
                monsterManager = MushroomAngryPool.GetFromPool(spawnPosition, Quaternion.identity);
                break;
        }
        // 오브젝트 풀로 hp바 생성
        MonsterHp monsterHpMng = MonsterHpBarPool.GetFromPool(spawnPosition, Quaternion.identity, BottomCanvas.transform);

        // 필요한 데이터 초기화
        monsterManager.SetMonsterHPMng(monsterHpMng);
        monsterManager.SetMonsterClass(monsterCls);

        monsterHpMng.HpBarFill_Init(monsterCls.GetMonsterCurrentHp());
        monsterHpMng.HpBarFill_End(monsterCls.GetMonsterMaxHp(), monsterCls.GetMonsterCurrentHp(), false);
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
