﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CharacterClass;

public class CharacterAttackMng : Subject, Observer
{
    #region 변수
    CharacterManager characMng;
    [SerializeField]int nAtkLevel;
    CharacterAniEventFinder eventInputer;
    [SerializeField] GameObject SwordColider;
    public bool isBattle;                   // 전투중인지 체크
    public bool isAnimationIng;             // 공격 애니메이션 진행 확인
    public bool isClick;                    // 추가 공격버튼 클릭 확인
    public bool isBrock;                    // 방어 진행 확인
    private bool isCoroutineFlag;           // 코루틴 제어 플래그
    Coroutine AttackModeChecker;            // 대기-아이들 전환 코루틴

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
        SwordColider.SetActive(false);
        AttackModeChecker = null;
        isAnimationIng = false;
        isClick = false;
        nAtkLevel = 101;
        eventInputer = gameObject.GetComponent<CharacterAniEventFinder>();        
        isCoroutineFlag = false;
    }

    void Start()
    {
        eventInputer.Attach(this);
        characMng = CharacterManager.Instance;
        
    }

    void Update()
    {
        isBattle = characMng.getIsBattle();
        if (!isBattle)
            return;

        if (nAtkLevel >= (int)e_AttackLevel.AtkMax)
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
    }
    #endregion

    #region 스킬 공격

    public void AttackSkillStart()
    {
        Element.e_Element element = characMng.GetElement();
        
        
    }

    void FireTypeAtkSkill(Element.e_Element element)
    {

    }

    void WaterTypeAtkSkill(Element.e_Element element)
    {

    }
    void PlantTypeAtkSkill(Element.e_Element element)
    {

    }
    void LightningTypeAtkSkill(Element.e_Element element)
    {

    }
    void WindTypeAtkSkill(Element.e_Element element)
    {

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
            Debug.Log("characMng.SetIsBattle(false)");
            isCoroutineFlag = false;
            AttackModeChecker = null;
        }
    }


    public void ShildAct()
    {
        nAtkLevel = (int)e_AttackLevel.Brock;
        NotifyAtkLevel((e_AttackLevel)nAtkLevel);
        isBrock = true;
        FlagValueReset();
    }

    public void FlagValueReset()
    {
        if (isAnimationIng)
            isAnimationIng = false;
        if (isClick)
            isClick = false;
    }
    #endregion


    #region 옵저버패턴
    public void AtkLevelNotify(e_AttackLevel level) {}//사용안함

    public void BlinkValueNotify(CharacterControlMng.e_BlinkPos value){ }//사용안함

    public void GetBlinkEndNotify(){ }//사용안함

    public void GetBlinkStartNotify(){ }//사용안함

    public void GetBrockEndNotify()
    {
        Debug.Log(nameof(isBrock) + ":" + isBrock);
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

    public void GetEnemyFindNotify(List<Transform> findList)
    {
    }
    #endregion
}
