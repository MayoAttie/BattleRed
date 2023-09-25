using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemClass : Objects
{
    int nNumber;
    int nCost;
    int nLevel;
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
    }

    #region 게터세터
    public void SetNumber(int nNumber) { this.nNumber = nNumber; }
    public void SetCost(int nCost) { this.nCost = nCost; }
    public void SetLevel(int nLevel) { this.nLevel= nLevel; }
    public int GetNumber() { return nNumber;}
    public int GetCost() { return nCost;}
    public int GetLevel() { return nLevel; }
    public string GetContent() { return sContent; }
    #endregion
}
