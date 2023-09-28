using System;
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
    private List<ItemClass> ItemDataList;
    private List<WeaponAndEquipCls> WeaponAndEquipmentDataList;
    private List<Tuple<string,List<WEAPON_EQUIP_STATE_DATA_BASE>>> list_WeaponAndEquipData;
    private List<Tuple<string, List<WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA>>> list_WeaponAndEquipLimitBreakResourceData;

    // 기타
    [SerializeField] Transform objectPoolSavePos;

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
        public int MAIN_STAT;
        public float SUB_STAT;
        public int LIMIT_LEVEL;

        public WEAPON_EQUIP_STATE_DATA_BASE(int level, int mainStat, float subStat, int limitLevel)
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
        int EFFECT_LEVEL;
        string EFFECT_TEXT;

        public WEAPON_EQUIP_EFFECT_DATA_BASE(int eFFECT_LEVEL, string eFFECT_TEXT)
        {
            EFFECT_LEVEL = eFFECT_LEVEL;
            EFFECT_TEXT = eFFECT_TEXT;
        }
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

        #region 디폴트 아이템 데이터 DB 테이블 채우기
        ItemDataList = new List<ItemClass>
        { 
            new ItemClass("육성 아이템","칼바람 울프의 젖니",1,false,1,500,0,"늑대 무리는 그의 축복을 받은 근위병이다. 새끼 늑대의 젖니라도 상당한 저력을 가지고 있다.\n과거의 신들은 인간을 사랑한다는 책임을 짊어지고 있었다. 따라서, 늑대 무리를 인솔하지만 버림받은 아이를 입양하고 방랑자들을 받아들인 「안드리우스」는 아주 이상하다",""),
            new ItemClass("육성 아이템","칼바람 울프의 이빨",3,false,1,1000,0,"늑대의 삶엔 전투가 끊이지 않아 뼈가 부서지는 사투도 적지 않다.\n「안드리우스」는 인간은 실망만을 안겨다 줄 뿐이지만 순진한 아이들은 죄가 없다고 여겼다. 늑대 무리가 아이를 선택했고 아이 또한 늑대를 선택했다. 그렇게 그들은 「루피카」-운명의 가족이 되었다",""),
            new ItemClass("육성 아이템","칼바람 울프의 부서진 이빨",4,false,1,2000,0,"늑대 무리는 인간에게 같은 무리가 없으면 고독함을 느낄 것이란 걸 충분히 이해하고 있다. 명예로운 부서진 이빨은 늑대들의 이별 선물로 몸을 보호해주는 마력이 있다고 한다.\n머나먼 세계에는 어머니 늑대가 위대한 쌍둥이를 입양했다는 전설이 있다. 늑대와 사람들이 함께 산 「집」은 「늑대의 동굴」 즉 「루페르카」로 불렸고 이는 이 세계의 「루피카」라는 뜻이다",""),
            new ItemClass("육성 아이템","지맥의 낡은 가지",1,false,1,500,0,"지하 깊은 곳의 마른 나뭇가지. 오랜 세월이 지났지만 알록달록한 나무껍질에서 내재된 힘을 느낄 수 있다",""),
            new ItemClass("육성 아이템","지맥의 마른 잎",3,false,1,1000,0,"지하 깊은 곳에서 자라는 식물의 가지. 줄기에서 멀리 떨어져 있지만 여전히 요동치는 힘이 희미하게 남아있다",""),
            new ItemClass("육성 아이템","지맥의 새싹",4,false,1,2000,0,"세계 각지에 뿌리를 내린 거목이 있다고 한다.\n이 가지는 그 거목의 일부라고 한다. 누군가에게 꺾여 멀리 이동한 적이 없는 듯하며, 왕성한 생명력으로 새잎이 자랐다",""),
            new ItemClass("육성 아이템","슬라임 응축액",1,false,1,500,0,"슬라임을 덮고 있는 걸쭉한 액체. 각지의 원소 공방에서 가장 흔히 보이는 원료이다",""),
            new ItemClass("육성 아이템","슬라임청",3,false,1,1000,0,"정제된 슬라임의 분비물. 피부에 닿으면 해로우니 반드시 피해야 한다",""),
            new ItemClass("육성 아이템","슬라임 원액",4,false,1,2000,0,"슬라임 농축 원액. 가만히 두면 불규칙하게 움직인다",""),
            new ItemClass("육성 아이템","라이언 투사의 족쇄",1,false,1,500,0,"사람들은 늘 영웅의 이야기를 과장한다. 심지어 과거 영웅을 속박했던 족쇄도 자유의 적으로 승화됐다.\r\n때문에 이 족쇄도 비범한 힘을 지니게 되었다",""),
            new ItemClass("육성 아이템","라이언 투사의 쇠사슬",3,false,1,1000,0,"무기에 돌파의 힘을 부여하는 재료.\n과거 대영웅 바네사가 찼던 족쇄—또는 그 시대의 모든 검투사들은 수갑을 찼다. 라이언 투사 또한 그 일원 중 한 명일 뿐이다",""),
            new ItemClass("육성 아이템","라이언 투사의 수갑",4,false,1,2000,0,"무기에 돌파의 힘을 부여하는 재료.\n바네사는 족쇄 또는 수갑에 묶여있던 게 아니다. 그녀가 빠져나올 마음만 먹었다면 언제든지 몬드의 평범한 금속으로 만든 족쇄를 파괴할 수 있었다. 몬드는 우수한 광석도 고향의 신의 불꽃도 없으니까….\n그녀를 막은 건 오직 부족민을 보호해야 한다는 책임감뿐이다",""),
            new ItemClass("육성 아이템","혼돈의 장치",1,false,1,500,0,"멈춰버린 고대 유적에서 나온 구조체. 몸체의 균형을 잡아주던 부분으로 정교한 공학적 감각이 돋보인다",""),
            new ItemClass("육성 아이템","혼돈의 회로",3,false,1,1000,0,"멈춰버린 고대 유적에서 나온 구조체. 이전에 논리 회로였던 부분으로, 아무도 해제할 수 없는 위대한 기술이 담겨있다",""),
            new ItemClass("육성 아이템","혼돈의 노심",4,false,1,2000,0,"멈춰버린 고대 유적에서 나온 구조체. 이전에 에너지의 핵심이었던 부분으로, 이렇게 신비로운 기술을 이해하고 재구성할 수 있다면 이 세계를 바꿀 수 있을지도 모른다",""),
            new ItemClass("육성 아이템","이능 두루마리",1,false,1,500,0,"어떤 마법에 관한 두루마리의 남은 조각인 듯하다. 이상한 기운과 불길한 온기를 은은하게 내뿜고 있다",""),
            new ItemClass("육성 아이템","봉마의 두루마리",3,false,1,1000,0,"대충 만들어진 난해한 고서 단편. 몇몇 마물은 단편에 있는 인물의 형상을 모방해 마법의 기적을 조금은 재현해 낼 수 있다",""),
            new ItemClass("육성 아이템","금주의 두루마리",4,false,1,2000,0,"고대 형식으로 제작된 고서 초본. 해독할 수 있는 사람이 드물다. 그 의미를 알아버린 학자는 결국 미쳐버린다고 한다",""),

            new ItemClass("광물", "철광석", 1, false, 1, 600, 1, "철광석. 뛰어난 대장장이에게 가면 빛을 발할 수 있다", ""),
            new ItemClass("광물", "백철", 3, false, 1, 1500, 1, "하얀색 철광석. 뛰어난 대장장이에게 가면 빛을 발할 수 있다", ""),
            new ItemClass("광물", "수정덩이", 4, false, 1, 4000, 1, "가공되지 않은 결정체. 세공을 해야 진정한 가치를 알 수 있다", ""),

            new ItemClass("음식", "달콤달콤 닭고기 스튜", 3, false, 1, 1500, 1, "꿀에 버무려 구운 새고기. 탱탱한 새고기에 넘쳐흐르는 육즙, 게다가 아삭아삭한 달콤달콤꽃까지 정말 맛있다", ""),
            new ItemClass("음식", "무스프", 1, false, 3, 700, 1, "무를 주재료로 만든 야채수프. 여유로운 오후처럼 싱그럽고 소박한 농촌의 향기를 풍긴다.", ""),

        };
        WeaponAndEquipmentDataList = new List<WeaponAndEquipCls>
        {
            new WeaponAndEquipCls("무기", "천공의 검", 5, true, 1, 100000, 19,20, "풍룡의 영광을 상징하는 기사검.\n잃어버렸다가 오늘날 되찾았다.\n현재 검에 바람 신의 축복이 깃들어 있으며, 푸른 하늘과 바람의 힘을 지니고 있다","" ,"치명타 확률이 4% 증가한다. 원소폭발 발동 시 파공의 기세를 획득한다: 이동속도+10%, 공격속도+10%. 일반 공격과 강공격이 명중 시 추가로 공격력 20%의 피해를 준다. 지속 시간: 12초",1,560,12,900,1000),
            new WeaponAndEquipCls("무기", "제례검", 4, false, 1, 65000, 3,20, "기나긴 세월을 거쳐 석화한 검은 의례적인 장식이 여전히 선명하게 보인다.\n시간의 바람에 씻긴 축복의 힘을 보유하고 있다", "","원소전투 스킬로 피해를 줄 때 40%의 확률로 해당 스킬의 재발동 대기시간이 초기화된다. 해당 효과는 30초마다 1회만 발동한다",1,470,13.3f,416,800),
            new WeaponAndEquipCls("무기", "여명신검", 3, false, 1, 30000, 10,20, "오래전 아침 햇살처럼 빛나던 보검. 이 검을 가진 자는 근거 없는 자신감에 가득 차게 된다. 검신의 빛나던 발광 재료는 이미 사라졌다", "","HP가 90% 초과 시 치명타 확률이 14% 증가한다",1,266,10f,990,1208),

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
            })
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


        // 유저 게임 데이터 초기화
        characterCls = new CharacterClass(300, 300, 0, 100, 50, 15, 1, 20, 3.0f, CharacterClass.eCharactgerState.e_NONE,50,120,50,"플레이어","Knight",0,true, 100);
        playerData.SetUserCharacter(characterCls);
        playerData.SetMora(156000);
        List<ItemClass> weaponList = new List<ItemClass>
        {
            new WeaponAndEquipCls("무기", "천공의 검", 5, true, 1, 100000, 19,20, "풍룡의 영광을 상징하는 기사검.\n잃어버렸다가 오늘날 되찾았다.\n현재 검에 바람 신의 축복이 깃들어 있으며, 푸른 하늘과 바람의 힘을 지니고 있다","" ,"치명타 확률이 4% 증가한다. 원소폭발 발동 시 파공의 기세를 획득한다: 이동속도+10%, 공격속도+10%. 일반 공격과 강공격이 명중 시 추가로 공격력 20%의 피해를 준다. 지속 시간: 12초",1,560,12,900,1000),
            new WeaponAndEquipCls("무기", "제례검", 4, false, 1, 65000, 3,20, "기나긴 세월을 거쳐 석화한 검은 의례적인 장식이 여전히 선명하게 보인다.\n시간의 바람에 씻긴 축복의 힘을 보유하고 있다", "","원소전투 스킬로 피해를 줄 때 40%의 확률로 해당 스킬의 재발동 대기시간이 초기화된다. 해당 효과는 30초마다 1회만 발동한다",1,470,13.3f,416,800),
            new WeaponAndEquipCls("무기", "여명신검", 3, false, 1, 30000, 10,20, "오래전 아침 햇살처럼 빛나던 보검. 이 검을 가진 자는 근거 없는 자신감에 가득 차게 된다. 검신의 빛나던 발광 재료는 이미 사라졌다", "","HP가 90% 초과 시 치명타 확률이 14% 증가한다",1,266,10f,990,1208)
        };
        playerData.SetHadWeaponList(weaponList);
        WeaponAndEquipItemStatusSet(weaponList);

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
            new ItemClass("광물", "수정덩이", 4, false, 1, 4000, 1, "가공되지 않은 결정체. 세공을 해야 진정한 가치를 알 수 있다", ""),
        };
        playerData.SetHadGemList(gemList);

        List<ItemClass> foodList = new List<ItemClass>
        {            
            new ItemClass("음식", "달콤달콤 닭고기 스튜", 3, false, 1, 1500, 1, "꿀에 버무려 구운 새고기. 탱탱한 새고기에 넘쳐흐르는 육즙, 게다가 아삭아삭한 달콤달콤꽃까지 정말 맛있다", ""),
            new ItemClass("음식", "무스프", 1, false, 3, 700, 1, "무를 주재료로 만든 야채수프. 여유로운 오후처럼 싱그럽고 소박한 농촌의 향기를 풍긴다.", ""),
        };
        playerData.SetHadFoodList(foodList);

        List<ItemClass> growList = new List<ItemClass>
        {
            new ItemClass("육성 아이템","칼바람 울프의 젖니",1,false,6,500,0,"늑대 무리는 그의 축복을 받은 근위병이다. 새끼 늑대의 젖니라도 상당한 저력을 가지고 있다.\n과거의 신들은 인간을 사랑한다는 책임을 짊어지고 있었다. 따라서, 늑대 무리를 인솔하지만 버림받은 아이를 입양하고 방랑자들을 받아들인 「안드리우스」는 아주 이상하다","")
        };
        playerData.SetHadGrowMaterialList(growList);

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

    #region DB 관련 기능 함수
    // 레벨에 따른, 장비의 메인 및 서브 스탯 설정 함수
    public void WeaponAndEquipItemStatusSet(List<ItemClass> itemList)
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
    public void WeaponAndEquipItemStatusSet(ItemClass item)
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
    public void WeaponAndEquipItemStatusSet(WeaponAndEquipCls item)
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
    #endregion


    #region 게터세터
    public UserClass GetUserClass() { return playerData; }
    public List<Tuple<string, List<WEAPON_EQUIP_STATE_DATA_BASE>>> GetList_WeaponAndEquipDataBase(){return list_WeaponAndEquipData; }
    public List<Tuple<string, List<WEAPON_EQUIP_LIMIT_BREAK_RESOURCE_DATA>>> GetList_WeaponAndEquipLimitBreakResourceData()
    {
        return list_WeaponAndEquipLimitBreakResourceData;
    }


    #endregion

}
