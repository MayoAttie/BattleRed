using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects
{
    private string sTag;
    private string sName;
    private int nGrade;
    private bool isActive;

    public Objects(string sTag, string sName, int nGrade, bool isActive)
    {
        this.sTag = sTag;
        this.sName = sName;
        this.nGrade = nGrade;
        this.isActive = isActive;
    }
        

    public Objects()
    {
    }


    #region 게터세터
    public void SetTag(string sTag)
    {
        this.sTag = sTag;   
    }

    public void SetName(string sName)
    {
        this.sName = sName;
    }
    public void SetCount(int nGrade)
    {
        this.nGrade = nGrade;
    }

    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
    }
    public void SetGrade(int grade) { this.nGrade = grade; }

    public string GetTag()
    {
        return sTag;
    }
    public string GetName()
    {
        return sName;
    }
    public int GetGrade()
    {
        return nGrade;
    }
    public bool GetIsActive()
    {
        return isActive;
    }
    #endregion

}
