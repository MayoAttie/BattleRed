﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MonsterAttack : MonoBehaviour, Observer
{
    #region 변수
    Monster monsterCls;                 // 부모 객체의 몬스터 클래스
    Animator animator;                  // 부모 객체 애니메이터
    List<Transform> TargetList;         // 전체 적 객체
    GameObject target;                  // 전투 대상 오브젝트
    public bool isChase;                       // 추격 중임을 체크하는 변수
    public bool isTargetInRange;               // 공격 범위 내, 적 접근 시 활성화
    public bool isBattle;                      // 전투 중임을 체크하는 변수
    float fSetChaseRange;               // 최대 추격 기동 범위 - 몬스터 매니저에서 받음
    protected NavMeshAgent navAgent;                // 추적용 navMesh 에이전트
    protected e_MonsterAttackLevel attackLevel;     // attackLevel 변수
    #endregion


    #region 구조체
    public enum e_MonsterAttackLevel
    {
        None,
        Chase,
        _1st,
        _2rd,
        _3th,
        _4th,
        _5th,
        Max
    }
    
    #endregion

    private void Awake()
    {
    }
    void Start()
    {
    }

    protected void Update()
    {
        if (isBattle == false)
            return;


        TargetChaseMove();
        ChaseCheck();
    }

    public MonsterAttack(Monster monsterCls, Animator animator, NavMeshAgent navAgent)
    {
        this.animator = animator;
        this.monsterCls = monsterCls;
        this.navAgent = navAgent;
    }

    public MonsterAttack(){ }


    public virtual void MonsterAttackStart()    // 공격 시작함수
    {
        isBattle = true;
        isChase = true;
    }

    public void TargetInputter(List<Transform> targets) // 전체 레인지 타깃 받기 함수
    {
        TargetList = targets;
        target = targets[0].gameObject;
    }


    // 몬스터 전투 해제용
    private void ChaseCheck()
    {
        if (target == null && isBattle)
        {
            isChase = false;
        }

        if (!isChase)
        {
            isBattle = false;
            MonsterManager mng = gameObject.GetComponent<MonsterManager>();
            mng.GetMonsterClass().SetMonsterState(Monster.e_MonsterState.None);
            mng.SetBattleActive(false);
            gameObject.GetComponent<AttackRange>().Detach(this);
        }
        else
        {
            gameObject.GetComponent<AttackRange>().Attach(this);
        }

    }

    #region 몬스터 타깃 추격


    void TargetChaseMove()
    {
        if (target != null && isBattle)
        {
            if(gameObject == null)
                Debug.Log("gameObject is null");
            // 타깃과 몬스터의 거리 계산
            float distanceToTarget = Vector3.Distance(transform.position, GetTarget().transform.position);

            if (GetTargetInRange())
            {
                navAgent.isStopped = true;
                return;
            }

            // 일정 거리 이상일 경우 추격 중단
            if (distanceToTarget > fSetChaseRange)
            {
                isChase = false;
                return;
            }

            isChase = true;
            navAgent.isStopped = false;

            // 타깃 방향으로 회전
            RotateTowardsTarget();

            // 타깃 방향으로 이동
            navAgent.SetDestination(TargetPositionRetrun());
        }
    }

    void RotateTowardsTarget()
    {
        Vector3 targetDirection = GetTarget().transform.position - transform.position;
        targetDirection.y = 0f; // 몬스터는 y축으로 회전하지 않도록 함
        Quaternion rotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * monsterCls.GetMonsterRotateSpeed());
    }

    Vector3 TargetPositionRetrun()
    {
        return GetTarget().transform.position;
    }


    #endregion

    #region 세터게터
    public void SetMonsetrCls(Monster monster)
    {
        monsterCls = monster;
    }
    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
    }
    public void SetChaseActive(bool isChase)
    {
        this.isChase = isChase;
    }
    public void SetAttackLevel(e_MonsterAttackLevel attackLevel)
    {
        this.attackLevel = attackLevel;
    }
    public void SetBattleActive(bool isBattle)
    {
        this.isBattle = isBattle;
    }
    public void SetNavMeshAgent(NavMeshAgent navAgent)
    {
        this.navAgent = navAgent;
    }
    public void SetChaseRange(float fSetChaseRange)
    {
        this.fSetChaseRange = fSetChaseRange;
    }

    public Monster GetMonsterCls()
    {
        return monsterCls;
    }
    public Animator GetAnimator()
    {
        return animator;
    }
    public bool GetChaseActive()
    {
        return isChase;
    }
    public GameObject GetTarget()
    {
        return target;
    }
    public e_MonsterAttackLevel GetAttackLevel()
    {
        return attackLevel;
    }
    public List<Transform> GetTargetList()
    {
        return TargetList;
    }
    public bool GetTargetInRange()
    {
        return isTargetInRange;
    }
    public bool GetBattleActive()
    {
        return isBattle;
    }

    #endregion

    #region 옵저버 패턴
    public void AttackEventNotify(int num)
    {
    }

    public void AttackEventStartNotify()
    {
    }

    public void AtkLevelNotify(CharacterAttackMng.e_AttackLevel level)
    {
    }

    public void BlinkValueNotify(CharacterControlMng.e_BlinkPos value)
    {
    }

    public void GetBlinkEndNotify()
    {
    }

    public void GetBlinkStartNotify()
    {
    }

    public void GetBrockEndNotify()
    {
    }

    public void GetEnemyFindNotify(List<Transform> findList)    // 공격 가능한 대상, 접근시 bool-true
    {
        Debug.Log("findList.Count : " + findList.Count);
        isTargetInRange = findList.Exists(tmp => tmp.gameObject == target);
    }
    #endregion

}
