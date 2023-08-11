using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element_Interaction : MonoBehaviour
{
    // 불 + 물 == 150% 데미지 (보정 데미지 반환)
    public static int FireToWater(CharacterClass chCls)
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
    public static float FireToLighting(CharacterClass chCls)
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
    public static float FireToPlant(CharacterClass chCls)
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
    public static int WaterToFire(CharacterClass chCls)
    {
        Element element = chCls.GetEncountElement();
        element.SetElement(Element.e_Element.None);
        element.SetIsActive(false);
        chCls.SetEncountElement(element);

        int damage = chCls.GetAttack();
        return damage*2;
    }

    // 물 + 번개 == 범위 확산 (확산 범위 반환)
    public static float WaterToLighting(CharacterClass chCls)
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
    public static void WaterToPlant(CharacterClass chCls)
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

}
