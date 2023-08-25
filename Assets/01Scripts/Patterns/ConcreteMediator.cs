using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using static Element_Interaction;
// ConcreteMediator 클래스
// 옵저버 패턴 활용을 위한 Subject 상속
public class CombatMediator : Subject ,ICombatMediator
{

    #region 캐릭터
    // 캐릭터 평타 공격 판정 함수
    public void Mediator_CharacterAttack(CharacterClass character, MonsterManager targetMonster)
    {
        float criticalDamage = character.GetCriticalDamage();
        float criticalPercentage = character.GetCriticalPercentage();
        int attackPower = character.GetAttack();

        var mobMng = targetMonster.GetMonsterClass();

        int mobDef = mobMng.GetMonsterDef();
        int mobCurrentHp = mobMng.GetMonsterCurrentHp();
        int mobMaxHp = mobMng.GetMonsterMaxHp();

        // 크리티컬 확률을 기반으로 크리티컬 결정
        float randomValue = UnityEngine.Random.Range(0f, 1f);
        bool isCritical = randomValue < (criticalPercentage / 100);

        // 공격력 - 방어력
        int damageWithoutCritical = attackPower - mobDef;
        if (damageWithoutCritical < 0) return;

        // 크리티컬 데미지 배율 적용
        float criticalDamageMultiplier = criticalDamage/100;
        int totalCriticalDamage = Mathf.FloorToInt(attackPower * criticalDamageMultiplier);

        // 크리티컬이 발생한 경우 크리티컬 데미지 추가
        int damage = damageWithoutCritical + (isCritical ? totalCriticalDamage : 0);

        DamageTextManager.Instance.CreateFloatingText(damage.ToString(), targetMonster.GetMonsterHeadPosition(), Color.white);

        //몬스터 피깎
        int tmp = mobCurrentHp - damage;
        mobMng.SetMonsterCurrentHP(tmp);

    }

    // 공격용 스킬 함수
    public void Mediator_CharacterSkillAttack(CharacterClass character, CharacterManager characMng, MonsterManager mobManager)
    {
        var targetMonster = mobManager.GetMonsterClass();
        Transform characTransform = characMng.GetComponent<Transform>();
        Element element = character.GetCurrnetElement();
        Element.e_Element mobElement = targetMonster.GetMonsterHittedElement().GetElement();
        switch(element.GetElement())
        {
            case Element.e_Element.Fire:
                if(mobElement != Element.e_Element.None)
                {
                    int damage = -1;
                    if (mobElement == Element.e_Element.Water)   // 캐릭터 원소 == 불 / 적부착 == 물
                    {
                        damage = c_FireToWater(character,targetMonster);
                        int mobHp = targetMonster.GetMonsterCurrentHp();
                        targetMonster.SetMonsterCurrentHP(mobHp-damage);
                    }
                    else if(mobElement == Element.e_Element.Plant)  // 캐릭터 원소 == 불 / 적부착 풀
                    {
                        Element_Interaction.Instance.c_FireToPlant(character,mobManager);           
                    }
                    else if(mobElement == Element.e_Element.Lightning)  // 캐릭터 원소 == 불 / 적부착 번개
                    {
                        float diffusionRange = c_FireToLightning(character);
                        damage = c_FireToLightningGetDamage(character,targetMonster);

                        DiffusionFunc(diffusionRange, damage, 0, Element.e_Element.Fire, characTransform);

                    }
                    else if(mobElement == Element.e_Element.Wind)  // 캐릭터 원소 == 불 / 적 부착 바람
                    {
                        float diffusionRange = c_FireToWind(character);
                        damage = c_FireToWindGetDamage(character, targetMonster);

                        // 바람 원소로 확산할 경우, 데미지 반감
                        DiffusionFunc(diffusionRange, damage, 2, Element.e_Element.Fire, characTransform);

                    }
                    else    // 중복 원소일 경우, 데미지만 계산만 돌림.
                    {
                        damage = c_ElementSet(character, targetMonster);
                        int mobHp = targetMonster.GetMonsterCurrentHp();
                        targetMonster.SetMonsterCurrentHP(mobHp - damage);
                    }
                    if(damage != -1)
                    DamageTextManager.Instance.CreateFloatingText(damage.ToString(), mobManager.GetMonsterHeadPosition(), Color.red);
                }
                else    // 적에 부착된 원소가 없을 경우 원소 부착
                {
                    int damage = c_ElementSet(character,targetMonster);
                    int mobHp = targetMonster.GetMonsterCurrentHp();
                    targetMonster.SetMonsterCurrentHP(mobHp-damage);
                    DamageTextManager.Instance.CreateFloatingText(damage.ToString(), mobManager.GetMonsterHeadPosition(), Color.red);
                }
                    break;


            case Element.e_Element.Water:
                if (mobElement != Element.e_Element.None)
                {
                    int damage = -1;
                    if (mobElement == Element.e_Element.Fire)   // 캐릭터 원소 == 물 / 적부착 == 불
                    {
                        damage = c_WaterToFire(character, targetMonster);
                        int mobHp = targetMonster.GetMonsterCurrentHp();
                        targetMonster.SetMonsterCurrentHP(mobHp - damage);
                    }
                    else if (mobElement == Element.e_Element.Plant)  // 캐릭터 원소 == 물 / 적부착 풀
                    {
                        Element_Interaction.Instance.c_WaterToPlant(character, characTransform);
                    }
                    else if (mobElement == Element.e_Element.Lightning)  // 캐릭터 원소 == 물 / 적부착 번개
                    {
                        float diffusionRange = c_WaterToLightning(character);
                        damage = c_WaterToLightningGetDamage(character, targetMonster);

                        DiffusionFunc(diffusionRange, damage, 0, Element.e_Element.Water, characTransform);
                    }
                    else if (mobElement == Element.e_Element.Wind)  // 캐릭터 원소 == 물 / 적부착 바람
                    {
                        float diffusionRange = c_WaterToWind(character);
                        damage = c_WaterToWindGetDamage(character, targetMonster);

                        // 바람 원소로 확산할 경우, 데미지 반감
                        DiffusionFunc(diffusionRange, damage, 2, Element.e_Element.Water, characTransform);
                    }
                    else    // 중복 원소일 경우, 데미지만 계산만 돌림.
                    {
                        damage = c_ElementSet(character, targetMonster);
                        int mobHp = targetMonster.GetMonsterCurrentHp();
                        targetMonster.SetMonsterCurrentHP(mobHp - damage);
                    }
                    if(damage != -1)
                    DamageTextManager.Instance.CreateFloatingText(damage.ToString(), mobManager.GetMonsterHeadPosition(), Color.blue);
                }
                else    // 적에 부착된 원소가 없을 경우 원소 부착
                {
                    int damage = c_ElementSet(character, targetMonster);
                    int mobHp = targetMonster.GetMonsterCurrentHp();
                    targetMonster.SetMonsterCurrentHP(mobHp - damage);
                    DamageTextManager.Instance.CreateFloatingText(damage.ToString(), mobManager.GetMonsterHeadPosition(), Color.blue);
                }
                break;


            case Element.e_Element.Plant:
                if (mobElement != Element.e_Element.None)
                {
                    if (mobElement == Element.e_Element.Fire)   // 캐릭터 원소 == 풀 / 적부착 == 불
                    {
                        Element_Interaction.Instance.c_PlantToFire(character,mobManager);
                        float diffusionRange = c_PlantToFireGetRange(character);

                        DiffusionFunc(diffusionRange, 0, 0, Element.e_Element.Plant, characTransform);
                    }
                    else if (mobElement == Element.e_Element.Water)  // 캐릭터 원소 == 풀 / 적부착 물
                    {
                        Element_Interaction.Instance.c_WaterToPlant(character, characTransform);
                    }
                    else if (mobElement == Element.e_Element.Lightning)  // 캐릭터 원소 == 풀 / 적부착 번개
                    {
                        Element_Interaction.Instance.c_LightningToPlant(character, mobManager);
                    }
                    else if (mobElement == Element.e_Element.Wind)  // 캐릭터 원소 == 풀 / 적부착 바람
                    {
                        float diffusionRange = c_PlantToWind(character);
                        int damage = c_PlantToWindGetDamage(character, targetMonster);

                        // 바람 원소로 확산할 경우, 데미지 반감
                        DiffusionFunc(diffusionRange, damage, 2, Element.e_Element.Plant, characTransform);
                    }
                    else    // 중복 원소일 경우, 데미지만 계산만 돌림.
                    {
                        int damage = c_ElementSet(character, targetMonster);
                        int mobHp = targetMonster.GetMonsterCurrentHp();
                        targetMonster.SetMonsterCurrentHP(mobHp - damage);
                        DamageTextManager.Instance.CreateFloatingText(damage.ToString(), mobManager.GetMonsterHeadPosition(), Color.green);
                    }

                }
                else    // 적에 부착된 원소가 없을 경우 원소 부착
                {
                    int damage = c_ElementSet(character, targetMonster);
                    int mobHp = targetMonster.GetMonsterCurrentHp();
                    targetMonster.SetMonsterCurrentHP(mobHp - damage);
                    DamageTextManager.Instance.CreateFloatingText(damage.ToString(), mobManager.GetMonsterHeadPosition(), Color.green);

                }
                break;


            case Element.e_Element.Lightning:
                if (mobElement != Element.e_Element.None)
                {
                    if (mobElement == Element.e_Element.Fire)   // 캐릭터 원소 == 번개 / 적부착 == 불
                    {
                        float diffusionRange = c_LightningToFire(character);
                        int damage = c_LightningToFireGetDamage(character, targetMonster);
                        DiffusionFunc(diffusionRange, damage, 0, Element.e_Element.Lightning, characTransform);
                    }
                    else if (mobElement == Element.e_Element.Water)  // 캐릭터 원소 == 번개 / 적부착 물
                    {
                        Element_Interaction.Instance.c_LightningToWater(character, mobManager);
                    }
                    else if (mobElement == Element.e_Element.Plant)  // 캐릭터 원소 == 번개 / 적부착 풀
                    {
                        Element_Interaction.Instance.c_LightningToPlant(character, mobManager);
                    }
                    else if (mobElement == Element.e_Element.Wind)  // 캐릭터 원소 == 번개 / 적부착 바람
                    {
                        float diffusionRange = c_LightningToWind(character);
                        int damage = c_LightningToWindGetDamage(character, targetMonster);
                        DiffusionFunc(diffusionRange, damage,2,Element.e_Element.Lightning,characTransform);
                    }
                    else    // 중복 원소일 경우, 데미지만 계산만 돌림.
                    {
                        int damage = c_ElementSet(character, targetMonster);
                        int mobHp = targetMonster.GetMonsterCurrentHp();
                        targetMonster.SetMonsterCurrentHP(mobHp - damage);
                    }
                }
                else    // 적에 부착된 원소가 없을 경우 원소 부착
                {
                    int damage = c_ElementSet(character, targetMonster);
                    int mobHp = targetMonster.GetMonsterCurrentHp();
                    targetMonster.SetMonsterCurrentHP(mobHp - damage);
                    DamageTextManager.Instance.CreateFloatingText(damage.ToString(), mobManager.GetMonsterHeadPosition(), new Color(0.5f, 0f, 0.5f));

                }
                break;


            case Element.e_Element.Wind:
                if (mobElement != Element.e_Element.None)
                {
                    if (mobElement == Element.e_Element.Fire)   // 캐릭터 원소 == 바람 / 적부착 == 불
                    {
                        float diffusionRange = c_FireToWind(character);
                        int damage = c_FireToWindGetDamage(character, targetMonster);

                        // 바람 원소로 확산할 경우, 데미지 반감
                        DiffusionFunc(diffusionRange, damage, 2, Element.e_Element.Fire, characTransform);
                    }
                    else if (mobElement == Element.e_Element.Water)  // 캐릭터 원소 == 바람 / 적부착 물
                    {
                        float diffusionRange = c_WaterToWind(character);
                        int damage = c_WaterToWindGetDamage(character, targetMonster);

                        // 바람 원소로 확산할 경우, 데미지 반감
                        DiffusionFunc(diffusionRange, damage, 2, Element.e_Element.Water, characTransform);
                    }
                    else if (mobElement == Element.e_Element.Plant)  // 캐릭터 원소 == 바람 / 적부착 풀
                    {
                        float diffusionRange = c_PlantToWind(character);
                        int damage = c_PlantToWindGetDamage(character, targetMonster);

                        // 바람 원소로 확산할 경우, 데미지 반감
                        DiffusionFunc(diffusionRange, damage, 2, Element.e_Element.Plant, characTransform);
                    }
                    else if (mobElement == Element.e_Element.Lightning)  // 캐릭터 원소 == 바람 / 적부착 번개
                    {
                        float diffusionRange = c_LightningToWind(character);
                        int damage = c_LightningToWindGetDamage(character, targetMonster);

                        // 바람 원소로 확산할 경우, 데미지 반감
                        DiffusionFunc(diffusionRange, damage, 2, Element.e_Element.Lightning, characTransform);
                    }
                }
                else    // 적에 부착된 원소가 없을 경우 원소 부착
                {
                    int damage = c_ElementSet(character, targetMonster);
                    int mobHp = targetMonster.GetMonsterCurrentHp();
                    targetMonster.SetMonsterCurrentHP(mobHp - damage);
                    DamageTextManager.Instance.CreateFloatingText(damage.ToString(), mobManager.GetMonsterHeadPosition(), Color.cyan);
                }
                break;
        }

        
    }

    //확산(확산 범위, 피해량, 피해량 보정값, 부착될 원소타입)
    void DiffusionFunc(float diffusionRange, int damage, int offset, Element.e_Element elementType, Transform characTransform)
    {
        //  범위 내 적에게 데미지
        Collider[] colliders = Physics.OverlapSphere(characTransform.position, diffusionRange, LayerMask.GetMask("Monster"));


        foreach (Collider collider in colliders)
        {
            int attackPower = damage;
            Monster rangeMob = collider.GetComponent<MonsterManager>().GetMonsterClass();
            var monMng =  collider.GetComponent<MonsterManager>();
            if (rangeMob != null)
            {
                if (rangeMob.GetMonsterHittedElement().GetElement() == Element.e_Element.None)
                {
                    rangeMob.GetMonsterHittedElement().SetElement(elementType);
                    rangeMob.GetMonsterHittedElement().SetIsActive(true);
                }
                // 확산으로 데미지 부여.
                int mobHp = rangeMob.GetMonsterCurrentHp();
                if (offset > 0)
                    attackPower /= offset;
                rangeMob.SetMonsterCurrentHP(mobHp - attackPower);
            }
            
            // 속성별 데미지 띄우기
            switch(elementType)
            {
                case Element.e_Element.Fire:
                    DamageTextManager.Instance.CreateFloatingText(damage.ToString(), monMng.GetMonsterHeadPosition(), Color.red);
                    break;
                case Element.e_Element.Water:
                    DamageTextManager.Instance.CreateFloatingText(damage.ToString(), monMng.GetMonsterHeadPosition(), Color.blue);
                    break;
                case Element.e_Element.Plant:
                    DamageTextManager.Instance.CreateFloatingText(damage.ToString(), monMng.GetMonsterHeadPosition(), Color.green);
                    break;
                case Element.e_Element.Lightning:
                    DamageTextManager.Instance.CreateFloatingText(damage.ToString(), monMng.GetMonsterHeadPosition(), new Color(0.5f, 0f, 0.5f));
                    break;
                case Element.e_Element.Wind:
                    DamageTextManager.Instance.CreateFloatingText(damage.ToString(), monMng.GetMonsterHeadPosition(), Color.cyan);
                    break;
            }
        }
    }






    #endregion


    #region 풀원핵
    // 풀 원핵 오브젝트 반응
    public bool Mediator_PlantObj(CharacterClass character, MonsterManager targetMonster)
    {
        bool isActive = false;
        Element element = character.GetCurrnetElement();

        switch(element.GetElement())
        {
            //발화 반응 == 풀원핵 + 화염
            case Element.e_Element.Fire:
                Element_Interaction.Instance.c_FireToPlant(character, targetMonster);
                isActive = true;
                break;

            //만개 반응 == 풀원핵 + 번개
            case Element.e_Element.Lightning:
                Element_Interaction.Instance.c_LightningToPlant(character, targetMonster);
                isActive = true;
                break;

        }

        return isActive;
    }

    #endregion


    #region 몬스터

    // 몬스터 공격 판정함수
    public bool Mediator_MonsterAttack(Monster monster, CharacterClass targetCharacter)
    {
        UI_Manager.Instance.HpBarFill_Init(targetCharacter.GetCurrentHp());

        bool isElementAttackActive = false;
        int atkPower = monster.GetMonsterAtkPower();
        int characHp = targetCharacter.GetCurrentHp();
        int characDef = targetCharacter.GetDeffense();
        Element elementMng =  monster.GetMonsterHaveElement();
        int damage = 0;

        if (elementMng.GetElement() == Element.e_Element.None || elementMng.GetIsActive() == false)
        {
            // 공격력 - 방어력
            damage = atkPower - characDef;
            if (damage <= 0) // 방어력 감쇄로 데미지가 음수 값일 경우, 1보정.
                damage = 1;

            //캐릭터 피깎
            int tmp = characHp - damage;
            Debug.Log(damage);
            targetCharacter.SetCurrentHp(tmp);
            // UI HP바 표기
            UI_Manager.Instance.HpBarFill_End(targetCharacter.GetMaxHp(), targetCharacter.GetCurrentHp(), false);
        }
        else
        {
            // 캐릭터의 부착용 원소 객체를 인스턴스화
            Element characElement = targetCharacter.GetEncountElement();

            // 캐릭터의 현재 원소 적용 상태에 따라서 분기
            switch (characElement.GetElement())
            {
                case Element.e_Element.None:
                    // 캐릭터에게 원소 상태를 부착.
                    characElement.SetElement(elementMng.GetElement());
                    characElement.SetIsActive(true);
                    break;
                case Element.e_Element.Fire:   // 플레이어 == 불 타입 + N원소
                    if (elementMng.GetElement() == Element.e_Element.Water)
                    {
                        damage = m_FireToWater(monster, targetCharacter);
                    }
                    else if(elementMng.GetElement() == Element.e_Element.Plant)
                    {
                        damage = m_FireToPlant(monster, targetCharacter);
                    }
                    else if (elementMng.GetElement() == Element.e_Element.Lightning)
                    {
                        damage = m_FireToLightning(monster, targetCharacter);
                    }
                    break;
                case Element.e_Element.Water:   // 플레이어 == 물 타입 + N원소
                    if (elementMng.GetElement() == Element.e_Element.Fire)
                    {
                        damage = m_WaterToFire(monster, targetCharacter);
                    }
                    else if (elementMng.GetElement() == Element.e_Element.Plant)
                    {
                        damage = m_WaterToPlant(monster, targetCharacter);
                    }
                    else if (elementMng.GetElement() == Element.e_Element.Lightning)
                    {
                        damage = m_WaterToLightning(monster, targetCharacter);
                    }
                    break;
                case Element.e_Element.Plant:   // 플레이어 == 풀 타입 + N원소
                    if (elementMng.GetElement() == Element.e_Element.Water)
                    {
                        damage = m_PlantToWater(monster, targetCharacter);

                    }
                    else if (elementMng.GetElement() == Element.e_Element.Fire)
                    {
                        damage = m_PlantToFire(monster, targetCharacter);
                    }
                    else if (elementMng.GetElement() == Element.e_Element.Lightning)
                    {
                        damage = m_PlantToLightning(monster, targetCharacter);
                    }
                    break;
                case Element.e_Element.Lightning:   // 플레이어 == 라이트닝 타입 + N원소
                    if (elementMng.GetElement() == Element.e_Element.Water)
                    {
                        damage = m_LightningToWater(monster, targetCharacter);
                    }
                    else if (elementMng.GetElement() == Element.e_Element.Plant)
                    {
                        damage = m_LightningToPlant(monster, targetCharacter);
                    }
                    else if (elementMng.GetElement() == Element.e_Element.Fire)
                    {
                        damage = m_LightningToFire(monster, targetCharacter);
                    }
                    break;
                case Element.e_Element.Wind:   // 플레이어 == 바람 타입 + N원소
                    if (elementMng.GetElement() == Element.e_Element.Water)
                    {
                        damage = m_WindToWater(monster, targetCharacter);
                    }
                    else if (elementMng.GetElement() == Element.e_Element.Plant)
                    {
                        damage = m_WindToPlant(monster, targetCharacter);
                    }
                    else if (elementMng.GetElement() == Element.e_Element.Fire)
                    {
                        damage = m_WindToFire(monster, targetCharacter);
                    }
                    else if(elementMng.GetElement() == Element.e_Element.Lightning)
                    {
                        damage = m_WindToLightning(monster, targetCharacter);
                    }
                    break;
            }
            // 동일 속성 간 원소 중첩이기에, 데미지 수식을 벗어남
            if(damage<=0)   // 원소 변화 없이, 데미지만 계산
            {
                // 공격력 - 방어력
                damage = atkPower - characDef;
                if (damage < 0) // 방어력 감쇄로 데미지가 음수 값일 경우, 1보정.
                    damage = 1;
            }

            //캐릭터 피깎
            int tmp = characHp - damage;
            targetCharacter.SetCurrentHp(tmp);
            // UI HP바 표기
            UI_Manager.Instance.HpBarFill_End(targetCharacter.GetMaxHp(), targetCharacter.GetCurrentHp(), false);


            // 원소 공격 유무 액티브 활성화
            isElementAttackActive = true;
        }

        return isElementAttackActive;
    }


    #endregion

}