using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using static Monster;
using static HandlePauseTool;
public class MonsterManager : Subject, Observer
{

    #region 변수
    Monster monster;            // 몬스터 데이터 클래스파일
    Animator MobAnimator;       // 애니메이터
    MonsterAttack monsterAtk;                           // 몬스터 공격 제어 함수
    MonsterAniControl monsterController;                 // 몬스터 애니메이션 컨트롤러 함수
    float fPosX;                // 이동제어 용 실수 변수
    float fPosZ;                // 이동제어 용 실수 변수
    bool isBattle;              // 전투 체크 변수
    bool isHit;                 // 히트 체크 변수
    bool isSturn;               // 스턴 체크 변수
    bool isDead;                // 죽음 체크 변수
    bool isIdle;                // 아이들 상태 유무 변수
    bool isDeadFlag;
    bool isDetected;            // 캐릭터에게 탐지당한 적.
    bool isStaticFixed;          // 몬스터를 동적으로 관리하지 않음. 즉 일정 범위를 벗어나더라도 파괴되지 않음.
    NavMeshAgent navMeshController;                     // NavMesh AI 컨트롤러
    List<Transform> taretList;                          // 적 인식 Ragne 내의 객체 List
    public Monster.e_MonsterState state;                // 몬스터의 상태 변수(체크용)
    public float fElapsedTime;                          // 시간 변수
    Vector3 v3_startPos;                                // 몬스터 처음 위치
    MonsterAttack.e_MonsterAttackLevel monsterAtkLevel; // 몬스터 어택 단계 변수
    MonsterHp monsterHPMng;                             // 몬스터 HpBar 매니저 함수
    Image[] monsterHpbar_img;                           // 몬스터 hp바 이미지
    int playerrLayer;                                   // 플레이어 Layer 번호
    Coroutine MonsterInitCoroutine;                     // 코루틴 제어용 코루틴변수
    private Rigidbody rigidbody;                                        // rigdBody컴포넌트

    [SerializeField] GameObject atkColliderBox;         // 몬스터 공격 충돌체크용 콜라이더박스
    public float fPatrolTimeNumber;        // 순찰 시간 변수
    public float fIdleTimeNumber;          // 대기 시간 변수
    public float fChaseRange;              // 최대 기동 범위
    public float fMovePointRange;          // 이동 포인트 범위

    MonsterControl_Info monsterControl_info;            // 몬스터 행동 제어변수 동적 조정값 저장 변수

    public int hp;                      //체크용
    public Element.e_Element hitted;    //체크용

    public MonsterManager(Monster monster)
    {
        this.monster = monster;
    }
    #endregion

    #region 구조체

    public struct MonsterControl_Info
    {
        public float patrolTime;
        public float idleTime;
        public float chaseRange;
        public float moveRange;

        public MonsterControl_Info(float patrolTime, float idleTime, float chaseRange, float moveRange)
        {
            this.patrolTime = patrolTime;
            this.idleTime = idleTime;
            this.chaseRange = chaseRange;
            this.moveRange = moveRange;
        }
    }

    #endregion



    private void Awake()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        playerrLayer = LayerMask.NameToLayer("Player");
        atkColliderBox.SetActive(false);
        MobAnimator = gameObject.GetComponent<Animator>();
        navMeshController= gameObject.GetComponent<NavMeshAgent>();
        gameObject.GetComponent<CharacterViewRange>().Attach(this);
    }
    private void OnEnable()
    {
        // 변수 초기화
        MonsterInitCoroutine = null;
        monsterControl_info = default;
        monsterHpbar_img = null;
        taretList =  new List<Transform>();
        fPosX = 0;
        fPosZ = 0;
        isBattle = false;
        isStaticFixed = false;
        isSturn = false;
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
        if(MonsterInitCoroutine!=null)
            StopCoroutine(MonsterInitCoroutine);

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
        if (monsterHPMng != null && MonsterInitCoroutine == null)
            MonsterInitCoroutine = StartCoroutine(CheckCollidersPeriodically());

        hitted = monster.GetMonsterHittedElement().GetElement(); // 몬스터 피격 원소 체크
        hp = monster.GetMonsterCurrentHp(); // 몬스터 hp 체크
        state = monster.GetMonsterState();  // 몬스터 상태 반영
        Monster_AI_Process();               // AI 종합 제어 함수
        monsterController._Monster = monster;                                                           // 컨트롤러의 몬스터 클래스 재세팅
        monsterController.MonsterAnimationController(ref fPosZ, ref fPosX, ref monsterAtkLevel);       // 몬스터 애니메이션 컨틀롤러
        if(isDead)
            StartCoroutine(WaitForDeadAnimation()); // 사망 함수

        MonsterHpMngBroadCastPosition();            // 몬스터 HP바 위치 포지션 수정
    }
    private void FixedUpdate()
    {
    }


    #region AI
    // AI 종합 제어 프로세스
    void Monster_AI_Process()
    {
        if (monster.GetMonsterCurrentHp() > 0)
        {
            if (isHit == true || isSturn)
            {
                if (isHit)
                { 
                    monster.SetMonsterState(e_MonsterState.Hit);

                    // 애니메이션 동작 완료 시, 공격 상태 전환 및 히트 상태 해제
                    if (MobAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit") &&
                    MobAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                    {
                        isHit = false;
                        monster.SetMonsterState(e_MonsterState.Attack);
                    }
                }
                else if(isSturn)
                {
                    monster.SetMonsterState(e_MonsterState.Sturn);

                    // 애니메이션 동작 완료 시, 공격 상태 전환 및 스턴 상태 해제
                    if (MobAnimator.GetCurrentAnimatorStateInfo(0).IsName("Sturn") &&
                    MobAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                    {
                        isSturn = false;
                        monster.SetMonsterState(e_MonsterState.Attack);
                    }
                }
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
                ItemDropManager.Instance.ItemDrop(this.transform, "무기");    // 무기 드랍
                GameManager.Instance.CactusPool.ReturnToPool(monsterManager);
                break;
            case "MushroomAngry":
                ItemDropManager.Instance.ItemDrop(this.transform, "성유물");   // 성유물 드랍
                GameManager.Instance.MushroomAngryPool.ReturnToPool(monsterManager);
                break;
            case "Golem_Boss":
                GameManager.Instance.GolemBossPool.ReturnToPool(monsterManager);
                break;
            default: break;
        }
        GameManager.Instance.MonsterHpBarPool.ReturnToPool(monsterHPMng);
    }



    void AttackModeRangeChecker()
    {
        // 최대 기동 범위와 시작 포인트의 거리를 계산.
        float distanceToStartPos = Vector3.Distance(transform.position, v3_startPos);

        if(monsterControl_info.chaseRange == default)
        {
            if (distanceToStartPos > fChaseRange)   // 기동 범위를 벗어났을 경우,
            {
                monsterAtk.SetChaseActive(false);
                isBattle = false;
            }
        }
        else
        {
            if (distanceToStartPos > monsterControl_info.chaseRange)   // 기동 범위를 벗어났을 경우,
            {
                monsterAtk.SetChaseActive(false);
                isBattle = false;
            }
        }
    }

    public void WhenHittedChaseToTarget(CharacterManager userManager)
    {
        if (monster.GetMonsterState() == e_MonsterState.Attack)
            return;

        monster.SetMonsterState(Monster.e_MonsterState.Attack);
        monsterAtk.TargetInputter(userManager.transform);
    }

    //히트 시, 경직 누적 함수
    public void HitStrunPointAccumulate(float fPoint)
    {
        monsterController.HitStrunAccumulate(fPoint, ref isHit, ref isSturn);
    }


    #region Move_AI
    // 이동 및 순찰 제어 함수
    void PatrolAndIdle()
    {
        // Precedence 타입인 경우
        if (monster.GetMonsterType() == e_MonsterType.Precedence)
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
        else if (monster.GetMonsterType() == e_MonsterType.Counter)
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

            if (navMeshController.remainingDistance <= stoppingDistanceWithCorrection)
            {   // 보정된 도착지 값 안에 들어왔을 경우에, 새로운 경로를 찾고 도착지로 설정
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

        if (monsterControl_info.patrolTime == default)
        {
            if (fElapsedTime >= fPatrolTimeNumber)
            {
                fElapsedTime = 0f;
                isIdle = true;
                // 랜덤한 위치로 이동
            }
        }
        else
        {
            if (fElapsedTime >= monsterControl_info.patrolTime)
            {
                fElapsedTime = 0f;
                isIdle = true;
                // 랜덤한 위치로 이동
            }
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
        if(monsterControl_info.idleTime == default)
        {
            if (fElapsedTime >= fIdleTimeNumber)
            {
                fElapsedTime = 0f;
                isIdle = false;
            }
        }
        else
        {
            if (fElapsedTime >= monsterControl_info.idleTime)
            {
                fElapsedTime = 0f;
                isIdle = false;
            }
        }

    }

    // 객체 주변의 랜덤한 위치를 반환하는 함수
    Vector3 GetRandomPosition()
    {
        // 객체 주변의 랜덤한 범위 설정
        float randomRadius = 0;

        if (monsterControl_info.moveRange == default)
            randomRadius = fMovePointRange;
        else
            randomRadius = monsterControl_info.moveRange;

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
        float distanceThreshold = 0;

        if(monsterControl_info.chaseRange == default)
        {
            distanceThreshold = fChaseRange;
        }
        else
        {
            distanceThreshold = monsterControl_info.chaseRange;
        }

        float distanceToStart = Vector3.Distance(transform.position, v3_startPos);

        return distanceToStart >= distanceThreshold;
    }
    #endregion

    
    #endregion

    #region 애니메이션 관리
    //void MonsterAnimationController()
    //{   
    //    switch (monster.GetMonsterState())
    //    {
    //        case Monster.e_MonsterState.Idle:               // 정지
    //            MobAnimator.SetInteger("Controller", 0);
    //            MobAnimator.SetFloat("zPos", fPosZ);
    //            MobAnimator.SetFloat("xPos", fPosX);
    //            break;
    //        case Monster.e_MonsterState.Walk:               // 이동
    //            MobAnimator.SetInteger("Controller", -1);
    //            MobAnimator.SetFloat("zPos", fPosZ);
    //            MobAnimator.SetFloat("xPos", fPosX);
    //            break;
    //        case Monster.e_MonsterState.Attack:             // 공격
    //            if(monsterAtkLevel == MonsterAttack.e_MonsterAttackLevel.Chase)
    //            {
    //                MobAnimator.SetInteger("Controller", -1);
    //                MobAnimator.SetFloat("zPos", fPosZ);
    //                MobAnimator.SetFloat("xPos", fPosX);
    //            }
    //            else if(monsterAtkLevel == MonsterAttack.e_MonsterAttackLevel._1st)
    //            {
    //                MobAnimator.SetFloat("zPos", fPosZ);
    //                MobAnimator.SetFloat("xPos", fPosX);
    //                MobAnimator.SetInteger("Controller", 11);
    //            }
    //            else if (monsterAtkLevel == MonsterAttack.e_MonsterAttackLevel._2rd)
    //            {
    //                MobAnimator.SetFloat("zPos", fPosZ);
    //                MobAnimator.SetFloat("xPos", fPosX);
    //                MobAnimator.SetInteger("Controller", 12);
    //            }
    //            else if (monsterAtkLevel == MonsterAttack.e_MonsterAttackLevel._3th)
    //            {
    //                MobAnimator.SetFloat("zPos", fPosZ);
    //                MobAnimator.SetFloat("xPos", fPosX);
    //                MobAnimator.SetInteger("Controller", 13);
    //            }
    //            else if (monsterAtkLevel == MonsterAttack.e_MonsterAttackLevel._4th)
    //            {
    //                MobAnimator.SetFloat("zPos", fPosZ);
    //                MobAnimator.SetFloat("xPos", fPosX);
    //                MobAnimator.SetInteger("Controller", 14);
    //            }
    //            break;
    //        case Monster.e_MonsterState.LookAround:
    //            MobAnimator.SetInteger("Controller", 1);
    //            break;
    //        case Monster.e_MonsterState.Hit:
    //            MobAnimator.SetInteger("Controller", 2);
    //            MobAnimator.SetFloat("zPos", 0);
    //            MobAnimator.SetFloat("xPos", 0);
    //            break;
    //        case Monster.e_MonsterState.Sturn:
    //            MobAnimator.SetInteger("Controller", 3);
    //            break;
    //        case Monster.e_MonsterState.Die:
    //            MobAnimator.SetInteger("Controller", 4);
    //            MobAnimator.SetFloat("zPos", 0);
    //            MobAnimator.SetFloat("xPos", 0);
    //            break;
    //        default: break;
    //    }
    //}
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
                if (monsterControl_info.chaseRange == default)
                    monsterAtk.SetChaseRange(fChaseRange);
                else
                    monsterAtk.SetChaseRange(monsterControl_info.chaseRange);
                monsterAtk.SetAtkColliderBox(atkColliderBox);
                monsterAtk._MonsterMng = this;
                // 애니컨트롤러 추가
                monsterController = gameObject.AddComponent<MonsterAniControl_type1>();
                monsterController._Monster = monster;
                monsterController._MobAnimator = MobAnimator;
                break;
            case "MushroomAngry":
                monsterAtk = gameObject.AddComponent<MobAngryMushroomAttack>();
                monsterAtk.SetMonsetrCls(monster);
                monsterAtk.SetNavMeshAgent(navMeshController);
                if (monsterControl_info.chaseRange == default)
                    monsterAtk.SetChaseRange(fChaseRange);
                else
                    monsterAtk.SetChaseRange(monsterControl_info.chaseRange);
                monsterAtk.SetAtkColliderBox(atkColliderBox);
                monsterAtk._MonsterMng = this;
                // 애니컨트롤러 추가
                monsterController = gameObject.AddComponent<MonsterAniControl_type1>();
                monsterController._Monster = monster;
                monsterController._MobAnimator = MobAnimator;
                break;
            case "Golem_Boss":
                monsterAtk = gameObject.AddComponent<MobGolemBossAttack>();
                monsterAtk.SetMonsetrCls(monster);
                monsterAtk.SetNavMeshAgent(navMeshController);
                if (monsterControl_info.chaseRange == default)
                    monsterAtk.SetChaseRange(fChaseRange);
                else
                    monsterAtk.SetChaseRange(monsterControl_info.chaseRange);
                monsterAtk.SetAtkColliderBox(atkColliderBox);
                monsterAtk._MonsterMng = this;
                // 애니컨트롤러 추가
                monsterController = gameObject.AddComponent<MonsterAniControl_type2>();
                monsterController._Monster = monster;
                monsterController._MobAnimator = MobAnimator;

                break;
            default: break;
        }
    }

    void MonsterHpMngBroadCastPosition()
    {
        if (monsterHPMng != null)
            monsterHPMng.SetMonsterPos(GetMonsterHeadPosition());
    }

    private IEnumerator CheckCollidersPeriodically()
    {
        while (true)
        {
            WorldPlayerCheck();
            yield return new WaitForSeconds(0.2f);
        }
    }

    // 범위 내에 플레이어가 없을 경우, 몬스터 pool 리턴
    void WorldPlayerCheck()
    {
        if(gameObject.activeSelf)
        {

            if(!isStaticFixed)  // 동적 생성일때,
            {
                if (taretList != null && taretList.Count > 0)
                    return;

                Collider[] colliders = Physics.OverlapSphere(transform.position, 20, 1 << playerrLayer);
                Debug.Log("MonsterManager WorldPlayerCheck Count : " + colliders.Length);
                if (colliders.Length <= 0)
                    GameManager.Instance.RangeOutMonsterPoolReturn(this, monsterHPMng);
            }
            else                // 정적 생성일때,
            {
                if(monsterHpbar_img == null)
                    monsterHpbar_img = monsterHPMng.GetImages();

                if (taretList != null && taretList.Count > 0)
                {
                    foreach (var tmp in monsterHpbar_img)
                        tmp.enabled = true;
                    return;
                }

                if (isBattle)
                {
                    foreach (var tmp in monsterHpbar_img)
                        tmp.enabled = true;
                    return;
                }

                Collider[] colliders = Physics.OverlapSphere(transform.position, 20, 1 << playerrLayer);
                // 플레이어가 일정 범위 내에 없을 때,
                if (colliders.Length <= 0)
                {
                    foreach (var tmp in monsterHpbar_img)
                        tmp.enabled = false;
                }
                else
                {
                    foreach (var tmp in monsterHpbar_img)
                        tmp.enabled = true;
                }
            }
        }
    }

    #endregion



    #region 게터세터
    public void SetMonsterClass(Monster monsterCls){this.monster = monsterCls;}
    public void SetAnimatorFloatValue(float posX, float posZ){fPosX = posX; fPosZ = posZ;}
    public void SetBattleActive(bool isBattle){this.isBattle = isBattle;}
    public void SetMonsterAttackLevel(MonsterAttack.e_MonsterAttackLevel atkLevel){monsterAtkLevel = atkLevel;}
    public void SetMonsterHPMng(MonsterHp monsterHpMng) { monsterHPMng = monsterHpMng; }
    public void SetDetectedActive(bool isDetected) {  this.isDetected = isDetected;}
    public void SetStaticFixed(bool isStaticFixed) { this.isStaticFixed = isStaticFixed; }

    public Monster GetMonsterClass(){return this.monster;}
    public bool GetBattleActive(){return isBattle;}
    public MonsterHp GetMonsterHPMng() { return monsterHPMng; }
    public bool GetDetectedActive() { return isDetected;}
    public bool GetIsHit() { return isHit;}
    public bool GetStaticFixed() { return isStaticFixed; }
    public Rigidbody _Rigidbody
    {
        get { return rigidbody; }
    }

    public Vector3 GetMonsterHeadPosition()
    {
        // 몬스터 머리 위 위치를 계산하고 반환하는 로직 추가
        Vector3 headPosition = transform.position + Vector3.up * monster.GetMonsterHeadPos();
        return headPosition;
    }

    public void SetMonsterControlInfo(float patrolTime, float idleTime, float chaseRange, float moveRange)
    {
        // Setter 사용
        monsterControl_info.patrolTime = patrolTime;
        monsterControl_info.idleTime = idleTime;
        monsterControl_info.chaseRange = chaseRange;
        monsterControl_info.moveRange = moveRange;
    }
    public void SetMonsterControlInfo(MonsterControl_Info info) { monsterControl_info = info; }
    public MonsterControl_Info GetMonsterControl_Info() { return monsterControl_info; }




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
        if(monster.GetMonsterType() != e_MonsterType.Counter    )
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

    public void CheckPoint_PlayerPassNotify(int num){}

    public void WorldMapOpenNotify(){}

    public void WorldMapCloseNotify(){}

    #endregion


}
