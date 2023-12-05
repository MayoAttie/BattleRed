using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UIElements;

public class Monster : Objects
{
    #region 구조체
    public enum e_MonsterType
    {
        None = 0,
        Precedence,
        Counter,
        Elite,
        Boss,
        Max
    }
    public enum e_MonsterState
    {
        None,
        Idle,
        Walk,
        Attack,
        LookAround,
        Hit,
        Sturn,
        Die,
        Max
    }
    #endregion

    private e_MonsterState monsterState;
    private e_MonsterType monsterType;
    private int nMonsterMaxHp;
    private int nMonsterCurrnetHp;
    private int nMonsterExp;
    private int nMonsterAtkPower;
    private int nMonsterDef;
    private float fSturnPoint;
    private float fCurrentSturnPoint;
    private float fMonsterSpeed;
    private float fMonsterRotationSpeed;
    private float fMonsterHeadPos;
    private bool isQuicken;
    private Element monsterHaveElement;
    private Element monsterHittedElement;

    public Monster(string sTag, string sName, int nGrade, bool isActive,e_MonsterState monsterState, e_MonsterType monsterType, int nMonsterMaxHp, int nMonsterCurrnetHp, int nMonsterExp, int nMonsterAtkPower, int nMonsterDef, float fMonsterSpeed, float fMonsterRotationSpeed, Element.e_Element element, float fMonsterHeadPos, float fSturnPoint)
        : base(sTag, sName, nGrade, isActive)
    {
        this.monsterType = monsterType;
        this.nMonsterMaxHp = nMonsterMaxHp;
        this.nMonsterCurrnetHp = nMonsterCurrnetHp;
        this.nMonsterExp = nMonsterExp;
        this.nMonsterAtkPower = nMonsterAtkPower;
        this.nMonsterDef = nMonsterDef;
        this.fMonsterSpeed = fMonsterSpeed;
        this.monsterState = monsterState;
        this.fMonsterRotationSpeed = fMonsterRotationSpeed;
        this.fMonsterHeadPos = fMonsterHeadPos;
        this.fSturnPoint = fSturnPoint;
        isQuicken = false;
        monsterHaveElement = new Element(element, true, false);
        monsterHittedElement= new Element(Element.e_Element.None, false, false);
        fCurrentSturnPoint = 0;

    }

    public Monster()
    {
    }

    // 깊은 복사를 위한 함수
    public Monster Clone()
    {
        Monster cloneMonster = new Monster(
            this.GetTag(), this.GetName(), this.GetGrade(), this.GetIsActive(),
            this.GetMonsterState(), this.GetMonsterType(),
            this.GetMonsterMaxHp(), this.GetMonsterCurrentHp(), this.GetMonsterExp(),
            this.GetMonsterAtkPower(), this.GetMonsterDef(), this.GetMonsterSpeed(),
            this.GetMonsterRotateSpeed(), this.GetMonsterHaveElement().GetElement(),
            this.GetMonsterHeadPos(), this.GetMonsterSturnPoint()
        );

        // 추가로 Element 클래스도 복사
        cloneMonster.SetMonsterHaveElement(this.GetMonsterHaveElement().Clone());
        cloneMonster.SetMonsterHittedElement(this.GetMonsterHittedElement().Clone());

        return cloneMonster;
    }

    #region 게터세터
    public void SetMonsterState(e_MonsterState monsterState) { this.monsterState = monsterState; }
    public void SetMonsterType(e_MonsterType monsterType) { this.monsterType = monsterType; }
    public void SetMonsterCurrentHP(int hp) { nMonsterCurrnetHp = hp; }
    public void SetMonsterMaxHp(int hp) { nMonsterMaxHp = hp;}
    public void SetMonsterExp(int exp) { nMonsterExp = exp; }
    public void SetMonsterAtkPower(int atkPower) { nMonsterAtkPower = atkPower; }
    public void SetMonsterDef(int def) { nMonsterDef = def; }
    public void SetMonsterSpeed(float speed) { fMonsterSpeed= speed; }
    public void SetMonsterRotateSpeed(float rotate) { fMonsterRotationSpeed = rotate; }
    public void SetMonsterHaveElement(Element element) { monsterHaveElement= element; }
    public void SetMonsterHittedElement(Element element) { monsterHittedElement= element; }
    public void SetIsQuicken(bool isQuicken) { this.isQuicken = isQuicken; }
    public void SetMonsterHeadPos(float fMonsterHeadPos) {this.fMonsterHeadPos = fMonsterHeadPos; }
    public void SetMonsterSturnPoint(float fSturnPoint) {  this.fSturnPoint= fSturnPoint; }
    public void SetCurrentSturnPoint(float fCurrentSturnPoint) { this.fCurrentSturnPoint= fCurrentSturnPoint; }


    public e_MonsterState GetMonsterState() { return monsterState; }
    public e_MonsterType GetMonsterType() { return monsterType; }
    public int GetMonsterCurrentHp() { return nMonsterCurrnetHp; }
    public int GetMonsterMaxHp() { return nMonsterMaxHp; }
    public int GetMonsterExp() { return nMonsterExp; }
    public int GetMonsterAtkPower() { return nMonsterAtkPower; }
    public int GetMonsterDef() { return nMonsterDef; }
    public float GetMonsterSpeed() { return fMonsterSpeed;}
    public float GetMonsterRotateSpeed() { return fMonsterRotationSpeed; }
    public Element GetMonsterHaveElement() { return monsterHaveElement; }
    public Element GetMonsterHittedElement() { return monsterHittedElement; }
    public bool GetIsQuicken() { return isQuicken; }
    public float GetMonsterHeadPos() { return fMonsterHeadPos; }
    public float GetMonsterSturnPoint() { return fSturnPoint; }
    public float GetCurrentSturnPoint() { return fCurrentSturnPoint; }

    #endregion


}
