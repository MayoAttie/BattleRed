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
                Monster monsterCls = new Monster("몬스터", "Cactus", 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Precedence, 100, 100, 10, 10, 10, 1.8f, 100f);
                GameObject obj = Instantiate(mob);
                MonsterManager monsterManager = obj.GetComponent<MonsterManager>();
                monsterManager.SetMonsterClass(monsterCls);
                obj.transform.position = point.position; // 위치를 point의 위치로 이동
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
