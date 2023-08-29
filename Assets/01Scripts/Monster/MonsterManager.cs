using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using static Monster;
using static HandlePauseTool;

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
    bool isDeadFlag;
    NavMeshAgent navMeshController;                     // NavMesh AI 컨트롤러
    List<Transform> taretList;                          // 적 인식 Ragne 내의 객체 List
    public Monster.e_MonsterState state;                // 몬스터의 상태 변수(체크용)
    public float fElapsedTime;                          // 시간 변수
    Vector3 v3_startPos;                                // 몬스터 처음 위치
    MonsterAttack.e_MonsterAttackLevel monsterAtkLevel; // 몬스터 어택 단계 변수
    MonsterHp monsterHPMng;                             // 몬스터 HpBar 매니저 함수

    [SerializeField] GameObject atkColliderBox;         // 몬스터 공격 충돌체크용 콜라이더박스
    public float fPatrolTimeNumber;        // 순찰 시간 변수
    public float fIdleTimeNumber;          // 대기 시간 변수
    public float fChaseRange;              // 최대 기동 범위
    public float fMovePointRange;          // 이동 포인트 범위
    public int hp;                      //체크용
    public Element.e_Element hitted;    //체크용

    public MonsterManager(Monster monster)
    {
        this.monster = monster;
    }
    #endregion

    #region 구조체

    #endregion



    private void Awake()
    {
        atkColliderBox.SetActive(false);
        MobAnimator = gameObject.GetComponent<Animator>();
        navMeshController= gameObject.GetComponent<NavMeshAgent>();
        gameObject.GetComponent<CharacterViewRange>().Attach(this);
    }
    private void OnEnable()
    {
        // 변수 초기화
        fPosX = 0;
        fPosZ = 0;
        isBattle = false;
        isHit = false;
        isDead = false;
        isIdle = false;
        isDeadFlag = false;
        monsterAtkLevel = MonsterAttack.e_MonsterAttackLevel.None;
        v3_startPos = this.gameObject.transform.position;
        navMeshController.ResetPath();

        // 게임매니저의 이벤트에 구독
        GameManager.OnPauseStateChanged += HandlePauseStateChanged;

    }
    private void OnDisable()
    {
        // 게임매니저의 이벤트 구독 해제
        GameManager.OnPauseStateChanged -= HandlePauseStateChanged;
    }

    void Start()
    {
        navMeshController.speed = monster.GetMonsterSpeed();

        // 객체의 몬스터 Object 클래스 찾기.
        MonsterAtkDivider();
    }

    void Update()
    {
        hitted = monster.GetMonsterHittedElement().GetElement(); // 몬스터 피격 원소 체크
        hp = monster.GetMonsterCurrentHp(); // 몬스터 hp 체크
        state = monster.GetMonsterState();  // 몬스터 상태 반영
        Monster_AI_Process();
        MonsterAnimationController();       // 몬스터 애니메이션 컨틀롤러
        if(isDead)
            StartCoroutine(WaitForDeadAnimation());

        MonsterHpMngBroadCastPosition();
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
            isDead = true;
        }
    }

    IEnumerator WaitForDeadAnimation()
    {
        if(!isDeadFlag)
        {
            isDeadFlag = true;
            // 현재 재생 중인 애니메이션 정보 가져오기
            AnimatorStateInfo stateInfo = MobAnimator.GetCurrentAnimatorStateInfo(0);

            // 현재 애니메이션의 길이 및 경과 시간 가져오기
            float animationLength = stateInfo.length;

            yield return new WaitForSeconds(animationLength);

            // 애니메이션 종료 후 원하는 동작 수행
            DeadAnimation();
        }
    }

    // 몬스터 사망시, 오브젝트 풀 리턴
    void DeadAnimation()
    {
        MonsterManager monsterManager = GetComponent<MonsterManager>();
        string name = monster.GetName();
        switch (name)
        {
            case "Cactus":
                if (monsterManager != null)
                    GameManager.Instance.CactusPool.ReturnToPool(monsterManager);
                break;
            case "MushroomAngry":
                    GameManager.Instance.MushroomAngryPool.ReturnToPool(monsterManager);
                break;
            default: break;
        }
        GameManager.Instance.MonsterHpBarPool.ReturnToPool(monsterHPMng);
    }



    void AttackModeRangeChecker()
    {
        // 최대 기동 범위와 시작 포인트의 거리를 계산.
        float distanceToStartPos = Vector3.Distance(transform.position, v3_startPos);

        if (distanceToStartPos > fChaseRange)   // 기동 범위를 벗어났을 경우,
        {
            monsterAtk.SetChaseActive(false);
            isBattle = false;
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
        }
        else
        {
            // 도착 지점에 약간의 보정값을 추가한 임계값 설정
            float stoppingDistanceWithCorrection = navMeshController.stoppingDistance + 0.5f;

            // 이동 시간이 지나면 대기 상태로 변경
            if (navMeshController.remainingDistance <= stoppingDistanceWithCorrection)
            {
                Vector3 randomPosition = GetRandomPosition();
                navMeshController.SetDestination(randomPosition);
            }
        }
        // 이동 방향에 따라 fPosZ와 fPosX 값 할당
        Vector3 velocity = navMeshController.velocity;
        fPosZ = velocity.z;
        fPosX = velocity.x;
        monster.SetMonsterState(e_MonsterState.Walk);

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
        fPosZ = 0f;
        fPosX = 0f;
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
        // 이동 방향에 따라 fPosZ와 fPosX 값 할당
        Vector3 velocity = navMeshController.velocity;
        fPosZ = velocity.z;
        fPosX = velocity.x;
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
                MobAnimator.SetFloat("zPos", fPosZ);
                MobAnimator.SetFloat("xPos", fPosX);
                break;
            case Monster.e_MonsterState.Walk:
                MobAnimator.SetInteger("Controller", -1);
                MobAnimator.SetFloat("zPos", fPosZ);
                MobAnimator.SetFloat("xPos", fPosX);
                break;
            case Monster.e_MonsterState.Attack:
                if(monsterAtkLevel == MonsterAttack.e_MonsterAttackLevel.Chase)
                {
                    MobAnimator.SetInteger("Controller", -1);
                    MobAnimator.SetFloat("zPos", fPosZ);
                    MobAnimator.SetFloat("xPos", fPosX);
                }
                else if(monsterAtkLevel == MonsterAttack.e_MonsterAttackLevel._1st)
                {
                    MobAnimator.SetFloat("zPos", fPosZ);
                    MobAnimator.SetFloat("xPos", fPosX);
                    MobAnimator.SetInteger("Controller", 11);
                }
                else if (monsterAtkLevel == MonsterAttack.e_MonsterAttackLevel._2rd)
                {
                    MobAnimator.SetFloat("zPos", fPosZ);
                    MobAnimator.SetFloat("xPos", fPosX);
                    MobAnimator.SetInteger("Controller", 12);
                }
                else if (monsterAtkLevel == MonsterAttack.e_MonsterAttackLevel._3th)
                {
                    MobAnimator.SetFloat("zPos", fPosZ);
                    MobAnimator.SetFloat("xPos", fPosX);
                    MobAnimator.SetInteger("Controller", 13);
                }
                else if (monsterAtkLevel == MonsterAttack.e_MonsterAttackLevel._4th)
                {
                    MobAnimator.SetFloat("zPos", fPosZ);
                    MobAnimator.SetFloat("xPos", fPosX);
                    MobAnimator.SetInteger("Controller", 14);
                }
                break;
            case Monster.e_MonsterState.LookAround:
                MobAnimator.SetInteger("Controller", 1);
                break;
            case Monster.e_MonsterState.Hit:
                break;
            case Monster.e_MonsterState.Sturn:
                break;
            case Monster.e_MonsterState.Die:
                MobAnimator.SetInteger("Controller", 4);
                MobAnimator.SetFloat("zPos", 0);
                MobAnimator.SetFloat("xPos", 0);
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
                monsterAtk.SetNavMeshAgent(navMeshController);
                monsterAtk.SetChaseRange(fChaseRange);
                monsterAtk.SetAtkColliderBox(atkColliderBox);
                break;
            case "MushroomAngry":
                monsterAtk = gameObject.AddComponent<MobAngryMushroomAttack>();
                monsterAtk.SetMonsetrCls(monster);
                monsterAtk.SetNavMeshAgent(navMeshController);
                monsterAtk.SetChaseRange(fChaseRange);
                monsterAtk.SetAtkColliderBox(atkColliderBox);
                break;
            default: break;
        }
    }

    void MonsterHpMngBroadCastPosition()
    {
        if (monsterHPMng != null)
            monsterHPMng.SetMonsterPos(GetMonsterHeadPosition());
    }


    #endregion



    #region 게터세터
    public void SetMonsterClass(Monster monsterCls){this.monster = monsterCls;}
    public void SetAnimatorFloatValue(float posX, float posZ){fPosX = posX; fPosZ = posZ;}
    public void SetBattleActive(bool isBattle){this.isBattle = isBattle;}
    public void SetMonsterAttackLevel(MonsterAttack.e_MonsterAttackLevel atkLevel){monsterAtkLevel = atkLevel;}
    public void SetMonsterHPMng(MonsterHp monsterHpMng) { monsterHPMng = monsterHpMng; }


    public Monster GetMonsterClass(){return this.monster;}
    public bool GetBattleActive(){return isBattle;}
    public MonsterHp GetMonsterHPMng() { return monsterHPMng; }

    public Vector3 GetMonsterHeadPosition()
    {
        // 몬스터 머리 위 위치를 계산하고 반환하는 로직 추가
        Vector3 headPosition = transform.position + Vector3.up * monster.GetMonsterHeadPos();
        return headPosition;
    }


    #endregion

    #region 옵저버 패턴


    public void AtkLevelNotify(CharacterAttackMng.e_AttackLevel level)
    {
    }

    public void BlinkValueNotify(CharacterControlMng.e_BlinkPos value)
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

    public void AttackSkillStartNotify(){}

    public void AttackSkillEndNotify(){}

    #endregion


}
