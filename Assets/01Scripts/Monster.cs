using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Objects
{
    public enum e_MonsterType
    {
        None = 0,
        Precedence,
        Counter,
        Elite,
        Boss,
        Max
    }
    private e_MonsterType monsterType;
    private int nMonsterMaxHp;
    private int nMonsterCurrnetHp;
    private int nMonsterExp;
    private int nMonsterAtkPower;
    private int nMonsterDef;
    private float fMonsterSpeed;

    public Monster(string sTag, string sName, int nGrade, bool isActive ,e_MonsterType monsterType, int nMonsterMaxHp, int nMonsterCurrnetHp, int nMonsterExp, int nMonsterAtkPower, int nMonsterDef, float fMonsterSpeed)
        :base(sTag, sName, nGrade, isActive)
    {
        this.monsterType = monsterType;
        this.nMonsterMaxHp = nMonsterMaxHp;
        this.nMonsterCurrnetHp = nMonsterCurrnetHp;
        this.nMonsterExp = nMonsterExp;
        this.nMonsterAtkPower = nMonsterAtkPower;
        this.nMonsterDef = nMonsterDef;
        this.fMonsterSpeed = fMonsterSpeed;
    }

    public Monster()
    {
    }

    #region 게터세터
    public void SetMonsterType(e_MonsterType monsterType) { this.monsterType = monsterType; }
    public void SetMonsterCurrentHP(int hp) { nMonsterCurrnetHp = hp; }
    public void SetMonsterMaxHp(int hp) { nMonsterMaxHp = hp;}
    public void SetMonsterExp(int exp) { nMonsterExp = exp; }
    public void SetMonsterAtkPower(int atkPower) { nMonsterAtkPower = atkPower; }
    public void SetMonsterDef(int def) { nMonsterDef = def; }
    public void SetMonsterSpeed(float speed) { fMonsterSpeed= speed; }

    public e_MonsterType GetMonsetrType() { return monsterType; }
    public int GetMonsterCurrentHp() { return nMonsterCurrnetHp; }
    public int GetMonsterMaxHp() { return nMonsterMaxHp; }
    public int GetMonsterExp() { return nMonsterExp; }
    public int GetMonsterAtkPower() { return nMonsterAtkPower; }
    public int GetMonsterDef() { return nMonsterDef; }
    public float GetMonsterSpeed() { return fMonsterSpeed;}
    #endregion


}
