using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAniControl : MonoBehaviour
{
    
    Monster monster;
    Animator MobAnimator;
    public Monster _Monster
    {
        get { return monster; }
        set { monster = value; }
    }
    public Animator _MobAnimator
    {
        get { return MobAnimator; }
        set { MobAnimator = value; }
    }

    public virtual void MonsterAnimationController(ref float fPosZ, ref float fPosX, ref MonsterAttack.e_MonsterAttackLevel monsterAtkLevel)
    {

    }

    public virtual void HitStrunAccumulate(float fPoint, ref bool isHit, ref bool isSturn)
    {

    }
}
