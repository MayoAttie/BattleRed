using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;
using static CharacterUpgradeManager;
using static UI_UseToolClass;
using UnityEditor.PackageManager.Requests;
using System.Collections;

public class UI_Manager : EnergyBarManager
{
    // 싱글턴 인스턴스
    private static UI_Manager instance;
    public static UI_Manager Instance { get { return instance; } }

    #region 변수

    #region 인벤토리
    // 인벤토리 관련
    [Header("인벤토리 관련(Inventory's Values)")]
    public GameObject Inventory;                // 인벤토리 전체 객체
    
    [SerializeField]                        
    private InventoryButton[] invenButtons;                             // 인벤토리 아이템 타입 선택 버튼들(0-웨폰,1-장비,2-광물,3-음식)
    [SerializeField]
    private Transform scrollContent;                                    // 인벤토리 스크롤뷰 콘텐츠의 Transform
    [SerializeField]
    private GameObject scrollViewObj;                                   // 스크롤뷰 콘텐츠에 부착될 컴포넌트 프리팹
    [SerializeField]
    private GameObject ExpressFrame;                                    // 특정 아이템 선택시 아이템 정보 출력창
    [SerializeField]
    private GameObject SortSelectionButtonOnList;                       // Sort 선택 버튼 클릭 시, 띄우는 리스트 버튼 객체
    [SerializeField]
    private TextMeshProUGUI selectedOrderPrintText;                     // 선택한 정렬오더 출력용 TEXT

    private List<InvenItemObjClass> openUI_ItemList;                    // 선택한 아이템 타입의 객체 리스트
    private bool[] invenButtonIsClicked;                                // 인벤토리 아이템 타입선택 버튼 클릭 여부
    private bool isAscending;                                           // 오름차순 정렬 기준인지 판단
    private e_SortingOrder selected_SortOrder;                          // 현재 선택된 정렬 기준
    private e_InventoryTypeSelected invenType_Index;                    // 인벤토리 타입 인덱스
    private Tuple<ItemClass, InvenItemObjClass> selectedObjAndItemCls;  // 플레이어가 선택한 아이템

    #endregion

    #region 캐릭터 창
    // 캐릭터 창 관련
    [Header("캐릭터 정보 관련(CharacterInfo's Value)")]
    public GameObject PlayerInfoScreen;

    [SerializeField]
    private PlayerInfoUI_Button[] infoSelectButtons;        // 선택한 버튼 클래스 배열
    [SerializeField]
    private GameObject[] printInfoDataField;                // 출력되는 UI 오브젝트
    [SerializeField]
    private GameObject InfoObj_CloseButtion;                        // 캐릭터창 종료 버튼

    // 무기 창 관련
    private e_InfoButtonSelected info_Index;                // 선택한 정보 인덱스
    private bool isDetailScreenOpen;                        // 캐릭터 상세정보 프리팹 On/Off 플래그 변수
    private bool isWeaponChangeBtnClicked;                  // 무기 변경 클릭 제어 플래그
    private bool isWeaponUpgradeBtnClicked;                 // 무기 업글 클릭 제어 플래그
    private bool[] isWeaponUpgradeDetailObjClicked;         // 무기 업글 창_상세창 제어 플래그
    private TextMeshProUGUI txt_NeedMora;                   // 필요한 모라 표기
    private TextMeshProUGUI txt_CurrentHaveMora;            // 보유 모라 표기
    private ButtonClass2 weaponChangeButton;                // 무기 변경 버튼
    private int nWeaponUpgradeExp;                          // 무기 강화용 경험치 변수
    private int nWeaponUpgradeCost;                         // 무기 강화용 요구 재화
    private Transform OpenScrollToUpgradeMaterial;          // 무기 강화용 재화 출력용 스크롤 오브젝트
    private Transform popUpForUpgrade;                                              // 무기 강화 파트에서 사용할 팝업 객체
    private Dictionary<ItemClass, SelectButtonScript> dic_ItemClsForWeaponUpgrade;  // 무기 레벨업용 아이템 재화 - 버튼 자료구조
    private Image img_ExpBar;
    private e_SortingOrder allPull_OrderValue;                                      // 일괄처리 정렬 순서(Name==3성, Grade==5성, Level==4성)
    private e_SortingOrder weaponList_ForItemUpgradeResourceSortOrder;              // 무기 강화용 아이템 재화 출력 스크롤뷰 정렬 변수
    private bool isWeaponList_ForUpgradeResourceSortOrderAscending;                 // 무기 강화용 아이템 재화 출력 스크롤뷰 정렬 변수_내림차순/오름차순
    private bool isLimitBreakPossible;                                              // 무기 돌파 가능 유무 _ 재화로 판단
    private Transform scrollForUpgradeMaterials;                                    // 무기 레벨업용 아이템 재화 출력 스크롤뷰
    private SelectButtonScript selectBtn_ForUpgrade;                                // 플레이어가 선택한 selectButon

    // 성유물 창 관련
    private bool isDetailScreenOpenForEquip;                // 성유물 상세정보 창 오픈

    #endregion


    #endregion

    #region 구조체
    // 인벤토리 UI 창 - 아이템 타입 구조체
    public enum e_InventoryTypeSelected
    {
        Weapon =0,
        Equipment,
        Gem,
        Food
    }
    // 정렬 우선순위 구조체
    public enum e_SortingOrder
    {
        NameOrder           =1,
        LevelOrder          =2,
        GradeOrder          =3,
        ExpOrder            =4
    }
    // PlayerInfoScreen UI 창 - 정보 타입 구조체
    public enum e_InfoButtonSelected
    {
        Status =0,
        Weapon,
        Equipment,
        LimitBreaker,
        Skill
    }
    #endregion


    private void Awake()
    {
        // 싱글턴 인스턴스 설정
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 있다면 이 오브젝트는 파괴
        }

        // 인벤토리 창
        Inventory.SetActive(false); // 인벤토리는 기본적으로 Off
        openUI_ItemList = new List<InvenItemObjClass>();                    // 선택한 아이템 리스트
        isAscending = false;
        invenButtonIsClicked = new bool[] { false, false, false, false };   // 배열 초기화
        selected_SortOrder = e_SortingOrder.GradeOrder;
        invenType_Index = e_InventoryTypeSelected.Weapon;                                              // 선택한 타입 버튼 인덱스, 무기로 초기화
        SortSelectionButtonOnList.SetActive(false);                         // 정렬 리스트 목록 숨김

        // 캐릭터 정보창
        PlayerInfoScreen.SetActive(false);
        info_Index = e_InfoButtonSelected.Status;
        isDetailScreenOpen = false;         // 캐릭터 상세정보창 제어변수

        // 무기 창
        isWeaponChangeBtnClicked = false;   // 무기 전환 제어 변수
        nWeaponUpgradeExp = 0;
        dic_ItemClsForWeaponUpgrade = new Dictionary<ItemClass, SelectButtonScript>();

    }


    #region 인벤토리 UI 관리

    //인벤토리 오픈 버튼
    public void BagOpenButtonClick()
    {
        GameManager.Instance.PauseManager();
        Inventory.SetActive(true);
        ExpressFrame.SetActive(false);
        invenButtons[0].ButtonUIColorSet();
        InventoryViewItemTypeNotify(e_InventoryTypeSelected.Weapon);
        Debug.Log("일시정지");
    }
    // 인벤토리 종료 버튼
    public void BagCloseButtonClick()
    {
        for(int i=0; i< invenButtonIsClicked.Length; i++)   // 클릭체크 변수 초기화
            invenButtonIsClicked[i] = false;

        GameManager.Instance.PauseManager();
        Inventory.SetActive(false);

    }
    // 사용하기 버튼 클릭시 호출
    public void UsingButtonClick()
    {
        List<ItemClass> datas = new List<ItemClass>();

        // 무기와 성유물 장착
        if(invenType_Index == e_InventoryTypeSelected.Weapon || invenType_Index == e_InventoryTypeSelected.Equipment)
        {
            // 현재 타입에 따라서 분기하여 데이터 설정
            switch (invenType_Index)
            {
                case e_InventoryTypeSelected.Weapon:
                    datas = GameManager.Instance.GetUserClass().GetHadWeaponList();
                    break;
                case e_InventoryTypeSelected.Equipment:
                    datas = GameManager.Instance.GetUserClass().GetHadEquipmentList();
                    break;
            }

            // 기존의 아이템 중 활성 상태인 객체 가져오기
            var existingData = datas.FindAll(item => item.GetIsActive() == true && item.GetTag() == selectedObjAndItemCls.Item1.GetTag());
            foreach (var tmp in existingData)    // 활성 상태 취소
            {
                tmp.SetActive(false);
            }
            // 선택된 객체와 동일한 객체를 가져오기
            var findData = datas.Find(item => item.Equals(selectedObjAndItemCls.Item1));

            // 해당 객체를 유저 데이터의 착용 장비로 설정
            if(findData.GetTag() == "무기")
                GameManager.Instance.GetUserClass().SetUserEquippedWeapon(findData);
            else
            {
                switch(findData.GetTag())
                {
                    case "꽃":
                        GameManager.Instance.GetUserClass().SetUserEquippedEquipment(findData, 0);
                        break; 
                    case "깃털":
                        GameManager.Instance.GetUserClass().SetUserEquippedEquipment(findData, 1);
                        break;
                    case "모래":
                        GameManager.Instance.GetUserClass().SetUserEquippedEquipment(findData, 2);
                        break;
                    case "성배":
                        GameManager.Instance.GetUserClass().SetUserEquippedEquipment(findData, 3);
                        break;
                    case "왕관":
                        GameManager.Instance.GetUserClass().SetUserEquippedEquipment(findData, 4);
                        break;
                }
            }

            // 해당 객체 활성화
            findData.SetActive(true);

        }
        else if(invenType_Index == e_InventoryTypeSelected.Food)        // 소모성 아이템
        {
            datas = GameManager.Instance.GetUserClass().GetHadFoodList();
            var findData = datas.Find(item => item.Equals(selectedObjAndItemCls.Item1));
            
            // 소모 효과 구현
            

            // 아이템 감산
            if(findData.GetNumber() >1)     // 갯수가 1 초과라면, 아이템 소모
            {
                

                int num = findData.GetNumber();
                findData.SetNumber(--num);
            }
            else                            // 갯수가 1개 이하라면, 아이템 삭제
            {
                // 항목을 리스트에서 제거
                datas.Remove(findData);
                selectedObjAndItemCls = null;
            }
        }

        ScrollViewReset();                                   // 스크롤뷰 리셋
        ItemPrintByObj_Index((int)invenType_Index);         // 아이템 출력

        if(invenType_Index == e_InventoryTypeSelected.Food && selectedObjAndItemCls != null)
            selectedObjAndItemCls.Item2.ClickedUIApply();       // 소비템인 경우엔 사용 후에도 클릭된 상태 부여
    }

    #region 스크롤뷰 아이템 출력

    // 아이템 타입 버튼 클릭시 인덱스값 송신 함수
    public void InventoryViewItemTypeNotify(e_InventoryTypeSelected index)
    {
        invenType_Index = index;
        // 만약 선택한 타입이 이미 열람 중인 객체라면, 이벤트 리턴.
        if (invenButtonIsClicked[(int)invenType_Index] == true)
        {
            invenButtons[(int)invenType_Index].ButtonUIColorSet(); // UI 상태는 유지
            return;
        }
        // 타입 선택 버튼들을 순회하며, 해당 버튼 UI의 Active 상태를 Off하고 부울 초기화
        for (int i = 0; i < invenButtons.Length; i++)
        {
            if (invenButtons[i].GetClickActive() == true && i != (int)invenType_Index)
                invenButtons[i].ButtonUIColorSet();
            invenButtonIsClicked[i] = false;
        }

        invenButtonIsClicked[(int)invenType_Index] = true; // 클릭한 버튼 객체의 인덱스를 true.

        // 객체 타입에 따라서, 리스트 버튼의 텍스트 수정
        var mng = SortSelectionButtonOnList.GetComponent<InventorySortSelectButton>();
        mng.UI_TextSettingInit();
        TextRevise();

        ViewProcess();
    }

    private void ViewProcess()
    {
        ScrollViewReset();                              // 스크롤뷰 리셋
        ItemPrintByObj_Index((int)invenType_Index);        // 아이템 출력
        ExpressFrame.gameObject.SetActive(false);
    }

    // 인덱스 값에 따라서 아이템 출력
    // ascending == true : 오름차순, false : 내림차순
    private void ItemPrintByObj_Index(int index)
    {
        switch((GameManager.e_PoolItemType)index)
        {
            case GameManager.e_PoolItemType.Weapon:    //웨폰
                WeaponPrintAtScroll(scrollContent, selected_SortOrder, isAscending, openUI_ItemList);
                break;
            case GameManager.e_PoolItemType.Equip:     //장비
                EquipPrintAtScroll();
                break;
            case GameManager.e_PoolItemType.Gem:       //광물
                GemPrintAtScroll();
                break;
            case GameManager.e_PoolItemType.Food:      //음식
                FoodPrintAtScroll();
                break;
            default:
                break;
        }
    }

    // 스크롤뷰 리셋
    private void ScrollViewReset()
    {
        GameManager.Instance.WeaponItemPool.AllReturnToPool();
        GameManager.Instance.EquipItemPool.AllReturnToPool();
        GameManager.Instance.GemItemPool.AllReturnToPool();
        GameManager.Instance.FoodItemPool.AllReturnToPool();
        openUI_ItemList.Clear();
    }
    



    // 게임매니저의 데이터를 참조하여, 장비들을 스크롤뷰 콘텐츠에 출력
    void EquipPrintAtScroll()
    {
        var itemClses = GameManager.Instance.GetUserClass().GetHadEquipmentList();      // 저장된 아이템 목록

        SortingItemList(itemClses, selected_SortOrder, isAscending);


        // 오브젝트 풀로, UI객체 생성
        GameManager.Instance.ItemToObjPool(itemClses.Count, GameManager.e_PoolItemType.Equip, scrollContent);
        // 오브젝트 풀에 저장된 리스트 인스턴스화

        var datas = GameManager.Instance.EquipItemPool.GetPoolList();


        foreach (ItemClass data in itemClses)
        {
            foreach (InvenItemObjClass obj in datas)
            {
                if (obj.gameObject.activeSelf == false || obj.GetIsActive() == true)
                    continue;

                if (data.GetName() == "이국의 술잔")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[0]);
                else if (data.GetName() == "귀향의 깃털")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[1]);
                else if (data.GetName() == "이별의 모자")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[2]);
                else if (data.GetName() == "옛 벗의 마음")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[3]);
                else if (data.GetName() == "빛을 좆는 돌")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[4]);   

                else if (data.GetName() == "전투광의 해골잔")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[5]);
                else if (data.GetName() == "전투광의 깃털")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[6]);
                else if (data.GetName() == "전투광의 귀면")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[7]);
                else if (data.GetName() == "전투광의 장미")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[8]);
                else if (data.GetName() == "전투광의 시계")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[9]);

                else if (data.GetName() == "피에 물든 기사의 술잔")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[10]);
                else if (data.GetName() == "피에 물든 검은 깃털")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[11]);
                else if (data.GetName() == "피에 물든 철가면")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[12]);
                else if (data.GetName() == "피에 물든 강철 심장")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[13]);
                else if (data.GetName() == "피에 물든 기사의 시계")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[14]);
                else { break; }

                if (data.GetIsActive() == true) // 아이템이 활성 상태라면, 사용중임을 알림.
                    obj.EquippedItemUIPrint(true);

                obj.SetItemColor(data.GetGrade());
                obj.SetItemText("LV : " + data.GetLevel().ToString());
                obj.SetIsActive(true);
                obj.SetItemcls(data);
                openUI_ItemList.Add(obj);
                break;
            }
        }
    }

    // 게임매니저의 데이터를 참조하여, 광물들을 스크롤뷰 콘텐츠에 출력
    void GemPrintAtScroll()
    {
        var itemClses = GameManager.Instance.GetUserClass().GetHadGemList();      // 저장된 아이템 목록

        SortingItemList(itemClses, selected_SortOrder, isAscending);


        // 오브젝트 풀로, UI객체 생성
        GameManager.Instance.ItemToObjPool(itemClses.Count, GameManager.e_PoolItemType.Gem, scrollContent);
        // 오브젝트 풀에 저장된 리스트 인스턴스화

        var datas = GameManager.Instance.GemItemPool.GetPoolList();


        foreach (ItemClass data in itemClses)
        {
            foreach (InvenItemObjClass obj in datas)
            {
                if (obj.gameObject.activeSelf == false || obj.GetIsActive() == true)
                    continue;

                if (data.GetName() == "철광석")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.GemSprites[0]);
                else if (data.GetName() == "백철")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.GemSprites[1]);
                else if (data.GetName() == "수정덩이")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.GemSprites[2]);

                else if (data.GetName() == "정제용 하급 광물")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.GemSprites[3]);
                else if (data.GetName() == "정제용 광물")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.GemSprites[4]);
                else if (data.GetName() == "정제용 마법 광물")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.GemSprites[5]);
                
                else if (data.GetName() == "야박석")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.GemSprites[6]);
                else if (data.GetName() == "전기수정")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.GemSprites[7]);
                else if (data.GetName() == "콜라피스")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.GemSprites[8]);
                else { break; }

                obj.SetItemColor(data.GetGrade());
                obj.SetItemText("갯수 : " + data.GetNumber().ToString());
                obj.SetIsActive(true);
                obj.SetItemcls(data);
                openUI_ItemList.Add(obj);
                break;
            }
        }
    }

    // 게임매니저의 데이터를 참조하여, 음식들을 스크롤뷰 콘텐츠에 출력
    void FoodPrintAtScroll()
    {
        var itemClses = GameManager.Instance.GetUserClass().GetHadFoodList();      // 저장된 아이템 목록

        SortingItemList(itemClses, selected_SortOrder, isAscending);


        // 오브젝트 풀로, UI객체 생성
        GameManager.Instance.ItemToObjPool(itemClses.Count, GameManager.e_PoolItemType.Food, scrollContent);
        // 오브젝트 풀에 저장된 리스트 인스턴스화

        var datas = GameManager.Instance.FoodItemPool.GetPoolList();


        foreach (ItemClass data in itemClses)
        {
            foreach (InvenItemObjClass obj in datas)
            {
                if (obj.gameObject.activeSelf == false || obj.GetIsActive() == true)
                    continue;

                if (data.GetName() == "어부 토스트")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.FoodSprites[0]);
                else if (data.GetName() == "버섯닭꼬치")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.FoodSprites[1]);
                else if (data.GetName() == "달콤달콤 닭고기 스튜")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.FoodSprites[2]);
                
                else if (data.GetName() == "냉채수육")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.FoodSprites[3]);
                else if (data.GetName() == "허니캐럿그릴")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.FoodSprites[4]);

                else if (data.GetName() == "스테이크")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.FoodSprites[5]);
                else if (data.GetName() == "무스프")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.FoodSprites[6]);
                else if (data.GetName() == "몬드 감자전")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.FoodSprites[7]);
                
                else if (data.GetName() == "방랑자의 경험")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.FoodSprites[8]);
                else if (data.GetName() == "모험자의 경험")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.FoodSprites[9]);
                else if (data.GetName() == "영웅의 경험")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.FoodSprites[10]);
                else { break; }

                obj.SetItemColor(data.GetGrade());
                obj.SetItemText("갯수 : " + data.GetNumber().ToString());
                obj.SetIsActive(true);
                obj.SetItemcls(data);
                openUI_ItemList.Add(obj);
                break;
            }
        }
    }

    #endregion

    #region 스크롤뷰 정렬
    // 오름차순 내림차순 선택 버튼
    public void Descending_AscendingButton(GameObject clickedButton)
    {
        isAscending = !isAscending;
        var cls = clickedButton.GetComponent<ButtonClass2>();
        cls.AlphaValueChangeing();
        ViewProcess();
    }
    // 정렬기준 선택 버튼
    public void SortSelectionButton()
    {
        if(SortSelectionButtonOnList.activeSelf)
            SortSelectionButtonOnList.SetActive(false);
        else
        {
            var mng = SortSelectionButtonOnList.GetComponent<InventorySortSelectButton>();
            mng.UI_TextSettingInit();
            mng.HideButtonBackGround();
            SortSelectionButtonOnList.SetActive(true);
        }
    }
    // 리스트 내부 버튼에 연결, 클릭 이벤트 호출 함수
    public void GetSortIndex(int index) 
    {
        selected_SortOrder = (e_SortingOrder)index;
        TextRevise();
        ViewProcess();
    } 
    private void TextRevise()
    {
        // selected_SortOrder 값에 따라 분기하여 텍스트 출력
        switch (selected_SortOrder)
        {
            case e_SortingOrder.NameOrder:
                selectedOrderPrintText.text = "이름";
                break;
            case e_SortingOrder.LevelOrder:
                if (invenType_Index == e_InventoryTypeSelected.Weapon || invenType_Index  == e_InventoryTypeSelected.Equipment)
                    selectedOrderPrintText.text = "레벨";
                else
                    selectedOrderPrintText.text = "갯수";
                break;
            case e_SortingOrder.GradeOrder:
                selectedOrderPrintText.text = "희귀도";
                break;
        }
    }

    #endregion

    #region 아이템 선택 및 데이터 출력
    // 선택된 아이템의 데이터를 통신받음
    public void ClickedItemNotifyed(ItemClass itemCls, InvenItemObjClass clickedObj)
    {
        // 선택된 다른 객체의 클릭 상태를 취소함
        foreach(var tmp in openUI_ItemList)
        {
            if(tmp != clickedObj && tmp.GetButtonClicked() == true)
                tmp.ClickedUIApply();
        }
        ExpressFrame.SetActive(true);

        // 프레임에서 필요한 객체들을 인스턴스화
        Image titleFrame = ExpressFrame.transform.GetChild(0).GetComponent<Image>();
        TextMeshProUGUI titleText = titleFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Image topFrame = ExpressFrame.transform.GetChild(1).GetComponent<Image>();
        Image topImage = topFrame.transform.GetChild(1).GetComponent<Image>();
        TextMeshProUGUI bottomText = ExpressFrame.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();


        titleText.text = itemCls.GetName();
        bottomText.text = itemCls.GetContent();

        ItemUISetterByItemGrade(itemCls, titleFrame, topFrame);


        // 아이템 이미지 출력
        topImage.sprite = clickedObj.GetItemSprite();

        // 선택한 아이템 객체를 클래스 지역 변수에 저장
        selectedObjAndItemCls = new Tuple<ItemClass, InvenItemObjClass>(itemCls,clickedObj);

    }




    public void ExpressFrameReset()
    {
        ExpressFrame.gameObject.SetActive(false);
    }
    #endregion

    #endregion


    #region PlayerInfoScreen 오브젝트 스크립트

    // 인포 인덱스 값에 따라서, Active할 UI 프레임 분기
    private void CharaceterInfoPrint()
    {
        foreach (GameObject tmp in printInfoDataField)
        {
            tmp.SetActive(false);
        }

        switch (info_Index)
        {
            case e_InfoButtonSelected.Status:
                PlayerInfoScreen.SetActive(true);
                printInfoDataField[0].SetActive(true);
                PlayerDataPrint();
                // 상세화면 출력 오브젝트 False
                var obj = printInfoDataField[0].transform.GetChild(10).GetComponent<Transform>();
                obj.gameObject.SetActive(false);
                break;
            case e_InfoButtonSelected.Weapon:
                PlayerInfoScreen.SetActive(true);
                printInfoDataField[1].SetActive(true);
                EquippedWeaponPrint();
                break;
            case e_InfoButtonSelected.Equipment:
                PlayerInfoScreen.SetActive(true);
                printInfoDataField[2].SetActive(true);
                EquipmentPrint();
                break;
            case e_InfoButtonSelected.LimitBreaker:
                PlayerInfoScreen.SetActive(true);
                printInfoDataField[3].SetActive(true);
                break;
            case e_InfoButtonSelected.Skill:
                PlayerInfoScreen.SetActive(true);
                printInfoDataField[4].SetActive(true);
                break;
        }
        infoSelectButtons[(int)info_Index].OnOffSpriteSetting();    // 버튼 UI 수정 함수 호출

    }

    // 인포선택 버튼 Click 이벤트 시, 호출되는 함수 (인덱스는 선택한 정보 타입)
    public void InfoSelectButton(e_InfoButtonSelected index)
    {
        info_Index = index;

        // 버튼 UI가 클릭 상태라면, Off한다
        foreach (var tmp in infoSelectButtons)
        {
            if (tmp.GetClickedActive() == true)
                tmp.OnOffSpriteSetting();
        }

        CharaceterInfoPrint();
    }

    // 인포창 On 버튼 함수
    public void OpenPlayerInfoScreenButton()
    {
        // 캐릭터 속성 정보 디폴트로 출력
        info_Index = e_InfoButtonSelected.Status;
        CharaceterInfoPrint();



        GameManager.Instance.PauseManager();
    }
    //인포창 Off 버튼
    public void ClosePlayerInfoScreenButton()
    {
        // 버튼 UI가 클릭 상태라면, Off한다
        foreach (var tmp in infoSelectButtons)
        {
            if (tmp.GetClickedActive() == true)
                tmp.OnOffSpriteSetting();
        }

        // 디테일 정보 출력 도중이라면, 해당 화면 종료
        if (isDetailScreenOpen == true)
        {
            isDetailScreenOpen = false;

            var obj = printInfoDataField[0].transform.GetChild(10).GetComponent<Transform>();
            obj.gameObject.SetActive(false);

            InfoPrintTypeButtonActive();                            // 인포 UI 버튼 활성화
            infoSelectButtons[0].OnOffSpriteSetting();
        }
        // 무기 교체 버튼 클릭 시, 기능 변경
        else if(isWeaponChangeBtnClicked)
        {
            InfoPrintTypeButtonActive();
            CloseButtonSpriteToClose();                             // Close 스프라이트로 버튼 심볼 교체
            infoSelectButtons[1].OnOffSpriteSetting();              // 웨폰 인포 버튼 UI 활성화
            weaponChangeButton.AlphaValueChangeing();               // 무기 변경 버튼 알파값 변경
            ResetToWeaponItemObjectPoolDatas();
            isWeaponChangeBtnClicked = false;
        }
        // 무기 업글 버튼 클릭 시, 기능 변경
        else if(isWeaponUpgradeBtnClicked)
        {
            if(OpenScrollToUpgradeMaterial != null && OpenScrollToUpgradeMaterial.gameObject.activeSelf == true)
            {   // 무기 강화용 재화 출력용 스크롤 오브젝트가 Ative상태라면, 비활성화
                ResetToWeaponItemObjectPoolDatas();
                OpenScrollToUpgradeMaterial.gameObject.SetActive(false);
                return;
            }

            InfoPrintTypeButtonActive();
            CloseButtonSpriteToClose();                             // Close 스프라이트로 버튼 심볼 교체
            infoSelectButtons[1].OnOffSpriteSetting();              // 웨폰 인포 버튼 UI 활성화

            Transform screen = printInfoDataField[1].transform.GetChild(13).GetComponent<Transform>();    // 무기 업글 UI 오브젝트 인스턴스화
            screen.gameObject.SetActive(false);

            ResetToSelectButtonScriptPoolDatas();   // Pool 리턴 및 초기화
            ResetToWeaponItemObjectPoolDatas();     // Pool 리턴 및 초기화
            EquippedWeaponPrint();                  // 레벨업에 의한 데이터 변환에 대비한 UI 데이터 재출력
            isWeaponUpgradeBtnClicked = false;
        }
        else if(isDetailScreenOpenForEquip) // 성유물 - 성유물 상세정보 창 출력
        {
            isDetailScreenOpenForEquip = false;
            var obj = printInfoDataField[2].transform.GetChild(9).GetComponent<Transform>();
            obj.gameObject.SetActive(false);
            InfoPrintTypeButtonActive();
            infoSelectButtons[2].OnOffSpriteSetting();
        }
        else    // 종료
        {
            
            GameManager.Instance.PauseManager();
            PlayerInfoScreen.SetActive(false);
        }
    }


    private void CloseButtonSpriteToClose()
    {
        // 스프라이트 이미지(ToClose) 변경
        InfoObj_CloseButtion.GetComponent<ButtonClass2>().SetSymbolSprite(ItemSpritesSaver.Instance.SpritesSet[1]);
    }
    private void CloseButtonSpriteToBack()
    {
        // 스프라이트 이미지(ToBack) 변경
        InfoObj_CloseButtion.GetComponent<ButtonClass2>().SetSymbolSprite(ItemSpritesSaver.Instance.SpritesSet[0]);
    }
    private void InfoPrintTypeButtonUnActive()
    {
        // 출력 인포타입 버튼 Active_False
        for (int i = 0; i < infoSelectButtons.Length; i++)
        {
            infoSelectButtons[i].gameObject.SetActive(false);
        }
    }
    private void InfoPrintTypeButtonActive()
    {
        // 출력 인포타입 버튼 Active_True
        for (int i = 0; i < infoSelectButtons.Length; i++)
        {
            infoSelectButtons[i].gameObject.SetActive(true);
        }
    }


    // 캐릭터 데이터 출력
    #region 캐릭터 인포 UI 관리



    // 플레이어 캐릭터 정보 출력
    void PlayerDataPrint()
    {
        var datas = GameManager.Instance.characterCls;
        TextMeshProUGUI levelText = printInfoDataField[0].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI maxLevelText = printInfoDataField[0].transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI[] statusTexts = new TextMeshProUGUI[5];
        for (int i=0; i<5; i++) // 0-체력, 1-공격력, 2-방어력, 3- 원소마스터리 4- 스테미나
        {
            statusTexts[i] = printInfoDataField[0].transform.GetChild(4+i).GetChild(2).GetComponent<TextMeshProUGUI>();
        }

        levelText.text = datas.GetLeveL().ToString();
        maxLevelText.text = "/ "+datas.GetMaxLevel().ToString();
        statusTexts[0].text = datas.GetMaxHp().ToString();
        statusTexts[1].text = datas.GetAttack().ToString();
        statusTexts[2].text = datas.GetDeffense().ToString();
        statusTexts[3].text = datas.GetElementNum().ToString();
        statusTexts[4].text = datas.GetStamina().ToString();
    }

    // 캐릭터 상세 정보 출력 버튼
    public void DetailedStatusInfoPrint()
    {
        // 상세 데이터 화면 출력 - 인스턴스 초기화 및 객체 활성화
        isDetailScreenOpen = true;
        var obj = printInfoDataField[0].transform.GetChild(10).GetComponent<Transform>();
        obj.gameObject.SetActive(true);
        var datas = GameManager.Instance.characterCls;

        InfoPrintTypeButtonUnActive();

        Transform content = obj.GetChild(0).GetChild(0).GetChild(0).GetComponent<Transform>();
        TextMeshProUGUI[] statusTexts = new TextMeshProUGUI[7];

        // 데이터 출력을 위한 TextMeshPro 배열에 저장
        for(int i=0; i<5; i++)
        {
            statusTexts[i] = content.GetChild(1 + i).GetChild(2).GetComponent<TextMeshProUGUI>();
        }
        for(int i=0; i<2; i++)
        {
            statusTexts[5+i] = content.GetChild(8 + i).GetChild(2).GetComponent<TextMeshProUGUI>();
        }

        statusTexts[0].text = datas.GetMaxHp().ToString();
        statusTexts[1].text = datas.GetAttack().ToString();
        statusTexts[2].text = datas.GetDeffense().ToString();
        statusTexts[3].text = datas.GetElementNum().ToString();
        statusTexts[4].text = datas.GetStamina().ToString();
        statusTexts[5].text = datas.GetCriticalPercentage().ToString() + "%";
        statusTexts[6].text = datas.GetCriticalDamage().ToString();
    }

    #region 속성(Status) 프레임, 데이터 출력



    #endregion
    #endregion


    #region 무기 UI 관리

    #region 기본 데이터 출력

    // 장비의 기본적인 데이터 출력을 담당하는 함수
    void EquippedWeaponPrint()
    {
        // 무기 업글창 자식 객체 비활성화
        Transform screen = printInfoDataField[1].transform.GetChild(13).GetComponent<Transform>();    // 무기 업글 UI 오브젝트 인스턴스화
        screen.gameObject.SetActive(false);

        // 무기 데이터 가져오기
        ItemClass equippedData = GameManager.Instance.GetUserClass().GetUserEquippedWeapon();
        WeaponAndEquipCls equippedWeapon = equippedData as WeaponAndEquipCls; // 형변환

        if (equippedWeapon != null)
        {
            TextMeshProUGUI weaponNameTxt = printInfoDataField[1].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI weaponAttackPowerTxt = printInfoDataField[1].transform.GetChild(3).GetChild(2).GetComponent<TextMeshProUGUI>();
            Image[] weaponStars = printInfoDataField[1].transform.GetChild(5).GetComponentsInChildren<Image>();
            TextMeshProUGUI WeaponCurrentLevelTxt = printInfoDataField[1].transform.GetChild(6).GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI WeaponMaxLevelTxt = printInfoDataField[1].transform.GetChild(6).GetChild(2).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI weaponReforgeGradeTxt = printInfoDataField[1].transform.GetChild(7).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI weaponExplanTxt = printInfoDataField[1].transform.GetChild(9).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI weaponSkillTxt = printInfoDataField[1].transform.GetChild(8).GetComponent<TextMeshProUGUI>();
            Image WeaponImage = printInfoDataField[1].transform.GetChild(1).GetComponent<Image>();
            TextMeshProUGUI weaponStatusLabelTxt = printInfoDataField[1].transform.GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI weaponStatusTxt = printInfoDataField[1].transform.GetChild(4).GetChild(2).GetComponent<TextMeshProUGUI>();

            // 데이터 출력

            weaponNameTxt.text = equippedWeapon.GetName();  //이름
            weaponAttackPowerTxt.text = equippedWeapon.GetMainStat().ToString();    // 메인스텟(기초 공격력)
            
            // 성급 출력
            foreach(var tmp in weaponStars) { tmp.enabled=false; }
            int grade = equippedWeapon.GetGrade();
            weaponStars[0].enabled = true;
            for (int i= 0; i< grade; i++)
                weaponStars[1+i].enabled = true;

            WeaponCurrentLevelTxt.text = "LV."+equippedWeapon.GetLevel().ToString();    // 무기의 현재 레벨
            WeaponMaxLevelTxt.text = "/"+equippedWeapon.GetLimitLevel().ToString();     // 무기의 다음 레벨
            weaponReforgeGradeTxt.text = "재련 " + equippedWeapon.GetEffectLevel().ToString() + "단계";    //무기 재련 단계
            weaponExplanTxt.text = equippedWeapon.GetContent();
            weaponSkillTxt.text = equippedWeapon.GetEffectText();


            WeaponKindDivider(equippedWeapon, WeaponImage, weaponStatusLabelTxt, weaponStatusTxt);  // 무기 종류에 따라 분기하여 출력하는 함수 호출
        }

    }

    
    #endregion

    #region 무기 교체 버튼 클릭


    // 무기 교환 버튼 호출 함수
    public void WeaponChangeButtonClickEvent(ButtonClass2 clickedBtn)
    {
        if (isWeaponChangeBtnClicked) return;

        weaponChangeButton = clickedBtn;
        isWeaponChangeBtnClicked = true;

        InfoPrintTypeButtonUnActive();                          // 타입 선택 버튼 Active끄기
        CloseButtonSpriteToBack();

        // 스크롤뷰_콘텐트 객체 인스턴스화
        Transform scrollViewContentObj = printInfoDataField[1].transform.GetChild(12).GetChild(0).GetChild(0);
        clickedBtn.AlphaValueChangeing();                       // 클릭된 색상 유지
        WeaponPrintAtScroll(scrollViewContentObj, selected_SortOrder, isAscending, openUI_ItemList);              // 콘텐트에 UI 출력

        // 오브젝트 풀로 가져온 각 버튼에 Button이벤트를 Add함
        var objList = GameManager.Instance.WeaponItemPool.GetPoolList();
        foreach(var obj in objList )
        {
            if (obj.gameObject.activeSelf == false) continue;
            Button btn = obj.GetButton();
            if (btn != null)
            {
                btn.onClick.AddListener(() => WeaponChangeButtonEventHandler(obj));
            }
        }
    }

    // 아이템 교체를 위해서, InvenItemObjClass에 부착하는 클릭 이벤트 핸들러
    void WeaponChangeButtonEventHandler(InvenItemObjClass obj)
    {
        WeaponChangeFunction(obj.GetInvenItemObjClass());
    }

    // 아이템 교체 함수
    private void WeaponChangeFunction(InvenItemObjClass obj)
    {
        var cls = obj.GetItemcls();     //선택한 아이템 클래스

        if (cls.GetTag() != "무기")
            return;

        var WeaponsList = GameManager.Instance.GetUserClass().GetHadWeaponList();               // 보유하고 있는 아이템 리스트 인스턴스화

        ItemClass ExistingItem = GameManager.Instance.GetUserClass().GetUserEquippedWeapon();   // 기존 장착중인 아이템 가져오기
        ItemClass tmp = WeaponsList.Find(item => item.Equals(ExistingItem));                    // 기존 장착중인 아이템 리스트에서 찾기
        tmp.SetActive(false);                                                                   // 장착 중이던 아이템을 활성화 X

        ItemClass SelectItem = WeaponsList.Find(item => item.Equals(cls));              // 선택한 아이템 찾기
        SelectItem.SetActive(true);                                                     // 장착할 아이템 활성화 O
        GameManager.Instance.GetUserClass().SetUserEquippedWeapon(SelectItem);          // 선택한 아이템 장착


        // 새로 장착하여 변경된 아이템들을 UI에 출력
        Transform scrollViewContentObj = printInfoDataField[1].transform.GetChild(12).GetChild(0).GetChild(0);
        GameManager.Instance.WeaponItemPool.AllReturnToPool();  // 오브젝트 풀 리턴
        WeaponPrintAtScroll(scrollViewContentObj, selected_SortOrder, isAscending, openUI_ItemList);              // 콘텐트에 UI 출력
        EquippedWeaponPrint();
    }
    #endregion

    #region 무기 업그레이드_상세정보

    //  무기 업그레이드 버튼 클릭 시 호출 함수
    public void WeaponUpgeadeButtonClickEvent(int starter = 0)
    {
        if (isWeaponUpgradeBtnClicked)  // 이미 클릭된 상태라면 리턴
            return;

        if(isWeaponChangeBtnClicked)    // 무기 교체 버튼이 클릭 된 상태라면, 클릭과 관련된 출력 함수를 Reset하기 위해, 종료 버튼 호출
            ClosePlayerInfoScreenButton();

        // UI 세팅
        InfoPrintTypeButtonUnActive();
        CloseButtonSpriteToBack();

        Transform tmp = printInfoDataField[1].transform.GetChild(13).GetComponent<Transform>();    // 무기 업글 UI 오브젝트 인스턴스화
        tmp.gameObject.SetActive(true);
        isWeaponUpgradeBtnClicked = true;

        PlayerInfoUI_Button[] buttons = new PlayerInfoUI_Button[3];
        buttons = tmp.GetComponentsInChildren<PlayerInfoUI_Button>();          // 무기 업글 창에서 사용할 버튼 객체
        string[] texts = { "상세", "돌파", "재련" };



        // 무기 데이터 가져오기
        ItemClass equippedData = GameManager.Instance.GetUserClass().GetUserEquippedWeapon();
        WeaponAndEquipCls equippedWeapon = equippedData as WeaponAndEquipCls; // 형변환
        var weaponImage = tmp.GetChild(3).GetComponent<Image>();
        WeaponKindDivider(equippedWeapon, weaponImage); // 무기 일러스트 출력

        // 각 상세 기능 창을 제어하는 제어 플래그 초기화
        isWeaponUpgradeDetailObjClicked = new bool[3] { false, false, false };

        int index = 0;
        while (index < buttons.Length)
        {
            PlayerInfoUI_Button btnCls = buttons[index];
            btnCls.SetText(texts[index]);   // 텍스트 변경

            // 버튼에 연결되어 있는 기존의 리스너 이벤트 제거
            Button obj = btnCls.GetButton();
            obj.onClick.RemoveAllListeners();

            // 설정한 텍스트에 따라서, 분기하여 분기하여 클릭 이벤트 리스너 설정
            switch (btnCls.GetText())
            {
                case "상세":
                    Debug.Log(index);
                    obj.onClick.AddListener(() => WeaponUpgrade_DetailScreenButton(btnCls, equippedWeapon,tmp));
                    break;
                case "돌파":
                    Debug.Log(index);
                    obj.onClick.AddListener(() => WeaponUpgrade_LevelUpButton(btnCls, equippedWeapon, tmp));
                    break;
                case "재련":
                    Debug.Log(index);
                    obj.onClick.AddListener(() => WeaponUpgrade_ReforgeButton(btnCls, equippedWeapon, tmp));
                    break;
            }
            index++;

        }

        // 데이터 출력 Off
        for(int i=4; i<7; i++)
        {
            tmp.GetChild(i).GetComponent<Transform>().gameObject.SetActive(false);
        }
        // 디폴트 출력은 상세 정보 출력
        PlayerInfoUI_Button cls = buttons[starter];
        if(starter==0)
            WeaponUpgrade_DetailScreenButton(cls, equippedWeapon, tmp);
        else if(starter ==1)
            WeaponUpgrade_LevelUpButton(cls, equippedWeapon, tmp);
        else if(starter ==2)
            WeaponUpgrade_ReforgeButton(cls, equippedWeapon, tmp);
    }


    // 클릭한 객체를 제외한 나머지 객체의 UI 변경
    private void ClickedButtonsSetActiveReset(PlayerInfoUI_Button my, Transform parents, int CurrentIndex)
    {
        Transform tmp = printInfoDataField[1].transform.GetChild(13).GetComponent<Transform>();    // 무기 업글 UI 오브젝트 인스턴스화
        PlayerInfoUI_Button[] buttons = new PlayerInfoUI_Button[3];
        buttons = tmp.GetComponentsInChildren<PlayerInfoUI_Button>();          // 무기 업글 창에서 사용할 버튼 객체

        // 클릭 시 클릭한 객체를 제외한 나머지 UI 비활성화
        foreach (var button in buttons)
        {
            if (button.Equals(my))
            {
                if (button.GetClickedActive() == false)
                    button.OnOffSpriteSetting();
                continue;
            }

            if (button.GetIsCicked() == true)
                button.OnOffSpriteSetting();
        }

        // 데이터 출력 UI 오브젝트 활성화
        for (int i = 4; i < 7; i++)
        {
            var obj = parents.GetChild(i).GetComponent<Transform>();
            obj.gameObject.SetActive(false);

            if (i == CurrentIndex)
                obj.gameObject.SetActive(true);
        }
    }


    // 무기 상세정보 출력 함수
    private void WeaponUpgrade_DetailScreenButton(PlayerInfoUI_Button my, WeaponAndEquipCls equippedWeapon, Transform parents)
    {
        if (isWeaponUpgradeDetailObjClicked[0] == true)
            return;

        my.OnOffSpriteSetting();
        ClickedButtonsSetActiveReset(my, parents,4);
        var mainObject = parents.GetChild(4).GetComponent<Transform>();

        Transform scrollContent = mainObject.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Transform>();
        scrollContent.gameObject.SetActive(true);

        // 라벨 UI 인스턴스화
        Image labelBgr = scrollContent.transform.GetChild(0).GetComponent<Image>();
        TextMeshProUGUI labelText = labelBgr.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        labelText.text = equippedWeapon.GetName();                                       // 아이템 이름 출력

        // 탑 프레임 UI 인스턴스화
        Image TopFrame = scrollContent.transform.GetChild(1).GetComponent<Image>();         // 상단 프레임
        Image weaponImg = TopFrame.transform.GetChild(0).GetComponent<Image>();             // 무기 이미지
        Transform topContent = TopFrame.transform.GetChild(1).GetComponent<Transform>();
        TextMeshProUGUI subStatusLabel = topContent.transform.GetChild(1).GetComponent<TextMeshProUGUI>();  // 서브 스탯 라벨
        TextMeshProUGUI subStatusValue = topContent.transform.GetChild(2).GetComponent<TextMeshProUGUI>();  // 서브 스탯 값
        TextMeshProUGUI mainStatusLabel = topContent.transform.GetChild(3).GetComponent<TextMeshProUGUI>();  // 메인 스탯 라벨
        TextMeshProUGUI mainStatusValue = topContent.transform.GetChild(4).GetComponent<TextMeshProUGUI>();  // 메인 스탯 값
        Image[] weaponStars = topContent.transform.GetChild(5).GetComponentsInChildren<Image>();

        // 탑 프레임 데이터 출력
        ItemUISetterByItemGrade(equippedWeapon, labelBgr, TopFrame);                    // 아이템 등급별 백그라운드 세팅
        WeaponKindDivider(equippedWeapon, weaponImg, subStatusLabel, subStatusValue);    // 서브스텟 출력
        mainStatusLabel.text = "기초공격력";                                               // 메인 스텟 출력
        mainStatusValue.text = equippedWeapon.GetMainStat().ToString();                  // 메인 스텟 출력

        // 성급 출력
        foreach (var tmp in weaponStars) { tmp.enabled = false; }
        int grade = equippedWeapon.GetGrade();
        //weaponStars[0].enabled = true;
        for (int i = 0; i < grade; i++)
            weaponStars[i].enabled = true;



        // 바텀 프레임 UI 인스턴스화
        TextMeshProUGUI LevelTextVelue = scrollContent.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI LevelMaxText = scrollContent.GetChild(2).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ReforgeLevelText = scrollContent.GetChild(2).GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ReforgeGreadLevelText = scrollContent.GetChild(2).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI SkillContentText = scrollContent.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ContentText = scrollContent.GetChild(2).GetChild(3).GetComponent<TextMeshProUGUI>();

        // 바텀 프레임 데이터 출력
        LevelTextVelue.text = "LV."+equippedWeapon.GetLevel().ToString();
        LevelMaxText.text = "/ "+ equippedWeapon.GetLimitLevel().ToString();
        ReforgeLevelText.text = equippedWeapon.GetEffectLevel().ToString();
        ReforgeGreadLevelText.text = "재련 " + equippedWeapon.GetEffectLevel().ToString() + "단계";
        SkillContentText.text = equippedWeapon.GetEffectText();
        ContentText.text = equippedWeapon.GetContent();


        // 지역 변수 초기화
        dic_ItemClsForWeaponUpgrade.Clear();
        nWeaponUpgradeCost = 0;
        nWeaponUpgradeExp = 0;

        // 제어 플래그 수정
        isWeaponUpgradeDetailObjClicked[0] = true;
        isWeaponUpgradeDetailObjClicked[1] = false;
        isWeaponUpgradeDetailObjClicked[2] = false;

        // 오브젝트 풀 리턴 - LevelUpBuytton
        ResetToSelectButtonScriptPoolDatas();
    }

    #endregion

    #region 무기 업그레이드_레벨업

    // 무기 레벨업 UI 출력 함수
    private void WeaponUpgrade_LevelUpButton(PlayerInfoUI_Button my, WeaponAndEquipCls equippedWeapon, Transform parents)
    {
        if (isWeaponUpgradeDetailObjClicked[1] == true)
            return;

        my.OnOffSpriteSetting();
        ClickedButtonsSetActiveReset(my, parents, 5);
        var mainObject = parents.GetChild(5).GetComponent<Transform>();

        // 모라 TextMeshPro 인스턴스화
        txt_NeedMora = mainObject.GetChild(7).GetChild(1).GetComponent<TextMeshProUGUI>();
        txt_CurrentHaveMora = mainObject.GetChild(8).GetChild(1).GetComponent<TextMeshProUGUI>();
        txt_CurrentHaveMora.text = GameManager.Instance.GetUserClass().GetMora().ToString();

        // 데이터 초기화
        dic_ItemClsForWeaponUpgrade = new Dictionary<ItemClass, SelectButtonScript>();
        nWeaponUpgradeExp = 0;
        nWeaponUpgradeCost = 0;

        // 스텟 표기 UI 오브젝트
        Image MainStatBgr = mainObject.GetChild(0).gameObject.GetComponent<Image>();
        Image SubStatBgr = mainObject.GetChild(1).gameObject.GetComponent<Image>();
        Image[] statImgs = { MainStatBgr, SubStatBgr };

        // 각 속성 별 UI 오브젝트
        Transform LevelLimitBreak_Obj = mainObject.GetChild(5).GetComponent<Transform>();
        Transform LevelUpBreak_Obj = mainObject.GetChild(6).GetComponent<Transform>();
        
        // 사용버튼 오브젝트
        ButtonClass2 useBtn = mainObject.GetChild(2).GetComponent<ButtonClass2>();
        var useButtonObj = useBtn.GetButton();

        // 팝업 오브젝트 저장
        popUpForUpgrade = mainObject.GetChild(9).GetComponent<Transform>();
        popUpForUpgrade.gameObject.SetActive(false);

        int curLevelData = equippedWeapon.GetLevel();
        int maxLevelData = equippedWeapon.GetLimitLevel();
        int WeaponGrade = equippedWeapon.GetGrade();

        // 상세 정보는 레벨에 따라 분기하여 출력하므로, 우선 SetFalse함
        LevelLimitBreak_Obj.gameObject.SetActive(false);
        LevelUpBreak_Obj.gameObject.SetActive(false);

        // 현재레벨 == 한계레벨 (돌파요구)
        if (curLevelData == maxLevelData && equippedWeapon.GetCurrentExp()>= equippedWeapon.GetMaxExp())
        {
            LevelLimitBreak_Obj.gameObject.SetActive(true);


            // 상세 정보 UI 인스턴스화
            TextMeshProUGUI currentLevel = LevelLimitBreak_Obj.GetChild(5).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI LimitLevel = LevelLimitBreak_Obj.GetChild(7).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI LimitLevel2 = LevelLimitBreak_Obj.GetChild(8).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI[] levelTxts = { currentLevel, LimitLevel,LimitLevel2 };

            Image ArrawImg = LevelLimitBreak_Obj.GetChild(9).GetComponent<Image>();
            Transform SelectButtonPos_1 = LevelLimitBreak_Obj.GetChild(0).GetComponent<Transform>();
            Transform SelectButtonPos_2 = LevelLimitBreak_Obj.GetChild(1).GetComponent<Transform>();
            Transform SelectButtonPos_3 = LevelLimitBreak_Obj.GetChild(2).GetComponent<Transform>();
            isLimitBreakPossible = true;

            // 스탯 및 데이터 출력
            WeaponLevelUp_UI_Applyer(equippedWeapon, statImgs, levelTxts, ArrawImg);

            SelectButtonScript[] btnObjArr = new SelectButtonScript[3];
            btnObjArr[0] = GameManager.Instance.SelectButtonScriptPool.GetFromPool(Vector3.zero,Quaternion.identity,SelectButtonPos_1);
            btnObjArr[1] = GameManager.Instance.SelectButtonScriptPool.GetFromPool(Vector3.zero,Quaternion.identity,SelectButtonPos_2);
            btnObjArr[2] = GameManager.Instance.SelectButtonScriptPool.GetFromPool(Vector3.zero,Quaternion.identity,SelectButtonPos_3);

            // 각 버튼에 재료 UI 세팅
            WeaponLimitBreakResourcePrintUI(btnObjArr, equippedWeapon);

            // 원하는 위치로 이동하려면 Anchors 및 Pivot 조정
            for (int i = 0; i < btnObjArr.Length; i++)
            {
                // 버튼 클릭 이벤트 함수 연결
                btnObjArr[i].GetComponentInChildren<Button>().onClick.AddListener(() => WeaponLimitBreakResourcePrintButtonClickLeitener(btnObjArr[i]));
                RectTransform btnObjRectTransform = btnObjArr[i].GetComponent<RectTransform>();
                btnObjRectTransform.anchoredPosition = new Vector2(70f, -80f);
                btnObjRectTransform.pivot = new Vector2(0.5f, 0.5f); // Pivot을 UI 요소 중심으로 설정
            }
            useButtonObj.onClick.RemoveAllListeners();
            ButtonClass2_Reset(useBtn);
            useButtonObj.onClick.AddListener(() => WeaponLimitBreakClickEventListener(equippedWeapon));
            useBtn.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "돌파";

        }
        // 현재레벨 != 한계레벨 (레벨업 가능)
        else
        {
            LevelUpBreak_Obj.gameObject.SetActive(true);

            // 상세 정보 UI 인스턴스화
            ButtonClass2 AllSelectButton = LevelUpBreak_Obj.GetChild(0).GetComponent<ButtonClass2>();
            ButtonClass2 SelectSortButton = LevelUpBreak_Obj.GetChild(1).GetComponent<ButtonClass2>();
            InventorySortSelectButton SelectionButtonOnList = LevelUpBreak_Obj.GetChild(2).GetComponent<InventorySortSelectButton>();
            TextMeshProUGUI LevelText = LevelUpBreak_Obj.GetChild(4).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI LevelExpText = LevelUpBreak_Obj.GetChild(5).GetComponent<TextMeshProUGUI>();
            Image ExpProgressBar_Outline = LevelUpBreak_Obj.GetChild(6).GetComponent<Image>();
            Image ExpProgressBar_Inline = LevelUpBreak_Obj.GetChild(7).GetComponent<Image>();
            img_ExpBar = ExpProgressBar_Inline;
            Transform Select_Item_Scroll_Content = LevelUpBreak_Obj.GetChild(3).GetChild(0).GetChild(0).GetComponent<Transform>();
            OpenScrollToUpgradeMaterial = LevelUpBreak_Obj.GetChild(8).GetComponent<Transform>();

            // 사용 버튼 UI 수정 및 이벤트 리스너 연결

            TextMeshProUGUI[] levelTxts = { LevelText, LevelExpText };
            useButtonObj.onClick.RemoveAllListeners();
            ButtonClass2_Reset(useBtn);
            useButtonObj.onClick.AddListener(() => WeaponLevelUpClickEventListener(statImgs, levelTxts));
            useBtn.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "강화";

            // 모라 UI 출력
            var weaponTemp = GameManager.Instance.GetUserClass().GetUserEquippedWeapon() as WeaponAndEquipCls;
            NeededCostForUpgrade(nWeaponUpgradeExp, weaponTemp.GetMaxExp() - weaponTemp.GetCurrentExp(), ref nWeaponUpgradeCost, txt_NeedMora,txt_CurrentHaveMora,img_ExpBar);

            // SelectSortButton 클릭 이벤트 연결
            SelectSortButton.GetButton().onClick.AddListener(() => SelectSortButtonClickEventListener(SelectionButtonOnList, SelectSortButton));
            SelectSortButton.SetButtonTextInputter("★3 재료");

            // 일괄 처리 버튼  클릭 이벤트 리스너 연결
            AllSelectButton.GetButton().onClick.AddListener(() => AllClickWeaponButtonForUpgradeClickEventLeistener());

            // 주스탯 및 서브 스탯 & EXP와 레벨 출력
            WeaponLevelUp_UI_Applyer(equippedWeapon,statImgs, levelTxts);

            // exp바 출력
            ExpBarAlphaRevisePerpect(ExpProgressBar_Inline, equippedWeapon);


            // 리스트 버튼은 기본 비활성화
            string[] strings = { "★3 재료", "★4 재료", "★5 재료" };
            ListButtonObject_UI_Initer(SelectionButtonOnList, strings, e_SortingOrder.NameOrder);
            allPull_OrderValue = e_SortingOrder.NameOrder;                      // 디폴트는 3성 재료

            // 스크롤뷰 콘텐츠 채우기
            WeaponPrintAtScroll_BySelectButtonOnDefualtValue(Select_Item_Scroll_Content, 10);
            List<SelectButtonScript> selectButtonScripts = GameManager.Instance.SelectButtonScriptPool.GetPoolList();

            // 오브젝트 풀로 채운 스크롤뷰 버튼에 이벤트 리스너 연결
            foreach (var tmp in selectButtonScripts)
            {
                Button btn = tmp.GetButton();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => tmp.OnClickEventListener());
                btn.onClick.AddListener(() => ScrollViewOpenButtonClickEvent(tmp, OpenScrollToUpgradeMaterial));
                btn.onClick.AddListener(() => WeaponSelectForUpgradeBtnClickEventListener(tmp, OpenScrollToUpgradeMaterial));
            }

            // 업글용 무기 재료 출력 스크롤은 기본 SetFalse
            OpenScrollToUpgradeMaterial.gameObject.SetActive(false);

        }

        // 제어 플래그 수정
        isWeaponUpgradeDetailObjClicked[1] = true;
        isWeaponUpgradeDetailObjClicked[0] = false;
        isWeaponUpgradeDetailObjClicked[2] = false;

        Debug.Log("돌파 버튼 클릭");
    }


    /** 무기 레벨업 파트
     *  주요 기능1 - 무기의 레벨업 및 레벨업에 따른 무기 강화와 데이터 수정
     *  주요 기능2 - 무기를 한번에 자원으로 input할 일괄처리 기능
     *  주요 기능3 - 스크롤 뷰 상의 무기 정렬
     *  
     *  SelectButtonScript => 플레이어가 직접 재화로 소모할 무기를 선택
     *  InvenItemObjClass => 무기 선택 버튼 클릭 시, 출력되는 스크롤뷰에 부착되는 컴포넌트
     *  
     *  기능 구현 순서
     *  하이레벨 = 선택버튼 / 일괄처리 버튼 / 정렬 선택 버튼 / 사용 버튼
     *  미드레벨 = 선택버튼_스크롤뷰_버튼Obj / 정렬 선택 버튼_리스트 버튼 객체 기능 구현(일괄처리의 정렬 우선순위)
     *  로우레벨 =  선택버튼_스크롤뷰_버튼Obj_정렬 우선순위 선택 버튼
     */

    // 선택 버튼 파트
    // WeaponPrintAtScroll_BySelectButton _ 스크롤뷰 _ 콘텐츠 버튼(SelectButton) 이벤트 리스너
    void WeaponSelectForUpgradeBtnClickEventListener(SelectButtonScript slectBtnCls, Transform objectSet)
    {
        ButtonClass2 SortSelectButton = objectSet.GetChild(2).GetComponent<ButtonClass2>();                                       // 정렬 선택 버튼(리스트 버튼 On/Off)
        ButtonClass2 Descending_AscendingButton = objectSet.GetChild(3).GetComponent<ButtonClass2>();                             // 내림차순/오름차순 버튼
        InventorySortSelectButton SelectionButtonOnList = objectSet.GetChild(4).GetComponent<InventorySortSelectButton>();        // 리스트 버튼

        // 버튼 리스너 연결
        SortSelectButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        SortSelectButton.GetComponentInChildren<Button>().onClick.AddListener(()
            => WeaponPrintAtScroll_ForUpgrade_SortOrderBtnClick(SelectionButtonOnList, SortSelectButton));

        Descending_AscendingButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        Descending_AscendingButton.GetComponentInChildren<Button>().onClick.AddListener(WeaponPrintAtScroll_ForUpgrade_AscendingBtnClick);
        Descending_AscendingButton.GetComponentInChildren<Button>().onClick.AddListener(()=> PrintDataAtScrollViewForUpMaterials(selectBtn_ForUpgrade));

        // 정렬 디폴트 값 및 리스트 버튼 데이터 초기화
        weaponList_ForItemUpgradeResourceSortOrder = e_SortingOrder.ExpOrder;
        isWeaponList_ForUpgradeResourceSortOrderAscending = false;
        string[] strings = { "희귀도","레벨","기초경험치" };
        ListButtonObject_UI_Initer(SelectionButtonOnList, strings,e_SortingOrder.ExpOrder);
        SelectionButtonOnList.HideButtonBackGround(2);

        // 스크롤뷰 데이터 출력
        PrintDataAtScrollViewForUpMaterials(slectBtnCls);
    }

    private void PrintDataAtScrollViewForUpMaterials(SelectButtonScript slectBtnCls)
    {
        // 스크롤뷰 데이터 출력
        WeaponPrintAtScroll(scrollForUpgradeMaterials, weaponList_ForItemUpgradeResourceSortOrder, isWeaponList_ForUpgradeResourceSortOrderAscending, openUI_ItemList);

        var datas = GameManager.Instance.WeaponItemPool.GetPoolList();

        // 스크롤뷰 버튼 오브젝트 순회하며, 이벤트 리스너 연결 및 UI 데이터 초기화
        foreach (var tmp in datas)
        {
            // 비활성화되어 있는 객체는 스킵
            if (tmp.gameObject.activeSelf == false)
                continue;

            // 장비하고 있는 아이템은 제외
            if (tmp.GetItemcls().GetIsActive() == true)
            {
                GameManager.Instance.WeaponItemPool.ReturnToPool(tmp);
                continue;
            }

            if (dic_ItemClsForWeaponUpgrade.ContainsKey(tmp.GetItemcls()))
                tmp.SetSelectBtnSpriteOn(true);

            // 버튼 클릭 이벤트 리스너 해제 및 연결
            Button btn = tmp.GetButton();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => ClickMaterialItemForWeaponUpgrade(tmp, slectBtnCls));
            tmp.EquippedItemUIPrint(false); // 장비 중인 아이템 표시용 이미지는 필요 없으므로 False
        }
    }


    /* 순서
     * 기초경험치 == Button[2] == Grade
     * 레벨      == Button[1] == Level
     * 희귀도    == Button[0] == Name
     */
    // 정렬 버튼 파트(일괄처리 _ )
    // WeaponPrintAtScroll_BySelectButton _ 스크롤뷰 _ 정렬 버튼 이벤트 리스너
    private void WeaponPrintAtScroll_ForUpgrade_SortOrderBtnClick(InventorySortSelectButton selectBtnListObj, ButtonClass2 selectBtn)
    {
        selectBtnListObj.gameObject.SetActive(true);

        Button[] buttons = selectBtnListObj.GetButtons();

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.RemoveAllListeners();
        }

        // 리스트 버튼 객체에 이벤트 리스너 
        buttons[0].onClick.AddListener(() => SortOrderForUpgradeResorceWeapon(selectBtnListObj, "희귀도", selectBtn)); // 희귀도
        buttons[0].onClick.AddListener(() => PrintDataAtScrollViewForUpMaterials(selectBtn_ForUpgrade)); // 희귀도

        buttons[1].onClick.AddListener(() => SortOrderForUpgradeResorceWeapon(selectBtnListObj, "레벨", selectBtn)); // 레벨
        buttons[1].onClick.AddListener(() => PrintDataAtScrollViewForUpMaterials(selectBtn_ForUpgrade)); // 레벨

        buttons[2].onClick.AddListener(() => SortOrderForUpgradeResorceWeapon(selectBtnListObj, "기초경험치", selectBtn)); // 기초경험치
        buttons[2].onClick.AddListener(() => PrintDataAtScrollViewForUpMaterials(selectBtn_ForUpgrade)); // 기초경험치
    }


    // WeaponPrintAtScroll_BySelectButton _ 스크롤뷰 _ 정렬_리스트 버튼 이벤트 리스너
    private void SortOrderForUpgradeResorceWeapon(InventorySortSelectButton selectBtnListObj, string index ,ButtonClass2 selectBtn)
    {
        e_SortingOrder tmp = e_SortingOrder.ExpOrder;
        int indexNum = 0;
        if (index == "희귀도")
        {
            tmp = e_SortingOrder.GradeOrder;
            indexNum = 0;
        }
        else if (index == "레벨")
        {
            tmp = e_SortingOrder.LevelOrder;
            indexNum = 1;
        }
        else if (index == "기초경험치")
        {
            tmp = e_SortingOrder.ExpOrder;
            indexNum = 2;

        }
        selectBtnListObj.SetSortingOrder(tmp);
        weaponList_ForItemUpgradeResourceSortOrder = tmp;
        Debug.Log("allPull_OrderValue" + " == " + weaponList_ForItemUpgradeResourceSortOrder);

        selectBtnListObj.HideButtonBackGround(indexNum);
        selectBtnListObj.gameObject.SetActive(false);  // 비활성화
        selectBtn.SetButtonTextInputter(index);

        ResetToWeaponItemObjectPoolDatas();
    }


    // WeaponPrintAtScroll_BySelectButton _ 스크롤뷰 _ 정렬_오름차/내림차순 버튼 이벤트 리스너
    private void WeaponPrintAtScroll_ForUpgrade_AscendingBtnClick()
    {
        isWeaponList_ForUpgradeResourceSortOrderAscending = !isWeaponList_ForUpgradeResourceSortOrderAscending;

        ResetToWeaponItemObjectPoolDatas();
    }

    /*
     * nWeaponUpgradeExp
       dic_ItemClsForWeaponUpgrade 변수에 선택한 객체의 데이터 저장 후, 사용 버튼 시에, 해당 아이템을 강화 재료로 씀.
     */
    // WeaponPrintAtScroll_BySelectButton _ 스크롤뷰 _ 콘텐츠 버튼(SelectButton) - 스크롤뷰 부착 오브젝트 클릭 이벤트 리스너
    void ClickMaterialItemForWeaponUpgrade(InvenItemObjClass cls, SelectButtonScript selectBtnCls)
    {
        ItemClass tmp = cls.GetItemcls();
        WeaponAndEquipCls selecWeapon = tmp as WeaponAndEquipCls;

        // 처음 선택하는 객체일 경우,
        if(dic_ItemClsForWeaponUpgrade.ContainsKey(tmp) == false)
        {

            var equipWeapon = GameManager.Instance.GetUserClass().GetUserEquippedWeapon() as WeaponAndEquipCls;
            int needExp = equipWeapon.GetMaxExp() - equipWeapon.GetCurrentExp();

            if (nWeaponUpgradeExp >= needExp)   // 현재 누적 경험치가 요구 경험치를 초과 시, 리턴
            {
                PopUpScreenFunc(popUpForUpgrade, "최대 경험치에 도달했습니다!");
                return;
            }

            if (selectBtnCls.GetIsActive() == false)
            {

                // 리스트에 데이터 추가 및 exp 증가
                dic_ItemClsForWeaponUpgrade.Add(tmp, selectBtnCls);
                nWeaponUpgradeExp += selecWeapon.GetCurrentExp();
                cls.SetSelectBtnSpriteOn(true); // 선택했음을 UI로 표현
                NeededCostForUpgrade(nWeaponUpgradeExp, needExp, ref nWeaponUpgradeCost, txt_NeedMora,txt_CurrentHaveMora, img_ExpBar);

                // 버튼 객체의 UI 작업
                selectBtnCls.SetItemCls(tmp);
                selectBtnCls.SetIsActive(true);
                selectBtnCls.GetItemImage().enabled = true;
                WeaponKindDivider(selecWeapon, selectBtnCls.GetItemImage());
                selectBtnCls.SetItemText("LV. " + selecWeapon.GetLevel().ToString());
                ItemUISetterByItemGrade(tmp, selectBtnCls.GetTopBgr(), null);
            }
            else
            {
                // 오브젝트 풀의 리스트를 순회하며 빈 객체에 데이터 추가.
                var list = GameManager.Instance.SelectButtonScriptPool.GetPoolList();
                foreach(var data in list)
                {
                    if (data.GetIsActive()) // 이미 데이터를 가지고 있는 버튼의 경우, 넘어간다
                        continue;

                    // 선택한 데이터 추가
                    dic_ItemClsForWeaponUpgrade.Add(tmp, data);
                    nWeaponUpgradeExp += selecWeapon.GetCurrentExp();
                    cls.SetSelectBtnSpriteOn(true);
                    NeededCostForUpgrade(nWeaponUpgradeExp, needExp, ref nWeaponUpgradeCost, txt_NeedMora,txt_CurrentHaveMora, img_ExpBar);

                    // 버튼 객체의 UI 작업
                    data.SetItemCls(tmp);
                    data.SetIsActive(true);
                    data.GetItemImage().enabled = true;
                    WeaponKindDivider(selecWeapon, data.GetItemImage());
                    data.SetItemText("LV. " + selecWeapon.GetLevel().ToString());
                    ItemUISetterByItemGrade(tmp, data.GetTopBgr(), null);
                    break;
                }
            }
        }
        else
        {
            if (cls.GetItemcls() == selectBtnCls.GetItemClass())
            {
                WeaponAndEquipCls existCls = selectBtnCls.GetItemClass() as WeaponAndEquipCls;
                // 선택한 객체가 기존의 데이터와 동일한 객체일 경우에는 제외한다.
                nWeaponUpgradeExp -= existCls.GetCurrentExp();
                dic_ItemClsForWeaponUpgrade.Remove(selectBtnCls.GetItemClass());

                SelectButtonToDefault(selectBtnCls);

                cls.SetSelectBtnSpriteOn(false);
            }
            else
            {
                var datas = GameManager.Instance.WeaponItemPool.GetPoolList();
                foreach (var item in datas)
                {
                    if (item.GetItemcls() == cls.GetItemcls())
                    {
                        WeaponAndEquipCls existCls = item.GetItemcls() as WeaponAndEquipCls;
                        nWeaponUpgradeExp -= existCls.GetCurrentExp();
                        var btnObj = dic_ItemClsForWeaponUpgrade[item.GetItemcls()];
                        SelectButtonToDefault(btnObj);
                        dic_ItemClsForWeaponUpgrade.Remove(item.GetItemcls());


                        item.SetSelectBtnSpriteOn(false);
                        break;
                    }
                }
            }

            if(dic_ItemClsForWeaponUpgrade.Count<=0)
            {
                WeaponAndEquipCls originItem = GameManager.Instance.GetUserClass().GetUserEquippedWeapon() as WeaponAndEquipCls;
                ExpBarAlphaRevisePerpect(img_ExpBar, originItem);
            }
        }

        Debug.Log(nameof(nWeaponUpgradeExp )+":"+ nWeaponUpgradeExp);
        Debug.Log(nameof(dic_ItemClsForWeaponUpgrade.Count) +":"+ dic_ItemClsForWeaponUpgrade.Count);
    }


    /* 순서
     * 5성 == Button[2] == Grade
     * 4성 == Button[1] == Level
     * 3성 == Button[0] == Name
     */
    // 정렬 버튼 파트(일괄처리 _ )
    // 무기 강화창(레벨업 파트) - SelectSortButton 버튼의 이벤트 리스너
    void SelectSortButtonClickEventListener(InventorySortSelectButton SelectionButtonOnList, ButtonClass2 SelectSortButton)
    {
        // 리스트 버튼은 기본 활성화
        SelectionButtonOnList.gameObject.SetActive(true);
        Button[] buttons = SelectionButtonOnList.GetButtons();

        for (int i=0; i<buttons.Length; i++)
        {
            buttons[i].onClick.RemoveAllListeners();
        }
        // 리스트 버튼 객체에 이벤트 리스너 연결
        buttons[0].onClick.AddListener(() => StarOrderButtonClickListener(SelectionButtonOnList, 3, SelectSortButton));
        buttons[1].onClick.AddListener(() => StarOrderButtonClickListener(SelectionButtonOnList, 4, SelectSortButton));
        buttons[2].onClick.AddListener(() => StarOrderButtonClickListener(SelectionButtonOnList, 5, SelectSortButton));


    }
    // 무기 강화창(레벨업 파트) - 리스트 버튼 객체의 선택한 버튼 객체의 함수.
    void StarOrderButtonClickListener(InventorySortSelectButton SelectionButtonOnList, int selectNum, ButtonClass2 SelectSortButton)
    {
        e_SortingOrder tmp = e_SortingOrder.GradeOrder;
        if (selectNum == 3) tmp = e_SortingOrder.NameOrder;                 // 3성 이하 무기 일괄처리
        else if (selectNum == 4) tmp = e_SortingOrder.LevelOrder;           // 4성 이하 무기 일괄처리
        else if (selectNum == 5) tmp = e_SortingOrder.GradeOrder;           // 5성 이하 무기 일괄처리

        SelectionButtonOnList.SetSortingOrder(tmp);
        allPull_OrderValue = tmp;
        Debug.Log("allPull_OrderValue" + " == " + allPull_OrderValue);
        SelectionButtonOnList.HideButtonBackGround();
        SelectionButtonOnList.gameObject.SetActive(false);  // 비활성화
        NotifySelectStarOrderEventListener(selectNum, SelectSortButton);
    }
    void NotifySelectStarOrderEventListener(int selectNum, ButtonClass2 SelectSortButton)
    {
        Debug.Log("회귀도 : " + selectNum);
        SelectSortButton.SetButtonTextInputter("★"+ selectNum + " 재료");
    }


    // 일괄처리 파트
    // 무기 강화창(레벨업 파트) - 일괄처리 버튼 이벤트 리스너
    void AllClickWeaponButtonForUpgradeClickEventLeistener()
    {
        var selectButtonList = GameManager.Instance.SelectButtonScriptPool.GetPoolList();
        var weaponList = GameManager.Instance.GetUserClass().GetHadWeaponList();
        var equipedWeapon = GameManager.Instance.GetUserClass().GetUserEquippedWeapon() as WeaponAndEquipCls;

        if (nWeaponUpgradeExp >= equipedWeapon.GetMaxExp() - equipedWeapon.GetCurrentExp())
        {
            PopUpScreenFunc(popUpForUpgrade, "최대 경험치에 도달했습니다!");
            return;
        }

        // 보유하고 있는 아이템을 순회하며, 데이터를 출력한다
        foreach (var item in weaponList)
        {
            if (item.GetIsActive() == true)
                continue;

            // 누적 경험치가 요구 경험치 이상이라면 반복문 탈출
            if (nWeaponUpgradeExp >= equipedWeapon.GetMaxExp() - equipedWeapon.GetCurrentExp())
            {
                NeededCostForUpgrade(nWeaponUpgradeExp, equipedWeapon.GetMaxExp() - equipedWeapon.GetCurrentExp(), ref nWeaponUpgradeCost, txt_NeedMora, txt_CurrentHaveMora, img_ExpBar);
                break;
            }

            // 선택한 일괄처리 등급에 따라서, 아이템 pulling 변경
            switch(allPull_OrderValue)
            {
                case e_SortingOrder.NameOrder:            // 3성 이하
                    if (item.GetGrade() >= 4)
                        continue;
                    break;
                case e_SortingOrder.LevelOrder:            // 4성 이하
                    if (item.GetGrade() >= 5)
                        continue;
                    break;
                case e_SortingOrder.GradeOrder:             // 5성 이하
                    break;
            }

            foreach (var btn in selectButtonList)
            {
                if (btn.gameObject.activeSelf == false) continue;
                if (btn.GetIsActive() == true)      // 액티브가 활성화라면, 이미 재화로써 선택한 무기임
                    continue;
                //bool isActive = false;
                if (dic_ItemClsForWeaponUpgrade.ContainsKey(item) == false)
                {
                    var weaponData = item as WeaponAndEquipCls;
                    // 선택한 데이터 추가
                    dic_ItemClsForWeaponUpgrade.Add(item, btn);
                    nWeaponUpgradeExp += weaponData.GetCurrentExp();

                    // 버튼 객체의 UI 작업
                    btn.SetItemCls(item);
                    btn.SetIsActive(true);
                    btn.GetItemImage().enabled = true;
                    WeaponKindDivider(weaponData, btn.GetItemImage());
                    btn.SetItemText("LV. " + weaponData.GetLevel().ToString());
                    ItemUISetterByItemGrade(item, btn.GetTopBgr(), null);
                    NeededCostForUpgrade(nWeaponUpgradeExp, equipedWeapon.GetMaxExp() - equipedWeapon.GetCurrentExp(), ref nWeaponUpgradeCost, txt_NeedMora, txt_CurrentHaveMora, img_ExpBar);
                    break;
                }
            }
        }
    }

    // 레벨업 파트
    // 레벨업(강화)_by사용 버튼 이벤트 리스너
    void WeaponLevelUpClickEventListener(Image[] statImages, TextMeshProUGUI[] levelTexts)
    {
        int mora = GameManager.Instance.GetUserClass().GetMora();
        if (mora >= nWeaponUpgradeCost)
        {
            var weaponCls = GameManager.Instance.GetUserClass().GetUserEquippedWeapon() as WeaponAndEquipCls;
            // 돌파 요구 단계에 도달 시, true를 반환 받는다.
            bool isLimit = WeaponExpUp_Upgrade(weaponCls,nWeaponUpgradeExp);   // 무기 경험치 업그레이드 클래스 호출하기
            Debug.Log(nameof(isLimit) + ":" + isLimit);

            // 유저 보유 모라 가져오기
            int haveMora = GameManager.Instance.GetUserClass().GetMora();
            haveMora -= nWeaponUpgradeCost;
            // 모라 최신화
            GameManager.Instance.GetUserClass().SetMora(haveMora);

            // 누적 경험치 초기화
            nWeaponUpgradeExp = 0;

            // 무기 데이터 삭제
            foreach(var tmp in dic_ItemClsForWeaponUpgrade)
            {
                ItemClass item = tmp.Key;
                GameManager.Instance.GetUserClass().GetHadWeaponList().Remove(item);
                SelectButtonToDefault(tmp.Value);
            }

            // 선택버튼에서 무기 UI 삭제
            dic_ItemClsForWeaponUpgrade.Clear();


            // UI최신화
            if (isLimit)    // 돌파 단계 도달 시,
            {
                //isWeaponUpgradeBtnClicked = false;
                ResetToWeaponItemObjectPoolDatas();
                ResetToSelectButtonScriptPoolDatas();
                ClosePlayerInfoScreenButton();
                WeaponUpgeadeButtonClickEvent(1);   // 돌파 UI 출력을 위한 출력 함수 재호출
            }
            else
            {
                WeaponLevelUp_UI_Applyer(weaponCls,statImages, levelTexts);
                NeededCostForUpgrade(nWeaponUpgradeExp, 0, ref nWeaponUpgradeCost , txt_NeedMora, txt_CurrentHaveMora);
                ExpBarAlphaRevisePerpect(img_ExpBar, weaponCls);
            }
        }
    }




    /** 무기 돌파 파트
     *  주요 기능1 - 돌파 후, 다음 능력치 출력. 돌파에 필요한 재료들을 출력
     *  주요 기능2 - 재료 조건 검사 후, 관련 클래스와 연계하여 무기 돌파 기능 구현
     *  
     *  WeaponLimitBreakResourcePrintButtonClickLeitener - 무기 돌파 재료 출력 UI 객체의 버튼(재화 획득 위치 출력 등과 연계 가능)
     *  ㄴ 아직 미구현
     *  
     */

    // 무기 돌파 재료 UI 출력 함수
    private void WeaponLimitBreakResourcePrintUI(SelectButtonScript[] btnUIs, WeaponAndEquipCls weaponCls)
    {
        var refItem = GameManager.Instance.GetList_WeaponAndEquipLimitBreakResourceData();
        var refList = refItem.Find(tmp => tmp.Item1.Equals(weaponCls.GetName()));
        var dataList = refList.Item2.FindAll(tmp => tmp.TARGET_ITEM_LEVEL.Equals(weaponCls.GetLimitLevel()));

        for(int i=0; i< btnUIs.Length; i++)
        {
            var itemCls = dataList[i].RESOURCE_ITEM;
            int num = dataList[i].RESOURCE_NUMBER;
            Sprite spriteImg = WeaponAndEquipLimitBreak_UI_Dvider(itemCls);

            btnUIs[i].SetIsActive(true);
            btnUIs[i].SetItemCls(itemCls);                   // 클래스 세팅
            btnUIs[i].SetItemSprite(spriteImg);              // 아이템 이미지 세팅
            btnUIs[i].SetItemColor(itemCls.GetGrade());      // 색상 변환
            btnUIs[i].SetItemText(num.ToString());           // 개수 출력
            btnUIs[i].GetItemImage().enabled= true;

            // 보유 여부 및 개수 판단으로 돌파 가능 여부 검사
            var userData = GameManager.Instance.GetUserClass().GetHadGrowMaterialList().Find(tmp => tmp.GetName().Equals(itemCls.GetName()));
            if (userData != null)
            {
                int haveNum = userData.GetNumber();
                if (haveNum < num)
                {
                    isLimitBreakPossible = false;
                    btnUIs[i].GetItemTxt().color = Color.red;
                }
                btnUIs[i].GetItemTxt().color = Color.green;
                // 등급에 비례해서 요구 가격 계산
                nWeaponUpgradeCost += (int)(haveNum * userData.GetGrade() * 1000);
            }
            else
            {
                isLimitBreakPossible = false;
                btnUIs[i].GetItemTxt().color = Color.red;
            }
        }
    }

    // 무기 돌파 재료 출력 버튼 클릭 함수
    private void WeaponLimitBreakResourcePrintButtonClickLeitener(SelectButtonScript btnCls)
    {
        
    }



    // 레벨업(강화)_by돌파 버튼 이벤트 리스너
    void WeaponLimitBreakClickEventListener(WeaponAndEquipCls weapon)
    {
        // 필요 모라 조건 검사 및, 돌파 가능 여부 확인 후 돌파 진행
        int mora = GameManager.Instance.GetUserClass().GetMora();
        if(mora >= nWeaponUpgradeCost && isLimitBreakPossible == true)
        {
            WeaponLimitBreakFunction(weapon);
            WeaponExpUp_Upgrade(weapon, 0);         // 경험치 세팅을 위한, Exp업그레이드 함수 호출
            
            // 유저 보유 모라 가져오기
            int haveMora = GameManager.Instance.GetUserClass().GetMora();
            haveMora -= nWeaponUpgradeCost;
            // 모라 최신화
            GameManager.Instance.GetUserClass().SetMora(haveMora);


            // UI 최신화
            //isWeaponUpgradeBtnClicked = false;
            ResetToWeaponItemObjectPoolDatas();
            ResetToSelectButtonScriptPoolDatas();
            ClosePlayerInfoScreenButton();
            WeaponUpgeadeButtonClickEvent(1);   // 돌파 UI 출력을 위한 출력 함수 재호출
        }
        else
        {
            PopUpScreenFunc(popUpForUpgrade, "재료가 부족합니다!");
        }


    }

    #endregion

    #region 무기 업그레이드_재련

    // 무기 재련 출력 함수
    private void WeaponUpgrade_ReforgeButton(PlayerInfoUI_Button my, WeaponAndEquipCls equippedWeapon, Transform parents)
    {
        if (isWeaponUpgradeDetailObjClicked[2] == true)
            return;

        // 오브젝트풀 리턴
        ResetToWeaponItemObjectPoolDatas();
        ResetToSelectButtonScriptPoolDatas();

        my.OnOffSpriteSetting();
        ClickedButtonsSetActiveReset(my,parents,6);
        var mainObject = parents.GetChild(6).GetComponent<Transform>();

        // 지역 변수 초기화
        dic_ItemClsForWeaponUpgrade.Clear();
        nWeaponUpgradeCost = 0;
        nWeaponUpgradeExp = 0;

        /// 각 객체 인스턴스화
        // 텍스트 객체
        TextMeshProUGUI txt_ReforgeLevelText = mainObject.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txt_WeaponName = mainObject.GetChild(4).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txt_EffContentText = mainObject.GetChild(5).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txt_NextEffContentText = mainObject.GetChild(6).GetComponent<TextMeshProUGUI>();

        // 모라 TextMeshPro 인스턴스화
        txt_NeedMora = mainObject.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        txt_CurrentHaveMora = mainObject.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
        txt_CurrentHaveMora.text = GameManager.Instance.GetUserClass().GetMora().ToString();
        // 사용 버튼
        ButtonClass2 cls_UseButton = mainObject.GetChild(3).GetComponent<ButtonClass2>();
        // 선택 버튼 위치 좌표 트랜스폼
        Transform tr_WeaponResourceSelectPos = mainObject.GetChild(7).GetComponent<Transform>();
        // 스크롤뷰 오브젝트
        Transform tr_ScrollViewObjectSet = mainObject.GetChild(9).GetComponent<Transform>();
        ButtonClass2 cls_selectSortButton = tr_ScrollViewObjectSet.GetChild(2).GetComponent<ButtonClass2>();
        ButtonClass2 cls_Descending_AscendingButton = tr_ScrollViewObjectSet.GetChild(3).GetComponent<ButtonClass2>();
        InventorySortSelectButton cls_SelectionButtonOnList = tr_ScrollViewObjectSet.GetChild(4).GetComponent<InventorySortSelectButton>();


        /// 객체에 데이터 출력

        // 기본 텍스트 데이터
        txt_ReforgeLevelText.text = equippedWeapon.GetEffectLevel().ToString() + "단계";
        txt_WeaponName.text = equippedWeapon.GetName();
        txt_EffContentText.text = equippedWeapon.GetEffectText();

        // 다음 재련 단계 데이터 출력
        txt_NextEffContentText.text = "<b><color=#00FF00>다음 단계 효과 ● </color></b><color=#FFFFFF>" + GameManager.Instance.NextReforgeEffectData(equippedWeapon) + "</color>";

        // 재료 무기 선택용 버튼 오브젝트 출력
        SelectButtonScript btnObj = GameManager.Instance.SelectButtonScriptPool.GetFromPool(Vector3.zero, Quaternion.identity, tr_WeaponResourceSelectPos);
        SelectButtonToDefault(btnObj);
        // 원하는 위치로 이동하려면 Anchors 및 Pivot 조정
        RectTransform btnObjRectTransform = btnObj.GetComponent<RectTransform>();
        // 앵커와 피봇을 center-middle로 설정
        btnObjRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        btnObjRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        btnObjRectTransform.pivot = new Vector2(0.5f, 0.5f);
        // 원하는 위치로 이동
        btnObjRectTransform.anchoredPosition = new Vector2(0f, 0f);
        // 무기 선택 버튼 UI 출력


        // 버튼 클릭 이벤트 함수 연결
        //btnObj.GetComponentInChildren<Button>().onClick.AddListener();
        btnObj.GetButton().onClick.AddListener(() => ScrollViewOpenButtonClickEvent(btnObj, tr_ScrollViewObjectSet));
        btnObj.GetButton().onClick.AddListener(() => WeaponReforge_ButtonClickEvent(tr_ScrollViewObjectSet));
        
        cls_selectSortButton.GetButton().onClick.RemoveAllListeners();
        cls_selectSortButton.GetButton().onClick.AddListener(() => WeaponReforge_SortOrderBtnClick(cls_SelectionButtonOnList, cls_selectSortButton, tr_ScrollViewObjectSet));
        
        cls_Descending_AscendingButton.GetButton().onClick.RemoveAllListeners();
        cls_Descending_AscendingButton.GetButton().onClick.AddListener(WeaponPrintAtScroll_ForUpgrade_AscendingBtnClick);
        cls_Descending_AscendingButton.GetButton().onClick.AddListener(()=>WeaponReforge_ButtonClickEvent(tr_ScrollViewObjectSet));

        //스크롤뷰 객체
        // 정렬 디폴트 값 및 리스트 버튼 데이터 초기화
        weaponList_ForItemUpgradeResourceSortOrder = e_SortingOrder.ExpOrder;
        isWeaponList_ForUpgradeResourceSortOrderAscending = false;
        string[] strings = { "희귀도", "레벨", "기초경험치" };
        ListButtonObject_UI_Initer(cls_SelectionButtonOnList, strings, e_SortingOrder.ExpOrder);
        cls_SelectionButtonOnList.HideButtonBackGround(2);


        /// 사용 버튼 연결 및 객체 작업
        cls_UseButton.GetButton().onClick.RemoveAllListeners();
        ButtonClass2_Reset(cls_UseButton);
        cls_UseButton.GetButton().onClick.AddListener(() => WeaponReforge_UseButtonClickEvent());


        // 제어 플래그 수정
        isWeaponUpgradeDetailObjClicked[0] = false;
        isWeaponUpgradeDetailObjClicked[1] = false;
        isWeaponUpgradeDetailObjClicked[2] = true;

        tr_ScrollViewObjectSet.gameObject.SetActive(false);


    }
    // 무기 선택 버튼 클릭_스크롤 뷰 데이터 출력
    void WeaponReforge_ButtonClickEvent(Transform mainObj)
    {
        // 객체 인스턴스화
        InventorySortSelectButton cls_SelectionButtonOnList = mainObj.GetChild(4).GetComponent<InventorySortSelectButton>();
        
        WeaponPrintAtScroll(scrollForUpgradeMaterials, weaponList_ForItemUpgradeResourceSortOrder, isWeaponList_ForUpgradeResourceSortOrderAscending, openUI_ItemList);
        var datas = GameManager.Instance.WeaponItemPool.GetPoolList();
        ItemClass equip = GameManager.Instance.GetUserClass().GetUserEquippedWeapon();

        foreach(var tmp in datas)
        {
            // 비활성화되어 있는 객체는 스킵
            if (tmp.gameObject.activeSelf == false)
                continue;

            // 장비하고 있는 아이템은 제외
            if (tmp.GetItemcls().GetIsActive() == true)
            {
                GameManager.Instance.WeaponItemPool.ReturnToPool(tmp);
                continue;
            }
            // 장비하고 있는 아이템과 일치하지 않는 경우는 제외
            if(tmp.GetItemcls().GetName() != equip.GetName())
            {
                GameManager.Instance.WeaponItemPool.ReturnToPool(tmp);
                continue;
            }
            // 선택한 아이템과 동일한 경우도 제외
            if(tmp.GetItemcls() == selectBtn_ForUpgrade.GetItemClass())
            {
                GameManager.Instance.WeaponItemPool.ReturnToPool(tmp);
                continue;
            }

            // 버튼 클릭 이벤트 리스너 해제 및 연결
            Button btn = tmp.GetButton();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => WeaponReforge_SelectResource(tmp, mainObj));
            tmp.EquippedItemUIPrint(false); // 장비 중인 아이템 표시용 이미지는 필요 없으므로 False
        }

    }
    // 재화로 소모할 동일 장비 클릭 이벤트 리스너
    void WeaponReforge_SelectResource(InvenItemObjClass cls, Transform mainObj)
    {
        ItemClass itemCls = cls.GetItemcls();
        WeaponAndEquipCls weaponCls = itemCls as WeaponAndEquipCls;

        nWeaponUpgradeCost += itemCls.GetGrade() * 1000;
        txt_NeedMora.text = nWeaponUpgradeCost.ToString();

        // 버튼 객체 UI 작업
        selectBtn_ForUpgrade.SetIsActive(true);
        selectBtn_ForUpgrade.GetItemImage().enabled = true;
        selectBtn_ForUpgrade.SetItemCls(itemCls);
        WeaponKindDivider(weaponCls, selectBtn_ForUpgrade.GetItemImage());
        ItemUISetterByItemGrade(itemCls, selectBtn_ForUpgrade.GetTopBgr(), null);
        selectBtn_ForUpgrade.SetItemText("LV. " + weaponCls.GetLevel().ToString());

        mainObj.gameObject.SetActive(false);
    }

    // 정렬 선택 버튼 클릭 이벤트 리스너
    void WeaponReforge_SortOrderBtnClick(InventorySortSelectButton selectBtnListObj, ButtonClass2 selectBtn, Transform mainObj)
    {
        selectBtnListObj.gameObject.SetActive(true);

        Button[] buttons = selectBtnListObj.GetButtons();

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.RemoveAllListeners();
        }

        // 리스트 버튼 객체에 이벤트 리스너 
        buttons[0].onClick.AddListener(() => SortOrderForUpgradeResorceWeapon(selectBtnListObj, "희귀도", selectBtn)); // 희귀도
        buttons[0].onClick.AddListener(() => WeaponReforge_ButtonClickEvent(mainObj));                              // 희귀도

        buttons[1].onClick.AddListener(() => SortOrderForUpgradeResorceWeapon(selectBtnListObj, "레벨", selectBtn)); // 레벨
        buttons[1].onClick.AddListener(() => WeaponReforge_ButtonClickEvent(mainObj));                              // 레벨

        buttons[2].onClick.AddListener(() => SortOrderForUpgradeResorceWeapon(selectBtnListObj, "기초경험치", selectBtn)); // 기초경험치
        buttons[2].onClick.AddListener(() => WeaponReforge_ButtonClickEvent(mainObj));                                  // 기초경험치
    }
    // 재련_ 사용버튼 클릭
    void WeaponReforge_UseButtonClickEvent()
    {
        if(nWeaponUpgradeCost<=GameManager.Instance.GetUserClass().GetMora() && selectBtn_ForUpgrade.GetItemClass() != null)
        {
            WeaponAndEquipCls weaponCls = GameManager.Instance.GetUserClass().GetUserEquippedWeapon() as WeaponAndEquipCls;

            var removeTmp = GameManager.Instance.GetUserClass().GetHadWeaponList().Find(tmp => tmp.Equals(selectBtn_ForUpgrade.GetItemClass()));
            GameManager.Instance.GetUserClass().GetHadWeaponList().Remove(removeTmp);

            int havMora = GameManager.Instance.GetUserClass().GetMora();
            GameManager.Instance.GetUserClass().SetMora(havMora - nWeaponUpgradeCost);
            txt_CurrentHaveMora.text = GameManager.Instance.GetUserClass().GetMora().ToString();
            txt_NeedMora.text = "0";
            nWeaponUpgradeCost = 0;

            // 유저 무기 데이터 수정
            WeaponReforgeFunction(weaponCls);

            // UI 반영
            //isWeaponUpgradeBtnClicked = false;
            ResetToWeaponItemObjectPoolDatas();
            ResetToSelectButtonScriptPoolDatas();
            ClosePlayerInfoScreenButton();
            WeaponUpgeadeButtonClickEvent(2);
        }
    }
    #endregion

    #endregion



    #region 성유물 UI 관리
    void EquipmentPrint()
    {
        // 장비 중인 성유물 객체 데이터 프린트 함수
        PrintEquippedEquipment();

        ///버튼 객체 인스턴스화
        ButtonClass2 cls_MoreInformationBtn = printInfoDataField[2].transform.GetChild(5).GetComponent<ButtonClass2>();
        ButtonClass2 cls_ChangeButton = printInfoDataField[2].transform.GetChild(8).GetComponent<ButtonClass2>();
        var detailPrintobj = printInfoDataField[2].transform.GetChild(9).GetComponent<Transform>();
        detailPrintobj.gameObject.SetActive(false);

        // 상세정보 출력 버튼 이벤트 리스너 연결
        cls_MoreInformationBtn.GetButton().onClick.RemoveAllListeners();
        ButtonClass2_Reset(cls_MoreInformationBtn);
        cls_MoreInformationBtn.GetButton().onClick.AddListener(()=>DetailedEquipInfoPrint());

        // 성유물 교체 버튼 이벤트 리스너 연결


    }
    // 장비 중인 성유물 객체 데이터 프린트 함수_최상단 기본 창
    void PrintEquippedEquipment()
    {
        /// 인스턴스 초기화
        // 기본 능력치
        TextMeshProUGUI txtHp = printInfoDataField[2].transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtAtk = printInfoDataField[2].transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtDef = printInfoDataField[2].transform.GetChild(3).GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtElemnt = printInfoDataField[2].transform.GetChild(4).GetChild(2).GetComponent<TextMeshProUGUI>();
        // 세트 효과
        TextMeshProUGUI txtSetSynergy_1 = printInfoDataField[2].transform.GetChild(6).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtSetSynergy_2 = printInfoDataField[2].transform.GetChild(7).GetComponent<TextMeshProUGUI>();

        var EquipmentList = GameManager.Instance.GetUserClass().GetUserEquippedEquipment();
        
        // 세트 효과 파악
        Dictionary<string, int> setFinder = new Dictionary<string, int>();
        txtSetSynergy_1.text = "";
        txtSetSynergy_2.text = "";

        var datas = ReturnEquipmentStatSum(EquipmentList, setFinder);

        int hp = (int)datas[0];
        int atk = (int)datas[1];
        int def = (int)datas[2];
        int element = (int)datas[3];

        // 세트 효과를 기본 능력치에 반영 및, UI에 출력함
        foreach (var kvp in setFinder)
        {
            string set = kvp.Key;   // 세트 키 가져오기
            int count = kvp.Value; // 세트 개수 가져오기

            if(count >=4)
            {
                var db = GameManager.Instance.GetList_EquipmentSetSynergyData();
                var dbItem1 = db.Find(tmp => tmp.EQUIPMENT_SET_NAME.Equals(set) && tmp.EQUIPMENT_SET_NUMBER == 2);
                var dbItem2 = db.Find(tmp => tmp.EQUIPMENT_SET_NAME.Equals(set) && tmp.EQUIPMENT_SET_NUMBER == 4);
                ApplyEquipmentEffect(dbItem1, ref atk, ref def, ref hp, ref element);
                ApplyEquipmentEffect(dbItem2, ref atk, ref def, ref hp, ref element);

                txtSetSynergy_1.text = set + "\n" + "2세트 "+ dbItem1.EQUIPMENT_SET_EFFECT_TEXT;
                txtSetSynergy_2.text = set + "\n" + "4세트 "+ dbItem2.EQUIPMENT_SET_EFFECT_TEXT;
                break;
            }
            else if(count >=2)
            {
                var db = GameManager.Instance.GetList_EquipmentSetSynergyData();
                var dbItem = db.Find(tmp => tmp.EQUIPMENT_SET_NAME.Equals(set) && tmp.EQUIPMENT_SET_NUMBER == 2);

                ApplyEquipmentEffect(dbItem, ref atk, ref def, ref hp, ref element);

                if (txtSetSynergy_1.text == "")
                    txtSetSynergy_1.text = set + "\n" + "2세트 " + dbItem.EQUIPMENT_SET_EFFECT_TEXT;
                else
                    txtSetSynergy_2.text = set + "\n" + "2세트 " + dbItem.EQUIPMENT_SET_EFFECT_TEXT;
                continue;
            }
        }


        txtHp.text = hp.ToString();
        txtDef.text = def.ToString();
        txtAtk.text = atk.ToString();
        txtElemnt.text = element.ToString();
    }
    // 중복 코드를 메서드로 추출 - hp, atk, def, element, criRate, criDamage 순서로 반환
    float[] ReturnEquipmentStatSum(ItemClass[] EquipmentList, Dictionary<string, int> setFinder)
    {
        // 기본 데이터
        float hp = 0;
        float atk = 0;
        float def = 0;
        float element = 0;
        float criticalRate = 0;
        float criticalDamage = 0;

        // 세트 효과 파악
        foreach (var tmp in EquipmentList)
        {
            if (tmp == null)
                continue;
            var reData = tmp as WeaponAndEquipCls;
            // 기본 데이터 출력
            if (reData.GetTag() == "꽃")
                hp += (int)reData.GetMainStat();
            else if (reData.GetTag() == "깃털")
                atk += (int)reData.GetMainStat();
            else
            {
                string reDataText = reData.GetEffectText(); // reData.GetEffectText()로부터 문자열 가져오기
                string pivot = ""; // pivot 문자열 초기화

                if (reDataText.Length > 7)
                {
                    // 8번째 값부터 끝까지의 부분을 pivot에 저장
                    pivot = reDataText.Substring(7);
                }
                //Debug.Log("pivot :: => "+pivot);
                switch (pivot)
                {
                    case "공격력":
                        {
                            float mul = reData.GetMainStat() * 0.01f;
                            int atkPluser = (int)(mul * atk);
                            atk += atkPluser;
                        }
                        break;
                    case "방어력":
                        {
                            float mul = reData.GetMainStat() * 0.01f;
                            int defPluser = (int)(mul * def);
                            def += defPluser;
                        }
                        break;
                    case "체력":
                        {
                            float mul = reData.GetMainStat() * 0.01f;
                            int hpPluser = (int)(mul * hp);
                            hp += hpPluser;
                        }
                        break;
                    case "원소 마스터리":
                        {
                            int value = (int)reData.GetMainStat();
                            element += value;
                        }
                        break;
                    case "치명타 확률":
                        {
                            float vlaue = reData.GetMainStat();
                            criticalRate+= vlaue;
                        }
                        break;
                    case "치명타 피해":
                        {
                            float value = reData.GetMainStat();
                            criticalDamage+= value;
                        }
                        break;
                    default: break;
                }
            }

            // Dictionary에 키가 없으면 0으로 초기화하고 1을 더함
            string set = reData.GetSet();
            if (!setFinder.ContainsKey(set))
                setFinder[set] = 0;

            setFinder[set]++;

        }

        float[] answers = {hp, atk, def, element, criticalRate , criticalDamage };
        return answers;

    }
    // 성유물 데이터에 따라 성유물 스탯 수정
    void ApplyEquipmentEffect(GameManager.EQUIPMENT_SET_SYNERGY_DATA_BASE dbItem, ref int atk, ref int def, ref int hp, ref int element)
    {
        float mul = dbItem.EQUIPMENT_SET_EFFECT_VALUE * 0.01f;
        int pluser = 0;
        
        if(dbItem.EQUIPMENT_SET_EFFECT_ELEMENT.Equals("공격력"))
        {
            pluser = (int)(mul * atk);
            atk += pluser;
        }
        else if (dbItem.EQUIPMENT_SET_EFFECT_ELEMENT.Equals("방어력"))
        {
            pluser = (int)(mul * def);
            def += pluser;
        }
        else if (dbItem.EQUIPMENT_SET_EFFECT_ELEMENT.Equals("체력"))
        {
            pluser = (int)(mul * hp);
            hp += pluser;
        }
        else if (dbItem.EQUIPMENT_SET_EFFECT_ELEMENT.Equals("원소 마스터리"))
            element += (int)dbItem.EQUIPMENT_SET_EFFECT_VALUE;
        //Debug.Log("pluser :: => " + pluser);
    }
    void ApplyEquipmentEffect(GameManager.EQUIPMENT_SET_SYNERGY_DATA_BASE dbItem, ref int atk, ref int def, ref int hp, ref int element, ref float citicalRate, ref float criticalDamage)
    {
        float mul = dbItem.EQUIPMENT_SET_EFFECT_VALUE * 0.01f;
        int pluser = 0;

        if (dbItem.EQUIPMENT_SET_EFFECT_ELEMENT.Equals("공격력"))
        {
            pluser = (int)(mul * atk);
            atk += pluser;
        }
        else if (dbItem.EQUIPMENT_SET_EFFECT_ELEMENT.Equals("방어력"))
        {
            pluser = (int)(mul * def);
            def += pluser;
        }
        else if (dbItem.EQUIPMENT_SET_EFFECT_ELEMENT.Equals("체력"))
        {
            pluser = (int)(mul * hp);
            hp += pluser;
        }
        else if (dbItem.EQUIPMENT_SET_EFFECT_ELEMENT.Equals("원소 마스터리"))
            element += (int)dbItem.EQUIPMENT_SET_EFFECT_VALUE;
        else if (dbItem.EQUIPMENT_SET_EFFECT_ELEMENT.Equals("치명타 확률"))
            citicalRate += dbItem.EQUIPMENT_SET_EFFECT_VALUE;
        else if (dbItem.EQUIPMENT_SET_EFFECT_ELEMENT.Equals("치명타 데미지"))
            criticalDamage += dbItem.EQUIPMENT_SET_EFFECT_VALUE;
    }

    // 상세 정보창 출력
    void DetailedEquipInfoPrint()
    {
        // 상세 데이터 화면 출력 - 인스턴스 초기화 및 객체 활성화
        //isDetailScreenOpen = true;
        InfoPrintTypeButtonUnActive();
        isDetailScreenOpenForEquip = true;
        var obj = printInfoDataField[2].transform.GetChild(9).GetComponent<Transform>();
        obj.gameObject.SetActive(true);

        Transform content = obj.GetChild(0).GetChild(0).GetChild(0).GetComponent<Transform>();
        TextMeshProUGUI[] statusTexts = new TextMeshProUGUI[7];
        // 데이터 출력을 위한 TextMeshPro 배열에 저장
        for (int i = 0; i < 5; i++)
        {
            statusTexts[i] = content.GetChild(1 + i).GetChild(2).GetComponent<TextMeshProUGUI>();
        }
        for (int i = 0; i < 2; i++)
        {
            statusTexts[5 + i] = content.GetChild(8 + i).GetChild(2).GetComponent<TextMeshProUGUI>();
        }


        // 출력 데이터 연산
        var EquipmentList = GameManager.Instance.GetUserClass().GetUserEquippedEquipment();

        // 세트 효과 파악
        Dictionary<string, int> setFinder = new Dictionary<string, int>();

        var equipDatas = ReturnEquipmentStatSum(EquipmentList, setFinder);

        int hp = (int)equipDatas[0];
        int atk = (int)equipDatas[1];
        int def = (int)equipDatas[2];
        int element = (int)equipDatas[3];
        float criticalRate = equipDatas[4];
        float criticalDamage = equipDatas[5];
        int stamina = 0;


        // 세트 효과를 기본 능력치에 반영 및, UI에 출력함
        foreach (var kvp in setFinder)
        {
            string set = kvp.Key;   // 세트 키 가져오기
            int count = kvp.Value; // 세트 개수 가져오기

            if (count >= 4)
            {
                var db = GameManager.Instance.GetList_EquipmentSetSynergyData();
                var dbItem1 = db.Find(tmp => tmp.EQUIPMENT_SET_NAME.Equals(set) && tmp.EQUIPMENT_SET_NUMBER == 2);
                var dbItem2 = db.Find(tmp => tmp.EQUIPMENT_SET_NAME.Equals(set) && tmp.EQUIPMENT_SET_NUMBER == 4);
                ApplyEquipmentEffect(dbItem1, ref atk, ref def, ref hp, ref element, ref criticalRate, ref criticalDamage);
                ApplyEquipmentEffect(dbItem2, ref atk, ref def, ref hp, ref element, ref criticalRate, ref criticalDamage);
                break;
            }
            else if (count >= 2)
            {
                var db = GameManager.Instance.GetList_EquipmentSetSynergyData();
                var dbItem = db.Find(tmp => tmp.EQUIPMENT_SET_NAME.Equals(set) && tmp.EQUIPMENT_SET_NUMBER == 2);
                ApplyEquipmentEffect(dbItem, ref atk, ref def, ref hp, ref element, ref criticalRate, ref criticalDamage);
                continue;
            }
        }

        statusTexts[0].text = hp.ToString();
        statusTexts[1].text = atk.ToString();
        statusTexts[2].text = def.ToString();
        statusTexts[3].text = element.ToString();
        statusTexts[4].text = stamina.ToString();
        statusTexts[5].text = criticalRate.ToString() + "%";
        statusTexts[6].text = criticalDamage.ToString();
    }

    #endregion


    #region 운명의 별자리 UI 관리


    #endregion


    #region 특성 UI 관리

    #endregion


    #endregion

    #region 기타 및 툴

    private void ResetToWeaponItemObjectPoolDatas()
    {
        // 오브젝트 풀로 가져온 각 버튼에 Button이벤트를 Remove함
        var objList = GameManager.Instance.WeaponItemPool.GetPoolList();
        foreach (var obj in objList)
        {
            //if (obj.gameObject.activeSelf == false) continue;
            Button btn = obj.GetButton();
            if (btn != null)
            {
                // 모든 이벤트 리스너를 제거하고, 기존의 이벤트 리스너를 다시 부착한다.
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => obj.OnClickEventer());
                btn.onClick.AddListener(() => obj.ClickedUIApply());
            }
        }
        GameManager.Instance.WeaponItemPool.AllReturnToPool();  // 웨폰 UI 오브젝트풀 리턴
    }
    private void ResetToSelectButtonScriptPoolDatas()
    {
        // 오브젝트 풀로 가져온 각 버튼에 Button이벤트를 Remove함
        var objList = GameManager.Instance.SelectButtonScriptPool.GetPoolList();
        foreach (var obj in objList)
        {
            //if (obj.gameObject.activeSelf == false) continue;
            Button btn = obj.GetButton();
            if (btn != null)
            {
                // 모든 이벤트 리스너를 제거하고, 기존의 이벤트 리스너를 다시 부착한다.
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => obj.OnClickEventListener());
            }
        }
        GameManager.Instance.WeaponItemPool.AllReturnToPool();  // 웨폰 UI 오브젝트풀 리턴
    }
    private void ScrollViewOpenButtonClickEvent(SelectButtonScript clickedBtn, Transform mainobj)
    {
        mainobj.gameObject.SetActive(true);
        ResetToWeaponItemObjectPoolDatas();
        selectBtn_ForUpgrade = clickedBtn;
        // 객체 인스턴스화
        InventorySortSelectButton cls_SelectionButtonOnList = mainobj.GetChild(4).GetComponent<InventorySortSelectButton>();
        scrollForUpgradeMaterials = mainobj.GetChild(1).GetChild(0).GetComponent<Transform>();
 
        // 리스트버튼은 기본 False
        cls_SelectionButtonOnList.gameObject.SetActive(false);

        // 스크롤뷰 종료 버튼 활성화
        Button outButton = mainobj.GetChild(7).GetComponent<Button>();
        outButton.gameObject.SetActive(true);
        outButton.onClick.RemoveAllListeners();
        outButton.onClick.AddListener(() => ScrollObjectOffButton(mainobj.gameObject, outButton));
    }

    #region 정렬 레거시

    //void SortingItemList(List<ItemClass> clsList)
    //{
    //    switch (selected_SortOrder)
    //    {
    //        case e_SortingOrder.GradeOrder: // 등급을 기준으로 정렬
    //            if (isAscending)
    //                clsList.Sort((item1, item2) => item1.GetGrade().CompareTo(item2.GetGrade()));
    //            else
    //                clsList.Sort((item1, item2) => item2.GetGrade().CompareTo(item1.GetGrade()));
    //            break;
    //        case e_SortingOrder.LevelOrder: // 레벨을 기준으로 정렬
    //            if (isAscending)
    //                clsList.Sort((item1, item2) => item1.GetLevel().CompareTo(item2.GetLevel()));
    //            else
    //                clsList.Sort((item1, item2) => item2.GetLevel().CompareTo(item1.GetLevel()));
    //            break;
    //        case e_SortingOrder.NameOrder: // 이름을 기준으로 정렬
    //            if (isAscending)
    //                clsList.Sort((item1, item2) => string.Compare(item1.GetName(), item2.GetName()));
    //            else
    //                clsList.Sort((item1, item2) => string.Compare(item2.GetName(), item1.GetName()));
    //            break;
    //        default:
    //            break;
    //    }
    //}
    #endregion

    public e_InventoryTypeSelected GetnSelectedInvenIdx(){return invenType_Index; }

    #endregion
}
