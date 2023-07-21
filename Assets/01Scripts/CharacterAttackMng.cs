using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterClass;

public class CharacterAttackMng : MonoBehaviour
{
    public enum e_AttackLevel
    {
        None = 99,
        AttackMode,
        Attack1,
        Attack2,
        Attack3,
        Max
    }

    CharacterManager characMng;
    [SerializeField]int nAtkLevel;
    Animator animator; 

    private void Awake()
    {
        nAtkLevel = 99;
        animator = gameObject.transform.GetChild(0).GetComponent<Animator>();
    }

    void Start()
    {
        characMng = CharacterManager.Instance;
    }

    void Update()
    {
    }

    public void CharaceterAttackCheck()
    {
        if(nAtkLevel >= (int)e_AttackLevel.Max)
            nAtkLevel = (int)e_AttackLevel.None;
        nAtkLevel++;

        characMng.GetCharacterClass().setState(eCharactgerState.e_ATTACK);
        characMng.SetIsBattle(true);

        switch ((e_AttackLevel)nAtkLevel)
        {
            case e_AttackLevel.AttackMode:
                animator.SetInteger("Controller", nAtkLevel);
                break;
            case e_AttackLevel.Attack1:
                animator.SetInteger("Controller", nAtkLevel);
                break;
            case e_AttackLevel.Attack2:
                animator.SetInteger("Controller", nAtkLevel);
                break;
            case e_AttackLevel.Attack3:
                animator.SetInteger("Controller", nAtkLevel);
                break;
            default: break;
        }
    }


    

    

}
