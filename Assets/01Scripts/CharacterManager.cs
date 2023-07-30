using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterManager : Singleton<CharacterManager>, Observer
{
    private CharacterClass clsCharacter;        // 캐릭터 데이터 클래스
    private Animator aniController;             // 캐릭터 제어 애니메이터
    private bool isBattle;                      // 전투 체크 변수
    private CharacterAttackMng.e_AttackLevel atkLevel;  // 공격 단계 변수
    private CharacterControlMng.e_BlinkPos blinkValue;  // 회피기 방향 변수
    [SerializeField]private Element.e_Element element;                  // 선택한 원소 변수

    private float xPos;                         // 캐릭터 애니메이션 플로트 변수
    private float zPos;                         // 캐릭터 애니메이션 플로트 변수
    private float runX;                         // 캐릭터 애니메이션 플로트 변수
    private float runZ;                         // 캐릭터 애니메이션 플로트 변수
    [SerializeField] GameObject SatelliteObj;   // 원소 체크용 위성 객체

    public CharacterClass.eCharactgerState clsState;
    bool isClickedCoolCheck;                    // 버튼 쿨타임 코루틴 객체

    private void Awake()
    {
        isBattle = false;
        gameObject.GetComponent<CharacterAttackMng>().Attach(this);
        gameObject.GetComponent<CharacterControlMng>().Attach(this);
        aniController = gameObject.GetComponent<Animator>();
        SatelliteObj.gameObject.SetActive(false);
    }
    private void Start()
    {
        clsCharacter = GameManager.Instance.characterCls;
    }

    private void Update()
    {
        clsState= clsCharacter.getState();
        clsCharacter = GameManager.Instance.characterCls;
        Debug.Log(nameof(clsCharacter.getState)+":" + clsCharacter.getState()) ;
        CharacterStateActor();
        FloatAnimatorValueFunc();
        SatelliteParticleColorSwitch();
    }


    // 캐릭터 애니메이터 제어 함수
    public void CharacterStateActor()
    {

        switch (clsCharacter.getState())
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

        // ElementSwitchButton 오브젝트를 이름으로 정확히 찾아서 ButtonClass 컴포넌트를 가져옴
        GameObject elementSwitchButton = GameObject.Find("ElementSwitchButton");
        if (elementSwitchButton != null)
        {
            ButtonClass btncls = elementSwitchButton.GetComponent<ButtonClass>();
            
            StartCoroutine(ButtonClickedCoolTime(2f, btncls));
        }
    }

    IEnumerator ButtonClickedCoolTime(float time, ButtonClass ClickedBtnCls)
    {
        isClickedCoolCheck = true;
        float elapsedTime = 0f;
        float fillAmountStart = 0f;
        float fillAmountEnd = 1f;
        float duration = time;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float currentFillAmount = Mathf.Lerp(fillAmountStart, fillAmountEnd, t);
            ClickedBtnCls.SetInsideImageFillAmount(currentFillAmount);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ClickedBtnCls.SetInsideImageFillAmount(fillAmountEnd);

        SatelliteObj.SetActive(false);
        isClickedCoolCheck = false;
    }

    void OffSatellite()
    {
        SatelliteObj.SetActive(false);
    }

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


    #region 게터세터
    public bool getIsBattle()
    {
        return isBattle;
    }

    public void SetIsBattle(bool b) { isBattle = b; }
    public bool GetIsBattle() { return isBattle; }

    public Element.e_Element GetElement()
    {
        return element;
    }

    public void SetCharacterClass(CharacterClass cls)
    {
        clsCharacter = cls;
    }
    public CharacterClass GetCharacterClass()
    {
        return clsCharacter;
    }

    public void AnimatorFloatValueSetter(float zPos, float xPos)
    {
        this.xPos = xPos;
        this.zPos = zPos;
        runX = xPos;
        runZ = zPos;

    }
    #endregion

    #region 옵저버 패턴
    public void AttackEventNotify(int num){}

    public void AttackEventStartNotify(){}
    public void GetBlinkEndNotify(){}

    public void AtkLevelNotify(CharacterAttackMng.e_AttackLevel level)
    {   //어택 매니저의 공격 프로세스 레벨을 받음.
        atkLevel = level;
    }

    public void BlinkValueNotify(CharacterControlMng.e_BlinkPos value)
    {   // 컨트롤 매니저의 블링크 값을 받음.
        blinkValue = value;
    }

    public void GetBlinkStartNotify(){}

    public void GetBrockEndNotify(){}

    #endregion
}