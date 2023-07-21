using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterManager : MonoBehaviour
{
    protected CharacterClass clsCharacter;
    protected Animator aniController;


    private void Awake()
    {
        aniController = gameObject.GetComponent<Animator>();
    }
    private void Start()
    {
    }

    void Update()
    {
    }
    private void LateUpdate()
    {
        clsCharacter = GameManager.Instance.characterCls;
        CharacterStateActor();  // 캐릭터 애니메이터 제어 함수
            
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



}