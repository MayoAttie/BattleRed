using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DungeonMng_1 : DungeonManager, Observer
{
    bool isAfterSetting = false;
    bool[] is_checkPointSpawn;                  // 체크포인트 몬스터 스폰 제어플래그
    public List<GameObject> list_wave_1;
    // 1번 던전 매니저

    void Awake()
    {
        base.Awake();
        list_wave_1 = new List<GameObject>();
        is_checkPointSpawn = new bool[1];
        GameManager.Instance.CactusPool.AllReturnToPool();
        GameManager.Instance.MushroomAngryPool.AllReturnToPool();
        GameManager.Instance.GolemBossPool.AllReturnToPool();
    }

    void Start()
    {
        base.Start();
    }

    void Update()
    {
        base.Update();
        AfterSetting();
    }

    #region 몬스터 스폰

    void MonsterSpawn()
    {

        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("DungeonSpwanPoint");
        Transform parents = GetMonstersTrasform();

        foreach (var tmp in spawnPoints)
        {
            string pointName = tmp.name;
            string remainingCharacters = pointName.Substring(10);



            switch (remainingCharacters)
            {
                case "1":
                case "5":
                case "6":
                case "8":
                case "9":
                case "11":
                    {
                        Monster monsterCls = new Monster("몬스터", "Cactus", 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Precedence,
                                        1200, 1200, 10, 15, 15, 1.8f, 100f, Element.e_Element.None, 1.5f, 20);

                        MonsterManager.MonsterControl_Info info = new MonsterManager.MonsterControl_Info(5, 20, 20, 5);

                        GameManager.Instance.SpawnMonster_StaticSet(tmp.transform.position, "Cactus", monsterCls, info, parents);
                    }
                    break;
                case "15":
                case "16":
                    // 1차 웨이브 포인트 저장
                    list_wave_1.Add(tmp);
                    break;
                case "2":
                case "3":
                case "4":
                case "7":
                case "10":
                case "12":
                    {
                        // 1차 웨이브 몬스터 저장
                        Monster monsterCls = new Monster("몬스터", "MushroomAngry", 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Precedence,
                                        1000, 1000, 10, 25, 15, 1.8f, 100f, Element.e_Element.None, 1.5f, 20);
                        MonsterManager.MonsterControl_Info info = new MonsterManager.MonsterControl_Info(5, 20, 20, 5);

                        GameManager.Instance.SpawnMonster_StaticSet(tmp.transform.position, "MushroomAngry", monsterCls, info, parents);
                    }
                    break;
                case "13":
                case "14":
                case "17":
                    // 1차 웨이브 포인트 저장
                    list_wave_1.Add(tmp);
                    break;
                case "Boss":    
                    {
                        Monster monsterCls = new Monster("몬스터", "Golem_Boss", 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Boss,
                2000, 2000, 250, 40, 25, 1.8f, 100f, Element.e_Element.None, 1.5f, 60);

                        GameManager.Instance.SpawnMonster_StaticSet(tmp.transform.position, "Golem_Boss", monsterCls);
                    }
                    break;
            }
        }
    }

    void CheckPointPassRespawn(int num)
    {
        Transform parents = GetMonstersTrasform();
        if (is_checkPointSpawn[num] == true)
            return;
        is_checkPointSpawn[num] = true;

        if (num == 0)
        {
            int index = 0;
            foreach(var tmp in list_wave_1)
            {
                if(index <2)
                {
                    // 1차 웨이브 몬스터 스폰
                    Monster monsterCls = new Monster("몬스터", "Cactus", 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Precedence,
                                    1200, 1200, 10, 15, 15, 1.8f, 100f, Element.e_Element.None, 1.5f, 20);

                    MonsterManager.MonsterControl_Info info = new MonsterManager.MonsterControl_Info(5, 20, 20, 5);

                    var mob = GameManager.Instance.SpawnMonster_StaticSet(tmp.transform.position, "Cactus", monsterCls, info, parents);
                }
                else
                {
                    Monster monsterCls = new Monster("몬스터", "MushroomAngry", 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Precedence,
                1000, 1000, 10, 25, 15, 1.8f, 100f, Element.e_Element.None, 1.5f, 20);
                    MonsterManager.MonsterControl_Info info = new MonsterManager.MonsterControl_Info(5, 20, 20, 5);

                    var mob = GameManager.Instance.SpawnMonster_StaticSet(tmp.transform.position, "MushroomAngry", monsterCls, info, parents);
                }
                index++;
            }
        }


    }



    void AfterSetting()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj != null && !isAfterSetting)
        {
            MonsterSpawn();
            isAfterSetting = true;
            
            // 체크포인트에 옵저버 패턴 연결
            GameObject[] checkPoints = GameObject.FindGameObjectsWithTag("CheckPoint");
            foreach (var tmp in checkPoints)
            {
                tmp.GetComponent<CheckPointNotify>().Attach(this);
            }
        }

    }

    // 플레이어 캐릭터가 체크 포인트에 있는지 파악 후, 기능.


    public override void ExitDungeon()
    {
        GameManager.Instance.CactusPool.AllReturnToPool();
        GameManager.Instance.MushroomAngryPool.AllReturnToPool();
        GameManager.Instance.GolemBossPool.AllReturnToPool();
        base.ExitDungeon();

    }



    #endregion


    #region 옵저버 패턴


    public void AtkLevelNotify(CharacterAttackMng.e_AttackLevel level){}

    public void BlinkValueNotify(CharacterControlMng.e_BlinkPos value){}

    public void GetEnemyFindNotify(List<Transform> findList){}

    public void AttackSkillStartNotify(){}

    public void AttackSkillEndNotify(){}

    // 캐릭터가 특정 포인트 지났을 시에, 몬스터 재활성화
    public void CheckPoint_PlayerPassNotify(int num)
    {
        CheckPointPassRespawn(num);
    }

    public void WorldMapOpenNotify(){}

    public void WorldMapCloseNotify(){}

    public void ConvertToTargetStateNotify(List<Vector3> listTarget){}


    #endregion
}
