using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAniControl_type2 : MonsterAniControl
{

    public override void MonsterAnimationController(ref float fPosZ, ref float fPosX, ref MonsterAttack.e_MonsterAttackLevel monsterAtkLevel)
    {
        switch (_Monster.GetMonsterState())
        {
            case Monster.e_MonsterState.Idle:               // 정지
                _MobAnimator.SetInteger("Controller", 0);
                break;
            case Monster.e_MonsterState.Walk:               // 이동
                _MobAnimator.SetInteger("Controller", 1);
                break;
            case Monster.e_MonsterState.Attack:             // 공격
                if (monsterAtkLevel == MonsterAttack.e_MonsterAttackLevel.Chase)
                {
                    _MobAnimator.SetInteger("Controller", 1);
                }
                else if (monsterAtkLevel == MonsterAttack.e_MonsterAttackLevel._1st)        // 공격1
                {
                    _MobAnimator.SetInteger("Controller", 11);
                }
                else if (monsterAtkLevel == MonsterAttack.e_MonsterAttackLevel._2rd)        // 공격2
                {
                    _MobAnimator.SetInteger("Controller", 12);
                }
                else if (monsterAtkLevel == MonsterAttack.e_MonsterAttackLevel._3th)            // 스킬
                {
                    _MobAnimator.SetInteger("Controller", 13);
                }
                else if (monsterAtkLevel == MonsterAttack.e_MonsterAttackLevel._4th)            // 대기
                {
                    _MobAnimator.SetInteger("Controller", 0);
                }
                break;
            case Monster.e_MonsterState.Hit:
                _MobAnimator.SetInteger("Controller", 2);
                break;
            case Monster.e_MonsterState.Die:
                _MobAnimator.SetInteger("Controller", 5);
                break;
            default: break;
        }
    }


    public override void HitStrunAccumulate(float fPoint, ref bool isHit, ref bool isSturn)
    {
        float maxSturnPoint = _Monster.GetMonsterSturnPoint();
        float currentSturnPoint = _Monster.GetCurrentSturnPoint();
        float tmp = fPoint + currentSturnPoint;

        // 경직도 계산
        if (tmp > maxSturnPoint)
        {
            _Monster.SetCurrentSturnPoint(0);

            isHit = true;
        }
        else
        {
            _Monster.SetCurrentSturnPoint(tmp);
        }
    }

}
