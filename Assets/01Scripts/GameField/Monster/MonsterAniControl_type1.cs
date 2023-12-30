using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAniControl_type1 : MonsterAniControl
{
    public override void MonsterAnimationController(ref float fPosZ, ref float fPosX, ref MonsterAttack.e_MonsterAttackLevel monsterAtkLevel)
    {
        switch (_Monster.GetMonsterState())
        {
            case Monster.e_MonsterState.Idle:               // 정지
                _MobAnimator.SetInteger("Controller", 0);
                _MobAnimator.SetFloat("zPos", fPosZ);
                _MobAnimator.SetFloat("xPos", fPosX);
                break;
            case Monster.e_MonsterState.Walk:               // 이동
                _MobAnimator.SetInteger("Controller", -1);
                _MobAnimator.SetFloat("zPos", fPosZ);
                _MobAnimator.SetFloat("xPos", fPosX);
                break;
            case Monster.e_MonsterState.Attack:             // 공격
                if (monsterAtkLevel == MonsterAttack.e_MonsterAttackLevel.Chase)
                {
                    _MobAnimator.SetInteger("Controller", -1);
                    _MobAnimator.SetFloat("zPos", fPosZ);
                    _MobAnimator.SetFloat("xPos", fPosX);
                }
                else if (monsterAtkLevel == MonsterAttack.e_MonsterAttackLevel._1st)
                {
                    _MobAnimator.SetFloat("zPos", fPosZ);
                    _MobAnimator.SetFloat("xPos", fPosX);
                    _MobAnimator.SetInteger("Controller", 11);
                }
                else if (monsterAtkLevel == MonsterAttack.e_MonsterAttackLevel._2rd)
                {
                    _MobAnimator.SetFloat("zPos", fPosZ);
                    _MobAnimator.SetFloat("xPos", fPosX);
                    _MobAnimator.SetInteger("Controller", 12);
                }
                else if (monsterAtkLevel == MonsterAttack.e_MonsterAttackLevel._3th)
                {
                    _MobAnimator.SetFloat("zPos", fPosZ);
                    _MobAnimator.SetFloat("xPos", fPosX);
                    _MobAnimator.SetInteger("Controller", 13);
                }
                else if (monsterAtkLevel == MonsterAttack.e_MonsterAttackLevel._4th)
                {
                    _MobAnimator.SetFloat("zPos", fPosZ);
                    _MobAnimator.SetFloat("xPos", fPosX);
                    _MobAnimator.SetInteger("Controller", 14);
                }
                break;
            case Monster.e_MonsterState.LookAround:
                _MobAnimator.SetInteger("Controller", 1);
                break;
            case Monster.e_MonsterState.Hit:
                _MobAnimator.SetInteger("Controller", 2);
                _MobAnimator.SetFloat("zPos", 0);
                _MobAnimator.SetFloat("xPos", 0);
                break;
            case Monster.e_MonsterState.Sturn:
                _MobAnimator.SetInteger("Controller", 3);
                break;
            case Monster.e_MonsterState.Die:
                _MobAnimator.SetInteger("Controller", 4);
                _MobAnimator.SetFloat("zPos", 0);
                _MobAnimator.SetFloat("xPos", 0);
                break;
            default: break;
        }
    }


}
