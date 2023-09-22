using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HandlePauseTool;
public class GameManager : Singleton<GameManager>
{
    #region 변수
    // 프리팹
    public CharacterClass characterCls;
    public GameObject[] Monsters;
    public GameObject MonsterHpBar;
    public GameObject InventoryItemObj;
    public GameObject SelectButton;

    // 오브젝트 풀
    public ObjectPool<MonsterManager> CactusPool;
    public ObjectPool<MonsterManager> MushroomAngryPool;
    public ObjectPool<MonsterHp> MonsterHpBarPool;
    public ObjectPool<InvenItemObjClass> WeaponItemPool;
    public ObjectPool<InvenItemObjClass> EquipItemPool;
    public ObjectPool<InvenItemObjClass> GemItemPool;
    public ObjectPool<InvenItemObjClass> FoodItemPool;
    public ObjectPool<SelectButtonScript> SelectButtonScriptPool;


    // 캔버스
    public Canvas BottomCanvas;
    public Canvas TopCanvas;
    public Camera HpCamera;

    // 일시정지 체크
    private bool isPaused;

    // 게임 데이터
    private UserClass playerData;


    // 기타
    [SerializeField] Transform objectPoolSavePos;

    #endregion

    #region 구조체
    public enum e_PoolItemType
    {
        Weapon,
        Equip,
        Gem,
        Food,
        Max
    }
    #endregion


    private void Awake()
    {
        // 변수 초기화
        isPaused = false;
        playerData = new UserClass();

        // 오브젝트 풀 초기화
        CactusPool = new ObjectPool<MonsterManager>(Monsters[0],10, objectPoolSavePos);
        MushroomAngryPool = new ObjectPool<MonsterManager>(Monsters[1],10, objectPoolSavePos);
        MonsterHpBarPool = new ObjectPool<MonsterHp>(MonsterHpBar, 15, objectPoolSavePos);
        WeaponItemPool = new ObjectPool<InvenItemObjClass>(InventoryItemObj, 5, objectPoolSavePos);
        EquipItemPool = new ObjectPool<InvenItemObjClass>(InventoryItemObj, 5, objectPoolSavePos);
        GemItemPool = new ObjectPool<InvenItemObjClass>(InventoryItemObj, 5, objectPoolSavePos);
        FoodItemPool = new ObjectPool<InvenItemObjClass>(InventoryItemObj, 5, objectPoolSavePos);
        SelectButtonScriptPool = new ObjectPool<SelectButtonScript>(SelectButton,5,objectPoolSavePos);


        // 게임 데이터 초기화
        characterCls = new CharacterClass(300, 300, 0, 100, 50, 15, 1, 20, 3.0f, CharacterClass.eCharactgerState.e_NONE,50,120,50,"플레이어","Knight",0,true, 100);
        playerData.SetUserCharacter(characterCls);

        List<ItemClass> weaponList = new List<ItemClass>
        {
            new WeaponAndEquipCls("무기", "천공의 검", 5, true, 1, 100000, 1,20, "풍룡의 영광을 상징하는 기사검.\n잃어버렸다가 오늘날 되찾았다.\n현재 검에 바람 신의 축복이 깃들어 있으며, 푸른 하늘과 바람의 힘을 지니고 있다","" ,"치명타 확률이 4% 증가한다. 원소폭발 발동 시 파공의 기세를 획득한다: 이동속도+10%, 공격속도+10%. 일반 공격과 강공격이 명중 시 추가로 공격력 20%의 피해를 준다. 지속 시간: 12초",1,560,12,100,1000),
            new WeaponAndEquipCls("무기", "제례검", 4, false, 1, 65000, 3,20, "기나긴 세월을 거쳐 석화한 검은 의례적인 장식이 여전히 선명하게 보인다.\n시간의 바람에 씻긴 축복의 힘을 보유하고 있다", "","원소전투 스킬로 피해를 줄 때 40%의 확률로 해당 스킬의 재발동 대기시간이 초기화된다. 해당 효과는 30초마다 1회만 발동한다",1,470,13.3f,416,800)
        };
        playerData.SetHadWeaponList(weaponList);

        List<ItemClass> equipList = new List<ItemClass>
        {
            new WeaponAndEquipCls("성배", "이국의 술잔", 3, false, 1, 9000, 1,20, "한때 이 소박한 백자 술잔엔 기쁨의 술이 가득 차 있었다", "행자의 마음","",1,40,15,150,400),
            new WeaponAndEquipCls("깃털", "귀향의 깃털", 3, false, 1, 10000, 1,20, "푸른색 화살 깃 위에 나그네의 저 멀리 떠나가는 미련이 서려 있다", "행자의 마음","",1,400,9,100,400),
            new WeaponAndEquipCls("왕관", "이별의 모자", 3, false, 1, 8000, 1,20, "봄바람의 기운을 발산하는 버드나무 왕관", "행자의 마음","",1,25,5,0,400),
            new WeaponAndEquipCls("꽃", "옛 벗의 마음", 3, false, 1, 7000, 1, 20, "푸른빛의 작은 꽃. 꽃줄기에 오래된 누군가의 리본이 묶여있다", "행자의 마음", "", 1,555,15,150,400),
            new WeaponAndEquipCls("모래", "빛을 좆는 돌", 3, false, 1, 8000, 1, 20, "산전수전 다 겪은 돌시계는 언제나 고요 속에서 일월순환을 기록한다", "행자의 마음", "", 1,30,10,390,400),

            new WeaponAndEquipCls("성배", "전투광의 해골잔", 4, true, 1, 30000, 1,20, "이름 모를 거대한 짐승의 뼈로 만든 컵이다.\n사냥으로 얻은 전리품이다", "전투광","",1,46,20,200,400),
            new WeaponAndEquipCls("깃털", "전투광의 깃털", 4, false, 1, 32000, 1,20, "전설 속의 초상을 알리던 새의 검푸른 꽁지 깃털이다.\n일부분이 붉은색으로 변해버렸다", "전투광","",1,450,12,250,400),
            new WeaponAndEquipCls("왕관", "전투광의 귀면", 4, false, 1, 28000, 1, 20, "생사를 가리지 않는 싸움에서 반쯤 부서진 흉측한 강철 가면", "전투광", "", 1,30,9,0,400),
            new WeaponAndEquipCls("꽃", "전투광의 장미", 4, false, 1, 25000, 1, 20, "핏빛의 정교한 꽃송이는 광전사의 기질과 미묘하게 어울린다", "전투광", "", 1,600,18,0,400),
            new WeaponAndEquipCls("모래", "전투광의 시계", 4, false, 1, 28000, 1, 20, "영원히 무정하게 흐르는 기물(器物)\n광전사에게 전쟁터, 그리고 인간 세상에서의 시간이 얼마 남지 않았음을 일깨워준다", "전투광", "", 1,33,17,0,400),

            new WeaponAndEquipCls("성배", "피에 물든 기사의 술잔", 5, false, 1, 60000, 1,20, "핏빛 기사가 지닌 어두운 금속 잔.\n겉은 검은 연기와 굳어버린 피로 인해 밤처럼 새까맣다", "피에 물든 기사도","",1,50,25,100,400),
            new WeaponAndEquipCls("깃털", "피에 물든 검은 깃털", 5, false, 1, 67000, 1, 20, "기사의 망토에 붙어 있던 까마귀 깃털.\n검은 피에 반복적으로 물들어 완전히 검은색으로 변했다", "피에 물든 기사도", "", 1,510,18,100,400),
            new WeaponAndEquipCls("왕관", "피에 물든 철가면", 5, true, 1, 55000, 1, 20, "기사가 자신의 얼굴을 가릴 때 사용하던 철가면.\n가면 아래의 얼굴은 수많은 사람들이 상상의 나래를 펼치게 했다", "피에 물든 기사도", "", 1,36,10,90,400),
            new WeaponAndEquipCls("꽃", "피에 물든 강철 심장", 5, false, 1, 51000, 1, 20, "피에 검게 물들어 강철과 같은 강도를 가지게 될 정도로 말라버린 꽃.\n과거 이 꽃의 주인에겐 일종의 기념품이지 않았을까", "피에 물든 기사도", "", 1,660,18,160,400),
            new WeaponAndEquipCls("모래", "피에 물든 기사의 술잔", 5, false, 1, 55000, 1, 20, "핏빛 기사가 지닌 어두운 금속 잔.\n겉은 검은 연기와 굳어버린 피로 인해 밤처럼 새까맣다", "피에 물든 기사도", "", 1,35,20,266,400)
        };
        playerData.SetHadEquipmentList(equipList);

        List<ItemClass> gemList = new List<ItemClass>
        {
            new ItemClass("광물", "철광석", 1, false, 1, 600, 1, "철광석. 뛰어난 대장장이에게 가면 빛을 발할 수 있다", ""),
            new ItemClass("광물", "백철", 3, false, 1, 1500, 1, "하얀색 철광석. 뛰어난 대장장이에게 가면 빛을 발할 수 있다", ""),
            new ItemClass("광물", "수정덩이", 4, false, 1, 4000, 1, "가공되지 않은 결정체. 세공을 해야 진정한 가치를 알 수 있다", "")
        };
        playerData.SetHadGemList(gemList);

        List<ItemClass> foodList = new List<ItemClass>
        {            new ItemClass("음식", "달콤달콤 닭고기 스튜", 3, false, 1, 1500, 1, "꿀에 버무려 구운 새고기. 탱탱한 새고기에 넘쳐흐르는 육즙, 게다가 아삭아삭한 달콤달콤꽃까지 정말 맛있다", ""),
            new ItemClass("음식", "무스프", 1, false, 3, 700, 1, "무를 주재료로 만든 야채수프. 여유로운 오후처럼 싱그럽고 소박한 농촌의 향기를 풍긴다.", ""),

        };
        playerData.SetHadFoodList(foodList);

        playerData.SetUserEquippedWeapon(playerData.GetHadWeaponList().Find(item => item.GetIsActive() == true));
        playerData.SetUserEquippedEquipment(playerData.GetHadEquipmentList().FindAll(item => item.GetIsActive() == true).ToArray());
    }

    void Start()
    {


    }

    void Update()
    {
    }

    private void FixedUpdate()
    {
        
    }

    #region 일시정지
    public static event Action<bool> OnPauseStateChanged;

    public void PauseManager()
    {
        isPaused = !isPaused;

        // 일시정지 상태 변경시 이벤트 발생
        OnPauseStateChanged?.Invoke(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }
    public bool GetPauseActive() { return isPaused; }

    #endregion

    #region 몬스터 스폰
    // Transform에 따라, 몬스터 확인 후, 생성 함수 호출
    public void MonsterSpawn(Transform point)
    {
        // 스폰 포인트의 이름을 파싱하여, 번호를 분류.
        GameObject spawnObj = point.gameObject;
        string objName = spawnObj.name;
        string remainingCharacters = objName.Substring(12);

        // 탐색한 콜라이더 내의 몬스터 이름들을 저장하는 리스트 생성.
        Collider[] colliders = Physics.OverlapSphere(point.position, 20, 1 << LayerMask.NameToLayer("Monster"));

        string monsterName;

        // 분류한 번호에 따라, 분기하여 해당하는 몬스터를 생성.
        switch (remainingCharacters)
        {
            case "1":
            case "2":
            case "3":
                monsterName = "Cactus";
                for (int i = 0; i < 2; i++)
                {
                    Vector3 spawnPosition = point.position + new Vector3(i * 2, 0, 0);
                    int extraHealth = characterCls.GetLeveL() * 100;
                    int extraAttack = 0;
                    SpawnMonster(colliders, monsterName, spawnPosition, extraHealth, extraAttack);
                }
                break;
            case "4":
            case "5":
                monsterName = "MushroomAngry";
                for (int i = 0; i < 2; i++)
                {
                    Vector3 spawnPosition = point.position + new Vector3(i * 2, 0, 0);
                    int extraHealth = 0;
                    int extraAttack = characterCls.GetLeveL() * 10; ;
                    SpawnMonster(colliders, monsterName, spawnPosition, extraHealth, extraAttack);
                }
                break;
        }

        #region 레거시
        //// 오브젝트 풀을 이용해 몬스터 생성
        //foreach (var mob in Monsters)
        //{
        //    if (mob.name == "Cactus")
        //    {
        //        Vector3 spawnPosition = point.position + new Vector3(1 * 2, 0, 0);
        //        int extraHealth = characterCls.GetLeveL()*100;
        //        int extraAttack = 0;
        //        Monster monsterCls = new Monster("몬스터", mob.name, 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Precedence, 200 + extraHealth, 200 + extraHealth, 10 + extraAttack, 15, 15, 1.8f, 100f, Element.e_Element.None, 1.5f);
        //        // 오브젝트 풀로 Hp와 몬스터 객체 생성
        //        MonsterManager monsterManager = CactusPool.GetFromPool(spawnPosition, Quaternion.identity);
        //        MonsterHp monsterHpMng = MonsterHpBarPool.GetFromPool(spawnPosition, Quaternion.identity,BottomCanvas.transform);
        //        monsterManager.SetMonsterHPMng(monsterHpMng);
        //        monsterManager.SetMonsterClass(monsterCls);
        //        //hp FillAmount 초기화
        //        monsterHpMng.HpBarFill_Init(monsterCls.GetMonsterCurrentHp());
        //        monsterHpMng.HpBarFill_End(monsterCls.GetMonsterMaxHp(), monsterCls.GetMonsterCurrentHp(), false);
        //    }
        //    else if (mob.name == "MushroomAngry")
        //    {
        //        Vector3 spawnPosition = point.position + new Vector3(2 * 2, 0, 0);
        //        int extraHealth = 0;
        //        int extraAttack = characterCls.GetLeveL() * 10;
        //        Monster monsterCls = new Monster("몬스터", mob.name, 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Precedence, 200 + extraHealth, 200 + extraHealth, 10 + extraAttack, 15, 15, 1.8f, 100f, Element.e_Element.None, 1.5f);
        //        // 오브젝트 풀로 Hp와 몬스터 객체 생성
        //        MonsterManager monsterManager = MushroomAngryPool.GetFromPool(spawnPosition, Quaternion.identity);
        //        MonsterHp monsterHpMng = MonsterHpBarPool.GetFromPool(spawnPosition, Quaternion.identity, BottomCanvas.transform);
        //        monsterManager.SetMonsterHPMng(monsterHpMng);
        //        monsterManager.SetMonsterClass(monsterCls);
        //        //hp FillAmount 초기화
        //        monsterHpMng.HpBarFill_Init(monsterCls.GetMonsterCurrentHp());
        //        monsterHpMng.HpBarFill_End(monsterCls.GetMonsterMaxHp(), monsterCls.GetMonsterCurrentHp(), false);
        //    }
        //}
        #endregion
    }

    // 몬스터 생성 함수
    private void SpawnMonster(Collider[] colliders, string monsterName, Vector3 spawnPosition, int extraHealth, int extraAttack)
    {
        foreach(Collider collider in colliders)
        {
            var mng = collider.GetComponent<MonsterManager>();
            if (mng.GetMonsterClass().GetName() == monsterName)
                return;
        }

        Monster monsterCls = new Monster("몬스터", monsterName, 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Precedence, 200 + extraHealth, 200 + extraHealth, 10 + extraAttack, 15, 15, 1.8f, 100f, Element.e_Element.None, 1.5f);

        MonsterManager monsterManager = null;

        //오브젝트 풀로 몬스터 생성
        switch (monsterName)
        {
            case "Cactus":
                monsterManager = CactusPool.GetFromPool(spawnPosition, Quaternion.identity);
                break;
            case "MushroomAngry":
                monsterManager = MushroomAngryPool.GetFromPool(spawnPosition, Quaternion.identity);
                break;
        }
        // 오브젝트 풀로 hp바 생성
        MonsterHp monsterHpMng = MonsterHpBarPool.GetFromPool(spawnPosition, Quaternion.identity, BottomCanvas.transform);

        // 필요한 데이터 초기화
        monsterManager.SetMonsterHPMng(monsterHpMng);
        monsterManager.SetMonsterClass(monsterCls);

        monsterHpMng.HpBarFill_Init(monsterCls.GetMonsterCurrentHp());
        monsterHpMng.HpBarFill_End(monsterCls.GetMonsterMaxHp(), monsterCls.GetMonsterCurrentHp(), false);
    }

    #endregion


    #region 몬스터 제거
    public void RangeOutMonsterPoolReturn(MonsterManager mobMng, MonsterHp mobHpMng)
    {
        MonsterHpBarPool.ReturnToPool(mobHpMng);

        string mobName = mobMng.GetMonsterClass().GetName();
        switch(mobName)
        {
            case "Cactus":
                CactusPool.ReturnToPool(mobMng);
                break;
            case "MushroomAngry":
                MushroomAngryPool.ReturnToPool(mobMng);
                break;
        }
    }
    #region 레거시

    //// 일정 범위 내의 몬스터를 제외한 나머지 몬스터를 오브젝트 풀에 리턴.
    //public void MonsterReturnToPool(Collider[] detectedMonster)
    //{
    //    List<MonsterManager> cactusList = CactusPool.GetPoolList();
    //    List<MonsterManager> AngryMushList = MushroomAngryPool.GetPoolList();

    //    // 오브젝트 풀을 순회.
    //    if (cactusList.Count > 0)
    //    {
    //        // Cactus을 저장한 리스트를 순회하며, 활성화 여부 체크, 및 detectedMonster 내의 객체 체크
    //        for (int i = 0; i < cactusList.Count; i++)
    //        {
    //            if (!cactusList[i].gameObject.activeSelf)
    //                continue;
    //            bool isDetected = false;
    //            MonsterManager compareMob = cactusList[i];
    //            for (int j = 0; j < detectedMonster.Length; j++)
    //            {
    //                MonsterManager detectedMob = detectedMonster[j].GetComponent<MonsterManager>(); 
    //                if (compareMob == detectedMob)
    //                {
    //                    isDetected = true;
    //                    break;
    //                }
    //            }
    //            if (!isDetected)
    //            {
    //                // 탐지된 적이 아닐 경우엔 다시 오브젝트풀에 리턴.
    //                MonsterHpBarPool.ReturnToPool(compareMob.GetMonsterHPMng());
    //                CactusPool.ReturnToPool(compareMob);
    //            }
    //        }
    //    }
    //    if (AngryMushList.Count > 0)
    //    {
    //        for (int i = 0; i < AngryMushList.Count; i++)
    //        {
    //            if (!AngryMushList[i].gameObject.activeSelf)
    //                continue;
    //            bool isDetected = false;
    //            MonsterManager compareMob = AngryMushList[i];
    //            for (int j = 0; j < detectedMonster.Length; j++)
    //            {
    //                MonsterManager detectedMob = detectedMonster[j].GetComponent<MonsterManager>(); // 수정된 부분
    //                if (compareMob == detectedMob)
    //                {
    //                    isDetected = true;
    //                    break;
    //                }
    //            }
    //            if (!isDetected)
    //            {
    //                MonsterHpBarPool.ReturnToPool(compareMob.GetMonsterHPMng());
    //                MushroomAngryPool.ReturnToPool(compareMob);
    //            }
    //        }
    //    }
    //}
    #endregion
    #endregion


    #region 아이템 데이터 오브젝트 풀 셋
    public void ItemToObjPool(int num, e_PoolItemType type, Transform parent)
    {
        // 매개변수만큼 오브젝트 풀에서 UI 오브젝트 생성
        for(int i=0; i<num; i++)
        {
            switch(type)
            {
                case e_PoolItemType.Weapon:
                    WeaponItemPool.GetFromPool(Vector3.zero, Quaternion.identity, parent.transform);
                    break;
                case e_PoolItemType.Equip:
                    EquipItemPool.GetFromPool(Vector3.zero, Quaternion.identity, parent.transform);
                    break;
                case e_PoolItemType.Gem:
                    GemItemPool.GetFromPool(Vector3.zero, Quaternion.identity, parent.transform);
                    break;
                case e_PoolItemType.Food:
                    FoodItemPool.GetFromPool(Vector3.zero, Quaternion.identity, parent.transform);
                    break;
            }
        }
    }
    public void ItemToObjPool_SelectButton(int num, Transform parent)
    {
        for(int i=0; i<num; i++)
        {
            SelectButtonScriptPool.GetFromPool(Vector3.zero, Quaternion.identity, parent);
        }
    }
    #endregion



    #region 게터세터
    public UserClass GetUserClass() { return playerData; }



    #endregion

}
