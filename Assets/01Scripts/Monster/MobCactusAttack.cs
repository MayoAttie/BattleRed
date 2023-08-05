using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MobCactusAttack : MonsterAttack
{
    int atkMaxLevel = 2;

    void Start()
    {

    }

    void Update()
    {
        base.Update();
    }
    public MobCactusAttack(Monster monsterObj, Animator animator, NavMeshAgent navAgent) : base(monsterObj, animator, navAgent) {}
    public MobCactusAttack(){}


    public override void MonsterAttackStart()   // 공격시작함수
    {
        base.MonsterAttackStart();
        if(GetTargetInRange() == false)
        {
            
        }
    }






}
