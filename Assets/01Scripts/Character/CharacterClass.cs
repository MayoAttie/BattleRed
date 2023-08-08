using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterClass : Objects
{
    public enum eCharactgerState
    {
        e_NONE =0,
        e_Idle,
        e_WALK,
        e_RUN,
        e_JUMP,
        e_AVOID,
        e_ATTACK,
        e_HIT,
        e_DEAD,
        e_MAX
    }

    int nCurrentHp;
    int nMaxHp;
    int nCurrentExp;
    int nMaxExp;
    int nAttack;
    int nDefense;
    int nLevel;
    float fSpeed;
    eCharactgerState eCharacState;
    Element.e_Element eCharacElement;

    public CharacterClass(int nCurrentHp, int nMaxHp, int nCurrentExp, int nMaxExp, int nAttack, int nDefense, int nLevel, float fSpeed, eCharactgerState eCharacState, Element.e_Element eCharacElement, string sTag, string sName, int nGrade, bool isActive) : base(sTag, sName,nGrade,isActive)
    {
        this.nCurrentHp = nCurrentHp;
        this.nMaxHp = nMaxHp;
        this.nCurrentExp = nCurrentExp;
        this.nMaxExp = nMaxExp;
        this.nAttack = nAttack;
        this.nDefense = nDefense;
        this.nLevel = nLevel;
        this.fSpeed = fSpeed;
        this.eCharacState = eCharacState;
        this.eCharacElement = eCharacElement;
    }

    public CharacterClass()
    {
    }

    public float getSpeed()
    {
        return fSpeed;
    }
    public eCharactgerState getState()
    {
        return eCharacState;
    }
    public void setState(eCharactgerState state)
    {
        eCharacState = state;
    }

}
