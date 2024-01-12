using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CharacterClass;
using static HandlePauseTool;
using static UI_UseToolClass;
public class CharacterAttackMng : Subject, Observer
{
    #region 변수
    CharacterManager characMng;
    [SerializeField]int nAtkLevel;
    GameObject SwordColider;
    GameObject darkCurtain;             // 스킬 시전 시 배경
    bool isBattle;                   // 전투중인지 체크
    bool isAnimationIng;             // 공격 애니메이션 진행 확인
    bool isClick;                    // 추가 공격버튼 클릭 확인
    bool isBrock;                    // 방어 진행 확인
    private bool isCoroutineFlag;           // 코루틴 제어 플래그
    Coroutine AttackModeChecker;            // 대기-아이들 전환 코루틴
    TimelineManager timeline_manager;

    float fSkillCooltie;
    float fCurSkillTime;
    bool isClickedCoolCheck;

    #endregion

    #region 구조체
    public enum e_AttackLevel
    {
        None = 99,
        Brock,              //99
        AttackMode,         //100
        Attack1,            //101
        Attack2,            //102
        Attack3,            //103
        AtkMax,             //104
        AtkSkill = 110,     //110
        Max                 //111
    }

    #endregion


    private void Awake()
    {
        timeline_manager = gameObject.GetComponent<TimelineManager>();
        SwordColider = transform.GetChild(5).GetChild(0).gameObject;
        darkCurtain = transform.GetChild(7).gameObject;
        darkCurtain.SetActive(false);
        SwordColider.SetActive(false);
        AttackModeChecker = null;
        isAnimationIng = false;
        isClick = false;
        nAtkLevel = 101;
        isCoroutineFlag = false;
    }
    private void OnEnable()
    {
        fCurSkillTime = 0;
        isClickedCoolCheck = false;
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
        characMng = CharacterManager.Instance;
    }

    void Update()
    {
        if (!characMng.IsControl)
            return;

        isBattle = characMng.GetIsBattle();
        if (!isBattle)
            return;

        if (nAtkLevel >= (int)e_AttackLevel.Max)
            nAtkLevel = (int)e_AttackLevel.AttackMode;

        if (AttackModeChecker == null && nAtkLevel == (int)e_AttackLevel.AttackMode)
            AttackModeChecker = StartCoroutine(CheckAttackMode());


    }
    #region 평타 공격

    // 버튼으로 호출되는 공격 함수
    public void CharaceterAttackCheck()
    {

        isClick = true;
        characMng.SetIsBattle(true);
        if (isAnimationIng) // 애니메이션 동작중일 경우 리턴
            return;
        nAtkLevel++;
        // 애니메이션 제어
        NotifyAtkLevel((e_AttackLevel)nAtkLevel);   // 바뀐 공격 상태를 캐릭터 매니저에 알림
        characMng.GetCharacterClass().SetState(eCharactgerState.e_ATTACK);
        //Invoke("OffBattleMode", 8f);
    }

    void ReturnIdle(int num)
    {

        if (isClick)  // 버튼 클릭 여부 확인 후, 공격 루프
        {
            nAtkLevel = num + 1;
            Debug.Log(nameof(nAtkLevel) + ":" + nAtkLevel);
            if(nAtkLevel == (int)e_AttackLevel.AtkMax)
                nAtkLevel = (int)e_AttackLevel.Attack1;
            // 애니메이션 제어
            NotifyAtkLevel((e_AttackLevel)nAtkLevel);
            return;
        }
        isAnimationIng = false;                         // 애니메이션 동작 종료
        nAtkLevel = (int)e_AttackLevel.AttackMode;      // 공격 대기 자세로 변경
        NotifyAtkLevel((e_AttackLevel)nAtkLevel);       // 캐릭터 매니저에 알림
        //Invoke("OffBattleMode", 8f);
    }

    public void AttackEventNotify(int num)  //동작 애니메이션 종료 때, 호출
    {
        SwordColider.SetActive(false);
        ReturnIdle(num);
    }

    public void AttackEventStartNotify()    // 동작 애니메이션 시작 때, 호출
    {
        SwordColider.SetActive(true);
        isClick = false;
        isAnimationIng = true;  // 애니메이션 동작 시작

        //타임라인에 재생 그룹을 호출
        int index = nAtkLevel % 100;
        timeline_manager.PlayTimeline_getIndex(index-1);
    }
    #endregion

    #region 스킬 공격

    public void AttackSkillStart()
    {
        if (darkCurtain.activeSelf == true)
            return;

        if (isClickedCoolCheck == true)
            return;

        darkCurtain.SetActive(true);
        characMng.SetIsBattle(true);
        Element.e_Element element = characMng.GetElement();
        nAtkLevel = (int)e_AttackLevel.AtkSkill;

        // 공격 레벨 변경을 알림
        NotifyAtkLevel((e_AttackLevel)nAtkLevel);
        timeline_manager.PlayTimeline_getIndex(10);
        // 캐릭터의 상태를 공격 상태로 설정
        characMng.GetCharacterClass().SetState(eCharactgerState.e_ATTACK);

        float time = CharacterManager.Instance.GetCharacterClass().GetSkillCoolTime();

        GameObject skillButton = GameObject.Find("AtkSkillButton");
        if(skillButton != null)
        {
            var btnCls = skillButton.GetComponent<ButtonClass>();
            StartCoroutine(ButtonClickedCoolTime(time, btnCls));
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

        isClickedCoolCheck = false; // 클릭 쿨타임이 끝났음을 나타내는 플래그를 false로 설정
    }

    public void AttackSkilAniStart()
    {
        // 어두운 커튼이 활성화된 경우
        if (darkCurtain.activeSelf == true)
            Invoke("CurtainOff", 1.1f); // 1.1초 후에 CurtainOff 함수 호출
        SwordColider.SetActive(true);

        // 공격 스킬 시작을 알림 -> 카메라/조명 제어 함수
        NotifyAttackSkillStart();
    }
    public void CurtainOff()
    {
        if (darkCurtain.activeSelf == false)
            return;
        darkCurtain.SetActive(false);

        // 공격 스킬 종료를 알림 -> 카메라/조명 제어 함수
        NotifyAttackSkillEnd();
    }
    public void AttackSkillAniEnd()
    {
        // 스킬 종료에 따른, 원소 활성상태 Off
        if (characMng.GetCharacterClass().GetCurrnetElement().GetIsActive())
            characMng.GetCharacterClass().GetCurrnetElement().SetIsActive(false);

        SwordColider.SetActive(false);

        // 스킬로 인한 특정 행동 강제 시, 발생할 문제를 해결하기 위한 각 제어 플래그 초기화
        FlagValueReset();   
        nAtkLevel = (int)e_AttackLevel.AttackMode;
        
        // 공격 대기 모드로 전환
        NotifyAtkLevel((e_AttackLevel)nAtkLevel);
        characMng.GetCharacterClass().SetState(eCharactgerState.e_ATTACK);
    }

    #endregion

    #region 기타 함수

    public void OffBattleMode()        // 대기상태 시간 체크 후 아이들로 초기화
    {
        if (isClick)
            return;
        if (isAnimationIng)
            return;
        if (characMng.GetCharacterClass().GetState() == eCharactgerState.e_RUN)
            return;
        if (isBrock)
            return;

        characMng.GetCharacterClass().SetState(eCharactgerState.e_Idle);
        characMng.SetIsBattle(false);
        Debug.Log("characMng.SetIsBattle(false)");
    }

    IEnumerator CheckAttackMode()   // 대기상태 시간 체크 후 아이들로 초기화
    {
        if (!isCoroutineFlag)
        {
            isCoroutineFlag = true;
            yield return new WaitForSeconds(8f);

            if (isClick)
            {
                isCoroutineFlag = false;
                AttackModeChecker = null;
                yield break;
            }
            if (isAnimationIng)
            {
                isCoroutineFlag = false;
                AttackModeChecker = null;
                yield break;
            }
            if (characMng.GetCharacterClass().GetState() == eCharactgerState.e_RUN)
            {
                isCoroutineFlag = false;
                AttackModeChecker = null;
                yield break;
            }
            if (isBrock)
            {
                isCoroutineFlag = false;
                AttackModeChecker = null;
                yield break;
            }

            characMng.GetCharacterClass().SetState(eCharactgerState.e_Idle);
            characMng.SetIsBattle(false);
            isCoroutineFlag = false;
            AttackModeChecker = null;
        }
    }

    // 막기 동작 시작 함수
    public void ShildAct()
    {
        nAtkLevel = (int)e_AttackLevel.Brock;
        NotifyAtkLevel((e_AttackLevel)nAtkLevel);
        isBrock = true;
        FlagValueReset();
    }

    // 특정 동작으로 애니메이션 강제 해제 시, 플래그 변수 초기화
    public void FlagValueReset()
    {
        if (isAnimationIng)
            isAnimationIng = false;
        if (isClick)
            isClick = false;
    }
    #endregion

    public void CharacterAttackMng_ControllerSet()
    {
        var parents = GameObject.FindGameObjectWithTag("Controller").transform;
        if (parents != null)
        {
            ButtonClass attackBtn = parents.GetChild(2).GetComponent<ButtonClass>();
            var attackBtnObj = attackBtn.GetButton();
            attackBtnObj.onClick.RemoveAllListeners();
            ButtonClass_Reset(attackBtn);
            attackBtnObj.onClick.AddListener(() => CharaceterAttackCheck());

            ButtonClass skillBtn = parents.GetChild(6).GetComponent<ButtonClass>();
            var skillBtnObj = skillBtn.GetButton();
            skillBtnObj.onClick.RemoveAllListeners();
            ButtonClass_Reset(skillBtn);
            skillBtnObj.onClick.AddListener(() => AttackSkillStart());
        }
    }


    #region 옵저버패턴
    public void AtkLevelNotify(e_AttackLevel level) {}//사용안함

    public void BlinkValueNotify(CharacterControlMng.e_BlinkPos value){ }//사용안함


    // 막기, 엔드 프레임 호출 함수
    public void GetBrockEndNotify()
    {
        //Debug.Log(nameof(isBrock) + ":" + isBrock);
        isBrock = false;

        if (isBattle)
        {
            characMng.GetCharacterClass().SetState(eCharactgerState.e_Idle);
            nAtkLevel = (int)e_AttackLevel.AttackMode;
            NotifyAtkLevel((e_AttackLevel)nAtkLevel);
        }
        else
            OffBattleMode();
        
        Debug.Log(nameof(isBrock) + ":" + isBrock);

    }

    public void GetEnemyFindNotify(List<Transform> findList){}

    public void AttackSkillStartNotify(){}

    public void AttackSkillEndNotify(){}

    public void CheckPoint_PlayerPassNotify(int num){}


    #endregion
}
