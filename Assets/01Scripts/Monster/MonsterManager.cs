using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{

    Monster monster;            // 몬스터 데이터 클래스파일
    Animator MobAnimator;       // 애니메이터
    MonsterAttack monsterAtk;   // 몬스터 공격 제어 함수
    float fPosX;                // 이동제어 용 실수 변수
    float fPosZ;                // 이동제어 용 실수 변수
    bool isBattle;              // 전투 체크 변수
    public Monster.e_MonsterState state;
    private void Awake()
    {
        MobAnimator = gameObject.GetComponent<Animator>();
        monster = new Monster();
    }

    void Start()
    {
        state = Monster.e_MonsterState.Attack;
        monster.SetMonsterState(state);

        MonsterAtkDivider();
    }

    void Update()
    {
        state = monster.GetMonsterState();
        MonsterAnimationController();
    }

    void MonsterAnimationController()
    {
        switch(monster.GetMonsterState())
        {
            case Monster.e_MonsterState.Idle:
                MobAnimator.SetInteger("Controller", 0);
                break;
            case Monster.e_MonsterState.Walk:
                MobAnimator.SetInteger("Controller", -1);
                MobAnimator.SetFloat("zPos", fPosZ);
                MobAnimator.SetFloat("xPos", fPosX);
                break;
            case Monster.e_MonsterState.Attack:
                isBattle = true;
                monsterAtk.MonsterAttackStart();
                break;
            case Monster.e_MonsterState.LookAround:
                MobAnimator.SetInteger("Controller", 1);
                break;
            case Monster.e_MonsterState.Hit:
                break;
            case Monster.e_MonsterState.Sturn:
                break;
            case Monster.e_MonsterState.Die:
                break;
        }
    }

    void MonsterAtkDivider()
    {
        string name = monster.GetName();
        switch(name)
        {
            case "Cactus":
                monsterAtk = new MobCactusAttack(monster, MobAnimator);
                break;
            default: break;
        }
    }



    #region 게터세터
    public void SetMonsterClass(Monster monsterCls)
    {
        monster = monsterCls;
    }
    public Monster GetMonsterClass()
    {
        return monster;
    }

    public void SetAnimatorFloatValue(float posX, float posZ)
    {
        fPosX = posX;
        fPosZ = posZ;
    }

    #endregion


}
