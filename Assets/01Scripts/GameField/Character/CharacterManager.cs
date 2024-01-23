using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static HandlePauseTool;
using static UI_UseToolClass;

public class CharacterManager : Singleton<CharacterManager>, Observer
{
    #region 변수
    private CharacterClass clsCharacter;                    // 캐릭터 데이터 클래스
    private Animator aniController;                         // 캐릭터 제어 애니메이터
    private bool isBattle;                                  // 전투 체크 변수
    private CharacterAttackMng.e_AttackLevel atkLevel;      // 공격 단계 변수
    private CharacterControlMng.e_BlinkPos blinkValue;      // 회피기 방향 변수
    private Element.e_Element element;      // 선택한 원소 변수

    private float xPos;                               // 캐릭터 애니메이션 플로트 변수
    private float zPos;                               // 캐릭터 애니메이션 플로트 변수
    private float runX;                               // 캐릭터 애니메이션 플로트 변수
    private float runZ;                               // 캐릭터 애니메이션 플로트 변수
    
    GameObject SatelliteObj;                           // 원소 체크용 위성 객체
    bool isClickedCoolCheck;                          // 버튼 쿨타임 코루틴 객체
    bool isMonsterSpwan;                              // 스폰 제어 플래그
    bool isControl;

    int SpwanPoint;
    int monsterLayer;
    int interactionLayer;

    public List<Transform> targetsListIn10Range;       // 범위 내 몬스터 객체 리스트
    Dictionary<InteractionObject, DropItem_UI> dic_dropAndInterObj; // 상호작용 오브젝트와 ui 묶음

    CharacterControlMng controlMng;
    CharacterAttackMng attackMng;

    int characterHp;                                    // 테스트용
    CharacterClass.eCharactgerState clsState;           // 테스트 확인용
    bool elementGetActive;                              // 테스트용
    //Transform sideUI_ObjPrintTransform;                 // 상호작용 오브젝트 UI 출력용 스크롤뷰

    PathFinder pathFinder;                              // 캐릭터가 보유한 패스파인더 객체

    //매니저 함수
    Drop_Item_ScrolViewMng sideUI_ObjPrintTransformObject;          // 스크린 객체 매니저 
    ObjectManager objMng_instance;
    #endregion

    private void Awake()
    {
        isControl = true;
        isBattle = false;
        SatelliteObj = transform.GetChild(6).gameObject;
        targetsListIn10Range = new List<Transform>();
        controlMng = gameObject.GetComponent<CharacterControlMng>();
        attackMng = gameObject.GetComponent<CharacterAttackMng>();
        controlMng.Attach(this);                                        // 옵저버패턴 부착
        attackMng.Attach(this);     // 옵저버패턴 부착
        gameObject.GetComponent<CharacterViewRange>().Attach(this);     // 옵저버 패턴 부착
        aniController = gameObject.GetComponent<Animator>();            // 애니메이터 초기화
        SatelliteObj.gameObject.SetActive(false);                       // 원소 상태 표시용 위성 SetFalse
        objMng_instance = ObjectManager.Instance;
        pathFinder = gameObject.GetComponent<PathFinder>();
        dic_dropAndInterObj = new Dictionary<InteractionObject, DropItem_UI>();
    }

    private void Start()
    {
        clsCharacter = GameManager.Instance.GetUserClass().GetUserCharacter();           // 캐릭터 클래스 초기화
        UI_Manager.Instance.HpBarFill_Init(clsCharacter.GetCurrentHp());
        UI_Manager.Instance.HpBarFill_End(clsCharacter.GetMaxHp(), clsCharacter.GetCurrentHp(), true);
        element = clsCharacter.GetCurrnetElement().GetElement();    // 캐릭터 클래스 - 현재 원소 초기화

        ///DontDestroyOnLoad 객체의 파인드 방법 강구하기.
        ///객체를 찾지 못하는 오류 발생.
        ///씬 전환 간에 원활한 객체 생성 불가.
        ///방법1 : DontDestroy객체를 List에 저장하여 보관.

        sideUI_ObjPrintTransformObject = GameObject.Find("Drop_Item_ScrolView").GetComponent<Drop_Item_ScrolViewMng>();

        // 필요 레이어를 저장
        SpwanPoint = LayerMask.NameToLayer("SpwanPoint");
        monsterLayer = LayerMask.NameToLayer("Monster");
        interactionLayer = LayerMask.NameToLayer("InteractionObj");

        StartCoroutine("CheckCollidersPeriodically");

        //pathFinder.FindPathStart(objMng_instance.objectArray[1].transform.position);      // To Test

    }
    private void OnEnable()
    {
        // 게임매니저의 이벤트에 구독
        GameManager.OnPauseStateChanged += HandlePauseStateChanged;
        isMonsterSpwan = false;

    }

    private void OnDisable()
    {
        StopCoroutine("CheckCollidersPeriodically");
        //DataPrintScreenScrollManager.Instance.GetReturnToMainObject().SetActive(false);
        // 게임매니저의 이벤트 구독 해제
        GameManager.OnPauseStateChanged -= HandlePauseStateChanged;
    }

    private void Update()
    {
        if (!isControl)
            return;
        elementGetActive = clsCharacter.GetCurrnetElement().GetIsActive();
        characterHp = clsCharacter.GetCurrentHp();  // 테스트용
        clsState = clsCharacter.GetState();         // 테스트용
        clsCharacter = GameManager.Instance.GetUserClass().GetUserCharacter();       // 데이터 처리를 위한 캐릭터 클래스 최신화
        CharacterStateActor();                                  // 애니메이션 제어
        FloatAnimatorValueFunc();                               // 애니메이터 xz_flaot 변수 변경
        SatelliteParticleColorSwitch();                         // 원소 변경

    }
    private void FixedUpdate()
    {
    }

    #region 애니메이션 제어


    // 캐릭터 애니메이터 제어 함수
    public void CharacterStateActor()
    {
        switch (clsCharacter.GetState())
        {
            case CharacterClass.eCharactgerState.e_Idle:
                aniController.SetInteger("Controller", 0);
                break;
            case CharacterClass.eCharactgerState.e_WALK:
                aniController.SetInteger("Controller", -1);
                break;
            case CharacterClass.eCharactgerState.e_RUN:
                aniController.SetInteger("Controller", -1);
                break;
            case CharacterClass.eCharactgerState.e_JUMP:
                aniController.SetInteger("Controller", 1);
                break;
            case CharacterClass.eCharactgerState.e_AVOID:   // 블링크 값에 따라서, 애니메이션 제어
                if (blinkValue == CharacterControlMng.e_BlinkPos.Front)
                    aniController.SetInteger("Controller", 11);
                else if (blinkValue == CharacterControlMng.e_BlinkPos.Left)
                    aniController.SetInteger("Controller", 12);
                else if (blinkValue == CharacterControlMng.e_BlinkPos.Right)
                    aniController.SetInteger("Controller", 13);
                else if (blinkValue == CharacterControlMng.e_BlinkPos.Back)
                    aniController.SetInteger("Controller", 14);
                break;
            case CharacterClass.eCharactgerState.e_ATTACK:  // 공격 상태에 따라서, 애니메이션 제어
                isBattle = true;
                if (atkLevel == CharacterAttackMng.e_AttackLevel.AttackMode)
                    aniController.SetInteger("Controller", 100);
                else if (atkLevel == CharacterAttackMng.e_AttackLevel.Attack1)
                    aniController.SetInteger("Controller", 101);
                else if (atkLevel == CharacterAttackMng.e_AttackLevel.Attack2)
                    aniController.SetInteger("Controller", 102);
                else if (atkLevel == CharacterAttackMng.e_AttackLevel.Attack3)
                    aniController.SetInteger("Controller", 103);
                else if (atkLevel == CharacterAttackMng.e_AttackLevel.Brock)
                    aniController.SetInteger("Controller", 99);
                else if (atkLevel == CharacterAttackMng.e_AttackLevel.AtkSkill)
                    aniController.SetInteger("Controller", 110);
                break;
            case CharacterClass.eCharactgerState.e_HIT:
                break;
            case CharacterClass.eCharactgerState.e_DEAD: 
                break;
            default:
                aniController.SetInteger("Controller", -1);
                break;
        }
    }

    // 제어용 애니메이션 x,z좌표 수정
    private void FloatAnimatorValueFunc()
    {
        if(!isBattle)
        {
            runX= 0f;
            runZ= 0f;
        }
        else
        {
            zPos = 0f;
            xPos = 0f;
        }
        aniController.SetFloat("zPos", zPos);
        aniController.SetFloat("xPos", xPos);
        aniController.SetFloat("RunX", runX);
        aniController.SetFloat("RunZ", runZ);
    }
    #endregion

    #region 원소 제어

    // 원소 스위칭
    public void ElementSwitch()
    {
        if (isClickedCoolCheck)
            return;

        SatelliteObj.SetActive(true);
        int index = (int)element;
        index++;    // 원소 값 증가
        index %= (int)Element.e_Element.Max;    // 범위 모듈러 조정

        if ((Element.e_Element)index == Element.e_Element.None) //원소가 범위를 벗어날 경우 재배치
            index = (int)Element.e_Element.Fire;


        element = (Element.e_Element)index;

        Element characElement = new Element(element, false, false);
        clsCharacter.SetCurrentElement(characElement);

        // ElementSwitchButton 오브젝트를 이름으로 정확히 찾아서 ButtonClass 컴포넌트를 가져옴
        GameObject elementSwitchButton = GameObject.Find("ElementSwitchButton");
        if (elementSwitchButton != null)
        {
            var btncls = elementSwitchButton.GetComponent<ButtonClass>();
            
            StartCoroutine(ButtonClickedCoolTime(2f, btncls));
        }
    }
    // 버튼 쿨타임 함수
    IEnumerator ButtonClickedCoolTime(float time, ButtonClass ClickedBtnCls)
    {
        isClickedCoolCheck = true; // 클릭 쿨타임 중임을 나타내는 플래그를 true로 설정

        float elapsedTime = 0f; // 경과 시간 초기화
        float fillAmountStart = 0f;
        float fillAmountEnd = 1f;
        float duration = time; // 쿨타임 지속 시간 설정

        while (elapsedTime < duration)
        {
            // 시간에 따른 fillAmount를 계산하여 버튼의 내부 이미지에 설정
            float t = elapsedTime / duration;
            float currentFillAmount = Mathf.Lerp(fillAmountStart, fillAmountEnd, t);
            ClickedBtnCls.SetInsideImageFillAmount(currentFillAmount);

            elapsedTime += Time.deltaTime; // 경과 시간 업데이트
            yield return null; // 한 프레임을 기다림
        }

        // 쿨타임이 종료된 후 fillAmount를 최종 값으로 설정
        ClickedBtnCls.SetInsideImageFillAmount(fillAmountEnd);

        SatelliteObj.SetActive(false);
        isClickedCoolCheck = false; // 클릭 쿨타임이 끝났음을 나타내는 플래그를 false로 설정
    }

    // 원소 상태 표기 용 파티클 애니메이션 색상 전환
    void SatelliteParticleColorSwitch()
    {

        ParticleSystem eff = SatelliteObj.transform.GetChild(0).GetComponent<ParticleSystem>();
        var mainModule = eff.main; // eff 파티클 시스템의 메인 모듈에 접근

        switch (element)
        {
            case Element.e_Element.Fire:
                mainModule.startColor = Color.red; // 붉은색
                break;
            case Element.e_Element.Water:
                mainModule.startColor = Color.blue; // 파란색
                break;
            case Element.e_Element.Plant:
                mainModule.startColor = Color.green; // 녹색
                break;
            case Element.e_Element.Lightning:
                mainModule.startColor = new Color(0.5f, 0f, 0.5f); // 자주색 (RGB: 128, 0, 128)
                break;
            case Element.e_Element.Wind:
                mainModule.startColor = Color.cyan; // 하늘색
                break;
        }
    }

    #endregion


    #region 캐릭터 월드 Range 체크


    private IEnumerator CheckCollidersPeriodically()
    {
        while (true)
        {
            if (!isBattle)
            {
                World_InCharacterCheck_ForMonster();
                World_InteractionObjCheck();
            }
            If_DropUI_Null_ScrollOff();
            yield return new WaitForSeconds(0.2f);
        }
    }


    void World_InCharacterCheck_ForMonster()
    {
        if(isMonsterSpwan==false)
        {
            isMonsterSpwan = true;
            if (targetsListIn10Range.Count <= 0)
            {
                // 탐색범위 10
                Collider[] colliders = Physics.OverlapSphere(transform.position, 10, 1 << SpwanPoint);
                Collider[] Mobcolliders = Physics.OverlapSphere(transform.position, 10, 1 << monsterLayer);

                if (Mobcolliders.Length > 0)
                {
                    isMonsterSpwan = false;
                    return;
                }

                if (colliders.Length > 0)
                {
                    foreach (Collider collider in colliders)
                    {
                        GameManager.Instance.MonsterSpawn(collider.transform);
                    }
                }
            }

            isMonsterSpwan = false;
        }
    }
        
    void World_InteractionObjCheck()
    {
        if (targetsListIn10Range.Count > 0)
        {
            Collider[] InteractionObjs = Physics.OverlapSphere(transform.position, 3, 1 << interactionLayer);
            
            // 상호작용 오브젝트 탐색
            if (InteractionObjs.Length > 0)
            {
                
                for (int i = 0; i < InteractionObjs.Length; i++)
                {
                    Collider collider = InteractionObjs[i];
                    bool isCreate = true;
                    var data = collider.GetComponent<InteractionObject>();

                    foreach (var pair in dic_dropAndInterObj)
                    {
                        InteractionObject existingData = pair.Key;

                        if (existingData.Equals(data))
                        {
                            isCreate = false;
                            break;
                        }
                    }
                    // 중복되는 객체X => 객체 생성
                    if (isCreate)
                    {
                        // 반복/상호작용 여부 제어 플래그 확인
                        if (objMng_instance.IsOpenChecker[data] == true)
                        {
                            isCreate = false;
                        }
                        if (isCreate)
                        {
                            // 객체를 생성하고 함수 연결, 이후 관리용 딕셔너리에 Add
                            data.ObjectSetInit(sideUI_ObjPrintTransformObject.GetScrollObject());
                            objMng_instance.FunctionConnecter(data);
                            dic_dropAndInterObj.Add(data, data.GetDropItem_UI());
                        }
                    }
                }
            }


            // 클릭 된 객체는, ui 리턴 및 관리 딕셔너리에서 제외
            Dictionary<InteractionObject, DropItem_UI> copyList = new Dictionary<InteractionObject, DropItem_UI>(dic_dropAndInterObj);
            foreach (var tmp in copyList)
            {
                InteractionObject data = tmp.Key;
                if (objMng_instance.IsOpenChecker[data] == true)
                {
                    dic_dropAndInterObj.Remove(data);
                    GameManager.Instance.DropItemUI_Pool.ReturnToPool(tmp.Value);
                }
            }
        }
        else
        {   // 주위에 상호작용 오브젝트가 없을 경우에는, 오브젝트풀을 Return
            Dictionary<InteractionObject, DropItem_UI> copyList = new Dictionary<InteractionObject, DropItem_UI>(dic_dropAndInterObj);
            foreach (var pair in copyList)
            {
                InteractionObject key = pair.Key;

                dic_dropAndInterObj.Remove(key);
                GameManager.Instance.DropItemUI_Pool.ReturnToPool(pair.Value);
            }
        }
    }

    public void InteractionObjReturnCall(InteractionObject obj)
    {
        dic_dropAndInterObj.Remove(obj);
    }

    private void If_DropUI_Null_ScrollOff()
    {
        // 현재 사이드 스크린뷰에 띄울 객체가 없을 경우에는 객체 False
        var data1 = GameManager.Instance.DropItemUI_Pool.GetPoolList();
        var data2 = GameManager.Instance.InterectionObjUI_Pool.GetPoolList();

        bool shouldSetActive = data1.Any(item => item.gameObject.activeSelf) || data2.Any(item => item.gameObject.activeSelf);

        if (!shouldSetActive)
        {
            // sideUI_ObjPrintTransform의 부모 객체를 찾아서 비활성화
            sideUI_ObjPrintTransformObject.GetMainObject().gameObject.SetActive(false);

        }
        else
        {
            // sideUI_ObjPrintTransform의 부모 객체를 찾아서 활성화
            sideUI_ObjPrintTransformObject.GetMainObject().gameObject.SetActive(true);
        }
    }

    //레거시   
    //void World_OutMonsterCheck()  
    //{ 
    //    // 몬스터 레이어를 가진 객체를 배열에 저장 
    //    int monsterLayer = LayerMask.NameToLayer("Monster");  
    //    // 탐색범위 10
    //    Collider[] colliders = Physics.OverlapSphere(transform.position, 20, 1 << monsterLayer);
    //    if (colliders.Length > 0)
    //    {
    //        GameManager.Instance.MonsterReturnToPool(colliders);
    //    }
    //}

    #endregion


    // 씬 전환 간 캐릭터 동작함수 세팅 함수
    public void CharacterManagerConrollerBtnSet()
    {
        var parents = GameObject.FindGameObjectWithTag("Controller").transform;
        if(parents != null)
        {
            ButtonClass switchElementBtn = parents.GetChild(5).GetComponent<ButtonClass>();
            var btnObj = switchElementBtn.GetButton();
            btnObj.onClick.RemoveAllListeners();
            ButtonClass_Reset(switchElementBtn);
            btnObj.onClick.AddListener(() => ElementSwitch());
        }
    }



    #region 게터세터

    public bool GetIsBattle() { return isBattle; }
    public Element.e_Element GetElement(){return element;}
    public CharacterClass GetCharacterClass(){return clsCharacter;}
    public CharacterAttackMng.e_AttackLevel GetCharacterAtkLevel() { return atkLevel; }
    public void SetCharacterClass(CharacterClass cls){clsCharacter = cls;}
    public PathFinder _PathFinder
    {
        get { return pathFinder; }
    }
    public CharacterControlMng ControlMng
    {
        get { return controlMng; }
    }
    public CharacterAttackMng AttackMng
    {
        get { return attackMng; }
    }
    public bool IsControl
    {
        get { return isControl; }
        set { isControl = value; }
    }

    public void SetIsBattle(bool b) { isBattle = b; }
    public void AnimatorFloatValueSetter(float zPos, float xPos)
    {
        this.xPos = xPos;
        this.zPos = zPos;
        runX = xPos;
        runZ = zPos;
    }
    #endregion

    #region 옵저버 패턴

    public void AtkLevelNotify(CharacterAttackMng.e_AttackLevel level)
    {   //어택 매니저의 공격 프로세스 레벨을 받음.
        atkLevel = level;
    }

    public void BlinkValueNotify(CharacterControlMng.e_BlinkPos value)
    {   // 컨트롤 매니저의 블링크 값을 받음.
        blinkValue = value;
    }


    // 적 탐지 옵저버 패턴
    public void GetEnemyFindNotify(List<Transform> findList)
    {
        targetsListIn10Range = findList;
    }

    public void AttackSkillStartNotify(){}

    public void AttackSkillEndNotify(){}

    public void CheckPoint_PlayerPassNotify(int num){}

    public void WorldMapOpenNotify(){}

    public void WorldMapCloseNotify(){}

    public void ConvertToTargetStateNotify(List<Vector3> listTarget){}



    #endregion
}