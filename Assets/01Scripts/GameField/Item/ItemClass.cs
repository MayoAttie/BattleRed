using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemClass : Objects
{
    int nNumber;
    int nCost;
    int nLevel;
    int nId;
    string sContent;
    string sSet;

    public ItemClass()
    { }

    public ItemClass(string sTag, string sName, int nGrade, bool isActive, int nNumber, int nCost, int nLevel, string sContent, string sSet) : base(sTag, sName, nGrade, isActive)
    {
        this.nNumber = nNumber;
        this.nCost = nCost;
        this.nLevel = nLevel;
        this.sContent = sContent;
        this.sSet = sSet;
        this.nId = -1;
        //this.nId = GameManager.Instance.GetItem_Id_value();
        //GameManager.Instance.Item_Id_value_Upper();
    }

    #region 게터세터
    public void SetNumber(int nNumber) { this.nNumber = nNumber; }
    public void SetCost(int nCost) { this.nCost = nCost; }
    public void SetLevel(int nLevel) { this.nLevel= nLevel; }
    public void SetId(int id) { this.nId = id; }
    public int GetNumber() { return nNumber;}
    public int GetCost() { return nCost;}
    public int GetLevel() { return nLevel; }
    public string GetContent() { return sContent; }
    public string GetSet() { return sSet;}
    public int GetId() { return nId; }
    #endregion

    // 생성자 및 다른 멤버들은 그대로 두고 CopyFrom 메서드만 추가
    public virtual void CopyFrom(ItemClass other)
    {
        SetTag(other.GetTag());
        SetName(other.GetName());
        SetGrade(other.GetGrade());
        SetActive(other.GetIsActive());

        // 얕은 복사
        nNumber = other.nNumber;
        nCost = other.nCost;
        nLevel = other.nLevel;
        nId = other.nId;

        // 깊은 복사: 참조형 멤버들은 각각의 인스턴스를 생성하여 값을 복사
        sContent = string.Copy(other.sContent); //other.sContent; 
        sSet = string.Copy(other.sSet); //other.sSet; 
    }
}
