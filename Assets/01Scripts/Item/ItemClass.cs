using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemClass : Objects
{
    int nNumber;
    int nCost;

    public ItemClass()
    { }

    public ItemClass(string sTag, string sName, int nGrade, bool isActive, int nNumber, int nCost) : base(sTag, sName, nGrade, isActive)
    {
        this.nNumber = nNumber;
        this.nCost = nCost;
    }

    #region 게터세터
    public void SetNumber(int nNumber) { this.nNumber = nNumber; }
    public void SetCost(int nCost) { this.nCost = nCost; }
    public int GetNumber() { return nNumber;}
    public int GetCost() { return nCost;}
    #endregion
}
