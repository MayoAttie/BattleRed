using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;


public class CharacterManager : Singleton<CharacterManager>, Observer
{
    private CharacterClass clsCharacter;
    private Animator aniController;
    private bool isBattle;
    private CharacterAttackMng.e_AttackLevel atkLevel;
    private CharacterControlMng.e_BlinkPos blinkValue;

    private float xPos;
    private float zPos;
    private float runX;
    private float runZ;

    public CharacterClass.eCharactgerState clsState;

    private void Awake()
    {
        isBattle = false;
        gameObject.GetComponent<CharacterAttackMng>().Attach(this);
        gameObject.GetComponent<CharacterControlMng>().Attach(this);
        aniController = gameObject.transform.GetChild(0).GetComponent<Animator>();
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

    }
    private void LateUpdate()
    {
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
        this.zPos= zPos;
        runX = xPos;
        runZ = zPos;

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

    public bool getIsBattle()
    {
        return isBattle;
    }

    public void SetIsBattle(bool b) { isBattle = b; }
    public bool GetIsBattle() { return isBattle; }

    

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