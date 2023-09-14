using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAndEquipCls : ItemClass
{
    int nLimitLevel;            // 한계레벨
    int nMainStat;              // 메인스텟
    float nSubStat;               // 서브스텟
    int nEffectLevel;          // 재련 돌파 레벨
    string sEffectText;         // 재련 스킬 텍스트


    public WeaponAndEquipCls(string sTag, string sName, int nGrade, bool isActive, int nNumber, int nCost, int nLevel,int nLimitLevel, string sContent, string sSet, string sEffectText, int nEffectLevel, int nMainStat, float nSubStat) : base(sTag, sName, nGrade, isActive, nNumber, nCost, nLevel, sContent, sSet)
    {
        this.nLimitLevel = nLimitLevel;
        this.sEffectText = sEffectText;
        this.nEffectLevel = nEffectLevel;
        this.nMainStat = nMainStat;
        this.nSubStat = nSubStat;
    }

    public int GetLimitLevel() { return nLimitLevel; }
    public int GetEffectLevel() { return nEffectLevel; }
    public string GetEffectText() { return sEffectText; }
    public int GetMainStat() { return nMainStat; }
    public float GetSubStat() { return nSubStat; }

}
