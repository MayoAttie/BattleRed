﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Monster;

public class MonsterManager : MonoBehaviour, Observer
{

    #region 변수
    Monster monster;            // 몬스터 데이터 클래스파일
    Animator MobAnimator;       // 애니메이터
    MonsterAttack monsterAtk;   // 몬스터 공격 제어 함수
    float fPosX;                // 이동제어 용 실수 변수
    float fPosZ;                // 이동제어 용 실수 변수
    bool isBattle;              // 전투 체크 변수
    bool isHit;                 // 히트 체크 변수
    bool isDead;                // 죽음 체크 변수
    bool isIdle;                // 아이들 상태 유무 변수
    NavMeshAgent navMeshController;       // NavMesh AI 컨트롤러
    List<Transform> taretList;              // 적 인식 Ragne 내의 객체 List
    public Monster.e_MonsterState state;    // 몬스터의 상태 변수
    public float fElapsedTime;         // 시간 변수
    Vector3 v3_startPos;                    // 몬스터 처음 위치

    public float fPatrolTimeNumber;        // 순찰 시간 변수
    public float fIdleTimeNumber;          // 대기 시간 변수
    public float fChaseRange;              // 최대 기동 범위
    public float fMovePointRange;          // 이동 포인트 범위

    public MonsterManager(Monster monster)
    {
        this.monster = monster;
    }
    #endregion

    #region 구조체

    #endregion

    private void Awake()
    {
        MobAnimator = gameObject.GetComponent<Animator>();
        gameObject.GetComponent<CharacterViewRange>().Attach(this);
        navMeshController= gameObject.GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        v3_startPos = this.gameObject.transform.position;
        navMeshController.speed = monster.GetMonsterSpeed();

        // 객체의 몬스터 Object 클래스 찾기.
        MonsterAtkDivider();
    }

    void Update()
    {
        state = monster.GetMonsterState();  // 몬스터 상태 반영
        Monster_AI_Process();
        MonsterAnimationController();       // 몬스터 애니메이션 컨틀롤러
    }



    #region AI
    // AI 종합 제어 프로세스
    void Monster_AI_Process()
    {
        if (monster.GetMonsterCurrentHp() > 0)
        {
            if (isHit == true)
            {
                monster.SetMonsterState(e_MonsterState.Hit);

            }
            else if (monster.GetMonsterState() == e_MonsterState.Attack)
            {
                AttackModeRangeChecker();
                isBattle = true;
                monsterAtk.MonsterAttackStart();
            }
            else
            {
                PatrolAndIdle();
            }
        }
        else
        {
            monster.SetMonsterState(e_MonsterState.Die);
        }
    }

    // 특정 애니메이션 종료 후, None상태로 돌아왔을 때,
    void NondeStateChecker()
    {
        if (monster.GetMonsterState() == Monster.e_MonsterState.None)
        {
            
        }
    }

    void AttackModeRangeChecker()
    {
        // 최대 기동 범위와 시작 포인트의 거리를 계산.
        float distanceToStartPos = Vector3.Distance(transform.position, v3_startPos);

        if (distanceToStartPos > fChaseRange)   // 기동 범위를 벗어났을 경우,
        {
            monsterAtk.SetChaseActive(false);
        }

    }

    #region Move_AI
    // 이동 및 순찰 제어 함수
    void PatrolAndIdle()
    {
        // Precedence 타입인 경우
        if (monster.GetMonsetrType() == e_MonsterType.Precedence)
        {
            if (isIdle)
            {
                MonsterIdle();
            }
            else
            {
                // 일정 거리 이상 멀어졌을 때, 시작 위치로 돌아감
                if (HasExceededDistanceThreshold())
                {
                    ReturnToStart();
                    return;
                }

                MonsterPatrol();
            }
        }
        // Counter 타입인 경우
        else if (monster.GetMonsetrType() == e_MonsterType.Counter)
        {
            monster.SetMonsterState(e_MonsterState.Idle);
            // 아무 동작도 하지 않음 (Idle 상태)
        }
    }
    // 순찰 액션 함수
    void MonsterPatrol()
    {
        // 어택 상태일 경우, 경로 초기화
        if (monster.GetMonsterState() == e_MonsterState.Attack)
        {
            navMeshController.ResetPath();
            return;
        }

        navMeshController.isStopped = false;
        // 이동 중인데 목적지가 설정되지 않은 경우
        if (!navMeshController.hasPath)
        {
            // 랜덤한 위치로 이동
            Vector3 randomPosition = GetRandomPosition();
            navMeshController.SetDestination(randomPosition);
            monster.SetMonsterState(e_MonsterState.Walk);
        }
        else
        {
            // 도착 지점에 약간의 보정값을 추가한 임계값 설정
            float stoppingDistanceWithCorrection = navMeshController.stoppingDistance + 0.5f;

            // 이동 시간이 지나면 대기 상태로 변경
            if (navMeshController.remainingDistance <= stoppingDistanceWithCorrection)
            {
                monster.SetMonsterState(e_MonsterState.Walk);
                Vector3 randomPosition = GetRandomPosition();
                navMeshController.SetDestination(randomPosition);
            }
        }
        // 이동 방향에 따라 fPosZ와 fPosX 값 할당
        Vector3 velocity = navMeshController.velocity;
        fPosZ = velocity.z;
        fPosX = velocity.x;

        fElapsedTime += Time.deltaTime;
        if (fElapsedTime >= fPatrolTimeNumber)
        {
            fElapsedTime = 0f;
            isIdle = true;
            // 랜덤한 위치로 이동
        }
    }
    // 대기 액션 함수
    void MonsterIdle()
    {
        fElapsedTime += Time.deltaTime;
        navMeshController.isStopped = true;
        monster.SetMonsterState(e_MonsterState.Idle);
        // 대기 시간이 지나면 이동 상태로 변경
        if (fElapsedTime >= fIdleTimeNumber)
        {
            fElapsedTime = 0f;
            isIdle = false;
        }
    }

    // 객체 주변의 랜덤한 위치를 반환하는 함수
    Vector3 GetRandomPosition()
    {
        // 객체 주변의 랜덤한 범위 설정
        float randomRadius = fMovePointRange;
        Vector3 randomOffset = Random.insideUnitSphere * randomRadius;

        // 현재 위치에 랜덤한 범위를 더하여 샘플링
        Vector3 randomPoint = transform.position + randomOffset;
        NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, randomRadius, NavMesh.AllAreas);
        return hit.position;
    }

    void ReturnToStart()
    {
        // 처음 제자리로 돌아가는 동작을 구현합니다.
        navMeshController.SetDestination(v3_startPos);
        navMeshController.isStopped = false;
        monster.SetMonsterState(Monster.e_MonsterState.Walk);
    }

    bool HasExceededDistanceThreshold()
    {
        // 일정 거리 이상 멀어졌는지 확인하는 로직을 구현합니다.
        float distanceThreshold = fChaseRange;
        float distanceToStart = Vector3.Distance(transform.position, v3_startPos);

        return distanceToStart >= distanceThreshold;
    }
    #endregion

    
    #endregion

    #region 애니메이션 관리
    void MonsterAnimationController()
    {
        switch (monster.GetMonsterState())
        {
            case Monster.e_MonsterState.Idle:
                MobAnimator.SetInteger("Controller", 0);
                MobAnimator.SetFloat("zPos", 0);
                MobAnimator.SetFloat("xPos", 0);
                break;
            case Monster.e_MonsterState.Walk:
                MobAnimator.SetInteger("Controller", -1);
                MobAnimator.SetFloat("zPos", fPosZ);
                MobAnimator.SetFloat("xPos", fPosX);
                break;
            case Monster.e_MonsterState.Attack:

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
            default: break;
        }
    }
    #endregion

    #region 기타함수

    // 몬스터 Object 클래스 분류
    void MonsterAtkDivider()
    {
        string name = monster.GetName();
        switch (name)
        {
            case "Cactus":
                // 새로운 MobCactusAttack 컴포넌트 추가
                monsterAtk = gameObject.AddComponent<MobCactusAttack>();
                monsterAtk.SetMonsetrCls(monster);
                monsterAtk.SetAnimator(MobAnimator);
                monsterAtk.SetNavMeshAgent(navMeshController);
                monsterAtk.SetChaseRange(fChaseRange);
                break;
            default: break;
        }
    }
    #endregion



    #region 게터세터
    public void SetMonsterClass(Monster monsterCls)
    {
        this.monster = monsterCls;
    }
    public void SetAnimatorFloatValue(float posX, float posZ)
    {
        fPosX = posX;
        fPosZ = posZ;
    }
    public void SetBattleActive(bool isBattle)
    {
        this.isBattle = isBattle;
    }


    public Monster GetMonsterClass()
    {
        return this.monster;
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

    public void GetEnemyFindNotify(List<Transform> findList)
    {
        // 선공형일 경우, 범위 내에 적이 포착된 경우 공격 모드로 전환
        if(monster.GetMonsetrType() == e_MonsterType.Precedence)
        {
            if(findList != null && findList.Count>0)
            {
                monster.SetMonsterState(Monster.e_MonsterState.Attack);
                taretList = findList;
                monsterAtk.TargetInputter(findList);
            }
        }
    }
    #endregion


}
