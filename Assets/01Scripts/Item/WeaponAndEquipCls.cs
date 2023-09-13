using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAndEquipCls : ItemClass
{
    int nLimitLevel;            // 돌파레벨
    int nMainStat;              // 메인스텟
    int nSubStat;               // 서브스텟
    float fEffectNumber;        // 재련 스킬 파워
    string sEffectText;         // 재련 스킬 텍스트

    public WeaponAndEquipCls(string sTag, string sName, int nGrade, bool isActive, int nNumber, int nCost, int nLevel,int nLimitLevel, string sContent, string sSet, string sEffectText, float fEffectNumber,int nMainStat, int nSubStat) : base(sTag, sName, nGrade, isActive, nNumber, nCost, nLevel, sContent, sSet)
    {
        this.nLimitLevel = nLimitLevel;
        this.sEffectText = sEffectText;
        this.fEffectNumber = fEffectNumber;
        this.nMainStat = nMainStat;
        this.nSubStat = nSubStat;
    }

    public int GetLimitLevel() { return nLimitLevel; }
    public float GetEffectNumber() { return fEffectNumber; }
    public string GetEffectText() { return sEffectText; }
    public int GetMainStat() { return nMainStat; }
    public int GetSubStat() { return nSubStat; }

}
