using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharacterClass characterCls;
    public GameObject[] Monsters;
    public Transform[] MonsterSpawnPoint;
    private void Awake()
    {
        characterCls = new CharacterClass(300, 300, 0, 100, 50, 15, 1, 3.0f, CharacterClass.eCharactgerState.e_NONE,50,120,50,"플레이어","Knight",0,true);
    }

    void Start()
    {
        foreach (var mob in Monsters)
        {
            if (mob.name == "Cactus" || mob.name == "MushroomAngry")
            {
                // 랜덤한 인덱스를 구해서 해당 위치에 몬스터 생성
                int randomIndex = UnityEngine.Random.Range(0, MonsterSpawnPoint.Length);
                var point = MonsterSpawnPoint[randomIndex];

                Monster monsterCls = new Monster("몬스터", mob.name, 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Precedence, 100, 100, 10, 10, 10, 1.8f, 100f);
                GameObject obj = Instantiate(mob);
                MonsterManager monsterManager = obj.GetComponent<MonsterManager>();
                monsterManager.SetMonsterClass(monsterCls);
                obj.transform.position = point.position; // 위치를 랜덤하게 선택된 point의 위치로 이동
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
