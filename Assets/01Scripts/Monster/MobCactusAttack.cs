using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobCactusAttack : MonsterAttack
{
    void Start()
    {

    }

    void Update()
    {

    }
    public MobCactusAttack(Monster monsterObj, Animator animator) : base(monsterObj, animator){}
    public MobCactusAttack(){}


    public override void MonsterAttackStart()   // 공격시작함수
    {
        Debug.Log("MobCacutsAttack");
    }


    

}
