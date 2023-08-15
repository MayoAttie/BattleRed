using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class Element_Interaction : MonoBehaviour
{
    #region 캐릭터
    // 불 + 물 == 150% 데미지 (보정 데미지 반환)
    public static int c_FireToWater(CharacterClass chCls)
    {
        int damage;
        int offset;
        
        Element element = chCls.GetEncountElement();
        element.SetElement(Element.e_Element.None);
        element.SetIsActive(false);
        chCls.SetEncountElement(element);

        damage = chCls.GetAttack();
        offset = (int)(damage * 0.5f);
        return damage+ offset;
    }


    // 불 + 번개 == 범위 확산 (확산 범위 반환)
    public static float c_FireToLightning(CharacterClass chCls)
    {
        float range;

        Element element = chCls.GetEncountElement();
        element.SetElement(Element.e_Element.None);
        element.SetIsActive(false);
        chCls.SetEncountElement(element);
        
        range = chCls.GetElementNum()*0.15f;
        return range;
    }

    // 불 + 풀 == 지속피해 (데미지 지속 시간 반환)
    public static float c_FireToPlant(CharacterClass chCls)
    {
        float duration;
        
        Element element = chCls.GetEncountElement();
        element.SetElement(Element.e_Element.None);
        element.SetIsActive(false);
        chCls.SetEncountElement(element);
        
        duration = chCls.GetElementNum() * 0.1f;
        return duration;
    }

    // 물 + 불 == 200% 데미지 (보정 데미지 반환)
    public static int c_WaterToFire(CharacterClass chCls)
    {
        Element element = chCls.GetEncountElement();
        element.SetElement(Element.e_Element.None);
        element.SetIsActive(false);
        chCls.SetEncountElement(element);

        int damage = chCls.GetAttack();
        return damage*2;
    }

    // 물 + 번개 == 범위 확산 (확산 범위 반환)
    public static float c_WaterToLightning(CharacterClass chCls)
    {
        float range;

        Element element = chCls.GetEncountElement();
        element.SetElement(Element.e_Element.None);
        element.SetIsActive(false);
        chCls.SetEncountElement(element);

        range = chCls.GetElementNum() * 0.2f;
        return range;
    }

    // 물 + 풀 == 풀 원소 생성
    public static void c_WaterToPlant(CharacterClass chCls)
    {
        for (int i = 0; i < 5; i++)
        {
            Element element = chCls.GetChildElement(i);
            if(element.GetIsActive() == false)
            {
                element.SetElement(Element.e_Element.Plant);
                element.SetIsActive(true);
                chCls.SetChildElement(i, element);
                break;
            }
        }
    }
    #endregion


    #region 몬스터
    // 선 방계산 후 뎀증 
    public static int m_FireToWater(Monster monCls, CharacterClass chCls)
    {
        Element characElement = chCls.GetEncountElement();
        characElement.SetElement(Element.e_Element.None);
        characElement.SetIsActive(false);

        int damage;
        int characDef = chCls.GetDeffense();
        int attackPower = monCls.GetMonsterAtkPower();

        int DefRevisionValue = attackPower - characDef;
        if (DefRevisionValue < 0)
            DefRevisionValue = 1;

        damage = (int)(DefRevisionValue * 1.5f);

            return damage;
    }
    public static int m_FireToLightning(Monster monCls, CharacterClass chCls)
    {
        Element characElement = chCls.GetEncountElement();
        characElement.SetElement(Element.e_Element.None);
        characElement.SetIsActive(false);

        int damage;
        int characDef = chCls.GetDeffense();
        int attackPower = monCls.GetMonsterAtkPower();

        int DefRevisionValue = attackPower - characDef;
        if (DefRevisionValue < 0)
            DefRevisionValue = 1;

        damage = (int)(DefRevisionValue * 1.2f);

        return damage;
    }
    public static int m_FireToPlant(Monster monCls, CharacterClass chCls)
    {
        Element characElement = chCls.GetEncountElement();
        characElement.SetElement(Element.e_Element.None);
        characElement.SetIsActive(false);

        int damage;
        int characDef = chCls.GetDeffense();
        int attackPower = monCls.GetMonsterAtkPower();

        int DefRevisionValue = attackPower - characDef;
        if (DefRevisionValue < 0)
            DefRevisionValue = 1;

        damage = (int)(DefRevisionValue * 2.0f);

        return damage;

    }
    // 선뎀증 후 방계산
    public static int m_WaterToFire(Monster monCls, CharacterClass chCls)
    {
        Element characElement = chCls.GetEncountElement();
        characElement.SetElement(Element.e_Element.None);
        characElement.SetIsActive(false);

        int characDef = chCls.GetDeffense();
        int attackPower = (int)(monCls.GetMonsterAtkPower()*1.5f);

        int DefRevisionValue = attackPower - characDef;
        if (DefRevisionValue < 0)
            DefRevisionValue = 1;

        return DefRevisionValue;
    }
    public static int m_WaterToLightning(Monster monCls, CharacterClass chCls)
    {
        Element characElement = chCls.GetEncountElement();
        characElement.SetElement(Element.e_Element.None);
        characElement.SetIsActive(false);

        int characDef = chCls.GetDeffense();
        int attackPower = (int)(monCls.GetMonsterAtkPower() * 1.2f);

        int DefRevisionValue = attackPower - characDef;
        if (DefRevisionValue < 0)
            DefRevisionValue = 1;

        return DefRevisionValue;
    }
    public static int m_WaterToPlant(Monster monCls, CharacterClass chCls)
    {
        Element characElement = chCls.GetEncountElement();
        characElement.SetElement(Element.e_Element.None);
        characElement.SetIsActive(false);

        int characDef = chCls.GetDeffense();
        int attackPower = (int)(monCls.GetMonsterAtkPower() * 1.3f);

        int DefRevisionValue = attackPower - characDef;
        if (DefRevisionValue < 0)
            DefRevisionValue = 1;

        return DefRevisionValue;
    }
    // 높은 뎀, 방어력 비례 감산
    public static int m_LightningToFire(Monster monCls, CharacterClass chCls)
    {
        Element characElement = chCls.GetEncountElement();
        characElement.SetElement(Element.e_Element.None);
        characElement.SetIsActive(false);

        int damage;
        int characDef = chCls.GetDeffense();
        if (characDef >= 100) characDef = 95;
            int attackPower = monCls.GetMonsterAtkPower();

        damage = (int)(attackPower * 2.7f);

        // 방어력에 비례해서 데미지 깎기
        float damageReduction = Mathf.Clamp01(1f - characDef * 0.01f);
        damage = (int)(damage * damageReduction);

        return damage;
    }
    public static int m_LightningToWater(Monster monCls, CharacterClass chCls)
    {
        Element characElement = chCls.GetEncountElement();
        characElement.SetElement(Element.e_Element.None);
        characElement.SetIsActive(false);

        int damage;
        int characDef = chCls.GetDeffense();
        if (characDef >= 100) characDef = 95;
        int attackPower = monCls.GetMonsterAtkPower();

        damage = (int)(attackPower * 2.4f);

        // 방어력에 비례해서 데미지 깎기
        float damageReduction = Mathf.Clamp01(1f - characDef * 0.01f);
        damage = (int)(damage * damageReduction);

        return damage;
    }
    public static int m_LightningToPlant(Monster monCls, CharacterClass chCls)
    {
        Element characElement = chCls.GetEncountElement();
        characElement.SetElement(Element.e_Element.None);
        characElement.SetIsActive(false);

        int damage;
        int characDef = chCls.GetDeffense();
        if (characDef >= 100) characDef = 95;
        int attackPower = monCls.GetMonsterAtkPower();

        damage = (int)(attackPower * 3.0f);

        // 방어력에 비례해서 데미지 깎기
        float damageReduction = Mathf.Clamp01(1f - characDef * 0.01f);
        damage = (int)(damage * damageReduction);

        return damage;
    }
    // 방무뎀
    public static int m_PlantToFire(Monster monCls, CharacterClass chCls)
    {
        Element characElement = chCls.GetEncountElement();
        characElement.SetElement(Element.e_Element.None);
        characElement.SetIsActive(false);

        int damage = monCls.GetMonsterAtkPower();

        return (int)(damage * 1.4f);
    }
    public static int m_PlantToWater(Monster monCls, CharacterClass chCls)
    {
        Element characElement = chCls.GetEncountElement();
        characElement.SetElement(Element.e_Element.None);
        characElement.SetIsActive(false);

        int damage = monCls.GetMonsterAtkPower();

        return (int)(damage * 1.0f);
    }
    public static int m_PlantToLightning(Monster monCls, CharacterClass chCls)
    {
        Element characElement = chCls.GetEncountElement();
        characElement.SetElement(Element.e_Element.None);
        characElement.SetIsActive(false);

        int damage = monCls.GetMonsterAtkPower();

        return (int)(damage * 1.1f);
    }
    // 속성 변환
    public static int m_WindToFire(Monster monCls, CharacterClass chCls)
    {
        Element characElement = chCls.GetEncountElement();
        characElement.SetElement(Element.e_Element.Fire);
        characElement.SetIsActive(false);

        int damage;
        int characDef = chCls.GetDeffense();
        int attackPower = monCls.GetMonsterAtkPower();

        int DefRevisionValue = attackPower - characDef;
        if (DefRevisionValue < 0)
            DefRevisionValue = 1;

        damage = (int)(DefRevisionValue * 1.1f);

        return damage;
    }
    public static int m_WindToLightning(Monster monCls, CharacterClass chCls)
    {
        Element characElement = chCls.GetEncountElement();
        characElement.SetElement(Element.e_Element.Lightning);
        characElement.SetIsActive(false);

        int damage;
        int characDef = chCls.GetDeffense();
        int attackPower = monCls.GetMonsterAtkPower();

        int DefRevisionValue = attackPower - characDef;
        if (DefRevisionValue < 0)
            DefRevisionValue = 1;

        damage = (int)(DefRevisionValue * 1.1f);

        return damage;
    }
    public static int m_WindToWater(Monster monCls, CharacterClass chCls)
    {
        Element characElement = chCls.GetEncountElement();
        characElement.SetElement(Element.e_Element.Water);
        characElement.SetIsActive(false);

        int damage;
        int characDef = chCls.GetDeffense();
        int attackPower = monCls.GetMonsterAtkPower();

        int DefRevisionValue = attackPower - characDef;
        if (DefRevisionValue < 0)
            DefRevisionValue = 1;

        damage = (int)(DefRevisionValue * 1.1f);

        return damage;
    }
    public static int m_WindToPlant(Monster monCls, CharacterClass chCls)
    {
        Element characElement = chCls.GetEncountElement();
        characElement.SetElement(Element.e_Element.Plant);
        characElement.SetIsActive(false);

        int damage;
        int characDef = chCls.GetDeffense();
        int attackPower = monCls.GetMonsterAtkPower();

        int DefRevisionValue = attackPower - characDef;
        if (DefRevisionValue < 0)
            DefRevisionValue = 1;

        damage = (int)(DefRevisionValue * 1.1f);

        return damage;
    }

    #endregion

}
