using System.Collections;
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
    bool isBattle;
    bool isAnimationIng;
    bool isClick;

    #endregion

    #region 구조체
    public enum e_AttackLevel
    {
        None = 99,
        AttackMode,
        Attack1,
        Attack2,
        Attack3,
        Max
    }

    #endregion


    private void Awake()
    {
        isAnimationIng = false;
        isClick = false;
        nAtkLevel = 100;
        eventInputer = gameObject.transform.GetChild(0).GetComponent<CharacterAniEventFinder>();
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

        if (nAtkLevel > (int)e_AttackLevel.Max)
            nAtkLevel = (int)e_AttackLevel.AttackMode;

        
        
    }

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
        characMng.GetCharacterClass().setState(eCharactgerState.e_ATTACK);
        Invoke("OffBattleMode", 8f);
    }



    void ReturnIdle(int num)
    {
        //if (characMng.GetCharacterClass().getState() == CharacterClass.eCharactgerState.e_AVOID)
        //    return;

        if (isClick)  // 버튼 클릭 여부 확인 후, 공격 루프
        {
            nAtkLevel = num + 1;
            Debug.Log(nameof(nAtkLevel) + ":" + nAtkLevel);
            if(nAtkLevel == (int)e_AttackLevel.Max)
                nAtkLevel = (int)e_AttackLevel.Attack1;
            // 애니메이션 제어
            NotifyAtkLevel((e_AttackLevel)nAtkLevel);
            return;
        }
        isAnimationIng = false;                         // 애니메이션 동작 종료
        nAtkLevel = (int)e_AttackLevel.AttackMode;      // 공격 대기 자세로 변경
        NotifyAtkLevel((e_AttackLevel)nAtkLevel);       // 캐릭터 매니저에 알림
        Invoke("OffBattleMode", 8f);
    }

    public void AttackEventNotify(int num)  //동작 애니메이션 종료 때, 호출
    {
        ReturnIdle(num);
    }

    public void AttackEventStartNotify()    // 동작 애니메이션 시작 때, 호출
    {
        isClick = false;
        isAnimationIng = true;  // 애니메이션 동작 시작
    }

    public void OffBattleMode()        // 대기상태 시간 체크 후 아이들로 초기화
    {
        if (isClick)
            return;
        if (isAnimationIng)
            return;
        if (characMng.GetCharacterClass().getState() == eCharactgerState.e_RUN)
            return;

        characMng.GetCharacterClass().setState(eCharactgerState.e_Idle);
        characMng.SetIsBattle(false);
        Debug.Log("characMng.SetIsBattle(false)");
    }



    public void AtkLevelNotify(e_AttackLevel level) {}//사용안함

    public void BlinkValueNotify(CharacterControlMng.e_BlinkPos value){}

    public void GetBlinkEndNotify(){}

    public void GetBlinkStartNotify(){}
}
