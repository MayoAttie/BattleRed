using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class WeaponAndEquipCls : ItemClass
{
    int nLimitLevel;            // 한계레벨
    float nMainStat;            // 메인스텟
    float nSubStat;             // 서브스텟
    int nEffectLevel;           // 재련 돌파 레벨
    string sEffectText;         // 재련 스킬 텍스트
    int nCurrentExp;            // 현재 exp
    int nMaxExp;                // 최대 exp
    List<float> list_ExtraStat; // 엑스트라 스텟


    public WeaponAndEquipCls(string sTag, string sName, int nGrade, bool isActive, int nNumber, int nCost, int nLevel,int nLimitLevel, string sContent, string sSet, string sEffectText, int nEffectLevel, float nMainStat, float nSubStat, int nCurrentExp, int nMaxExp) : base(sTag, sName, nGrade, isActive, nNumber, nCost, nLevel, sContent, sSet)
    {
        this.nLimitLevel = nLimitLevel;
        this.sEffectText = sEffectText;
        this.nEffectLevel = nEffectLevel;
        this.nMainStat = nMainStat;
        this.nSubStat = nSubStat;
        this.nCurrentExp = nCurrentExp;
        this.nMaxExp = nMaxExp;
    }

    public int GetLimitLevel() { return nLimitLevel; }
    public int GetEffectLevel() { return nEffectLevel; }
    public string GetEffectText() { return sEffectText; }
    public int GetCurrentExp() { return nCurrentExp; }
    public int GetMaxExp() { return nMaxExp; }
    public float GetMainStat() { return nMainStat; }
    public float GetSubStat() { return nSubStat; }
    public List<float> GetExtraStat() { return list_ExtraStat; }

    public void SetCurrentExp(int nCurrentExp) { this.nCurrentExp = nCurrentExp; }
    public void SetMaxExp(int nMaxExp) { this.nMaxExp = nMaxExp; }
    public void SetLimitLevel(int nLimitLevel) { this.nLimitLevel = nLimitLevel;}
    public void SetEffectLevel(int nEffectLevel) { this.nEffectLevel = nEffectLevel; }
    public void SetSubStat(float nSubStat) { this.nSubStat = nSubStat;}
    public void SetMainStat(float nMainStat) { this.nMainStat = nMainStat; }
    public void SetEffectText(string sEffectText) { this.sEffectText = sEffectText; }
    public void SetExtraStat(List<float> stats) { this.list_ExtraStat = stats; }
}
