using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using static HandlePauseTool;
using static EquipmentSetSynergyMng;
public class Element_Interaction : Singleton<Element_Interaction>
{
    [SerializeField] private GameObject plantElement;
    public static int plantElementNum = 0;
    static int plantElementMaxNum = 5;
    public Queue<GameObject> plantQue = new Queue<GameObject>();

    private void OnEnable()
    {
        // 게임매니저의 이벤트에 구독
        GameManager.OnPauseStateChanged += HandlePauseStateChanged;
    }

    private void OnDisable()
    {
        // 게임매니저의 이벤트 구독 해제
        GameManager.OnPauseStateChanged -= HandlePauseStateChanged;
    }

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


        // 몬스터가 격화 상태라면, 촉진/발산 효과로 원소량에 따라 데미지 증가.
        if(targetMob.GetIsQuicken()==true)
        {
            if (chCls.GetCurrnetElement().GetElement() == Element.e_Element.Lightning ||
                chCls.GetCurrnetElement().GetElement() == Element.e_Element.Plant)
                damage += (int)(chCls.GetElementNum() * 1.2f);
        }


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
    public void c_FireToPlant(CharacterClass chCls,MonsterManager mobMng)
    {
        var targetMob = mobMng.GetMonsterClass();
        float time = c_FireToPlantGetTime(chCls);
        int damage = c_FireToPlantGetDamage(chCls, targetMob);

        StartCoroutine(CalculateDamageOverTime(mobMng, damage, time,Element.e_Element.Fire));
    }
    private IEnumerator CalculateDamageOverTime(MonsterManager mobMng, int damage, float duration, Element.e_Element element)
    {
        var mobHpMng = mobMng.GetMonsterHPMng();
        var mobCls = mobMng.GetMonsterClass();
        int maxHp = mobCls.GetMonsterMaxHp();   //최대체력 저장
        while (duration > 0)
        {
            //hp바 현재 체력 초기화
            mobHpMng.HpBarFill_Init(mobCls.GetMonsterCurrentHp());

            // 데미지 계산식
            int def = mobCls.GetMonsterDef();
            damage -= def;

            // 방어초과시 공격력 보정
            if (damage <= 0)
                damage = 2;

            // hp 수정
            int hp = mobCls.GetMonsterCurrentHp() - damage;
            mobCls.SetMonsterCurrentHP(hp);

            //hp바 반영
            mobHpMng.HpBarFill_End(maxHp, hp, false);

            SetSynergyCheckStarterAtPlaying(false, CharacterManager.Instance.GetCharacterClass(), mobMng, damage); // 세트 시너지 탐색 함수 호출

            // 데미지 플로팅
            switch (element)
            {
                case Element.e_Element.Fire:
                    DamageTextManager.Instance.CreateFloatingText(damage.ToString(), mobMng.GetMonsterHeadPosition(), Color.red);
                    break;
                case Element.e_Element.Water:
                    DamageTextManager.Instance.CreateFloatingText(damage.ToString(), mobMng.GetMonsterHeadPosition(), Color.blue);
                    break;
                case Element.e_Element.Plant:
                    DamageTextManager.Instance.CreateFloatingText(damage.ToString(), mobMng.GetMonsterHeadPosition(), Color.green);
                    break;
                case Element.e_Element.Lightning:
                    DamageTextManager.Instance.CreateFloatingText(damage.ToString(), mobMng.GetMonsterHeadPosition(), new Color(0.5f, 0f, 0.5f));
                    break;
                case Element.e_Element.Wind:
                    DamageTextManager.Instance.CreateFloatingText(damage.ToString(), mobMng.GetMonsterHeadPosition(), Color.cyan);
                    break;
            }

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
    // 불+풀 데미지 계산 공식 (원소 값 * 1.4f)
    private int c_FireToPlantGetDamage(CharacterClass chCls, Monster targetMob)
    {
        int damage = chCls.GetElementNum();

        Element element = targetMob.GetMonsterHittedElement();
        element.SetElement(Element.e_Element.None);
        element.SetIsActive(false);

        damage = (int)(damage * 1.4f);
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
        plantElementNum++;

        Vector3 randomOffset = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
        Vector3 spawnPosition = createPos.position + randomOffset;

        RaycastHit hit;
        if (Physics.Raycast(spawnPosition + Vector3.up * 10f, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                spawnPosition = hit.point;
                // 풀 원핵 생성
                var obj = Instantiate(plantElement, spawnPosition, Quaternion.identity);
                plantQue.Enqueue(obj);

                // 풀원핵 객체의 스크립트 처리
                var mng = obj.GetComponent<PlantElement>();
                mng.SetElement(new Element(Element.e_Element.Plant, false, true));
                mng.SetIdIndex(plantElementNum);
            }
        }

        // 원핵의 숫자가, 최대한계치를 넘었을 경우,
        if (plantElementNum>=plantElementMaxNum)
        {
            var obj = plantQue.Dequeue(); // 가장 위에 있는 객체를 가져옴
            var mng = obj.GetComponent<PlantElement>();
            float range = mng.GetDetectionRange();

            // 몬스터 레이어를 가진 객체를 배열에 저장
            int monsterLayer = LayerMask.NameToLayer("Monster");
            Collider[] colliders = Physics.OverlapSphere(obj.transform.position, range, 1 << monsterLayer);

            foreach (Collider collider in colliders)
            {
                var mobMng = collider.GetComponent<MonsterManager>();
                MonsterHp mobHpMng = mobMng.GetMonsterHPMng();
                Monster mobCls = mobMng.GetMonsterClass();
                mobHpMng.HpBarFill_Init(mobCls.GetMonsterCurrentHp());

                int damage = c_PlantToPlant(chCls, mobCls);  // 개화 데미지 계산 함수 호출
                int mobHp = mobCls.GetMonsterCurrentHp();
                
                mobCls.SetMonsterCurrentHP(mobHp - damage);  // 몬스터 체력 감소
                mobHpMng.HpBarFill_End(mobCls.GetMonsterMaxHp(), mobHp - damage, false);    //Hp바 반영

                SetSynergyCheckStarterAtPlaying(false, chCls, mobMng, damage);


                var mobPos = mobMng.GetMonsterHeadPosition();
                // 데미지 플로팅
                DamageTextManager.Instance.CreateFloatingText(damage.ToString(), mobPos, Color.green);
            }

            plantElementNum--;
            Destroy(obj); // 객체 파괴
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
    // 번개 + 불 범위 반환
    public static float c_LightningToFire(CharacterClass chCls)
    {
        float range;

        range = chCls.GetElementNum() * 0.1f;
        return range;
    }
    // 불 + 번개, 데미지 반환
    public static int c_LightningToFireGetDamage(CharacterClass chCls, Monster mobCls)
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


    // 번개 + 물 == 일정 시간 데미지
    public void c_LightningToWater(CharacterClass chCls, MonsterManager mobMng)
    {
        var targetMob = mobMng.GetMonsterClass();
        float time = c_LightningToWaterGetTime(chCls);
        int damage = c_LightningToWaterGetTime(chCls, targetMob);

        StartCoroutine(CalculateDamageOverTime(mobMng, damage, time,Element.e_Element.Lightning));
    }
    // 번개 + 물 == 지속피해 (데미지 지속 시간 반환)
    private float c_LightningToWaterGetTime(CharacterClass chCls)
    {
        float duration;

        duration = chCls.GetAttack() * 0.1f;
        return duration;
    }
    // 번개 + 물 데미지 계산 공식 (공격력 * 0.7f)
    private int c_LightningToWaterGetTime(CharacterClass chCls, Monster targetMob)
    {
        int damage = chCls.GetAttack();

        Element element = targetMob.GetMonsterHittedElement();
        element.SetElement(Element.e_Element.None);
        element.SetIsActive(false);

        damage = (int)(damage * 0.7f);
        return damage;
    }


    // 번개 + 풀 == 격화상태 부여, 번개or풀 속성 공격 시, 데미지 증가
    public void c_LightningToPlant(CharacterClass chCls, MonsterManager mobMng)
    {
        var mobCls = mobMng.GetMonsterClass();
        mobCls.GetMonsterHittedElement().SetElement(Element.e_Element.None);
        mobCls.GetMonsterHittedElement().SetIsActive(false);

        float duration = c_LightningToPlantGetTime(chCls);
        StartCoroutine(QuickenKeepTime(mobCls, duration));
    }
    // 번개 + 풀 == 격화 지속시간 (데미지 지속 시간 반환)
    private float c_LightningToPlantGetTime(CharacterClass chCls)
    {
        float duration;

        duration = chCls.GetElementNum() * 0.2f;
        return duration;
    }
    // 격화 타임 코루틴 함수
    private IEnumerator QuickenKeepTime(Monster mobCls, float duration)
    {
        while (duration > 0)
        {
            if(!mobCls.GetIsQuicken())
                mobCls.SetIsQuicken(true);

            // 시간 감소
            duration -= 1.0f;

            yield return new WaitForSeconds(1.0f);
        }

        mobCls.SetIsQuicken(false);
    }


    // 번개+바람, 확산 범위 반환
    public static float c_LightningToWind(CharacterClass chCls)
    {
        float range;

        range = chCls.GetElementNum() * 0.4f;
        return range;
    }
    // 번개+바람, 데미지 반환
    public static int c_LightningToWindGetDamage(CharacterClass chCls, Monster mobCls)
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



    #region 풀
    // 풀 + 풀, 개화반응
    public static int c_PlantToPlant(CharacterClass chCls, Monster mobCls)
    {
        int damage = chCls.GetElementNum();
        int offset = (int)(damage * 0.6f);

        Element element = mobCls.GetMonsterHittedElement();
        element.SetElement(Element.e_Element.Plant);
        element.SetIsActive(true);


        damage = CiriticalDamageReturn(chCls, offset);
        damage -= mobCls.GetMonsterDef();

        if (damage < 0)
            damage = 1;

        return damage;
    }


    // 풀 + 불 == 범위 확산 (확산 범위 반환)
    public static float c_PlantToFireGetRange(CharacterClass chCls)
    {
        float range;

        range = chCls.GetElementNum() * 0.1f;
        return range;
    }

    // 풀 + 불 == 일정 시간 데미지
    public void c_PlantToFire(CharacterClass chCls, MonsterManager mobMng)
    {
        var targetMob = mobMng.GetMonsterClass();
        float time = c_PlantToFireGetDuration(chCls);
        int damage = c_PlantToFireGetDamage(chCls, targetMob);

        StartCoroutine(CalculateDamageOverTime(mobMng, damage, time,Element.e_Element.Plant));
    }
    // 풀 + 불 == 지속피해 (데미지 지속 시간 반환)
    private float c_PlantToFireGetDuration(CharacterClass chCls)
    {
        float duration;

        duration = chCls.GetAttack() * 0.1f;
        return duration;
    }
    // 풀 + 불  데미지 계산 공식 (원소마스터리 *0.8f)
    private int c_PlantToFireGetDamage(CharacterClass chCls, Monster targetMob)
    {
        int damage = chCls.GetElementNum();

        Element mobElement = targetMob.GetMonsterHittedElement();
        mobElement.SetElement(Element.e_Element.None);
        mobElement.SetIsActive(false);

        damage = (int)(damage * 0.8f);

        return damage;
    }

    // 풀+바람, 확산 범위 반환
    public static float c_PlantToWind(CharacterClass chCls)
    {
        float range;

        range = chCls.GetElementNum() * 0.4f;
        return range;
    }
    // 풀+바람, 데미지 반환
    public static int c_PlantToWindGetDamage(CharacterClass chCls, Monster mobCls)
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
