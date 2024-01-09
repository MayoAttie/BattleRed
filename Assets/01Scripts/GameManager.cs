using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static HandlePauseTool;
using static CharacterUpgradeManager;
using static UI_UseToolClass;
public class GameManager : Singleton<GameManager>
{
    #region 변수
    // 프리팹
    public CharacterClass characterCls;
    public GameObject[] Monsters;
    public GameObject MonsterHpBar;
    public GameObject InventoryItemObj;
    public GameObject SelectButton;
    public GameObject[] DropItemObject;
    public GameObject DropItem_UI;
    public GameObject PlayerbleCharacter;

    // 오브젝트 풀
    public ObjectPool<MonsterManager> CactusPool;
    public ObjectPool<MonsterManager> MushroomAngryPool;
    public ObjectPool<MonsterManager> GolemBossPool;
    public ObjectPool<MonsterHp> MonsterHpBarPool;
    public ObjectPool<InvenItemObjClass> WeaponItemPool;
    public ObjectPool<InvenItemObjClass> EquipItemPool;
    public ObjectPool<InvenItemObjClass> GemItemPool;
    public ObjectPool<InvenItemObjClass> FoodItemPool;
    public ObjectPool<SelectButtonScript> SelectButtonScriptPool;
    public ObjectPool<DropItem> DropItem_1Pool;
    public ObjectPool<DropItem> DropItem_2Pool;
    public ObjectPool<DropItem> DropItem_3Pool;
    public ObjectPool<DropItem_UI> DropItemUI_Pool;
    public ObjectPool<DropItem_UI> InterectionObjUI_Pool;


    // 캔버스
    public Canvas BottomCanvas;
    public Canvas TopCanvas;
    public Camera HpCamera;

    // 일시정지 체크
    private bool isPaused;

    // 게임 데이터
    private UserClass playerData;
    private List<ItemClass> ItemDataList;   // 디폴트 아이템 리스트
    private List<WeaponAndEquipCls> WeaponAndEquipmentDataList; // 디폴트 무기/성유물 아이템 리스트
    private List<Tuple<string,List<WEAPON_EQUIP_STATE_DATA_BASE>>> list_WeaponAndEquipData;                                     // 무기 성유물 스탯 테이블
    private List<Tuple<string, List<WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA>>> list_WeaponAndEquipLimitBreakResourceData;        // 무기 성유물 돌파 재료 테이블
    private List<Tuple<string, List<WEAPON_EQUIP_EFFECT_DATA_BASE>>> list_WeaponAndEquipReforgeGradeData;                       // 무기 성유물 재련 능력치 테이블
    private List<EQUIPMENT_SET_SYNERGY_DATA_BASE> list_EquipmentSetSynergyData;                                                 // 성유물 세트 효과 테이블
    private Monster[] arr_MonsterData;                                                                                          // 스폰 포인트 IN 몬스터 클래스 테이블
    private List<Vector3> list_SpawnPoint;                                                                                      // 스폰 포인트 저장 리스트
    public List<SYNTHESIS_DATA_BASE> list_WeaponSynthesisData;
    public List<SYNTHESIS_DATA_BASE> list_EquipSynthesisData;
    public List<SYNTHESIS_DATA_BASE> list_EtcSynthesisData;
    


    // 기타
    [SerializeField] Transform objectPoolSavePos;


    // 전역변수
    static int nIdValue;

    #endregion

    #region 구조체
    public enum e_PoolItemType                  // 풀링 오브젝트의 타입
    {
        Weapon,
        Equip,
        Gem,
        Food,
        Max
    }
    public struct WEAPON_EQUIP_STATE_DATA_BASE  // 무기 및 성유물 스탯 테이블
    {
        public int LEVEL;
        public float MAIN_STAT;
        public float SUB_STAT;
        public int LIMIT_LEVEL;

        public WEAPON_EQUIP_STATE_DATA_BASE(int level, float mainStat, float subStat, int limitLevel)
        {
            LEVEL = level;
            MAIN_STAT = mainStat;
            SUB_STAT = subStat;
            LIMIT_LEVEL = limitLevel;
        }
    }
    public struct WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA    // 무기 및 성유물의 돌파 재료 테이블
    {
        public ItemClass RESOURCE_ITEM;
        public int RESOURCE_NUMBER;
        public int TARGET_ITEM_LEVEL;

        public WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA(ItemClass rESOURCE_ITEM, int rESOURCE_NUMBER, int tARGET_ITEM_LEVEL)
        {
            RESOURCE_ITEM = rESOURCE_ITEM;
            RESOURCE_NUMBER = rESOURCE_NUMBER;
            TARGET_ITEM_LEVEL = tARGET_ITEM_LEVEL;
        }
    }
    public struct WEAPON_EQUIP_EFFECT_DATA_BASE         // 무기 및 성유물의 재련 능력치 테이블
    {
        public int EFFECT_LEVEL;
        public string EFFECT_TEXT;

        public WEAPON_EQUIP_EFFECT_DATA_BASE(int eFFECT_LEVEL, string eFFECT_TEXT)
        {
            EFFECT_LEVEL = eFFECT_LEVEL;
            EFFECT_TEXT = eFFECT_TEXT;
        }
    }
    public struct EQUIPMENT_SET_SYNERGY_DATA_BASE       // 성유물 세트 효과 테이블
    {
        public string EQUIPMENT_SET_NAME;
        public int EQUIPMENT_SET_NUMBER;
        public string EQUIPMENT_SET_EFFECT_TEXT;
        public string EQUIPMENT_SET_EFFECT_ELEMENT;
        public float EQUIPMENT_SET_EFFECT_VALUE;

        public EQUIPMENT_SET_SYNERGY_DATA_BASE(string eQUIPMENT_SET_NAME, int eQUIPMENT_SET_NUMBER, string eQUIPMENT_SET_EFFECT_TEXT, string eQUIPMENT_SET_EFFECT_ELEMENT, float eQUIPMENT_SET_EFFECT_VALUE)
        {
            EQUIPMENT_SET_NAME = eQUIPMENT_SET_NAME;
            EQUIPMENT_SET_NUMBER = eQUIPMENT_SET_NUMBER;
            EQUIPMENT_SET_EFFECT_TEXT = eQUIPMENT_SET_EFFECT_TEXT;
            EQUIPMENT_SET_EFFECT_ELEMENT = eQUIPMENT_SET_EFFECT_ELEMENT;
            EQUIPMENT_SET_EFFECT_VALUE = eQUIPMENT_SET_EFFECT_VALUE;
        }
    }
    public struct SYNTHESIS_DATA_BASE
    {
        public string MATERIAL_1;
        public string MATERIAL_2;
        public int MATERIAL_1_NUM;
        public int MATERIAL_2_NUM;
        public string COMPLETE_iTEM;
        public SYNTHESIS_DATA_BASE(string material1, string material2, int material1Num, int material2Num, string completeItem)
        {
            MATERIAL_1 = material1;
            MATERIAL_2 = material2;
            MATERIAL_1_NUM = material1Num;
            MATERIAL_2_NUM = material2Num;
            COMPLETE_iTEM = completeItem;
        }
    }
    #endregion


    private void Awake()
    {
        // 변수 초기화
        isPaused = false;
        playerData = new UserClass();
        list_SpawnPoint = new List<Vector3>();

        // 오브젝트 풀 초기화
        CactusPool = new ObjectPool<MonsterManager>(Monsters[0],10, objectPoolSavePos);
        MushroomAngryPool = new ObjectPool<MonsterManager>(Monsters[1],10, objectPoolSavePos);
        GolemBossPool = new ObjectPool<MonsterManager>(Monsters[2], 1, objectPoolSavePos);
        MonsterHpBarPool = new ObjectPool<MonsterHp>(MonsterHpBar, 15, objectPoolSavePos);
        WeaponItemPool = new ObjectPool<InvenItemObjClass>(InventoryItemObj, 5, objectPoolSavePos);
        EquipItemPool = new ObjectPool<InvenItemObjClass>(InventoryItemObj, 5, objectPoolSavePos);
        GemItemPool = new ObjectPool<InvenItemObjClass>(InventoryItemObj, 5, objectPoolSavePos);
        FoodItemPool = new ObjectPool<InvenItemObjClass>(InventoryItemObj, 5, objectPoolSavePos);
        SelectButtonScriptPool = new ObjectPool<SelectButtonScript>(SelectButton,5,objectPoolSavePos);
        DropItem_1Pool = new ObjectPool<DropItem>(DropItemObject[0],3,objectPoolSavePos);
        DropItem_2Pool = new ObjectPool<DropItem>(DropItemObject[1],3,objectPoolSavePos);
        DropItem_3Pool = new ObjectPool<DropItem>(DropItemObject[2],3,objectPoolSavePos);
        DropItemUI_Pool = new ObjectPool<DropItem_UI>(DropItem_UI, 5, objectPoolSavePos);
        InterectionObjUI_Pool = new ObjectPool<DropItem_UI>(DropItem_UI, 3, objectPoolSavePos);

        #region 디폴트 아이템 데이터 DB 테이블 채우기
        ItemDataList = new List<ItemClass>
        {
            new ItemClass("육성 아이템","칼바람 울프의 젖니"         ,1,false,1,500,0,"늑대 무리는 그의 축복을 받은 근위병이다. 새끼 늑대의 젖니라도 상당한 저력을 가지고 있다.\n과거의 신들은 인간을 사랑한다는 책임을 짊어지고 있었다. 따라서, 늑대 무리를 인솔하지만 버림받은 아이를 입양하고 방랑자들을 받아들인 「안드리우스」는 아주 이상하다",""),
            new ItemClass("육성 아이템","칼바람 울프의 이빨"         ,3,false,1,1000,0,"늑대의 삶엔 전투가 끊이지 않아 뼈가 부서지는 사투도 적지 않다.\n「안드리우스」는 인간은 실망만을 안겨다 줄 뿐이지만 순진한 아이들은 죄가 없다고 여겼다. 늑대 무리가 아이를 선택했고 아이 또한 늑대를 선택했다. 그렇게 그들은 「루피카」-운명의 가족이 되었다",""),
            new ItemClass("육성 아이템","칼바람 울프의 부서진 이빨" ,4,false,1,2000,0,"늑대 무리는 인간에게 같은 무리가 없으면 고독함을 느낄 것이란 걸 충분히 이해하고 있다. 명예로운 부서진 이빨은 늑대들의 이별 선물로 몸을 보호해주는 마력이 있다고 한다.\n머나먼 세계에는 어머니 늑대가 위대한 쌍둥이를 입양했다는 전설이 있다. 늑대와 사람들이 함께 산 「집」은 「늑대의 동굴」 즉 「루페르카」로 불렸고 이는 이 세계의 「루피카」라는 뜻이다",""),
            new ItemClass("육성 아이템","지맥의 낡은 가지"             ,1,false,1,500,0,"지하 깊은 곳의 마른 나뭇가지. 오랜 세월이 지났지만 알록달록한 나무껍질에서 내재된 힘을 느낄 수 있다",""),
            new ItemClass("육성 아이템","지맥의 마른 잎"              ,3,false,1,1000,0,"지하 깊은 곳에서 자라는 식물의 가지. 줄기에서 멀리 떨어져 있지만 여전히 요동치는 힘이 희미하게 남아있다",""),
            new ItemClass("육성 아이템","지맥의 새싹"               ,4,false,1,2000,0,"세계 각지에 뿌리를 내린 거목이 있다고 한다.\n이 가지는 그 거목의 일부라고 한다. 누군가에게 꺾여 멀리 이동한 적이 없는 듯하며, 왕성한 생명력으로 새잎이 자랐다",""),
            new ItemClass("육성 아이템","슬라임 응축액"               ,1,false,1,500,0,"슬라임을 덮고 있는 걸쭉한 액체. 각지의 원소 공방에서 가장 흔히 보이는 원료이다",""),
            new ItemClass("육성 아이템","슬라임청"                ,3,false,1,1000,0,"정제된 슬라임의 분비물. 피부에 닿으면 해로우니 반드시 피해야 한다",""),
            new ItemClass("육성 아이템","슬라임 원액"               ,4,false,1,2000,0,"슬라임 농축 원액. 가만히 두면 불규칙하게 움직인다",""),
            new ItemClass("육성 아이템","라이언 투사의 족쇄"         ,1,false,1,500,0,"사람들은 늘 영웅의 이야기를 과장한다. 심지어 과거 영웅을 속박했던 족쇄도 자유의 적으로 승화됐다.\r\n때문에 이 족쇄도 비범한 힘을 지니게 되었다",""),
            new ItemClass("육성 아이템","라이언 투사의 쇠사슬"     ,3,false,1,1000,0,"무기에 돌파의 힘을 부여하는 재료.\n과거 대영웅 바네사가 찼던 족쇄—또는 그 시대의 모든 검투사들은 수갑을 찼다. 라이언 투사 또한 그 일원 중 한 명일 뿐이다",""),
            new ItemClass("육성 아이템","라이언 투사의 수갑"         ,4,false,1,2000,0,"무기에 돌파의 힘을 부여하는 재료.\n바네사는 족쇄 또는 수갑에 묶여있던 게 아니다. 그녀가 빠져나올 마음만 먹었다면 언제든지 몬드의 평범한 금속으로 만든 족쇄를 파괴할 수 있었다. 몬드는 우수한 광석도 고향의 신의 불꽃도 없으니까….\n그녀를 막은 건 오직 부족민을 보호해야 한다는 책임감뿐이다",""),
            new ItemClass("육성 아이템","혼돈의 장치"              ,1,false,1,500,0,"멈춰버린 고대 유적에서 나온 구조체. 몸체의 균형을 잡아주던 부분으로 정교한 공학적 감각이 돋보인다",""),
            new ItemClass("육성 아이템","혼돈의 회로"              ,3,false,1,1000,0,"멈춰버린 고대 유적에서 나온 구조체. 이전에 논리 회로였던 부분으로, 아무도 해제할 수 없는 위대한 기술이 담겨있다",""),
            new ItemClass("육성 아이템","혼돈의 노심"              ,4,false,1,2000,0,"멈춰버린 고대 유적에서 나온 구조체. 이전에 에너지의 핵심이었던 부분으로, 이렇게 신비로운 기술을 이해하고 재구성할 수 있다면 이 세계를 바꿀 수 있을지도 모른다",""),
            new ItemClass("육성 아이템","이능 두루마리"            ,1,false,1,500,0,"어떤 마법에 관한 두루마리의 남은 조각인 듯하다. 이상한 기운과 불길한 온기를 은은하게 내뿜고 있다",""),
            new ItemClass("육성 아이템","봉마의 두루마리"           ,3,false,1,1000,0,"대충 만들어진 난해한 고서 단편. 몇몇 마물은 단편에 있는 인물의 형상을 모방해 마법의 기적을 조금은 재현해 낼 수 있다",""),
            new ItemClass("육성 아이템","금주의 두루마리"           ,4,false,1,2000,0,"고대 형식으로 제작된 고서 초본. 해독할 수 있는 사람이 드물다. 그 의미를 알아버린 학자는 결국 미쳐버린다고 한다",""),

            new ItemClass("광물", "철광석", 1, false, 1, 600, 1, "철광석. 뛰어난 대장장이에게 가면 빛을 발할 수 있다", ""),
            new ItemClass("광물", "백철", 3, false, 1, 1500, 1, "하얀색 철광석. 뛰어난 대장장이에게 가면 빛을 발할 수 있다", ""),
            new ItemClass("광물", "수정덩이", 4, false, 1, 4000, 1, "가공되지 않은 결정체. 세공을 해야 진정한 가치를 알 수 있다", ""),

            new ItemClass("음식", "달콤달콤 닭고기 스튜", 1, false, 1, 1500, 1, "꿀에 버무려 구운 새고기. 탱탱한 새고기에 넘쳐흐르는 육즙, 게다가 아삭아삭한 달콤달콤꽃까지 정말 맛있다", ""),
            new ItemClass("음식", "무스프", 1, false, 1, 700, 1, "무를 주재료로 만든 야채수프. 여유로운 오후처럼 싱그럽고 소박한 농촌의 향기를 풍긴다.", ""),

            new ItemClass("기타", "열쇠", 1, false, 1,100,1,"문을 열기 위한 열쇠", "")

        };
        WeaponAndEquipmentDataList = new List<WeaponAndEquipCls>
        {
            new WeaponAndEquipCls("무기", "천공의 검", 5, false, 1, 100000, 1,20, "풍룡의 영광을 상징하는 기사검.\n잃어버렸다가 오늘날 되찾았다.\n현재 검에 바람 신의 축복이 깃들어 있으며, 푸른 하늘과 바람의 힘을 지니고 있다","" ,"치명타 확률이 <color=#00FFFF>4%</color> 증가한다. 원소폭발 발동 시 파공의 기세를 획득한다: 이동 속도 +10%, 공격 속도 +10% 일반 공격과 강공격이 명중 시 추가로 공격력 <color=#00FFFF>20%</color>의 피해를 준다. 지속 시간: 12초",1,46,12,0,700),
            new WeaponAndEquipCls("무기", "제례검", 4, false, 1, 65000, 1,20, "기나긴 세월을 거쳐 석화한 검은 의례적인 장식이 여전히 선명하게 보인다.\n시간의 바람에 씻긴 축복의 힘을 보유하고 있다", "","원소 전투 스킬로 피해를 줄 때 <color=#00FFFF>40%</color>의 확률로 해당 스킬의 재발동 대기시간이 초기화된다. 해당 효과는 <color=#00FFFF>30초</color>마다 1번 발동한다",1,41,13.3f,0,600),
            new WeaponAndEquipCls("무기", "여명신검", 3, false, 1, 30000, 1,20, "오래전 아침 햇살처럼 빛나던 보검. 이 검을 가진 자는 근거 없는 자신감에 가득 차게 된다. 검신의 빛나던 발광 재료는 이미 사라졌다", "","HP가 90% 초과 시 치명타 확률이 <color=#00FFFF>14%</color> 증가한다.",1,39,10f,0,500),

            new WeaponAndEquipCls("성배", "이국의 술잔", 3, false, 1, 9000, 1,20, "한때 이 소박한 백자 술잔엔 기쁨의 술이 가득 차 있었다", "행자의 마음","",1,40,15,0,200),
            new WeaponAndEquipCls("깃털", "귀향의 깃털", 3, false, 1, 10000, 1,20, "푸른색 화살 깃 위에 나그네의 저 멀리 떠나가는 미련이 서려 있다", "행자의 마음","",1,400,9,0,200),
            new WeaponAndEquipCls("왕관", "이별의 모자", 3, false, 1, 8000, 1,20, "봄바람의 기운을 발산하는 버드나무 왕관", "행자의 마음","",1,25,5,0,200),
            new WeaponAndEquipCls("꽃", "옛 벗의 마음", 3, false, 1, 7000, 1, 20, "푸른빛의 작은 꽃. 꽃줄기에 오래된 누군가의 리본이 묶여있다", "행자의 마음", "", 1,555,15,0,200),
            new WeaponAndEquipCls("모래", "빛을 좆는 돌", 3, false, 1, 8000, 1, 20, "산전수전 다 겪은 돌시계는 언제나 고요 속에서 일월순환을 기록한다", "행자의 마음", "", 1,30,10,0,200),

            new WeaponAndEquipCls("성배", "전투광의 해골잔", 4, true, 1, 30000, 1,20, "이름 모를 거대한 짐승의 뼈로 만든 컵이다.\n사냥으로 얻은 전리품이다", "전투광","",1,46,20,0,300),
            new WeaponAndEquipCls("깃털", "전투광의 깃털", 4, false, 1, 32000, 1,20, "전설 속의 초상을 알리던 새의 검푸른 꽁지 깃털이다.\n일부분이 붉은색으로 변해버렸다", "전투광","",1,450,12,0,300),
            new WeaponAndEquipCls("왕관", "전투광의 귀면", 4, false, 1, 28000, 1, 20, "생사를 가리지 않는 싸움에서 반쯤 부서진 흉측한 강철 가면", "전투광", "", 1,30,9,0,300),
            new WeaponAndEquipCls("꽃", "전투광의 장미", 4, false, 1, 25000, 1, 20, "핏빛의 정교한 꽃송이는 광전사의 기질과 미묘하게 어울린다", "전투광", "", 1,600,18,0,300),
            new WeaponAndEquipCls("모래", "전투광의 시계", 4, false, 1, 28000, 1, 20, "영원히 무정하게 흐르는 기물(器物)\n광전사에게 전쟁터, 그리고 인간 세상에서의 시간이 얼마 남지 않았음을 일깨워준다", "전투광", "", 1,33,17,0,300),

            new WeaponAndEquipCls("성배", "피에 물든 기사의 술잔", 5, false, 1, 60000, 1,20, "핏빛 기사가 지닌 어두운 금속 잔.\n겉은 검은 연기와 굳어버린 피로 인해 밤처럼 새까맣다", "피에 물든 기사도","",1,50,25,0,400),
            new WeaponAndEquipCls("깃털", "피에 물든 검은 깃털", 5, false, 1, 67000, 1, 20, "기사의 망토에 붙어 있던 까마귀 깃털.\n검은 피에 반복적으로 물들어 완전히 검은색으로 변했다", "피에 물든 기사도", "", 1,510,18,0,400),
            new WeaponAndEquipCls("왕관", "피에 물든 철가면", 5, true, 1, 55000, 1, 20, "기사가 자신의 얼굴을 가릴 때 사용하던 철가면.\n가면 아래의 얼굴은 수많은 사람들이 상상의 나래를 펼치게 했다", "피에 물든 기사도", "", 1,36,10,0,400),
            new WeaponAndEquipCls("꽃", "피에 물든 강철 심장", 5, false, 1, 51000, 1, 20, "피에 검게 물들어 강철과 같은 강도를 가지게 될 정도로 말라버린 꽃.\n과거 이 꽃의 주인에겐 일종의 기념품이지 않았을까", "피에 물든 기사도", "", 1,660,18,0,400),
            new WeaponAndEquipCls("모래", "피에 물든 기사의 시계", 5, false, 1, 55000, 1, 20, "기사가 과거에 사용했던 시계. 안의 액체가 모두 굳어 시계의 기능을 상실했다", "피에 물든 기사도", "", 1,35,20,0,400)

        };
        #endregion

        #region 무기/성유물 스텟 DB 테이블 채우기
        // DB채우기
        list_WeaponAndEquipData = new List<Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>>()
        {
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("천공의 검", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 46, 12,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 51, 12,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 56, 12,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 61, 12,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 66, 12,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 71, 12,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 76, 12,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 81, 12,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 86, 12,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 93, 12,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 98, 16.5f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 103, 16.5f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 108, 16.5f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(14, 113, 16.5f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(15, 118, 16.5f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(16, 123, 16.5f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(17, 133, 16.5f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(18, 138, 16.5f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(19, 143, 16.5f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(20, 153, 21.2f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(21, 163, 21.2f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(22, 168, 21.2f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(23, 173, 21.2f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(24, 178, 21.2f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(25, 183, 21.2f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(26, 188, 21.2f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(27, 193, 21.2f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(28, 198, 21.2f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(29, 203, 21.2f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(30, 216, 21.2f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(31, 221, 26.1f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(32, 226, 26.1f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(33, 231, 26.1f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(34, 236, 26.1f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(35, 241, 26.1f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(36, 246, 26.1f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(37, 251, 26.1f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(38, 256, 26.1f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(39, 261, 26.1f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(40, 266, 30.9f,40),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("제례검", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 41, 13.3f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 45, 13.3f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 49, 13.3f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 53, 13.3f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 57, 13.3f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 61, 13.3f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 65, 13.3f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 69, 13.3f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 73, 13.3f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 85, 18.1f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 89, 18.1f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 93, 18.1f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 97, 18.1f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(14, 101, 18.1f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(15, 105, 18.1f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(16, 109, 18.1f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(17, 113, 18.1f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(18, 117, 18.1f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(19, 121, 18.1f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(20, 125, 23.6f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(21, 129, 23.6f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(22, 133, 23.6f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(23, 137, 23.6f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(24, 141, 23.6f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(25, 145, 23.6f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(26, 149, 23.6f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(27, 153, 23.6f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(28, 157, 23.6f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(29, 161, 23.6f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(30, 165, 28.3f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(31, 172, 28.3f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(32, 178, 28.3f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(33, 182, 28.3f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(34, 186, 28.3f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(35, 190, 28.3f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(36, 194, 28.3f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(37, 198, 28.3f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(38, 202, 28.3f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(39, 206, 28.3f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(40, 210, 34.3f,40),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("여명신검", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 39, 10.2f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 42, 10.2f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 45, 10.2f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 48, 10.2f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 51, 10.2f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 54, 10.2f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 57, 10.2f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 60, 10.2f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 63, 10.2f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 67, 14.0f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 81, 14.0f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 84, 14.0f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 87, 14.0f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(14, 90, 14.0f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(15, 93, 14.0f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(16, 101, 14.0f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(17, 104, 14.0f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(18, 107, 14.0f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(19, 110, 14.0f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(20, 113, 18.0f,20),
                new WEAPON_EQUIP_STATE_DATA_BASE(21, 117, 18.0f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(22, 121, 18.0f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(23, 124, 18.0f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(24, 127, 18.0f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(25, 131, 18.0f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(26, 134, 18.0f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(27, 138, 18.0f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(28, 141, 18.0f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(29, 145, 18.0f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(30, 148, 22.0f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(31, 153, 22.0f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(32, 156, 22.0f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(33, 160, 22.0f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(34, 163, 22.0f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(35, 167, 22.0f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(36, 174, 22.0f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(37, 178, 22.0f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(38, 172, 22.0f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(39, 185, 22.0f,40),
                new WEAPON_EQUIP_STATE_DATA_BASE(40, 189, 26.3f,40),
            }),
            // 성유물 _ 레벨: 성유물 능력치 티어, 주능력치:주능력치, 서브 능력치:-
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("3성_꽃", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 430, 0,   12),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 552, 0,   12),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 674, 0,   12),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 796, 0,   12),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 918, 0,   12),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 1040, 0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 1162, 0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 1283, 0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 1405, 0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 1527, 0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 1649, 0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 1771, 0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 1893, 0, 12),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("4성_꽃", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 645, 0,   16),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 828, 0,   16),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 1011, 0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 1194, 0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 1377, 0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 1559, 0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 1742, 0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 1925, 0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 2108, 0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 2291, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 2474, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 2657, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 2839, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(14, 3022, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(15, 3388, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(16, 3571, 0, 16),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("5성_꽃", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 717,  0,  20),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 920,  0,  20),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 1123, 0,  20),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 1326, 0,  20),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 1530, 0,  20),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 1733, 0,  20),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 1936, 0,  20),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 2139, 0,  20),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 2342, 0,  20),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 2545, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 2749, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 2952, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 3155, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(14, 3358, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(15, 3561, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(16, 3764, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(17, 3967, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(18, 4171, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(19, 4374, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(20, 4780, 0, 20),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("3성_깃털", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 28,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 36,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 44,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 52,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 60,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 68,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 76,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 84,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 91,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 99,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 107, 0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 115, 0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 123, 0, 12),

            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("4성_깃털", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 42,  0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 54,  0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 66,  0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 78,  0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 90,  0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 102,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 113,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 125,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 137,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 149, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 161, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 173, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 185, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(14, 197, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(15, 209, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(16, 221, 0, 16),

            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("5성_깃털", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 47,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 60,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 73,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 86,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 100,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 113,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 126,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 139,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 152,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 166, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 179, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 192, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 205, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(14, 219, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(15, 232, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(16, 245, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(17, 258, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(18, 272, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(19, 285, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(20, 311, 0, 20),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("3성_성유물_체력", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 5.2f,   0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 6.7f,   0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 8.2f,   0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 9.7f,   0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 11.2f,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 12.7f,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 14.2f,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 15.6f,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 17.1f,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 18.6f, 0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 20.1f, 0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 21.6f, 0, 12),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("3성_성유물_공격력", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 5.2f,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 6.7f,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 8.2f,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 9.7f,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 11.2f, 0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 12.7f, 0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 14.2f, 0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 15.6f, 0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 17.1f, 0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 18.6f, 0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 20.1f, 0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 21.6f, 0, 12),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("3성_성유물_방어력", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 6.6f,   0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 8.4f,   0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 10.3f,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 12.1f,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 14.0f,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 15.8f,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 17.7f,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 19.6f,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 21.4f,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 23.3f, 0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 25.1f, 0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 27.0f, 0, 12),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("3성_성유물_원소 마스터리", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 21f,    0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 26.9f,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 32.9f,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 38.8f,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 44.8f,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 50.7f,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 56.7f,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 62.6f,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 68.5f,  0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 74.5f, 0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 80.4f, 0, 12),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 86.4f, 0, 12),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("4성_성유물_체력", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 6.3f,   0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 8.1f,   0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 9.9f,   0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 11.6f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 13.4f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 15.2f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 17f,    0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 18.8f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 20.6f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 22.3f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 24.1f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 25.9f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 27.7f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(14, 29.5f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(15, 31.3f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(16, 33f,   0, 16),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("4성_성유물_공격력", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 6.3f,  0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 8.1f,  0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 9.9f,  0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 11.6f, 0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 13.4f, 0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 15.2f, 0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 17f,   0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 18.8f, 0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 20.6f, 0,  16),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 22.3f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 24.1f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 25.9f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 27.7f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(14, 29.5f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(15, 31.3f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(16, 33f,   0, 16),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("4성_성유물_방어력", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 7.9f,   0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 10.1f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 12.3f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 14.6f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 16.8f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 19f,    0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 21.2f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 23.5f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 25.7f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 27.9f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 30.2f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 32.4f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 34.6f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(14, 26.8f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(15, 39.1f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(16, 41.3f, 0, 16),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("4성_성유물_원소 마스터리", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 25.2f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 32.3f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 39.4f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 46.6f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 53.7f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 60.8f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 68f,    0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 75.1f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 82.2f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 89.4f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 96.5f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 103.6f,0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 110.8f,0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(14, 117.9f,0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(15, 125f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(16, 132.2f,0, 16),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("5성_성유물_체력", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 7f,     0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 9.0f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 11f,    0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 12.9f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 14.9f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 16.9f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 18.9f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 20.9f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 22.8f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 24.8f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 26.8f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 28.8f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 30.8f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(14, 32.8f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(15, 34.7f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(16, 36.7f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(17, 38.7f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(18, 40.7f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(19, 42.7f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(20, 46.8f, 0, 20),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("5성_성유물_공격력", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 7f,     0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 9.0f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 11f,    0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 12.9f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 14.9f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 16.9f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 18.9f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 20.9f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 22.8f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 24.8f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 26.8f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 28.8f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 30.8f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(14, 32.8f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(15, 34.7f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(16, 36.7f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(17, 38.7f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(18, 40.7f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(19, 42.7f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(20, 46.8f, 0, 20),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("5성_성유물_방어력", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 8.7f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 11.2f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 13.7f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 16.2f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 18.6f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 21.1f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 23.6f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 26.1f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 28.6f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 31f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 33.5f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 36f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 38.5f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(14, 40.9f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(15, 43.4f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(16, 45.9f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(17, 48.4f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(18, 50.8f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(19, 53.3f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(20, 58.3f, 0, 20),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("5성_성유물_원소 마스터리", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 28f,     0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 35.9f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 43.8f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 51.8f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 59.7f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 67.6f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 75.5f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 83.5f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 91.4f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 99.3f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 107.2f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 115.2f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 123.1f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(14, 131f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(15, 138.9f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(16, 146.9f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(17, 154.8f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(18, 162.7f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(19, 170.6f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(20, 186.5f, 0, 20),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("3성_성유물_치명타 확률", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 3.5f,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 4.5f,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 5.5f,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 6.5f,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 7.5f,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 8.4f,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 9.4f,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 10.4f, 0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 11.4f, 0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 12.4f,0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 13.4f,0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 14.4f,0,  12),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("3성_성유물_치명타 피해", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 7f,     0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 9f,     0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 11f,    0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 12.9f,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 14.9f,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 16.9f,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 18.9f,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 20.9f,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 22.8f,  0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 24.8f, 0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 26.8f, 0,  12),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 28.8f, 0,  12),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("4성_성유물_치명타 확률", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 4.2f,   0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 5.4f,   0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 6.6f,   0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 7.8f,   0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 9f,     0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 10.1f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 11.3f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 12.5f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 13.7f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 14.9f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 16.1f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 17.3f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 18.5f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(14, 19.7f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(15, 20.8f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(16, 22f,   0, 16),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("4성_성유물_치명타 피해", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 8.4f,   0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 10.8f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 13.1f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 15.5f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 17.9f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 20.3f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 22.7f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 25f,    0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 27.4f,  0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 29.8f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 32.2f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 34.5f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 36.9f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(14, 39.3f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(15, 41.7f, 0, 16),
                new WEAPON_EQUIP_STATE_DATA_BASE(16, 44.1f, 0, 16),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("5성_성유물_치명타 확률", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 4.7f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 6f,     0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 7.3f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 8.6f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 9.9f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 11.3f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 12.6f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 13.9f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 15.2f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 16.6f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 17.9f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 19.2f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 20.5f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(14, 21.8f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(15, 23.2f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(16, 24.5f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(17, 25.8f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(18, 27.1f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(19, 28.4f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(20, 31.1f, 0, 20),
            }),
            new Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>("5성_성유물_치명타 피해", new List<WEAPON_EQUIP_STATE_DATA_BASE>
            {
                new WEAPON_EQUIP_STATE_DATA_BASE(1, 9.3f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(2, 12f,    0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(3, 14.6f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(4, 17.3f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(5, 19.9f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(6, 22.5f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(7, 25.2f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(8, 27.8f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(9, 30.5f,  0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(10, 33.1f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(11, 35.7f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(12, 38.4f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(13, 41f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(14, 43.7f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(15, 46.3f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(16, 49f,   0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(17, 51.6f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(18, 54.2f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(19, 56.9f, 0, 20),
                new WEAPON_EQUIP_STATE_DATA_BASE(20, 62.2f, 0, 20),
            }),
            
        };
        #endregion

        #region 무기/성유물 돌파 재료 DB 채우기
        list_WeaponAndEquipLimitBreakResourceData = new List<Tuple<string, List<WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA>>>()
        {
            new Tuple<string, List<WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA>>("천공의 검", new List<WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA>
            {
                new WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA(ItemDataList.Find(tmp=>tmp.GetName()=="칼바람 울프의 젖니"),5,20),
                new WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA(ItemDataList.Find(tmp=>tmp.GetName()=="지맥의 낡은 가지"),5,20),
                new WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA(ItemDataList.Find(tmp=>tmp.GetName()=="슬라임 응축액"),3,20),
                new WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA(ItemDataList.Find(tmp=>tmp.GetName()=="칼바람 울프의 이빨"),5,40),
                new WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA(ItemDataList.Find(tmp=>tmp.GetName()=="지맥의 마른 잎"),12,40),
                new WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA(ItemDataList.Find(tmp=>tmp.GetName()=="슬라임 응축액"),18,40),
            }),
            new Tuple<string, List<WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA>>("제례검", new List<WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA>
            {
                new WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA(ItemDataList.Find(tmp=>tmp.GetName()=="라이언 투사의 족쇄"),3,20),
                new WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA(ItemDataList.Find(tmp=>tmp.GetName()=="혼돈의 장치"),3,20),
                new WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA(ItemDataList.Find(tmp=>tmp.GetName()=="이능 두루마리"),3,20),
                new WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA(ItemDataList.Find(tmp=>tmp.GetName()=="라이언 투사의 쇠사슬"),3,40),
                new WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA(ItemDataList.Find(tmp=>tmp.GetName()=="혼돈의 회로"),8,40),
                new WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA(ItemDataList.Find(tmp=>tmp.GetName()=="이능 두루마리"),12,40),
            }),
            new Tuple<string, List<WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA>>("여명신검", new List<WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA>
            {
                new WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA(ItemDataList.Find(tmp=>tmp.GetName()=="칼바람 울프의 젖니"),2,20),
                new WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA(ItemDataList.Find(tmp=>tmp.GetName()=="지맥의 낡은 가지"),2,20),
                new WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA(ItemDataList.Find(tmp=>tmp.GetName()=="슬라임 응축액"),1,20),
                new WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA(ItemDataList.Find(tmp=>tmp.GetName()=="칼바람 울프의 이빨"),2,40),
                new WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA(ItemDataList.Find(tmp=>tmp.GetName()=="지맥의 마른 잎"),5,40),
                new WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA(ItemDataList.Find(tmp=>tmp.GetName()=="슬라임 응축액"),8,40),
            }),
        };
        #endregion

        #region 무기/성유물 재련 DB 채우기
        list_WeaponAndEquipReforgeGradeData = new List<Tuple<string, List<WEAPON_EQUIP_EFFECT_DATA_BASE>>>()
        {
            new Tuple<string, List<WEAPON_EQUIP_EFFECT_DATA_BASE>>("천공의 검", new List<WEAPON_EQUIP_EFFECT_DATA_BASE>
            {
                new WEAPON_EQUIP_EFFECT_DATA_BASE(1,"치명타 확률이 <color=#00FFFF>4%</color> 증가한다. 원소폭발 발동 시 파공의 기세를 획득한다: 이동 속도 +10%, 공격 속도 +10% 일반 공격과 강공격이 명중 시 추가로 공격력 <color=#00FFFF>20%</color>의 피해를 준다. 지속 시간: 12초"),
                new WEAPON_EQUIP_EFFECT_DATA_BASE(2,"치명타 확률이 <color=#00FFFF>5%</color> 증가한다. 원소폭발 발동 시 파공의 기세를 획득한다: 이동 속도 +10%, 공격 속도 +10% 일반 공격과 강공격이 명중 시 추가로 공격력 <color=#00FFFF>25%</color>의 피해를 준다. 지속 시간: 12초"),
                new WEAPON_EQUIP_EFFECT_DATA_BASE(3,"치명타 확률이 <color=#00FFFF>6%</color> 증가한다. 원소폭발 발동 시 파공의 기세를 획득한다: 이동 속도 +10%, 공격 속도 +10% 일반 공격과 강공격이 명중 시 추가로 공격력 <color=#00FFFF>30%</color>의 피해를 준다. 지속 시간: 12초"),
                new WEAPON_EQUIP_EFFECT_DATA_BASE(4,"치명타 확률이 <color=#00FFFF>7%</color> 증가한다. 원소폭발 발동 시 파공의 기세를 획득한다: 이동 속도 +10%, 공격 속도 +10% 일반 공격과 강공격이 명중 시 추가로 공격력 <color=#00FFFF>35%</color>의 피해를 준다. 지속 시간: 12초"),
                new WEAPON_EQUIP_EFFECT_DATA_BASE(5,"치명타 확률이 <color=#00FFFF>8%</color> 증가한다. 원소폭발 발동 시 파공의 기세를 획득한다: 이동 속도 +10%, 공격 속도 +10% 일반 공격과 강공격이 명중 시 추가로 공격력 <color=#00FFFF>40%</color>의 피해를 준다. 지속 시간: 12초")
            }),
            new Tuple<string, List<WEAPON_EQUIP_EFFECT_DATA_BASE>>("제례검", new List<WEAPON_EQUIP_EFFECT_DATA_BASE>
            {
                new WEAPON_EQUIP_EFFECT_DATA_BASE(1,"원소 전투 스킬로 피해를 줄 때 <color=#00FFFF>40%</color>의 확률로 해당 스킬의 재발동 대기시간이 초기화된다. 해당 효과는 <color=#00FFFF>30초</color>마다 1번 발동한다"),
                new WEAPON_EQUIP_EFFECT_DATA_BASE(2,"원소 전투 스킬로 피해를 줄 때 <color=#00FFFF>50%</color>의 확률로 해당 스킬의 재발동 대기시간이 초기화된다. 해당 효과는 <color=#00FFFF>26초</color>마다 1번 발동한다"),
                new WEAPON_EQUIP_EFFECT_DATA_BASE(3,"원소 전투 스킬로 피해를 줄 때 <color=#00FFFF>60%</color>의 확률로 해당 스킬의 재발동 대기시간이 초기화된다. 해당 효과는 <color=#00FFFF>22초</color>마다 1번 발동한다"),
                new WEAPON_EQUIP_EFFECT_DATA_BASE(4,"원소 전투 스킬로 피해를 줄 때 <color=#00FFFF>70%</color>의 확률로 해당 스킬의 재발동 대기시간이 초기화된다. 해당 효과는 <color=#00FFFF>19초</color>마다 1번 발동한다"),
                new WEAPON_EQUIP_EFFECT_DATA_BASE(5,"원소 전투 스킬로 피해를 줄 때 <color=#00FFFF>80%</color>의 확률로 해당 스킬의 재발동 대기시간이 초기화된다. 해당 효과는 <color=#00FFFF>16초</color>마다 1번 발동한다")
            }),
            new Tuple<string, List<WEAPON_EQUIP_EFFECT_DATA_BASE>>("여명신검",new List<WEAPON_EQUIP_EFFECT_DATA_BASE>
            {
                new WEAPON_EQUIP_EFFECT_DATA_BASE(1,"HP가 90% 초과 시 치명타 확률이 <color=#00FFFF>14%</color> 증가한다."),
                new WEAPON_EQUIP_EFFECT_DATA_BASE(2,"HP가 90% 초과 시 치명타 확률이 <color=#00FFFF>17.5%</color> 증가한다."),
                new WEAPON_EQUIP_EFFECT_DATA_BASE(3,"HP가 90% 초과 시 치명타 확률이 <color=#00FFFF>21%</color> 증가한다."),
                new WEAPON_EQUIP_EFFECT_DATA_BASE(4,"HP가 90% 초과 시 치명타 확률이 <color=#00FFFF>24.5%</color> 증가한다."),
                new WEAPON_EQUIP_EFFECT_DATA_BASE(5,"HP가 90% 초과 시 치명타 확률이 <color=#00FFFF>28%</color> 증가한다.")
            })
        };
        #endregion

        #region 성유물 세트효과 DB 채우기
        list_EquipmentSetSynergyData = new List<EQUIPMENT_SET_SYNERGY_DATA_BASE>()
        {
            new EQUIPMENT_SET_SYNERGY_DATA_BASE("행자의 마음",2,"공격력+18%","공격력",18),
            new EQUIPMENT_SET_SYNERGY_DATA_BASE("행자의 마음",4,"HP가 50%를 초과하는 적에게 가하는 피해가 30% 증가한다","피해량",30),
            new EQUIPMENT_SET_SYNERGY_DATA_BASE("전투광",2,"치명타 확률+12%","치명타 확률",12),
            new EQUIPMENT_SET_SYNERGY_DATA_BASE("전투광",4,"HP 70% 미만 시 치명타 확률이 추가로 24% 증가한다","치명타 확률",24),
            new EQUIPMENT_SET_SYNERGY_DATA_BASE("피에 물든 기사도",2,"공격력+18%","공격력",18),
            new EQUIPMENT_SET_SYNERGY_DATA_BASE("피에 물든 기사도",4,"해당 성유물 세트를 장착한 캐릭터가 한손검, 양손검, 장병기를 사용 시 캐릭터의 일반 공격으로 가하는 피해가 35% 증가한다","일반 공격",35),
        };
        #endregion

        #region 합성 DB 채우기
        list_WeaponSynthesisData = new List<SYNTHESIS_DATA_BASE>()
        {
            new SYNTHESIS_DATA_BASE("","철광석",0,40,"여명신검"),
            new SYNTHESIS_DATA_BASE("여명신검","백철",3,15,"제례검"),
            new SYNTHESIS_DATA_BASE("제례검","수정덩이",3,15,"천공의 검"),
        };
        list_EquipSynthesisData = new List<SYNTHESIS_DATA_BASE>()
        {
            new SYNTHESIS_DATA_BASE("","철광석",0,25,"이국의 술잔"),
            new SYNTHESIS_DATA_BASE("","철광석",0,25,"귀향의 깃털"),
            new SYNTHESIS_DATA_BASE("","철광석",0,25,"이별의 모자"),
            new SYNTHESIS_DATA_BASE("","철광석",0,25,"옛 벗의 마음"),
            new SYNTHESIS_DATA_BASE("","철광석",0,25,"빛을 좆는 돌"),
            new SYNTHESIS_DATA_BASE("이국의 술잔", "백철",3,10,"전투광의 해골잔"),
            new SYNTHESIS_DATA_BASE("귀향의 깃털", "백철",3,10,"전투광의 깃털"),
            new SYNTHESIS_DATA_BASE("이별의 모자", "백철",3,10,"전투광의 귀면"),
            new SYNTHESIS_DATA_BASE("옛 벗의 마음", "백철",3,10,"전투광의 장미"),
            new SYNTHESIS_DATA_BASE("빛을 좆는 돌","백철",3,10,"전투광의 시계"),
            new SYNTHESIS_DATA_BASE("전투광의 해골잔","수정덩이",3,10,"피에 물든 기사의 술잔"),
            new SYNTHESIS_DATA_BASE("전투광의 깃털", "수정덩이",3,10,"피에 물든 검은 깃털"),
            new SYNTHESIS_DATA_BASE("전투광의 귀면","수정덩이",3,10,"피에 물든 철가면"),
            new SYNTHESIS_DATA_BASE("전투광의 장미","수정덩이",3,10,"피에 물든 강철 심장"),
            new SYNTHESIS_DATA_BASE("전투광의 시계","수정덩이",3,10,"피에 물든 기사의 시계")
        };
        list_EtcSynthesisData = new List<SYNTHESIS_DATA_BASE>()
        {
            new SYNTHESIS_DATA_BASE("칼바람 울프의 젖니","모라",5,4000,"칼바람 울프의 이빨"),
            new SYNTHESIS_DATA_BASE("칼바람 울프의 이빨","모라",5,10000,"칼바람 울프의 부서진 이빨"),
            new SYNTHESIS_DATA_BASE("라이언 투사의 족쇄","모라",5,4000,"라이언 투사의 쇠사슬"),
            new SYNTHESIS_DATA_BASE("라이언 투사의 쇠사슬","모라",5,10000,"라이언 투사의 수갑"),
            new SYNTHESIS_DATA_BASE("지맥의 낡은 가지","모라",5,2000,"지맥의 마른 잎"),
            new SYNTHESIS_DATA_BASE("지맥의 마른 잎","모라",5,5000,"지맥의 새싹"),
            new SYNTHESIS_DATA_BASE("슬라임 응축액","모라",5,2000,"슬라임청"),
            new SYNTHESIS_DATA_BASE("슬라임청","모라",5,5000,"슬라임 원액"),
            new SYNTHESIS_DATA_BASE("혼돈의 장치","모라",5,2000,"혼돈의 회로"),
            new SYNTHESIS_DATA_BASE("혼돈의 회로","모라",5,5000,"혼돈의 노심"),
            new SYNTHESIS_DATA_BASE("이능 두루마리","모라",5,2000,"봉마의 두루마리"),
            new SYNTHESIS_DATA_BASE("봉마의 두루마리","모라",5,2000,"금주의 두루마리"),
        };
        #endregion

        #region 몬스터 DB 채우기
        arr_MonsterData = new Monster[]
        {
            //0
            new Monster("몬스터", "Cactus", 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Precedence,
                1100, 1100, 10, 15, 15, 1.8f, 100f, Element.e_Element.None, 1.5f, 20),
            //1
            new Monster("몬스터", "Cactus", 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Precedence,
                1100, 1100, 10, 15, 15, 1.8f, 100f, Element.e_Element.None, 1.5f, 20),
            //2
            new Monster("몬스터", "Cactus", 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Precedence,
                1100, 1100, 10, 15, 15, 1.8f, 100f, Element.e_Element.None, 1.5f, 20),
            //3
            new Monster("몬스터", "MushroomAngry", 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Precedence,
                1000, 1000, 10, 25, 15, 1.8f, 100f, Element.e_Element.None, 1.5f, 20),
            //4
            new Monster("몬스터", "MushroomAngry", 1, true, Monster.e_MonsterState.None, Monster.e_MonsterType.Precedence,
                1000, 1000, 10, 25, 15, 1.8f, 100f, Element.e_Element.None, 1.5f, 20),
        };
        #endregion


        // 유저 게임 데이터 초기화
        characterCls = new CharacterClass(300, 300, 0, 100, 50,20, 1, 20, 3.0f, CharacterClass.eCharactgerState.e_NONE,50,120,50,2.8f,"플레이어","Knight",0,true, 100,20,0,0,0);
        playerData.SetUserCharacter(characterCls);
        playerData.SetMora(156000);
        List<ItemClass> weaponList = new List<ItemClass>
        {
            new WeaponAndEquipCls("무기", "천공의 검", 5, true, 1, 100000, 19,20, "풍룡의 영광을 상징하는 기사검.\n잃어버렸다가 오늘날 되찾았다.\n현재 검에 바람 신의 축복이 깃들어 있으며, 푸른 하늘과 바람의 힘을 지니고 있다","" ,"치명타 확률이 <color=#00FFFF>4%</color> 증가한다. 원소폭발 발동 시 파공의 기세를 획득한다: 이동 속도 +10%, 공격 속도 +10% 일반 공격과 강공격이 명중 시 추가로 공격력 <color=#00FFFF>20%</color>의 피해를 준다. 지속 시간: 12초",1,560,12,900,1000),
            new WeaponAndEquipCls("무기", "제례검", 4, false, 1, 65000, 3,20, "기나긴 세월을 거쳐 석화한 검은 의례적인 장식이 여전히 선명하게 보인다.\n시간의 바람에 씻긴 축복의 힘을 보유하고 있다", "","원소 전투 스킬로 피해를 줄 때 <color=#00FFFF>40%</color>의 확률로 해당 스킬의 재발동 대기시간이 초기화된다. 해당 효과는 <color=#00FFFF>30초</color>마다 1번 발동한다",1,470,13.3f,416,800),
            new WeaponAndEquipCls("무기", "제례검", 4, false, 1, 65000, 1,20, "기나긴 세월을 거쳐 석화한 검은 의례적인 장식이 여전히 선명하게 보인다.\n시간의 바람에 씻긴 축복의 힘을 보유하고 있다", "","원소 전투 스킬로 피해를 줄 때 <color=#00FFFF>40%</color>의 확률로 해당 스킬의 재발동 대기시간이 초기화된다. 해당 효과는 <color=#00FFFF>30초</color>마다 1번 발동한다",1,470,13.3f,100,600),
            new WeaponAndEquipCls("무기", "제례검", 4, false, 1, 65000, 5,20, "기나긴 세월을 거쳐 석화한 검은 의례적인 장식이 여전히 선명하게 보인다.\n시간의 바람에 씻긴 축복의 힘을 보유하고 있다", "","원소 전투 스킬로 피해를 줄 때 <color=#00FFFF>40%</color>의 확률로 해당 스킬의 재발동 대기시간이 초기화된다. 해당 효과는 <color=#00FFFF>30초</color>마다 1번 발동한다",1,470,13.3f,650,1200),
            new WeaponAndEquipCls("무기", "여명신검", 3, false, 1, 30000, 10,20, "오래전 아침 햇살처럼 빛나던 보검. 이 검을 가진 자는 근거 없는 자신감에 가득 차게 된다. 검신의 빛나던 발광 재료는 이미 사라졌다", "","HP가 90% 초과 시 치명타 확률이 <color=#00FFFF>14%</color> 증가한다.",1,266,10f,990,1208)
        };
        playerData.SetHadWeaponList(weaponList);
        Item_Id_Generator(weaponList);
        WeaponItemStatusSet(weaponList);

        List<ItemClass> equipList = new List<ItemClass>
        {
            new WeaponAndEquipCls("성배", "이국의 술잔", 3, false, 1, 9000, 4,20, "한때 이 소박한 백자 술잔엔 기쁨의 술이 가득 차 있었다", "행자의 마음","",1,40,15,150,400),
            new WeaponAndEquipCls("깃털", "귀향의 깃털", 3, false, 1, 10000, 1,20, "푸른색 화살 깃 위에 나그네의 저 멀리 떠나가는 미련이 서려 있다", "행자의 마음","",1,400,9,100,400),
            new WeaponAndEquipCls("왕관", "이별의 모자", 3, false, 1, 8000, 3,20, "봄바람의 기운을 발산하는 버드나무 왕관", "행자의 마음","",1,25,5,500,1000),
            new WeaponAndEquipCls("꽃", "옛 벗의 마음", 3, false, 1, 7000, 1, 20, "푸른빛의 작은 꽃. 꽃줄기에 오래된 누군가의 리본이 묶여있다", "행자의 마음", "", 1,555,15,150,400),
            new WeaponAndEquipCls("모래", "빛을 좆는 돌", 3, false, 1, 8000, 1, 20, "산전수전 다 겪은 돌시계는 언제나 고요 속에서 일월순환을 기록한다", "행자의 마음", "", 1,30,10,390,400),
            new WeaponAndEquipCls("모래", "빛을 좆는 돌", 3, false, 1, 8000, 1, 20, "산전수전 다 겪은 돌시계는 언제나 고요 속에서 일월순환을 기록한다", "행자의 마음", "", 1,30,10,390,400),
            new WeaponAndEquipCls("모래", "빛을 좆는 돌", 3, false, 1, 8000, 1, 20, "산전수전 다 겪은 돌시계는 언제나 고요 속에서 일월순환을 기록한다", "행자의 마음", "", 1,30,10,390,400),
            new WeaponAndEquipCls("모래", "빛을 좆는 돌", 3, false, 1, 8000, 1, 20, "산전수전 다 겪은 돌시계는 언제나 고요 속에서 일월순환을 기록한다", "행자의 마음", "", 1,30,10,390,400),

            new WeaponAndEquipCls("성배", "전투광의 해골잔", 4, true, 1, 30000, 1,20, "이름 모를 거대한 짐승의 뼈로 만든 컵이다.\n사냥으로 얻은 전리품이다", "전투광","",1,46,20,200,400),
            new WeaponAndEquipCls("깃털", "전투광의 깃털", 4, false, 1, 32000, 2,20, "전설 속의 초상을 알리던 새의 검푸른 꽁지 깃털이다.\n일부분이 붉은색으로 변해버렸다", "전투광","",1,450,12,250,400),
            new WeaponAndEquipCls("왕관", "전투광의 귀면", 4, false, 1, 28000, 1, 20, "생사를 가리지 않는 싸움에서 반쯤 부서진 흉측한 강철 가면", "전투광", "", 1,30,9,0,400),
            new WeaponAndEquipCls("꽃", "전투광의 장미", 4, false, 1, 25000, 1, 20, "핏빛의 정교한 꽃송이는 광전사의 기질과 미묘하게 어울린다", "전투광", "", 1,600,18,0,400),
            new WeaponAndEquipCls("모래", "전투광의 시계", 4, false, 1, 28000, 5, 20, "영원히 무정하게 흐르는 기물(器物)\n광전사에게 전쟁터, 그리고 인간 세상에서의 시간이 얼마 남지 않았음을 일깨워준다", "전투광", "", 1,33,17,50,400),

            new WeaponAndEquipCls("성배", "피에 물든 기사의 술잔", 5, false, 1, 60000, 1,20, "핏빛 기사가 지닌 어두운 금속 잔.\n겉은 검은 연기와 굳어버린 피로 인해 밤처럼 새까맣다", "피에 물든 기사도","",1,50,25,100,400),
            new WeaponAndEquipCls("깃털", "피에 물든 검은 깃털", 5, false, 1, 67000, 2, 20, "기사의 망토에 붙어 있던 까마귀 깃털.\n검은 피에 반복적으로 물들어 완전히 검은색으로 변했다", "피에 물든 기사도", "", 1,510,18,100,400),
            new WeaponAndEquipCls("왕관", "피에 물든 철가면", 5, true, 1, 55000, 2, 20, "기사가 자신의 얼굴을 가릴 때 사용하던 철가면.\n가면 아래의 얼굴은 수많은 사람들이 상상의 나래를 펼치게 했다", "피에 물든 기사도", "", 1,36,10,90,400),
            new WeaponAndEquipCls("꽃", "피에 물든 강철 심장", 5, false, 1, 51000, 1, 20, "피에 검게 물들어 강철과 같은 강도를 가지게 될 정도로 말라버린 꽃.\n과거 이 꽃의 주인에겐 일종의 기념품이지 않았을까", "피에 물든 기사도", "", 1,660,18,160,400),
            new WeaponAndEquipCls("모래", "피에 물든 기사의 시계", 5, false, 1, 55000, 1, 20, "기사가 과거에 사용했던 시계. 안의 액체가 모두 굳어 시계의 기능을 상실했다", "피에 물든 기사도", "", 1,35,20,266,400)
        };
        EquipStatusRandomSelector(equipList);
        EquipItemStatusSet(equipList);
        Item_Id_Generator(equipList);
        playerData.SetHadEquipmentList(equipList);


        List<ItemClass> gemList = new List<ItemClass>
        {
            new ItemClass("광물", "철광석", 1, false, 1, 600, 1, "철광석. 뛰어난 대장장이에게 가면 빛을 발할 수 있다", ""),
            new ItemClass("광물", "백철", 3, false, 100, 1500, 1, "하얀색 철광석. 뛰어난 대장장이에게 가면 빛을 발할 수 있다", ""),
            new ItemClass("광물", "수정덩이", 4, false, 20, 4000, 1, "가공되지 않은 결정체. 세공을 해야 진정한 가치를 알 수 있다", ""),
        };
        Item_Id_Generator(gemList);
        playerData.SetHadGemList(gemList);

        List<ItemClass> foodList = new List<ItemClass>
        {            
            new ItemClass("음식", "달콤달콤 닭고기 스튜", 3, false, 1, 1500, 1, "꿀에 버무려 구운 새고기. 탱탱한 새고기에 넘쳐흐르는 육즙, 게다가 아삭아삭한 달콤달콤꽃까지 정말 맛있다", ""),
            new ItemClass("음식", "무스프", 1, false, 3, 700, 1, "무를 주재료로 만든 야채수프. 여유로운 오후처럼 싱그럽고 소박한 농촌의 향기를 풍긴다.", ""),
        };
        Item_Id_Generator(foodList);
        playerData.SetHadFoodList(foodList);

        List<ItemClass> growList = new List<ItemClass>
        {
            new ItemClass("육성 아이템","칼바람 울프의 젖니",1,false,2,500,0,"늑대 무리는 그의 축복을 받은 근위병이다. 새끼 늑대의 젖니라도 상당한 저력을 가지고 있다.\n과거의 신들은 인간을 사랑한다는 책임을 짊어지고 있었다. 따라서, 늑대 무리를 인솔하지만 버림받은 아이를 입양하고 방랑자들을 받아들인 「안드리우스」는 아주 이상하다",""),
            new ItemClass("육성 아이템","슬라임 응축액",1,false,20,500,0,"슬라임을 덮고 있는 걸쭉한 액체. 각지의 원소 공방에서 가장 흔히 보이는 원료이다",""),
            new ItemClass("육성 아이템","지맥의 낡은 가지",1,false,1,500,0,"지하 깊은 곳의 마른 나뭇가지. 오랜 세월이 지났지만 알록달록한 나무껍질에서 내재된 힘을 느낄 수 있다",""),
        };
        Item_Id_Generator(growList);
        playerData.SetHadGrowMaterialList(growList);

        playerData.SetUserEquippedWeapon(playerData.GetHadWeaponList().Find(item => item.GetIsActive() == true));
        playerData.SetUserEquippedEquipment(playerData.GetHadEquipmentList().FindAll(item => item.GetIsActive() == true).ToArray());


        CharacterDataReviseToWeapon();
        CharacterDataReviseToEquipment();
    }

    void Start()
    {
        ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Weapon);
        ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Equip);
        ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Gem);
        ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Food);

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
        int index = int.Parse(remainingCharacters);             // DB참조와 데이터 분류를 위한 인덱스 형변환
        // 탐색한 콜라이더 내의 몬스터 이름들을 저장하는 리스트 생성.
        Collider[] colliders = Physics.OverlapSphere(point.position, 20, 1 << LayerMask.NameToLayer("Monster"));

        string monsterName;

        // 분류한 번호에 따라, 분기하여 해당하는 몬스터를 생성.
        switch (index)
        {
            case 1:
            case 2:
            case 3:
                monsterName = "Cactus";
                for (int i = 0; i < 2; i++)
                {
                    Vector3 spawnPosition = point.position + new Vector3(i * 2, 0, 0);
                    SpawnMonster(index,colliders, monsterName, spawnPosition);
                }
                break;
            case 4:
            case 5:
                monsterName = "MushroomAngry";
                for (int i = 0; i < 2; i++)
                {
                    Vector3 spawnPosition = point.position + new Vector3(i * 2, 0, 0);
                    SpawnMonster(index,colliders, monsterName, spawnPosition);
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
    private void SpawnMonster(int index, Collider[] colliders, string monsterName, Vector3 spawnPosition)
    {
        foreach(Collider collider in colliders)
        {
            var mng = collider.GetComponent<MonsterManager>();
            if (mng.GetMonsterClass().GetName() == monsterName)
                return;
        }

        // DB를 참조하여, 몬스터 클래스 초기화
        Monster monsterCls = arr_MonsterData[index - 1].Clone();

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

    // 구역 세팅 몬스터 생성 함수
    public MonsterManager SpawnMonster_StaticSet(Vector3 spawnPos, string name, Monster monsterCls, MonsterManager.MonsterControl_Info controlInfo = default, Transform monsters = null)
    {

        MonsterManager monsterManager = null;

        switch (name)
        {
            case "Cactus":
                if (monsters != null)
                    monsterManager = CactusPool.GetFromPool(spawnPos, Quaternion.identity, monsters);
                else
                    monsterManager = CactusPool.GetFromPool(spawnPos, Quaternion.identity);
                break;
            case "MushroomAngry":
                if (monsters != null)
                    monsterManager = MushroomAngryPool.GetFromPool(spawnPos, Quaternion.identity, monsters);
                else
                    monsterManager = MushroomAngryPool.GetFromPool(spawnPos, Quaternion.identity);
                break;
            case "Golem_Boss":
                if (monsters != null)
                    monsterManager = GolemBossPool.GetFromPool(spawnPos, Quaternion.identity, monsters);
                else
                    monsterManager = GolemBossPool.GetFromPool(spawnPos, Quaternion.identity);
                break;
        }

        // 오브젝트 풀로 hp바 생성
        MonsterHp monsterHpMng = MonsterHpBarPool.GetFromPool(spawnPos, Quaternion.identity, BottomCanvas.transform);

        // 필요한 데이터 초기화
        monsterManager.SetMonsterHPMng(monsterHpMng);
        monsterManager.SetMonsterClass(monsterCls);
        monsterManager.SetStaticFixed(true);

        // 데이터가 디폴트 값이 아닐 경우에, 제어용 변수 셋.
        if (controlInfo.idleTime != default || controlInfo.patrolTime != default || controlInfo.chaseRange != default || controlInfo.moveRange != default)
        {
            monsterManager.SetMonsterControlInfo(controlInfo);
        }

        monsterHpMng.HpBarFill_Init(monsterCls.GetMonsterCurrentHp());
        monsterHpMng.HpBarFill_End(monsterCls.GetMonsterMaxHp(), monsterCls.GetMonsterCurrentHp(), false);

        return monsterManager;
    }
    //public void SpawnMonster_StaticSet(Vector3 spawnPos, string name, Monster monsterCls, MonsterManager.MonsterControl_Info controlInfo = default , Transform monsters = null)
    //{

    //    MonsterManager monsterManager = null;

    //    switch (name)
    //    {
    //        case "Cactus":
    //            if (monsters != null)
    //                monsterManager = CactusPool.GetFromPool(spawnPos, Quaternion.identity, monsters);
    //            else
    //                monsterManager = CactusPool.GetFromPool(spawnPos, Quaternion.identity);
    //            break;
    //        case "MushroomAngry":
    //            if (monsters != null)
    //                monsterManager = MushroomAngryPool.GetFromPool(spawnPos, Quaternion.identity, monsters);
    //            else
    //                monsterManager = MushroomAngryPool.GetFromPool(spawnPos, Quaternion.identity);
    //            break;
    //    }

    //    // 오브젝트 풀로 hp바 생성
    //    MonsterHp monsterHpMng = MonsterHpBarPool.GetFromPool(spawnPos, Quaternion.identity, BottomCanvas.transform);

    //    // 필요한 데이터 초기화
    //    monsterManager.SetMonsterHPMng(monsterHpMng);
    //    monsterManager.SetMonsterClass(monsterCls);
    //    monsterManager.SetStaticFixed(true);

    //    // 데이터가 디폴트 값이 아닐 경우에, 제어용 변수 셋.
    //    if(controlInfo.idleTime!=default || controlInfo.patrolTime !=default || controlInfo.chaseRange !=default || controlInfo.moveRange != default)
    //    {
    //        monsterManager.SetMonsterControlInfo(controlInfo);
    //    }

    //    monsterHpMng.HpBarFill_Init(monsterCls.GetMonsterCurrentHp());
    //    monsterHpMng.HpBarFill_End(monsterCls.GetMonsterMaxHp(), monsterCls.GetMonsterCurrentHp(), false);
    //}


    #endregion


    #region 몬스터 제거
    public void RangeOutMonsterPoolReturn(MonsterManager mobMng, MonsterHp mobHpMng)
    {
        var images = mobHpMng.GetImages();

        foreach (var tmp in images)
            tmp.enabled = true;

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

    #region 기타 함수
    // 아이템 아이디 값 생성기
    public void Item_Id_Generator<T>(T item) where T : ItemClass
    {
        item.SetId(nIdValue);
        nIdValue += 1;
    }
    public void Item_Id_Generator<T>(List<T> item) where T : ItemClass
    {
        foreach (var tmp in item)
        {
            tmp.SetId(nIdValue);
            nIdValue += 1;
        }
    }
    public List<T> Item_Id_Generator_Copied<T>(List<T> item) where T : WeaponAndEquipCls
    {
        List<T> result = new List<T>();

        foreach (var tmp in item)
        {
            T newItem = Activator.CreateInstance<T>();
            newItem.CopyFrom(tmp);
            newItem.SetId(nIdValue);
            nIdValue += 1;

            result.Add(newItem);
        }

        return result;
    }
    public int GetItem_Id_value() { return nIdValue; }
    public void Item_Id_value_Upper() { nIdValue += 1; }

    public void ChangeSceneManaging()
    {
        Transform startPos = null;
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        // 시작 지점 찾기
        foreach (GameObject tmp in allObjects)
        {
            if (tmp.CompareTag("StartPoint"))
            {
                startPos = tmp.transform;
                break;
            }
        }

        // 시작 지점이 없으면 종료
        if (startPos == null)
            return;

        // 시작 지점을 이용해 플레이어 생성
        var obj = Instantiate(PlayerbleCharacter, startPos);

        // 기타 작업들
        InterectionObjUI_Pool.AllReturnToPool();   // 상호작용 오브젝트 리턴

        ItemDropManager.Instance.SetCamera();
        obj.GetComponent<CharacterManager>().CharacterManagerConrollerBtnSet();
        obj.GetComponent<CharacterControlMng>().CharacterControlMng_ControllerSet();
        obj.GetComponent<CharacterAttackMng>().CharacterAttackMng_ControllerSet();

        UI_Manager.Instance.UI_Manager_ControllerSet();
        ObjectManager.Instance.ObjectFindSetter();

        CameraController cameraMng = null;

        // 카메라 및 미니맵 설정
        foreach (GameObject tmp in allObjects)
        {
            if (tmp.CompareTag("MainCamera"))
            {
                cameraMng = tmp.GetComponent<CameraController>();
                UI_Manager.Instance.GetWorldMap_Manager.MinimapCamera = cameraMng.MiniMapCamera;
                UI_Manager.Instance.GetWorldMap_Manager.CameraFarSize = cameraMng.MiniMapSize;
                UI_Manager.Instance.GetWorldMap_Manager._cameraController = cameraMng;
                break;
            }
        }

        // 스폰 포인트 위치 저장
        // 조건을 만족하는 GameObject들을 저장할 리스트
        List<GameObject> spawnPointObjects = new List<GameObject>();

        foreach (GameObject tmp in allObjects)
        {
            if (tmp.CompareTag("SpawnPoint"))
            {
                spawnPointObjects.Add(tmp);
            }
        }

        // 13번째 문자부터 오름차순으로 정렬
        spawnPointObjects.Sort((a, b) => a.name.Substring(12).CompareTo(b.name.Substring(12)));

        // 정렬된 GameObject의 위치를 list_SpawnPoint에 추가
        foreach (GameObject spawnPointObject in spawnPointObjects)
        {
            list_SpawnPoint.Add(spawnPointObject.transform.position);
        }

        // 카메라를 옵저버 패턴으로 송신하는 객체를 연결.
        Observer cameraMngObserver = cameraMng.GetComponent<Observer>();
        foreach (GameObject tmp in allObjects)
        {
            if(tmp.tag == "CameraAttachSubjects")
            {
                var subject= tmp.GetComponent<Subject>();
                subject.Attach(cameraMngObserver);
            }
        }

    }

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

    #region DB 관련 기능 함수
    /// <summary>
    /// 무기 - 무기는 GetName 값을 기준으로 데이터를 참조한다. 
    /// 무기는 이름을 기준으로, DB 데이터를 구분하며, 이름 - 레벨 순으로 참조하면 된다.
    /// </summary>
    // 레벨에 따른, 장비의 메인 및 서브 스탯 설정 함수
    public void WeaponItemStatusSet(List<ItemClass> itemList)
    {
        foreach (var tmp in itemList)
        {
            WeaponAndEquipCls item = tmp as WeaponAndEquipCls;

            var finder = list_WeaponAndEquipData.Find(data => data.Item1.Equals(item.GetName()));

            if (finder != null) // finder가 null이 아닌 경우에만 처리
            {
                // finder가 참조하는 Tuple의 Item2에서 조건을 만족하는 WEAPON_EQUIP_STATE_DATA_BASE를 추출
                var matchingData = finder.Item2.Find(inData => inData.LEVEL == item.GetLevel());
                item.SetMainStat(matchingData.MAIN_STAT);
                item.SetSubStat(matchingData.SUB_STAT);
                item.SetLimitLevel(matchingData.LIMIT_LEVEL);
            }
        }
    }
    public void WeaponItemStatusSet(ItemClass item)
    {
        WeaponAndEquipCls weItem = item as WeaponAndEquipCls;

        var finder = list_WeaponAndEquipData.Find(data => data.Item1.Equals(item.GetName()));

        if (finder != null) // finder가 null이 아닌 경우에만 처리
        {
            // finder가 참조하는 Tuple의 Item2에서 조건을 만족하는 WEAPON_EQUIP_STATE_DATA_BASE를 추출
            var matchingData = finder.Item2.Find(inData => inData.LEVEL == item.GetLevel());
            weItem.SetMainStat(matchingData.MAIN_STAT);
            weItem.SetSubStat(matchingData.SUB_STAT);
            weItem.SetLimitLevel(matchingData.LIMIT_LEVEL);
        }
    }
    public void WeaponItemStatusSet(WeaponAndEquipCls item)
    {
        var finder = list_WeaponAndEquipData.Find(data => data.Item1.Equals(item.GetName()));

        if (finder != null) // finder가 null이 아닌 경우에만 처리
        {
            // finder가 참조하는 Tuple의 Item2에서 조건을 만족하는 WEAPON_EQUIP_STATE_DATA_BASE를 추출
            var matchingData = finder.Item2.Find(inData => inData.LEVEL == item.GetLevel());
            item.SetMainStat(matchingData.MAIN_STAT);
            item.SetSubStat(matchingData.SUB_STAT);
            item.SetLimitLevel(matchingData.LIMIT_LEVEL);
        }
    }

    //다음 돌파 한계 레벨 반환 함수
    public int NextLimitLevelFinder(WeaponAndEquipCls item)
    {
        int nextValue =0;

        int currentLevel = item.GetLimitLevel();
        switch(currentLevel)
        {
            case 20:
                nextValue = 40;
                break;
            case 40:
                nextValue = 50;
                break;
            case 50:
                nextValue = 60;
                break;
            case 60:
                nextValue = -1;
                break;
        }
        Debug.Log(nameof(nextValue)+":" +nextValue);
        return nextValue;
    }

    // 다음 단계 재련 효과 텍스트 반환 함수
    public string NextReforgeEffectData(WeaponAndEquipCls item)
    {
        string str = string.Empty;
        var finder = list_WeaponAndEquipReforgeGradeData.Find(data => data.Item1.Equals(item.GetName()));
        var nextData = finder.Item2.Find(tmp => tmp.EFFECT_LEVEL.Equals(item.GetEffectLevel() + 1));
        str = nextData.EFFECT_TEXT;
        return str;
    }
    // 현재 재련 단계에 해당하는 효과 텍스트 세팅 함수
    public void SetReforgeEffectData(WeaponAndEquipCls item)
    {
        string str = string.Empty;
        var finder = list_WeaponAndEquipReforgeGradeData.Find(data => data.Item1.Equals(item.GetName()));
        var nextData = finder.Item2.Find(tmp => tmp.EFFECT_LEVEL.Equals(item.GetEffectLevel()));
        str = nextData.EFFECT_TEXT;
        item.SetEffectText(str);
    }


    /// <summary>
    /// 성유물 - 성유물은 GetEffectText 값을 기준으로 데이터를 참조한다. 
    /// 이는, 성유물의 경우 이름을 기준으로 DB 데이터를 구분하지 않기 때문
    /// </summary>
    public void EquipItemStatusSet(List<ItemClass> itemList)
    {
        foreach (var tmp in itemList)
        {
            WeaponAndEquipCls item = tmp as WeaponAndEquipCls;
            string str = item.GetEffectText();  // 성유물의 EffectText는, DB의 칼럼 값을 저장한다.

            var finder = list_WeaponAndEquipData.Find(data => data.Item1.Equals(str));

            if (finder != null) // finder가 null이 아닌 경우에만 처리
            {
                // finder가 참조하는 Tuple의 Item2에서 조건을 만족하는 WEAPON_EQUIP_STATE_DATA_BASE를 추출
                var matchingData = finder.Item2.Find(inData => inData.LEVEL == item.GetLevel());
                item.SetMainStat(matchingData.MAIN_STAT);
                item.SetSubStat(matchingData.SUB_STAT);
                item.SetLimitLevel(matchingData.LIMIT_LEVEL);
            }
        }
    }
    public void EquipItemStatusSet(ItemClass item)
    {
        WeaponAndEquipCls weItem = item as WeaponAndEquipCls;

        var finder = list_WeaponAndEquipData.Find(data => data.Item1.Equals(weItem.GetEffectText()));

        if (finder != null) // finder가 null이 아닌 경우에만 처리
        {
            // finder가 참조하는 Tuple의 Item2에서 조건을 만족하는 WEAPON_EQUIP_STATE_DATA_BASE를 추출
            var matchingData = finder.Item2.Find(inData => inData.LEVEL == item.GetLevel());
            weItem.SetMainStat(matchingData.MAIN_STAT);
            weItem.SetSubStat(matchingData.SUB_STAT);
            weItem.SetLimitLevel(matchingData.LIMIT_LEVEL);
        }
    }
    public void EquipItemStatusSet(WeaponAndEquipCls item)
    {
        var finder = list_WeaponAndEquipData.Find(data => data.Item1.Equals(item.GetEffectText()));

        if (finder != null) // finder가 null이 아닌 경우에만 처리
        {
            // finder가 참조하는 Tuple의 Item2에서 조건을 만족하는 WEAPON_EQUIP_STATE_DATA_BASE를 추출
            var matchingData = finder.Item2.Find(inData => inData.LEVEL == item.GetLevel());
            item.SetMainStat(matchingData.MAIN_STAT);
            item.SetSubStat(matchingData.SUB_STAT);
            item.SetLimitLevel(matchingData.LIMIT_LEVEL);
        }
    }

    // 랜덤 성유물 능력치 부여 함수
    public void EquipStatusRandomSelector(List<ItemClass> itemList)
    {
        foreach(ItemClass tmp in itemList)
        {
            var item = tmp as WeaponAndEquipCls;
            int nGrade = item.GetGrade();
            string tag = item.GetTag();
            string str = "";
            // 등급 분류
            switch (nGrade)
            {
                case 5:
                    str += "5성_";
                    break;
                case 4:
                    str += "4성_";
                    break;
                case 3:
                    str += "3성_";
                    break;
            }
            // 태그 분류
            if (tag == "꽃" || tag == "깃털")
                str += tag;
            else
            {
                str += "성유물_";
                string[] offset = { "치명타 피해", "치명타 확률", "원소 마스터리", "방어력", "공격력", "체력", "방어력", "체력", "원소 마스터리", "공격력" };

                // 현재 시간을 시드로 사용하여 랜덤 객체 생성
                //System.Random random = new System.Random(DateTime.Now.Millisecond);

                // 무작위로 offset 배열에서 값을 선택
                int index = UnityEngine.Random.Range(0, offset.Length);
                string selectedOffset = offset[index];

                str += selectedOffset;
            }
            item.SetEffectText(str);
        }
    }
    public void EquipStatusRandomSelector(WeaponAndEquipCls item)
    {
        int nGrade = item.GetGrade();
        string tag = item.GetTag();
        string str = "";
        // 등급 분류
        switch(nGrade)
        {
            case 5:
                str += "5성_";
                break;
            case 4:
                str += "4성_";
                break;
            case 3:
                str += "3성_";
                break;
        }
        // 태그 분류
        if(tag == "꽃" || tag == "깃털")
            str += tag;
        else
        {
            str += "성유물_";
            string[] offset = { "치명타 피해", "치명타 확률", "원소 마스터리", "방어력", "공격력", "체력", "방어력", "체력", "원소 마스터리", "공격력" };

            // 무작위로 offset 배열에서 값을 선택
            System.Random random = new System.Random();
            int index = random.Next(0, offset.Length);
            string selectedOffset = offset[index];

            str += selectedOffset;
        }
        item.SetEffectText(str);
    }
    #endregion


    #region 게터세터
    public UserClass GetUserClass() { return playerData; }
    public List<Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>> GetList_WeaponAndEquipDataBase(){return list_WeaponAndEquipData; }
    public List<Tuple<string, List<WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA>>> GetList_WeaponAndEquipLimitBreakResourceData()
    {
        return list_WeaponAndEquipLimitBreakResourceData;
    }
    public List<Tuple<string,List<WEAPON_EQUIP_EFFECT_DATA_BASE>>> GetList_WeaponAndEquipReforgeGradeData()
    {
        return list_WeaponAndEquipReforgeGradeData;
    }
    public List<EQUIPMENT_SET_SYNERGY_DATA_BASE> GetList_EquipmentSetSynergyData()
    {
        return list_EquipmentSetSynergyData;
    }
    public List<WeaponAndEquipCls> GetWeaponAndEquipmentDataList() { return WeaponAndEquipmentDataList; }
    public List<ItemClass> GetItemDataList() { return ItemDataList; }
    public List<SYNTHESIS_DATA_BASE> GetWeaponSynthesisData() { return list_WeaponSynthesisData; }
    public List<SYNTHESIS_DATA_BASE> GetEquipSynthesisData() { return list_EquipSynthesisData; }
    public List<SYNTHESIS_DATA_BASE> GetEtcSynthesisData() { return list_EtcSynthesisData; }
    public Monster[] GetMonsterData() { return arr_MonsterData; }
    public List<Vector3> List_SpawnPoint
    {
        get { return list_SpawnPoint; }
    }

    #endregion

}
