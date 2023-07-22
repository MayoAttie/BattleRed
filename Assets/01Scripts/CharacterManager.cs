﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;


public class CharacterManager : Singleton<CharacterManager>, Observer
{
    private CharacterClass clsCharacter;
    private Animator aniController;
    private bool isBattle;
    private CharacterAttackMng.e_AttackLevel atkLevel;

    private void Awake()
    {
        isBattle = false;
        gameObject.GetComponent<CharacterAttackMng>().Attach(this);
        aniController = gameObject.transform.GetChild(0).GetComponent<Animator>();
    }
    private void Start()
    {
        clsCharacter = GameManager.Instance.characterCls;
    }

    private void Update()
    {
        clsCharacter = GameManager.Instance.characterCls;
        Debug.Log(nameof(clsCharacter.getState)+":" + clsCharacter.getState()) ;
        CharacterStateActor();
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
                aniController.SetInteger("Controller", 104);
                break;
            case CharacterClass.eCharactgerState.e_JUMP:
                aniController.SetInteger("Controller", 1);
                break;
            case CharacterClass.eCharactgerState.e_AVOID:
                break;
            case CharacterClass.eCharactgerState.e_ATTACK:
                isBattle = true;
                if (atkLevel == CharacterAttackMng.e_AttackLevel.AttackMode)
                    aniController.SetInteger("Controller", 100);
                else if (atkLevel == CharacterAttackMng.e_AttackLevel.Attack1)
                    aniController.SetInteger("Controller", 101);
                else if (atkLevel == CharacterAttackMng.e_AttackLevel.Attack2)
                    aniController.SetInteger("Controller", 102);
                else if (atkLevel == CharacterAttackMng.e_AttackLevel.Attack3)
                    aniController.SetInteger("Controller", 103);
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
        aniController.SetFloat("zPos", zPos);
        aniController.SetFloat("xPos", xPos);
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

    public void AtkLevelNotify(CharacterAttackMng.e_AttackLevel level)
    {   //어택 매니저의 공격 프로세스 레벨을 받음.
        atkLevel = level;
    }
    #endregion
}