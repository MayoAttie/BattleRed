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
    int nMaxLevel;
    int nElementNum;
    int nStamina;
    float fCriticalDamage;
    float fCriticalPercentage;
    float fSpeed;
    eCharactgerState eCharacState;
    Element eCharacElement;         // 캐릭터가 사용하는 현재 원소
    Element eEncountElement;        // 캐릭터가 몬스터에게 피격당한 현재 원소 상태
    Element[] ChildElement;         // 캐릭터가 원소 효과로 생성할 원소

    

    public CharacterClass(int nCurrentHp, int nMaxHp, int nCurrentExp, int nMaxExp, int nAttack, int nDefense, int nLevel, int nMaxLevel, float fSpeed, eCharactgerState eCharacState, int nElementNum, float fCriticalDamage, float fCriticalPercentage, string sTag, string sName, int nGrade, bool isActive, int nStamina) : base(sTag, sName, nGrade, isActive)
    {
        this.nCurrentHp = nCurrentHp;
        this.nMaxHp = nMaxHp;
        this.nCurrentExp = nCurrentExp;
        this.nMaxExp = nMaxExp;
        this.nAttack = nAttack;
        this.nDefense = nDefense;
        this.nLevel = nLevel;
        this.nMaxLevel = nMaxLevel;
        this.fSpeed = fSpeed;
        this.eCharacState = eCharacState;
        this.eCharacElement = new Element(Element.e_Element.Fire, false, false);
        this.nElementNum = nElementNum;
        this.eEncountElement = new Element(Element.e_Element.None, false, false);
        this.fCriticalDamage = fCriticalDamage;
        this.fCriticalPercentage = fCriticalPercentage;

        ChildElement = new Element[5];
        for (int i = 0; i < ChildElement.Length; i++)
        {
            ChildElement[i] = new Element(Element.e_Element.None, false, true);
        }

        this.nStamina = nStamina;
    }

    public CharacterClass()
    {
    }

    #region 세터게터

    public eCharactgerState GetState(){return eCharacState;}
    public Element GetEncountElement(){return eEncountElement;}
    public Element GetCurrnetElement(){return eCharacElement;}
    public Element GetChildElement(int index){return ChildElement[index];}
    public float GetSpeed(){return fSpeed;}
    public int GetAttack(){return nAttack;}
    public int GetDeffense(){return nDefense;}
    public int GetElementNum(){return nElementNum;}
    public int GetCurrentHp(){return nCurrentHp;}
    public int GetMaxHp(){return nMaxHp;}
    public float GetCriticalDamage(){return fCriticalDamage;}
    public float GetCriticalPercentage(){return fCriticalPercentage;}
    public int GetLeveL() { return nLevel; }
    public int GetStamina() { return nStamina;}
    public int GetMaxLevel() { return nMaxLevel;}

    public void SetState(eCharactgerState state){eCharacState = state;}
    public void SetEncountElement(Element encountElement){eEncountElement = encountElement;}
    public void SetCurrentElement(Element element){eCharacElement = element;}
    public void SetChildElement(int index, Element element){ChildElement[index] = element;}
    public void SetAttack(int attack){nAttack = attack;}
    public void SetElementNum(int elementNum){nElementNum = elementNum;}
    public void SetCurrentHp(int hp){nCurrentHp = hp;}
    public void SetMaxHp(int hp){nMaxHp = hp;}

    #endregion


}
