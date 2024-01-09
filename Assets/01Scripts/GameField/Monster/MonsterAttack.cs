using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using static HandlePauseTool;

public class MonsterAttack : MonoBehaviour, Observer
{
    #region 변수
    Monster monsterCls;                        // 부모 객체의 몬스터 클래스
    List<Transform> TargetList;                // 전체 적 객체
    GameObject target;                         // 전투 대상 오브젝트
    public bool isChase;                       // 추격 중임을 체크하는 변수
    public bool isTargetInRange;               // 공격 범위 내, 적 접근 시 활성화
    public bool isBattle;                      // 전투 중임을 체크하는 변수
    bool isChageReturn;                         // 특수 행동으로 인한, 추적 종료.
    float fSetChaseRange;                      // 최대 추격 기동 범위 - 몬스터 매니저에서 받음
    MonsterManager monsterMng;                  // 몬스터 매니저 변수
    
    protected NavMeshAgent navAgent;                // 추적용 navMesh 에이전트
    protected e_MonsterAttackLevel attackLevel;     // attackLevel 변수
    protected bool isAtkAnimationConrolFlag;        // 몬스터 애니메이션 제어용 부울변수
    GameObject colliderBox;                         // 공격 충돌 체크용 콜라이더 박스
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


    protected virtual void OnEnable()
    {
        // 변수 초기화
        isChase = false;
        isTargetInRange = false;
        isBattle = false;
        isChageReturn = false;
        attackLevel = e_MonsterAttackLevel.None;
        isAtkAnimationConrolFlag = false;
        target = null;
        // 게임매니저의 이벤트에 구독
        GameManager.OnPauseStateChanged += HandlePauseStateChanged;
    }
    protected virtual void OnDisable()
    {
        // 게임매니저의 이벤트 구독 해제
        GameManager.OnPauseStateChanged -= HandlePauseStateChanged;
    }

    protected void Update()
    {
        if (isBattle == false)
            return;

        if (monsterCls.GetMonsterState() == Monster.e_MonsterState.Hit || monsterCls.GetMonsterState() == Monster.e_MonsterState.Sturn) // 히트 혹은 스턴 상태일 경우, 리턴
            return;

        TargetChaseMove();
    }

    public MonsterAttack(Monster monsterCls, NavMeshAgent navAgent)
    {
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
    public void TargetInputter(Transform target)
    {
        this.target = target.gameObject;
    }


    // 몬스터 전투 해제용
    public void EndPageChaseCheck()
    {
        if (target == null && isBattle)
        {
            isChase = false;    // 타깃이 없을 경우 추적 중지
        }

        if (!isChase)   // 추적이 중지일 경우에는 전투를 해제.
        {
            isBattle = false;
            monsterMng.GetMonsterClass().SetMonsterState(Monster.e_MonsterState.None);     // 몬스터 매니저에 전투 중지를 알림
            monsterMng.SetBattleActive(false);                                             // 몬스터 매니저에 전투 중지를 알림
            gameObject.GetComponent<AttackRange>().Detach(this);                    // 옵저버 패턴 해제
        }
        else
        {
            gameObject.GetComponent<AttackRange>().Attach(this);                    // 옵저버 패턴 부착
        }

    }

    #region 몬스터 타깃 추격


    void TargetChaseMove()
    {
        if (isChageReturn == true)
            return;

        if (target != null && isBattle)
        {
            
            // 타깃과 몬스터의 거리 계산
            float distanceToTarget = Vector3.Distance(transform.position, GetTarget().transform.position);

            if (isTargetInRange)
            {
                navAgent.isStopped = true;
                monsterMng.SetAnimatorFloatValue(0, 0);
                return;
            }

            // 일정 거리 이상일 경우 추격 중단
            if (distanceToTarget > fSetChaseRange)
            {
                isChase = false;
                isAtkAnimationConrolFlag = false;
                return;
            }

            isChase = true;
            navAgent.isStopped = false;
            isAtkAnimationConrolFlag = false;

            // 타깃 방향으로 회전
            RotateTowardsTarget();

            // 타깃 방향으로 이동
            navAgent.SetDestination(TargetPositionRetrun());

            // 이동 방향에 따라 fPosZ와 fPosX 값 할당
            Vector3 velocity = navAgent.velocity;
            monsterMng.SetAnimatorFloatValue(velocity.x, velocity.z);
            attackLevel = e_MonsterAttackLevel.Chase;
            monsterMng.SetMonsterAttackLevel(attackLevel);
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
    public void SetMonsetrCls(Monster monster){monsterCls = monster;}
    public void SetChaseActive(bool isChase){this.isChase = isChase;}
    public void SetAtkColliderBox(GameObject box){colliderBox = box;}
    public void SetAttackLevel(e_MonsterAttackLevel attackLevel)
    {
        this.attackLevel = attackLevel;
        monsterMng.SetAnimatorFloatValue(0, 0);
        monsterMng.SetMonsterAttackLevel(attackLevel);
    }
    public void SetBattleActive(bool isBattle){this.isBattle = isBattle;}
    public void SetNavMeshAgent(NavMeshAgent navAgent){this.navAgent = navAgent;}
    public void SetChaseRange(float fSetChaseRange){this.fSetChaseRange = fSetChaseRange;}

    public Monster GetMonsterCls(){return monsterCls;}
    public bool GetChaseActive(){return isChase;}
    public GameObject GetTarget(){return target;}
    public e_MonsterAttackLevel GetAttackLevel(){return attackLevel;}
    public List<Transform> GetTargetList(){return TargetList;}
    public bool GetTargetInRange(){return isTargetInRange;}
    public bool GetBattleActive(){return isBattle;}
    public GameObject GetAtkColliderBox(){return colliderBox;}

    public MonsterManager _MonsterMng
    {
        get { return monsterMng; }
        set { monsterMng = value; }
    }
    public bool IsChageReturn
    {
        get { return isChageReturn; }
        set { isChageReturn = value; }
    }

    #endregion

    #region 옵저버 패턴

    public void AtkLevelNotify(CharacterAttackMng.e_AttackLevel level)
    {
    }

    public void BlinkValueNotify(CharacterControlMng.e_BlinkPos value)
    {
    }


    public void GetEnemyFindNotify(List<Transform> findList)    // 공격 가능한 대상, 접근시 bool-true
    {
        isTargetInRange = findList.Exists(tmp => tmp.gameObject == target);
    }

    public void AttackSkillStartNotify(){}

    public void AttackSkillEndNotify(){}

    public void CheckPoint_PlayerPassNotify(int num){}

    public void WorldMapOpenNotify(){}

    public void WorldMapCloseNotify(){}

    public void ConvertToTargetStateNotify(List<Vector3> listTarget){}



    #endregion

}
