using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;


public class CharacterManager : Singleton<CharacterManager>
{
    protected CharacterClass clsCharacter;
    protected Animator aniController;
    protected bool isBattle;

    protected virtual void Awake()
    {
        isBattle = false;
        aniController = gameObject.transform.GetChild(0).GetComponent<Animator>();
    }
    virtual protected void Start()
    {
    }

    protected virtual void Update()
    {
        clsCharacter = GameManager.Instance.characterCls;
        Debug.Log(nameof(clsCharacter.getState)+":" + clsCharacter.getState()) ;
        CharacterStateActor();
    }
    protected virtual void LateUpdate()
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
                break;
            case CharacterClass.eCharactgerState.e_JUMP:
                aniController.SetInteger("Controller", 1);
                break;
            case CharacterClass.eCharactgerState.e_AVOID:
                break;
            case CharacterClass.eCharactgerState.e_ATTACK:
                isBattle = true;
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
}