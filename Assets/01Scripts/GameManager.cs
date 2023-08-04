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
        characterCls = new CharacterClass(1000, 100, 0, 100, 50, 15, 1, 3.0f, CharacterClass.eCharactgerState.e_NONE, Element.e_Element.None);
    }

    void Start()
    {
        foreach(var point in MonsterSpawnPoint)
        {
            foreach(var mob in Monsters) 
            {
                Instantiate(mob);
                MonsterManager mng = mob.GetComponent<MonsterManager>();
                Monster monsterCls = new Monster("몬스터", "Cactus", 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Precedence, 100, 100, 10, 10, 10, 5);
                mng.SetMonsterClass(monsterCls);
                mob.transform.position = point.position; // 위치를 point의 위치로 이동
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
