using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Element_Interaction : Singleton<Element_Interaction>
{
    [SerializeField] private GameObject plantElement;
    static int plantElementNum = 0;
    static int plantElementMaxNum = 5;

    #region 캐릭터

    // 치명타 계산 로직
    static int CiriticalDamageReturn(CharacterClass chCls, int offset)
    {
        int damage;
        float criticalDamage = chCls.GetCriticalDamage();
        float criticalPercentage = chCls.GetCriticalPercentage();
        int attackPower = chCls.GetAttack();

        damage = chCls.GetAttack() + offset;

        // 크리티컬 확률을 기반으로 크리티컬 결정
        float randomValue = UnityEngine.Random.Range(0f, 1f);
        bool isCritical = randomValue < (criticalPercentage / 100);

        // 크리티컬 데미지 배율 적용
        float criticalDamageMultiplier = criticalDamage / 100;
        int totalCriticalDamage = Mathf.FloorToInt(attackPower * criticalDamageMultiplier);

        // 크리티컬이 발생한 경우 크리티컬 데미지 추가
        damage += isCritical ? totalCriticalDamage : 0;


        return damage;
    }

    // 원소 부착
    public static int c_ElementSet(CharacterClass chCls, Monster targetMob)
    {
        int damage;
        int mobDef = targetMob.GetMonsterDef();

        var targetElement = targetMob.GetMonsterHittedElement();
        targetElement.SetElement(chCls.GetCurrnetElement().GetElement());
        targetElement.SetIsActive(true);

        
        damage = CiriticalDamageReturn(chCls,0);

        // 공격력 - 방어력
        int damageWithoutCritical = damage - mobDef;
        if (damageWithoutCritical < 0) return 1;

        return damage;
    }


    #region 불
    // 불 + 물 == 150% 데미지 (보정 데미지 반환)
    public static int c_FireToWater(CharacterClass chCls, Monster targetMob)
    {
        int damage = chCls.GetAttack();
        int offset;

        Element element = targetMob.GetMonsterHittedElement();
        element.SetElement(Element.e_Element.None);
        element.SetIsActive(false);

        offset = (int)(damage * 0.5f);      // 불 증뎀

        damage = CiriticalDamageReturn(chCls, offset);
        damage -= targetMob.GetMonsterDef();

        if (damage < 0)
            damage = 1;

        return damage;
    }



    // 불 + 번개 == 범위 확산 (확산 범위 반환)
    public static float c_FireToLightning(CharacterClass chCls)
    {
        float range;

        range = chCls.GetElementNum() * 0.1f;
        return range;
    }
    // 불 + 번개, 데미지 계산 공식
    public static int c_FireToLightningGetDamage(CharacterClass chCls, Monster mobCls)
    {
        int damage;

        Element element = mobCls.GetMonsterHittedElement();
        element.SetElement(Element.e_Element.None);
        element.SetIsActive(false);


        damage = CiriticalDamageReturn(chCls, 0);
        damage -= mobCls.GetMonsterDef();

        if(damage<0)
            damage = 1;


        return damage;
    }



    // 불 + 풀 == 일정 시간 데미지
    public void c_FireToPlant(CharacterClass chCls, Monster targetMob)
    {
        float time = c_FireToPlantGetTime(chCls);
        int damage = c_FireToPlantGetDamage(chCls, targetMob);

        StartCoroutine(CalculateDamageOverTime(targetMob, damage, time));
    }
    private IEnumerator CalculateDamageOverTime(Monster mobCls, int damage, float duration)
    {
        while (duration > 0)
        {
            // 데미지 계산식
            int def = mobCls.GetMonsterDef();
            damage -= def;

            // 방어초과시 공격력 보정
            if (damage <= 0)
                damage = 2;

            // hp 수정
            int hp = mobCls.GetMonsterCurrentHp() - damage;
            mobCls.SetMonsterCurrentHP(hp);

            // 시간 감소
            duration -= 1.0f;

            yield return new WaitForSeconds(1.0f);
        }
    }
    // 불 + 풀 == 지속피해 (데미지 지속 시간 반환)
    private float c_FireToPlantGetTime(CharacterClass chCls)
    {
        float duration;

        duration = chCls.GetElementNum() * 0.1f;
        return duration;
    }
    // 불+풀 데미지 계산 공식 (원소 값 * 1.2f)
    private int c_FireToPlantGetDamage(CharacterClass chCls, Monster targetMob)
    {
        int damage = chCls.GetElementNum();

        Element element = targetMob.GetMonsterHittedElement();
        element.SetElement(Element.e_Element.None);
        element.SetIsActive(false);

        damage = (int)(damage * 1.2f);
        return damage;
    }


    // 불+바람, 확산 범위 반환
    public static float c_FireToWind(CharacterClass chCls)
    {
        float range;

        range = chCls.GetElementNum() * 0.4f;
        return range;
    }
    // 불+바람, 데미지 반환
    public static int c_FireToWindGetDamage(CharacterClass chCls, Monster mobCls)
    {
        int damage;

        Element element = mobCls.GetMonsterHittedElement();
        element.SetElement(Element.e_Element.None);
        element.SetIsActive(false);


        damage = CiriticalDamageReturn(chCls, 0);
        damage -= mobCls.GetMonsterDef();

        if (damage < 0)
            damage = 1;


        return damage;
    }

    #endregion


    #region 물
    // 물 + 불 == 200% 데미지 (보정 데미지 반환)
    public static int c_WaterToFire(CharacterClass chCls, Monster targetMob)
    {
        int damage = chCls.GetAttack();
        int offset;

        Element element = targetMob.GetMonsterHittedElement();
        element.SetElement(Element.e_Element.None);
        element.SetIsActive(false);

        offset = (int)(damage * 1.0f);      // 불 증뎀

        damage = CiriticalDamageReturn(chCls, offset);
        damage -= targetMob.GetMonsterDef();

        if (damage < 0)
            damage = 1;

        return damage;
    }

    // 물 + 번개 == 범위 확산 (확산 범위 반환)
    public static float c_WaterToLightning(CharacterClass chCls)
    {
        float range;

        range = chCls.GetElementNum() * 0.2f;
        return range;
    }
    // 물 + 번개, 데미지 반환
    public static int c_WaterToLightningGetDamage(CharacterClass chCls, Monster mobCls)
    {
        int damage;

        Element element = mobCls.GetMonsterHittedElement();
        element.SetElement(Element.e_Element.None);
        element.SetIsActive(false);


        damage = CiriticalDamageReturn(chCls, 0);
        damage -= mobCls.GetMonsterDef();

        if (damage < 0)
            damage = 1;


        return damage;
    }

    // 물 + 풀 == 풀 원소 생성
    public void c_WaterToPlant(CharacterClass chCls, Transform createPos)
    {
        if(plantElementNum<plantElementMaxNum)
        {
            plantElementNum++;
            var element = chCls.GetCurrnetElement();
            element.SetElement(Element.e_Element.None);
            element.SetIsActive(false);

            Vector3 randomOffset = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
            Vector3 spawnPosition = createPos.position + randomOffset;

            RaycastHit hit;
            if (Physics.Raycast(spawnPosition + Vector3.up * 10f, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    spawnPosition = hit.point;
                    Instantiate(plantElement, spawnPosition, Quaternion.identity);
                }
            }
        }
    }


    // 물+바람, 확산 범위 반환
    public static float c_WaterToWind(CharacterClass chCls)
    {
        float range;

        range = chCls.GetElementNum() * 0.4f;
        return range;
    }
    // 물+바람, 데미지 반환
    public static int c_WaterToWindGetDamage(CharacterClass chCls, Monster mobCls)
    {
        int damage;

        Element element = mobCls.GetMonsterHittedElement();
        element.SetElement(Element.e_Element.None);
        element.SetIsActive(false);


        damage = CiriticalDamageReturn(chCls, 0);
        damage -= mobCls.GetMonsterDef();

        if (damage < 0)
            damage = 1;


        return damage;
    }

    #endregion


    #region 번개



    #endregion

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
