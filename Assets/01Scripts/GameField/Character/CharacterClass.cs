using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
    float Increase_Damage;
    float Increased_NormalAttackDamage;
    float Increased_SkillAttackDamage;
    float fSkill_coolTime;
    private Dictionary<int, int> itemAddHp = new Dictionary<int, int>();
    private Dictionary<int, int> itemAddAttack = new Dictionary<int, int>();
    private Dictionary<int, int> itemAddDefense = new Dictionary<int, int>();
    private Dictionary<Tuple<string, int>, float> equipSetApplied = new Dictionary<Tuple<string, int>, float>();
    int nLevel;
    int nMaxLevel;
    int nElementNum;
    int nStamina;
    float fElementCharge;
    float fCriticalDamage;
    float fCriticalPercentage;
    float fSpeed;
    eCharactgerState eCharacState;
    Element eCharacElement;         // 캐릭터가 사용하는 현재 원소
    Element eEncountElement;        // 캐릭터가 몬스터에게 피격당한 현재 원소 상태
    Element[] ChildElement;         // 캐릭터가 원소 효과로 생성할 원소

    

    public CharacterClass(int nCurrentHp, int nMaxHp, int nCurrentExp, int nMaxExp, int nAttack, int nDefense, int nLevel, int nMaxLevel, float fSpeed, eCharactgerState eCharacState, int nElementNum, float fCriticalDamage, float fCriticalPercentage,float skill_coolTime, string sTag, string sName, int nGrade, bool isActive, int nStamina, float elementcharge, float normalAtkDamage, float skillAtkDamage, float Increase_Damage) : base(sTag, sName, nGrade, isActive)
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
        this.fSkill_coolTime = skill_coolTime;
        this.eCharacElement = new Element(Element.e_Element.Fire, false, false);
        this.nElementNum = nElementNum;
        this.eEncountElement = new Element(Element.e_Element.None, false, false);
        this.fCriticalDamage = fCriticalDamage;
        this.fCriticalPercentage = fCriticalPercentage;
        this.fElementCharge = elementcharge;
        this.Increased_NormalAttackDamage = normalAtkDamage;
        this.Increased_SkillAttackDamage = skillAtkDamage;
        this.Increase_Damage = Increase_Damage;

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
    public float GetElementCharge() { return fElementCharge; }
    public float GetIncreased_NormalAttackDamage() { return Increased_NormalAttackDamage; }
    public float GetIncrease_SkillAttackDamage() { return Increased_SkillAttackDamage; }
    public float GetIncrease_Damage() { return Increase_Damage; }
    public float GetSkillCoolTime() { return fSkill_coolTime; }
    public int GetItemAddHp(int itemIndex)
    {
        if (itemAddHp.TryGetValue(itemIndex, out int value))
        {
            return value;
        }
        return 0;
    }

    public int GetItemAddAttack(int itemIndex)
    {
        if (itemAddAttack.TryGetValue(itemIndex, out int value))
        {
            return value;
        }
        return 0;
    }

    public int GetItemAddDefense(int itemIndex)
    {
        if (itemAddDefense.TryGetValue(itemIndex, out int value))
        {
            return value;
        }
        return 0;
    }
    // 세트 효과 딕셔너리의 킷값에 해당하는 값 반환
    public float GetEquipSetApplied(string setName, int setNum)
    {
        var key = new Tuple<string,int>(setName, setNum);
        if(equipSetApplied.TryGetValue(key, out float value))
        {
            return value;   
        }
        return 0;
    }
    public Dictionary<Tuple<string,int>,float> GetEquipSetAppliedDictionary()
    {
        return equipSetApplied; 
    }

    public void SetState(eCharactgerState state){eCharacState = state;}
    public void SetEncountElement(Element encountElement){eEncountElement = encountElement;}
    public void SetCurrentElement(Element element){eCharacElement = element;}
    public void SetChildElement(int index, Element element){ChildElement[index] = element;}
    public void SetAttack(int attack){nAttack = attack;}
    public void SetElementNum(int elementNum){nElementNum = elementNum;}
    public void SetCurrentHp(int hp){nCurrentHp = hp;}
    public void SetMaxHp(int hp){nMaxHp = hp;}
    public void SetCriticalDamage(float criticalDamage) { this.fCriticalDamage = criticalDamage; }
    public void SetElementCharge(float elementCharge) { this.fElementCharge = elementCharge; }
    public void SetCriticalPersentage(float ciriticalPercentage) { this.fCriticalPercentage = ciriticalPercentage; }
    public void SetDeffense(int nDefense) { this.nDefense= nDefense; }
    public void SetIncreased_NormalAttackDamage(float damage) { this.Increased_NormalAttackDamage= damage; }
    public void SetIncreased_SkillAttackDamage(float damage) { this.Increased_NormalAttackDamage= damage; }
    public void SetIncrease_Damage(float damage) { this.Increase_Damage= damage; }  
    public void SetSkillCoolTime(float fSkill_coolTime ) { this.fSkill_coolTime = fSkill_coolTime; }

    public void AddItemEffect(int itemIndex, int hp, int attack, int defense)
    {
        if(hp!=0) itemAddHp[itemIndex] = hp;
        if(attack != 0) itemAddAttack[itemIndex] = attack;
        if(defense != 0) itemAddDefense[itemIndex] = defense;
    }


    public void RemoveItemEffect(int itemIndex)
    {
        if (itemAddHp.ContainsKey(itemIndex))
            itemAddHp.Remove(itemIndex);
        if(itemAddAttack.ContainsKey(itemIndex))
            itemAddAttack.Remove(itemIndex);
        if(itemAddDefense.ContainsKey(itemIndex))
            itemAddDefense.Remove(itemIndex);
    }

    // 현재 장착 중인 세트 효과를 저장하는 딕셔너리 초기화
    public void AddEquipSetApplied(string setName, int setNum, float effectValue)
    {
        // 튜플을 키로 사용하기 위해 ValueTuple.Create를 사용
        var key = new Tuple<string, int>(setName, setNum);
        equipSetApplied[key] = effectValue;
    }
    // 장착한 세트 효과를 딕셔너리에서 제거
    public void RemoveEquipSetApplied(string setName, int setNum)
    {
        var key = new Tuple<string, int>(setName, setNum);
        if (equipSetApplied.ContainsKey(key))
            equipSetApplied.Remove(key);
    }

    #endregion


}
