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

    // 오브젝트 풀
    public ObjectPool<MonsterManager> CactusPool;
    public ObjectPool<MonsterManager> MushroomAngryPool;
    public ObjectPool<MonsterHp> MonsterHpBarPool;
    public ObjectPool<InvenItemObjClass> WeaponItemPool;
    public ObjectPool<InvenItemObjClass> EquipItemPool;
    public ObjectPool<InvenItemObjClass> GemItemPool;
    public ObjectPool<InvenItemObjClass> FoodItemPool;


    // 캔버스
    public Canvas BottomCanvas;
    public Canvas TopCanvas;
    public Camera HpCamera;

    // 일시정지 체크
    private bool isPaused;

    // 게임 데이터
    private List<ItemClass> list_WeaponItemClasses;
    private List<ItemClass> list_EquipItemClasses;
    private List<ItemClass> list_GemItemClasses;
    private List<ItemClass> list_FoodItemClasses;

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
        list_WeaponItemClasses = new List<ItemClass>();
        list_EquipItemClasses = new List<ItemClass>();
        list_GemItemClasses = new List<ItemClass>();
        list_FoodItemClasses = new List<ItemClass>();
        // 오브젝트 풀 초기화
        CactusPool = new ObjectPool<MonsterManager>(Monsters[0],10, objectPoolSavePos);
        MushroomAngryPool = new ObjectPool<MonsterManager>(Monsters[1],10, objectPoolSavePos);
        MonsterHpBarPool = new ObjectPool<MonsterHp>(MonsterHpBar, 15, objectPoolSavePos);
        WeaponItemPool = new ObjectPool<InvenItemObjClass>(InventoryItemObj, 5, objectPoolSavePos);
        EquipItemPool = new ObjectPool<InvenItemObjClass>(InventoryItemObj, 5, objectPoolSavePos);
        GemItemPool = new ObjectPool<InvenItemObjClass>(InventoryItemObj, 5, objectPoolSavePos);
        FoodItemPool = new ObjectPool<InvenItemObjClass>(InventoryItemObj, 5, objectPoolSavePos);

        // 게임 데이터 초기화
        characterCls = new CharacterClass(300, 300, 0, 100, 50, 15, 1, 3.0f, CharacterClass.eCharactgerState.e_NONE,50,120,50,"플레이어","Knight",0,true);
        list_WeaponItemClasses.Add(new ItemClass("무기", "천공의 검", 5, false, 1, 100000, 1, "풍룡의 영광을 상징하는 기사검.\n잃어버렸다가 오늘날 되찾았다.\n현재 검에 바람 신의 축복이 깃들어 있으며, 푸른 하늘과 바람의 힘을 지니고 있다"));
        list_WeaponItemClasses.Add(new ItemClass("무기", "제례검", 4, false, 1, 65000, 3, "기나긴 세월을 거쳐 석화한 검은 의례적인 장식이 여전히 선명하게 보인다.\n시간의 바람에 씻긴 축복의 힘을 보유하고 있다"));


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
    public void WeaponItemToObjPool(int num, e_PoolItemType type, Transform parent)
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
    #endregion



    #region 게터세터
    public List<ItemClass> GetWeaponItemClass() { return list_WeaponItemClasses; }
    public List<ItemClass> GetEquipItemClass() { return list_EquipItemClasses; }
    public List<ItemClass> GetGemItemClass() { return list_GemItemClasses; }
    public List<ItemClass> GetFoodItemClass() { return list_FoodItemClasses; }



    #endregion

}
