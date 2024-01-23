using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;
using static CharacterUpgradeManager;
using static UI_UseToolClass;
using static GameManager;

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
    private Dictionary<ItemClass, SelectButtonScript> dic_ItemClsForUpgrade;  // 무기 레벨업용 아이템 재화 - 버튼 자료구조
    private Image img_ExpBar;
    private e_SortingOrder allPull_OrderValue;                                      // 일괄처리 정렬 순서(Name==3성, Grade==5성, Level==4성)
    private e_SortingOrder list_ForItemUpgradeResourceSortOrder;              // 무기 강화용 아이템 재화 출력 스크롤뷰 정렬 변수
    private bool isList_ForUpgradeResourceSortOrderAscending;                 // 무기 강화용 아이템 재화 출력 스크롤뷰 정렬 변수_내림차순/오름차순
    private bool isLimitBreakPossible;                                              // 무기 돌파 가능 유무 _ 재화로 판단
    private Transform scrollForUpgradeMaterials;                                    // 무기 레벨업용 아이템 재화 출력 스크롤뷰
    private SelectButtonScript selectBtn_ForUpgrade;                                // 플레이어가 선택한 selectButon

    // 성유물 창 관련
    private bool isDetailScreenOpenForEquip;                // 성유물 상세정보 창 오픈
    private bool isEquipmentPrintScrollOpen;                // 성유물 리스트 스크롤뷰 오픈
    private TouchPadController touchPad;                    // 성유물 회전 선택 객체 처리용 터치패드
    private TouchPadController.e_TouchSlideDic touchDic;    // 성유물 회전 선택 객체 처리용 터치 방향
    private ButtonRotateSetCls rotateBtnObj;                // 성유물 회전 선택 객체
    private e_SortingOrder scrollPrintEquipOrder;           // 성유물 스크롤 뷰 출력 정렬 변수
    private bool isScrollPrintEquipAscending;               // 성유물 스크롤 뷰 출력 정렬 변수     
    private int nSelected_EquipIndex;                       // 선택한 성유물 타입 인덱스 변수
    private bool isUpgrade_Info_Open;                       // 성유물 업그레이드 버튼 클릭 변수
    private bool[] isEquipUpgradeDetailObjClicked;          // 성유물 업글 창_상세창 제어 플래그

    #endregion

    #region 기타
    [Header("기타(Etc)")]
    [SerializeField] GetResourcePrintUI getResourcePrintUI;             // 재료 획득처 출력 객체
    [SerializeField] WorldMap_Manager wolrdMap_manager;                 // 월드맵 매니저 객체
    
    #endregion


    #region 오브젝트 관련 변수

    [Header("합성대 (Synthesis's Values)")]
    public InventoryButton[] SynthesisTypeButtons;                             // 인벤토리 아이템 타입 선택 버튼들(0- 성유물 합성,1- 무기 합성, 2-기타 합성)
    public GameObject SynthesisObjectScreen;                                    // 합성대 UI 오브젝트
    public Synthesis synthesis;


    #endregion


    #endregion

    #region 구조체
    // 인벤토리 UI 창 - 아이템 타입 구조체
    public enum e_InventoryTypeSelected
    {
        None = -1,
        Weapon =0,
        Equipment,
        Gem,
        Food,
        Etc
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
        invenButtonIsClicked = new bool[] { false, false, false, false, false };   // 배열 초기화
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
        dic_ItemClsForUpgrade = new Dictionary<ItemClass, SelectButtonScript>();

        // 오브젝트 관련
        SynthesisObjectScreen.gameObject.SetActive(false);
        

    }
   
    private void Start()
    {
        synthesis = new Synthesis();
    }
    private void Update()
    {
        if (touchPad != null && touchPad.gameObject.activeSelf == true)
        {
            touchDic = touchPad.GetDirectionHorizontal();
            if (touchDic == TouchPadController.e_TouchSlideDic.Right)
            {
                rotateBtnObj.AnimationActiveOff();
                rotateBtnObj.SetAnimationSpeed(-2);
                rotateBtnObj.AnimationActiveOn();
            }
            else if (touchDic == TouchPadController.e_TouchSlideDic.Left)
            {
                rotateBtnObj.AnimationActiveOff();
                rotateBtnObj.AnimationActiveOn();
                rotateBtnObj.SetAnimationSpeed(2);
            }
        }
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
            {
                CharacterDataReviseWhenWeaponTakeOff();
                GameManager.Instance.GetUserClass().SetUserEquippedWeapon(findData);
                CharacterDataReviseToWeapon();
            }
            else
            {
                switch(findData.GetTag())
                {
                    case "꽃":
                        CharacterDataReviseWhenEquipmentTakeOff(0);
                        GameManager.Instance.GetUserClass().SetUserEquippedEquipment(findData, 0);
                        CharacterDataReviseToEquipment(0);
                        break; 
                    case "깃털":
                        CharacterDataReviseWhenEquipmentTakeOff(1);
                        GameManager.Instance.GetUserClass().SetUserEquippedEquipment(findData, 1);
                        CharacterDataReviseToEquipment(1);
                        break;
                    case "모래":
                        CharacterDataReviseWhenEquipmentTakeOff(2);
                        GameManager.Instance.GetUserClass().SetUserEquippedEquipment(findData, 2);
                        CharacterDataReviseToEquipment(2);
                        break;
                    case "성배":
                        CharacterDataReviseWhenEquipmentTakeOff(3);
                        GameManager.Instance.GetUserClass().SetUserEquippedEquipment(findData, 3);
                        CharacterDataReviseToEquipment(3);
                        break;
                    case "왕관":
                        CharacterDataReviseWhenEquipmentTakeOff(4);
                        GameManager.Instance.GetUserClass().SetUserEquippedEquipment(findData, 4);
                        CharacterDataReviseToEquipment(4);
                        break;
                }
                // 세트 효과 반영 갱신
                EquipmentSetSynergyUnApplyer();
                EquipmentSetSynergyApplyer();
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
        else if(invenType_Index == e_InventoryTypeSelected.Etc)
        {
            return;
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
        switch((e_InventoryTypeSelected)index)
        {
            case e_InventoryTypeSelected.Weapon:    //웨폰
                WeaponPrintAtScroll(scrollContent, selected_SortOrder, isAscending, openUI_ItemList);
                break;
            case e_InventoryTypeSelected.Equipment:     //장비
                EquipPrintAtScroll(scrollContent, selected_SortOrder, isAscending, openUI_ItemList);
                break;
            case e_InventoryTypeSelected.Gem:       //광물
                GemPrintAtScroll();
                break;
            case e_InventoryTypeSelected.Food:      //음식
                FoodPrintAtScroll();
                break;
            case e_InventoryTypeSelected.Etc:       // 기타
                EtcPrintAtScroll(scrollContent, selected_SortOrder, isAscending, openUI_ItemList);
                break;
            default:
                break;
        }
    }

    // 스크롤뷰 리셋
    private void ScrollViewReset()
    {
        ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Weapon);
        ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Equip);
        ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Gem);
        ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Food);
        openUI_ItemList.Clear();
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

                GemKindDivider(data, obj.GetTopItemImage());

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

                FoodKindDivider(data, obj.GetTopItemImage());

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

    #region 기본 함수

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
            ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Weapon);
            isWeaponChangeBtnClicked = false;
        }
        // 무기 업글 버튼 클릭 시, 기능 변경
        else if(isWeaponUpgradeBtnClicked)
        {
            if(OpenScrollToUpgradeMaterial != null && OpenScrollToUpgradeMaterial.gameObject.activeSelf == true)
            {   // 무기 강화용 재화 출력용 스크롤 오브젝트가 Ative상태라면, 비활성화
                ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Weapon);
                OpenScrollToUpgradeMaterial.gameObject.SetActive(false);
                return;
            }

            InfoPrintTypeButtonActive();
            CloseButtonSpriteToClose();                             // Close 스프라이트로 버튼 심볼 교체
            infoSelectButtons[1].OnOffSpriteSetting();              // 웨폰 인포 버튼 UI 활성화

            Transform screen = printInfoDataField[1].transform.GetChild(13).GetComponent<Transform>();    // 무기 업글 UI 오브젝트 인스턴스화
            screen.gameObject.SetActive(false);

            ResetToSelectButtonScriptPoolDatas();   // Pool 리턴 및 초기화
            ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Weapon);     // Pool 리턴 및 초기화
            EquippedWeaponPrint();                  // 레벨업에 의한 데이터 변환에 대비한 UI 데이터 재출력
            isWeaponUpgradeBtnClicked = false;
        }
        else if(isDetailScreenOpenForEquip) // 성유물 - 성유물 상세정보 창 출력
        {
            isDetailScreenOpenForEquip = false;
            var obj = printInfoDataField[2].transform.GetChild(1).GetChild(6).GetComponent<Transform>();
            obj.gameObject.SetActive(false);
            InfoPrintTypeButtonActive();
            infoSelectButtons[2].OnOffSpriteSetting();

            RotateEquipEquipMentPrint();            // 성유물 기본 창의 회전 성유물 UI 오브젝트 재활성화
        }
        else if(isEquipmentPrintScrollOpen)         // 보유 성유물 리스트 ScrollView에 출력 중일 때,
        {
            if(isUpgrade_Info_Open)// 성유물 강화 버튼 클릭 이벤트 실행중일때
            {
                printInfoDataField[2].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                printInfoDataField[2].transform.GetChild(2).gameObject.SetActive(true);
                printInfoDataField[2].transform.GetChild(3).gameObject.SetActive(true);
                printInfoDataField[2].transform.GetChild(7).gameObject.SetActive(false);
                WhenEquipPrint_SelectEquipMover(nSelected_EquipIndex);
                CloseButtonSpriteToClose();
                Debug.Log("nSelected_EquipIndex :: " + nSelected_EquipIndex);

                isUpgrade_Info_Open = false;
                return;
            }

            CloseButtonSpriteToClose();
            printInfoDataField[2].transform.GetChild(1).gameObject.SetActive(true);
            printInfoDataField[2].transform.GetChild(6).gameObject.SetActive(false);
            ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Equip);      // 오브젝트 풀 리턴
            PrintEquippedEquipment();                // 기본 데이터 재출력'
            rotateBtnObj.AnimationActiveOn();
            RotateEquipEquipMentPrint();             // 성유물 기본 창의 회전 성유물 UI 오브젝트 재활성화
            InfoPrintTypeButtonActive();             // 버튼 UI 재활성화 
            infoSelectButtons[2].OnOffSpriteSetting();
            isEquipmentPrintScrollOpen = false;
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
    #endregion

    #region 캐릭터 인포 UI 관리
    // 캐릭터 데이터 출력

    #region 출력

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
        TextMeshProUGUI[] statusTexts = new TextMeshProUGUI[11];

        // 데이터 출력을 위한 TextMeshPro 배열에 저장
        for(int i=0; i<5; i++)
        {
            statusTexts[i] = content.GetChild(1 + i).GetChild(2).GetComponent<TextMeshProUGUI>();
        }
        for(int i=0; i<6; i++)
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
        statusTexts[7].text = datas.GetElementCharge().ToString();
        statusTexts[8].text = datas.GetIncreased_NormalAttackDamage().ToString();
        statusTexts[9].text = datas.GetIncrease_SkillAttackDamage().ToString();
        statusTexts[10].text = datas.GetIncrease_Damage().ToString();
    }

    #endregion

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

        CharacterDataReviseWhenWeaponTakeOff(); // 아이템 데이터 감산
        ItemClass ExistingItem = GameManager.Instance.GetUserClass().GetUserEquippedWeapon();   // 기존 장착중인 아이템 가져오기
        ItemClass tmp = WeaponsList.Find(item => item.Equals(ExistingItem));                    // 기존 장착중인 아이템 리스트에서 찾기
        tmp.SetActive(false);                                                                   // 장착 중이던 아이템을 활성화 X

        ItemClass SelectItem = WeaponsList.Find(item => item.Equals(cls));              // 선택한 아이템 찾기
        SelectItem.SetActive(true);                                                     // 장착할 아이템 활성화 O
        GameManager.Instance.GetUserClass().SetUserEquippedWeapon(SelectItem);          // 선택한 아이템 장착
        CharacterDataReviseToWeapon();          // 아이템 데이터 증가

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
    private void ClickedButtonsSetActiveReset(PlayerInfoUI_Button my, Transform parents, int CurrentIndex, int[] loopValue)
    {
        PlayerInfoUI_Button[] buttons = new PlayerInfoUI_Button[3];
        buttons = parents.GetComponentsInChildren<PlayerInfoUI_Button>();          // 무기 업글 창에서 사용할 버튼 객체

        // 클릭 시 클릭한 객체를 제외한 나머지 UI 비활성화
        foreach (var button in buttons)
        {
            if (button == null) continue;
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
        for (int i = loopValue[0]; i < loopValue[1]; i++)
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
        ClickedButtonsSetActiveReset(my, parents,4, new int[] {4,7});
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
        dic_ItemClsForUpgrade.Clear();
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
        ClickedButtonsSetActiveReset(my, parents, 5, new int[] {4,7});
        var mainObject = parents.GetChild(5).GetComponent<Transform>();

        // 모라 TextMeshPro 인스턴스화
        txt_NeedMora = mainObject.GetChild(7).GetChild(1).GetComponent<TextMeshProUGUI>();
        txt_CurrentHaveMora = mainObject.GetChild(8).GetChild(1).GetComponent<TextMeshProUGUI>();
        txt_CurrentHaveMora.text = GameManager.Instance.GetUserClass().GetMora().ToString();

        // 데이터 초기화
        dic_ItemClsForUpgrade = new Dictionary<ItemClass, SelectButtonScript>();
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

            foreach (var i in btnObjArr)
                SelectButtonToDefault(i);

            // 각 버튼에 재료 UI 세팅
            WeaponLimitBreakResourcePrintUI(btnObjArr, equippedWeapon);
            
            // 원하는 위치로 이동하려면 Anchors 및 Pivot 조정
            for (int i = 0; i < btnObjArr.Length; i++)
            {
                // 버튼 클릭 이벤트 함수 연결
                int index = i;
                btnObjArr[index].GetButton().onClick.RemoveAllListeners();
                btnObjArr[index].GetButton().onClick.AddListener(() => btnObjArr[index].OnClickEventListener());
                btnObjArr[index].GetButton().onClick.AddListener(() => WeaponLimitBreakResourcePrintButtonClickLeitener(btnObjArr[index]));
                RectTransform btnObjRectTransform = btnObjArr[index].GetComponent<RectTransform>();
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
            AllSelectButton.GetButton().onClick.AddListener(() => AllClickWeaponButtonForUpgradeClickEventLeistener(e_PoolItemType.Weapon));

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
                btn.onClick.AddListener(() => ScrollViewOpenButtonClickEvent(tmp, OpenScrollToUpgradeMaterial,e_PoolItemType.Weapon));
                btn.onClick.AddListener(() => WeaponSelectForUpgradeBtnClickEventListener(tmp, OpenScrollToUpgradeMaterial, e_PoolItemType.Weapon));
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
    void WeaponSelectForUpgradeBtnClickEventListener(SelectButtonScript slectBtnCls, Transform objectSet, e_PoolItemType type, List<InvenItemObjClass> listObj = null)
    {
        ButtonClass2 SortSelectButton = objectSet.GetChild(2).GetComponent<ButtonClass2>();                                       // 정렬 선택 버튼(리스트 버튼 On/Off)
        ButtonClass2 Descending_AscendingButton = objectSet.GetChild(3).GetComponent<ButtonClass2>();                             // 내림차순/오름차순 버튼
        InventorySortSelectButton SelectionButtonOnList = objectSet.GetChild(4).GetComponent<InventorySortSelectButton>();        // 리스트 버튼

        // 버튼 리스너 연결
        SortSelectButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        SortSelectButton.GetComponentInChildren<Button>().onClick.AddListener(()
            => WeaponPrintAtScroll_ForUpgrade_SortOrderBtnClick(SelectionButtonOnList, SortSelectButton, type));

        Descending_AscendingButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        Descending_AscendingButton.GetComponentInChildren<Button>().onClick.AddListener(()=>AscendingValueReversalFunction(ref isList_ForUpgradeResourceSortOrderAscending, type));
        Descending_AscendingButton.GetComponentInChildren<Button>().onClick.AddListener(()=> PrintDataAtScrollViewForUpMaterials(selectBtn_ForUpgrade, type));

        // 정렬 디폴트 값 및 리스트 버튼 데이터 초기화
        list_ForItemUpgradeResourceSortOrder = e_SortingOrder.ExpOrder;
        isList_ForUpgradeResourceSortOrderAscending = false;
        string[] strings = { "희귀도","레벨","기초경험치" };
        ListButtonObject_UI_Initer(SelectionButtonOnList, strings,e_SortingOrder.ExpOrder);
        SelectionButtonOnList.HideButtonBackGround(2);

        // 스크롤뷰 데이터 출력
        if(listObj==null)
            PrintDataAtScrollViewForUpMaterials(slectBtnCls,type);
        else
            PrintDataAtScrollViewForUpMaterials(slectBtnCls, type,listObj);
    }

    private void PrintDataAtScrollViewForUpMaterials(SelectButtonScript slectBtnCls, e_PoolItemType type,List<InvenItemObjClass> listObj = null)
    {
        if(type == e_PoolItemType.Weapon)
        {
            List<InvenItemObjClass> datas = null;
            // 스크롤뷰 데이터 출력
            if (listObj == null)    // listObj 파라미터가 널이 아닐 경우, 데이터를 초기화
            {
                WeaponPrintAtScroll(scrollForUpgradeMaterials, list_ForItemUpgradeResourceSortOrder, isList_ForUpgradeResourceSortOrderAscending, openUI_ItemList);
                datas = GameManager.Instance.WeaponItemPool.GetPoolList();
            }
            else
            {
                WeaponPrintAtScroll(scrollForUpgradeMaterials, list_ForItemUpgradeResourceSortOrder, isList_ForUpgradeResourceSortOrderAscending, openUI_ItemList, listObj);
                datas = listObj;
            }


            // 스크롤뷰 버튼 오브젝트 순회하며, 이벤트 리스너 연결 및 UI 데이터 초기화
            foreach (var tmp in datas)
            {
                // 비활성화되어 있는 객체는 스킵
                if (tmp.gameObject.activeSelf == false)
                    continue;

                // 장비하고 있는 아이템은 제외
                if (tmp.GetItemcls().GetIsActive() == true)
                {
                    tmp.SetIsActive(false);

                    if (listObj != null)    // 파라미터 널 여부 확인 후, 리턴 객체 결정
                    {
                        if (GameManager.Instance.WeaponItemPool.GetPoolList().Equals(datas))
                            GameManager.Instance.WeaponItemPool.ReturnToPool(tmp);

                        else if(GameManager.Instance.EquipItemPool.GetPoolList().Equals(datas))
                            GameManager.Instance.EquipItemPool.ReturnToPool(tmp);

                        else if(GameManager.Instance.GemItemPool.GetPoolList().Equals(datas))
                            GameManager.Instance.GemItemPool.ReturnToPool(tmp);

                        else if(GameManager.Instance.FoodItemPool.GetPoolList().Equals(datas))
                            GameManager.Instance.FoodItemPool.ReturnToPool(tmp);
                    }
                    else
                    {
                        GameManager.Instance.WeaponItemPool.ReturnToPool(tmp);
                    }
                    continue;
                }

                if (dic_ItemClsForUpgrade.ContainsKey(tmp.GetItemcls()))
                    tmp.SetSelectBtnSpriteOn(true);

                // 버튼 클릭 이벤트 리스너 해제 및 연결
                Button btn = tmp.GetButton();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => ClickMaterialItemForWeaponUpgrade(tmp, slectBtnCls, listObj));
                tmp.EquippedItemUIPrint(false); // 장비 중인 아이템 표시용 이미지는 필요 없으므로 False
            }
        }
        else
        {
            List<InvenItemObjClass> datas = null;
            if (listObj == null)    // listObj 파라미터가 널이 아닐 경우, 데이터를 초기화
            {
                EquipPrintAtScroll(scrollForUpgradeMaterials, list_ForItemUpgradeResourceSortOrder, isList_ForUpgradeResourceSortOrderAscending, openUI_ItemList);
                datas = GameManager.Instance.EquipItemPool.GetPoolList(); 
            }
            else
            {
                EquipPrintAtScroll(scrollForUpgradeMaterials, list_ForItemUpgradeResourceSortOrder, isList_ForUpgradeResourceSortOrderAscending, openUI_ItemList, listObj);
                datas = listObj;
            }

            // 스크롤뷰 버튼 오브젝트 순회하며, 이벤트 리스너 연결 및 UI 데이터 초기화
            foreach (var tmp in datas)
            {
                // 비활성화되어 있는 객체는 스킵
                if (tmp.gameObject.activeSelf == false)
                    continue;

                // 장비하고 있는 아이템은 제외
                if (tmp.GetItemcls().GetIsActive() == true)
                {
                    tmp.SetIsActive(false);

                    if (listObj != null)    // 파라미터 널 여부 확인
                    {
                        if (GameManager.Instance.WeaponItemPool.GetPoolList().Equals(datas))
                            GameManager.Instance.WeaponItemPool.ReturnToPool(tmp);

                        else if (GameManager.Instance.EquipItemPool.GetPoolList().Equals(datas))
                            GameManager.Instance.EquipItemPool.ReturnToPool(tmp);

                        else if (GameManager.Instance.GemItemPool.GetPoolList().Equals(datas))
                            GameManager.Instance.GemItemPool.ReturnToPool(tmp);

                        else if (GameManager.Instance.FoodItemPool.GetPoolList().Equals(datas))
                            GameManager.Instance.FoodItemPool.ReturnToPool(tmp);
                    }
                    else
                    {
                        GameManager.Instance.EquipItemPool.ReturnToPool(tmp);
                    }
                    continue;
                }

                if (dic_ItemClsForUpgrade.ContainsKey(tmp.GetItemcls()))
                    tmp.SetSelectBtnSpriteOn(true);

                // 버튼 클릭 이벤트 리스너 해제 및 연결
                Button btn = tmp.GetButton();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => ClickMaterialItemForEquipUpgrade(tmp, slectBtnCls, listObj));
                tmp.EquippedItemUIPrint(false); // 장비 중인 아  이템 표시용 이미지는 필요 없으므로 False
            }
        }
        
    }


    /* 순서
     * 기초경험치 == Button[2] == Grade
     * 레벨      == Button[1] == Level
     * 희귀도    == Button[0] == Name
     */
    // 정렬 버튼 파트(일괄처리 _ )
    // WeaponPrintAtScroll_BySelectButton _ 스크롤뷰 _ 정렬 버튼 이벤트 리스너
    private void WeaponPrintAtScroll_ForUpgrade_SortOrderBtnClick(InventorySortSelectButton selectBtnListObj, ButtonClass2 selectBtn, e_PoolItemType type)
    {
        selectBtnListObj.gameObject.SetActive(true);

        Button[] buttons = selectBtnListObj.GetButtons();

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.RemoveAllListeners();
        }

        // 리스트 버튼 객체에 이벤트 리스너 
        buttons[0].onClick.AddListener(() => SortOrderForUpgradeResorce(selectBtnListObj, "희귀도", selectBtn, type, ref list_ForItemUpgradeResourceSortOrder)); // 희귀도
        buttons[0].onClick.AddListener(() => PrintDataAtScrollViewForUpMaterials(selectBtn_ForUpgrade, type)); // 희귀도

        buttons[1].onClick.AddListener(() => SortOrderForUpgradeResorce(selectBtnListObj, "레벨", selectBtn, type, ref list_ForItemUpgradeResourceSortOrder)); // 레벨
        buttons[1].onClick.AddListener(() => PrintDataAtScrollViewForUpMaterials(selectBtn_ForUpgrade, type)); // 레벨

        buttons[2].onClick.AddListener(() => SortOrderForUpgradeResorce(selectBtnListObj, "기초경험치", selectBtn, type, ref list_ForItemUpgradeResourceSortOrder)); // 기초경험치
        buttons[2].onClick.AddListener(() => PrintDataAtScrollViewForUpMaterials(selectBtn_ForUpgrade, type)); // 기초경험치
    }

    /*
     * nWeaponUpgradeExp
       dic_ItemClsForWeaponUpgrade 변수에 선택한 객체의 데이터 저장 후, 사용 버튼 시에, 해당 아이템을 강화 재료로 씀.
     */
    // WeaponPrintAtScroll_BySelectButton _ 스크롤뷰 _ 콘텐츠 버튼(SelectButton) - 스크롤뷰 부착 오브젝트 클릭 이벤트 리스너
    void ClickMaterialItemForWeaponUpgrade(InvenItemObjClass cls, SelectButtonScript selectBtnCls, List<InvenItemObjClass> listObj = null)
    {
        ItemClass tmp = cls.GetItemcls();
        WeaponAndEquipCls selecWeapon = tmp as WeaponAndEquipCls;

        // 처음 선택하는 객체일 경우,
        if(dic_ItemClsForUpgrade.ContainsKey(tmp) == false)
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
                dic_ItemClsForUpgrade.Add(tmp, selectBtnCls);
                nWeaponUpgradeExp += selecWeapon.GetGrade() * 50;
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
                    dic_ItemClsForUpgrade.Add(tmp, data);
                    nWeaponUpgradeExp += selecWeapon.GetGrade() * 50;
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
                nWeaponUpgradeExp -= existCls.GetGrade() * 50;
                nWeaponUpgradeExp -= existCls.GetCurrentExp();
                dic_ItemClsForUpgrade.Remove(selectBtnCls.GetItemClass());

                SelectButtonToDefault(selectBtnCls);

                cls.SetSelectBtnSpriteOn(false);
            }
            else
            {
                List<InvenItemObjClass> datas;
                if (listObj != null)
                    datas = listObj;
                else
                    datas = GameManager.Instance.WeaponItemPool.GetPoolList();

                foreach (var item in datas)
                {
                    if (item.GetItemcls() == cls.GetItemcls())
                    {
                        WeaponAndEquipCls existCls = item.GetItemcls() as WeaponAndEquipCls;
                        nWeaponUpgradeExp -= existCls.GetGrade() * 50;
                        nWeaponUpgradeExp -= existCls.GetCurrentExp();
                        var btnObj = dic_ItemClsForUpgrade[item.GetItemcls()];
                        SelectButtonToDefault(btnObj);
                        dic_ItemClsForUpgrade.Remove(item.GetItemcls());


                        item.SetSelectBtnSpriteOn(false);
                        break;
                    }
                }
            }

            if(dic_ItemClsForUpgrade.Count<=0)
            {
                WeaponAndEquipCls originItem = GameManager.Instance.GetUserClass().GetUserEquippedWeapon() as WeaponAndEquipCls;
                ExpBarAlphaRevisePerpect(img_ExpBar, originItem);
            }
        }

        Debug.Log(nameof(nWeaponUpgradeExp )+":"+ nWeaponUpgradeExp);
        Debug.Log(nameof(dic_ItemClsForUpgrade.Count) +":"+ dic_ItemClsForUpgrade.Count);
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
    void AllClickWeaponButtonForUpgradeClickEventLeistener(e_PoolItemType type)
    {
        var selectButtonList = GameManager.Instance.SelectButtonScriptPool.GetPoolList();
        List<ItemClass> itemList = new List<ItemClass>();
        WeaponAndEquipCls equipItem = null;
        if(type == e_PoolItemType.Weapon)
        {
            
            itemList = GameManager.Instance.GetUserClass().GetHadWeaponList();
            equipItem = GameManager.Instance.GetUserClass().GetUserEquippedWeapon() as WeaponAndEquipCls;
        }
        else if(type == e_PoolItemType.Equip)
        {
            itemList = GameManager.Instance.GetUserClass().GetHadEquipmentList();
            equipItem = GameManager.Instance.GetUserClass().GetUserEquippedEquipment()[nSelected_EquipIndex] as WeaponAndEquipCls;
        }
        SortingItemList(itemList, list_ForItemUpgradeResourceSortOrder, isList_ForUpgradeResourceSortOrderAscending);

        if (nWeaponUpgradeExp >= equipItem.GetMaxExp() - equipItem.GetCurrentExp())
        {
            PopUpScreenFunc(popUpForUpgrade, "최대 경험치에 도달했습니다!");
            return;
        }

        // 보유하고 있는 아이템을 순회하며, 데이터를 출력한다
        foreach (var item in itemList)
        {
            if (item.GetIsActive() == true)
                continue;

            // 누적 경험치가 요구 경험치 이상이라면 반복문 탈출
            if (nWeaponUpgradeExp >= equipItem.GetMaxExp() - equipItem.GetCurrentExp())
            {
                NeededCostForUpgrade(nWeaponUpgradeExp, equipItem.GetMaxExp() - equipItem.GetCurrentExp(), ref nWeaponUpgradeCost, txt_NeedMora, txt_CurrentHaveMora, img_ExpBar);
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
                if (dic_ItemClsForUpgrade.ContainsKey(item) == false)
                {
                    var weaponData = item as WeaponAndEquipCls;
                    // 선택한 데이터 추가
                    dic_ItemClsForUpgrade.Add(item, btn);
                    nWeaponUpgradeExp += weaponData.GetGrade() * 50;
                    nWeaponUpgradeExp += weaponData.GetCurrentExp();

                    // 버튼 객체의 UI 작업
                    btn.SetItemCls(item);
                    btn.SetIsActive(true);
                    btn.GetItemImage().enabled = true;

                    if(type == e_PoolItemType.Weapon)
                        WeaponKindDivider(weaponData, btn.GetItemImage());
                    else if(type == e_PoolItemType.Equip)
                        EquipmentKindDivider(weaponData, btn.GetItemImage());

                    btn.SetItemText("LV. " + weaponData.GetLevel().ToString());
                    ItemUISetterByItemGrade(item, btn.GetTopBgr(), null);
                    NeededCostForUpgrade(nWeaponUpgradeExp, equipItem.GetMaxExp() - equipItem.GetCurrentExp(), ref nWeaponUpgradeCost, txt_NeedMora, txt_CurrentHaveMora, img_ExpBar);
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
            foreach(var tmp in dic_ItemClsForUpgrade)
            {
                ItemClass item = tmp.Key;
                GameManager.Instance.GetUserClass().GetHadWeaponList().Remove(item);
                SelectButtonToDefault(tmp.Value);
            }

            // 선택버튼에서 무기 UI 삭제
            dic_ItemClsForUpgrade.Clear();


            // UI최신화
            if (isLimit)    // 돌파 단계 도달 시,
            {
                //isWeaponUpgradeBtnClicked = false;
                ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Weapon);
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
            int index = i;
            var itemCls = dataList[index].RESOURCE_ITEM;
            int num = dataList[index].RESOURCE_NUMBER;
            Sprite spriteImg = WeaponAndEquipLimitBreak_UI_Dvider(itemCls);

            btnUIs[index].SetIsActive(true);
            btnUIs[index].SetItemCls(itemCls);                   // 클래스 세팅
            btnUIs[index].SetItemSprite(spriteImg);              // 아이템 이미지 세팅
            btnUIs[index].SetItemColor(itemCls.GetGrade());      // 색상 변환
            btnUIs[index].SetItemText(num.ToString());           // 개수 출력
            btnUIs[index].GetItemImage().enabled= true;

            // 보유 여부 및 개수 판단으로 돌파 가능 여부 검사
            var userData = GameManager.Instance.GetUserClass().GetHadGrowMaterialList().Find(tmp => tmp.GetName().Equals(itemCls.GetName()));
            if (userData != null)
            {
                int haveNum = userData.GetNumber();
                if (haveNum < num)
                {
                    isLimitBreakPossible = false;
                    btnUIs[index].GetItemTxt().color = Color.red;
                }
                else
                {
                    btnUIs[index].GetItemTxt().color = Color.green;
                }
                // 등급에 비례해서 요구 가격 계산
                nWeaponUpgradeCost += (int)(haveNum * userData.GetGrade() * 1000);
            }
            else
            {
                isLimitBreakPossible = false;
                btnUIs[index].GetItemTxt().color = Color.red;
            }
        }
    }

    // 무기 돌파 재료 출력 버튼 클릭 함수
    private void WeaponLimitBreakResourcePrintButtonClickLeitener(SelectButtonScript btnCls)
    {
        // 돌파재료가 모자란 경우,
        if(btnCls.GetItemTxt().color == Color.red)
        {
            getResourcePrintUI.Scroll_Start(btnCls.GetItemClass());
        }
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
            ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Weapon);
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
        ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Weapon);
        ResetToSelectButtonScriptPoolDatas();

        my.OnOffSpriteSetting();
        ClickedButtonsSetActiveReset(my,parents,6, new int[] { 4, 7 });
        var mainObject = parents.GetChild(6).GetComponent<Transform>();

        // 지역 변수 초기화
        dic_ItemClsForUpgrade.Clear();
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
        btnObj.GetButton().onClick.AddListener(() => ScrollViewOpenButtonClickEvent(btnObj, tr_ScrollViewObjectSet,e_PoolItemType.Weapon));
        btnObj.GetButton().onClick.AddListener(() => WeaponReforge_ButtonClickEvent(tr_ScrollViewObjectSet));
        
        cls_selectSortButton.GetButton().onClick.RemoveAllListeners();
        cls_selectSortButton.GetButton().onClick.AddListener(() => WeaponReforge_SortOrderBtnClick(cls_SelectionButtonOnList, cls_selectSortButton, tr_ScrollViewObjectSet));
        
        cls_Descending_AscendingButton.GetButton().onClick.RemoveAllListeners();
        cls_Descending_AscendingButton.GetButton().onClick.AddListener(() => AscendingValueReversalFunction(ref isList_ForUpgradeResourceSortOrderAscending, e_PoolItemType.Weapon));
        cls_Descending_AscendingButton.GetButton().onClick.AddListener(()=>WeaponReforge_ButtonClickEvent(tr_ScrollViewObjectSet));

        //스크롤뷰 객체
        // 정렬 디폴트 값 및 리스트 버튼 데이터 초기화
        list_ForItemUpgradeResourceSortOrder = e_SortingOrder.ExpOrder;
        isList_ForUpgradeResourceSortOrderAscending = false;
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
        
        WeaponPrintAtScroll(scrollForUpgradeMaterials, list_ForItemUpgradeResourceSortOrder, isList_ForUpgradeResourceSortOrderAscending, openUI_ItemList);
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
                tmp.SetIsActive(false);
                GameManager.Instance.WeaponItemPool.ReturnToPool(tmp);
                continue;
            }
            // 장비하고 있는 아이템과 일치하지 않는 경우는 제외
            if(tmp.GetItemcls().GetName() != equip.GetName())
            {
                tmp.SetIsActive(false);
                GameManager.Instance.WeaponItemPool.ReturnToPool(tmp);
                continue;
            }
            // 선택한 아이템과 동일한 경우도 제외
            if(tmp.GetItemcls() == selectBtn_ForUpgrade.GetItemClass())
            {
                tmp.SetIsActive(false);
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
        buttons[0].onClick.AddListener(() => SortOrderForUpgradeResorce(selectBtnListObj, "희귀도", selectBtn, e_PoolItemType.Weapon, ref list_ForItemUpgradeResourceSortOrder)); // 희귀도
        buttons[0].onClick.AddListener(() => WeaponReforge_ButtonClickEvent(mainObj));                              // 희귀도

        buttons[1].onClick.AddListener(() => SortOrderForUpgradeResorce(selectBtnListObj, "레벨", selectBtn, e_PoolItemType.Weapon, ref list_ForItemUpgradeResourceSortOrder)); // 레벨
        buttons[1].onClick.AddListener(() => WeaponReforge_ButtonClickEvent(mainObj));                              // 레벨

        buttons[2].onClick.AddListener(() => SortOrderForUpgradeResorce(selectBtnListObj, "기초경험치", selectBtn, e_PoolItemType.Weapon, ref list_ForItemUpgradeResourceSortOrder)); // 기초경험치
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
            ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Weapon);
            ResetToSelectButtonScriptPoolDatas();
            ClosePlayerInfoScreenButton();
            WeaponUpgeadeButtonClickEvent(2);
        }
    }
    #endregion

    #endregion


    #region 성유물 UI 관리

    #region 기본 데이터 출력
    void EquipmentPrint()
    {
        // 장비 중인 성유물 객체 데이터 프린트 함수
        PrintEquippedEquipment();

        ///버튼 객체 인스턴스화
        ButtonClass2 cls_MoreInformationBtn = printInfoDataField[2].transform.GetChild(1).GetChild(4).GetComponent<ButtonClass2>();
        ButtonClass2 cls_ChangeButton = printInfoDataField[2].transform.GetChild(1).GetChild(5).GetComponent<ButtonClass2>();
        var detailPrintobj = printInfoDataField[2].transform.GetChild(1).GetChild(6).GetComponent<Transform>();
        detailPrintobj.gameObject.SetActive(false);

        // 상세정보 출력 버튼 이벤트 리스너 연결
        cls_MoreInformationBtn.GetButton().onClick.RemoveAllListeners();
        ButtonClass2_Reset(cls_MoreInformationBtn);
        cls_MoreInformationBtn.GetButton().onClick.AddListener(()=>DetailedEquipInfoPrint());

        // 변수 관리
        touchDic = TouchPadController.e_TouchSlideDic.None;
        nSelected_EquipIndex = 0;
        isUpgrade_Info_Open = false;

        // 성유물 교체 버튼 이벤트 리스너 연결 
        cls_ChangeButton.GetButton().onClick.RemoveAllListeners();
        ButtonClass2_Reset(cls_ChangeButton);
        cls_ChangeButton.GetButton().onClick.AddListener(() => WhenEquipPrint_SelectEquipMover(0)); // 디폴트는 0번

        // 터치패드 인스턴스화
        touchPad = printInfoDataField[2].transform.GetChild(4).GetComponent<TouchPadController>();
        // 성유물 회전 버튼 이벤트 리스너 연결 및 데이터 출력
        rotateBtnObj = printInfoDataField[2].transform.GetChild(5).GetComponent<ButtonRotateSetCls>();
        RotateEquipEquipMentPrint();

        // 정렬 변수 초기화
        scrollPrintEquipOrder = e_SortingOrder.GradeOrder;
        isScrollPrintEquipAscending = false;

        // 스크롤뷰 객체 비활성화
        printInfoDataField[2].transform.GetChild(6).gameObject.SetActive(false);
        // 스크롤뷰 객체 내부 UI객체 초기 세팅
        InventorySortSelectButton btnListObj = printInfoDataField[2].transform.GetChild(6).GetChild(4).GetComponent<InventorySortSelectButton>();
        // 정렬 디폴트 값 및 리스트 버튼 데이터 초기화
        string[] strings = { "이름", "레벨", "희귀도" };
        ListButtonObject_UI_Initer(btnListObj, strings, scrollPrintEquipOrder);
        btnListObj.HideButtonBackGround(2);
        
        ButtonClass2 selectOrderBtn = printInfoDataField[2].transform.GetChild(6).GetChild(2).GetComponent<ButtonClass2>();
        selectOrderBtn.SetButtonTextInputter("희귀도");

        // 스크롤뷰 - 강화 버튼 이벤트리스너 연결 작업
        var upgradeBtn = printInfoDataField[2].transform.GetChild(6).GetChild(17).GetComponent<ButtonClass2>();
        upgradeBtn.GetButton().onClick.RemoveAllListeners();
        ButtonClass2_Reset(upgradeBtn);
        upgradeBtn.GetButton().onClick.AddListener(() => EquipmentUpgradeButtonClickEvent(0));

        // 불필요한 객체 비활성화
        printInfoDataField[2].transform.GetChild(7).gameObject.SetActive(false);
    }
    // 장비 중인 성유물 객체 데이터 프린트 함수_최상단 기본 창
    void PrintEquippedEquipment()
    {
        /// 인스턴스 초기화
        // 기본 능력치
        TextMeshProUGUI txtHp = printInfoDataField[2].transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtAtk = printInfoDataField[2].transform.GetChild(1).GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtDef = printInfoDataField[2].transform.GetChild(1).GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtElemnt = printInfoDataField[2].transform.GetChild(1).GetChild(3).GetChild(2).GetComponent<TextMeshProUGUI>();
        // 세트 효과
        TextMeshProUGUI txtSetSynergy_1 = printInfoDataField[2].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtSetSynergy_2 = printInfoDataField[2].transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        var EquipmentList = GameManager.Instance.GetUserClass().GetUserEquippedEquipment();
        // 세트 텍스트 오리지널 컬러
        Color originColor = new Color(163f / 255f, 210f / 255f, 113f / 255f);

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
        txtSetSynergy_1.color = originColor;
        txtSetSynergy_2.color = originColor;

        txtHp.text = hp.ToString();
        txtDef.text = def.ToString();
        txtAtk.text = atk.ToString();
        txtElemnt.text = element.ToString();

    }
    void RotateEquipEquipMentPrint()
    {
        touchPad.gameObject.SetActive(true);
        rotateBtnObj.gameObject.SetActive(true);

        var imgs = rotateBtnObj.GetImages();                                            // 회전 객체의 자식 컴포넌트 인스턴스화
        var insideimgs = rotateBtnObj.GetInsideImages();                                // 회전 객체의 자식 컴포넌트 인스턴스화
        var btns = rotateBtnObj.GetButtons();                                           // 회전 객체의 자식 컴포넌트 인스턴스화
        var equips = GameManager.Instance.GetUserClass().GetUserEquippedEquipment();    // 장착한 성유물
        for (int i = 0; i < equips.Length; i++)
        {
            int num = i;
            btns[i].onClick.RemoveAllListeners();                                       // 버튼 함수 해제1
            btns[i].onClick.AddListener(() => WhenEquipPrint_SelectEquipMover(num));                        // 버튼 함수 연결
            if (equips[i] == null)
                continue;
            var item = equips[i] as WeaponAndEquipCls;
            EquipmentKindDivider(item, insideimgs[i]);
            ItemUISetterByItemGrade(item, imgs[i]);
            
            Color imageColor = insideimgs[i].color;      // 이미지의 현재 색상을 가져옴
            imageColor.a = 1f;                           // 알파값을 0으로 설정하여 투명하게 만듦
            insideimgs[i].color = imageColor;            // 이미지의 색상을 변경
        }
    }
    
    // 상세 정보창 출력
    void DetailedEquipInfoPrint()
    {
        // 상세 데이터 화면 출력 - 인스턴스 초기화 및 객체 활성화
        touchPad.gameObject.SetActive(false);
        rotateBtnObj.gameObject.SetActive(false);

        InfoPrintTypeButtonUnActive();
        isDetailScreenOpenForEquip = true;
        var obj = printInfoDataField[2].transform.GetChild(1).GetChild(6).GetComponent<Transform>();
        obj.gameObject.SetActive(true);

        Transform content = obj.GetChild(0).GetChild(0).GetChild(0).GetComponent<Transform>();
        TextMeshProUGUI[] statusTexts = new TextMeshProUGUI[11];
        // 데이터 출력을 위한 TextMeshPro 배열에 저장
        for (int i = 0; i < 5; i++)
        {
            statusTexts[i] = content.GetChild(1 + i).GetChild(2).GetComponent<TextMeshProUGUI>();
        }
        for (int i = 0; i < 6; i++)
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
        float elementCharge = 0;
        float normalAtkDamage = 0;
        float skillAtkDamage = 0;
        float damageIncrease = 0;
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
                ApplyEquipmentEffect(dbItem1, ref atk, ref def, ref hp, ref element, ref criticalRate, ref criticalDamage, ref elementCharge, ref normalAtkDamage, ref skillAtkDamage, ref damageIncrease);
                ApplyEquipmentEffect(dbItem2, ref atk, ref def, ref hp, ref element, ref criticalRate, ref criticalDamage, ref elementCharge, ref normalAtkDamage, ref skillAtkDamage, ref damageIncrease);
                break;
            }
            else if (count >= 2)
            {
                var db = GameManager.Instance.GetList_EquipmentSetSynergyData();
                var dbItem = db.Find(tmp => tmp.EQUIPMENT_SET_NAME.Equals(set) && tmp.EQUIPMENT_SET_NUMBER == 2);
                ApplyEquipmentEffect(dbItem, ref atk, ref def, ref hp, ref element, ref criticalRate, ref criticalDamage, ref elementCharge, ref normalAtkDamage, ref skillAtkDamage, ref damageIncrease);
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
        statusTexts[7].text = elementCharge.ToString();
        statusTexts[8].text = normalAtkDamage.ToString();
        statusTexts[9].text = skillAtkDamage.ToString();
        statusTexts[10].text = damageIncrease.ToString();

    }

    #endregion

    #region 보유 성유물 스크롤뷰 출력

    /** 회전 성유물 버튼 객체
     *  버튼Obj세트 0~4번 // => 애니메이션 1~5 // 가장 좌측 버튼 객체로부터 시작하여 반시계방향으로 돌며 ++1
     *  0 - 꽃
     *  1 - 깃털
     *  2 - 모래
     *  3 - 성배
     *  4 - 왕관
     */
    // 보유 성유물 스크롤뷰 출력 함수
    void PrintHadEquipmentAtScroll(int index)
    {
        nSelected_EquipIndex = index;
        ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Equip);

        var mainObj = printInfoDataField[2].transform.GetChild(6).GetComponent<Transform>();
        ButtonClass2[] buttons = new ButtonClass2[5];
        for(int i=0; i<5; i++)                  //  성유물 타입 버튼 인스턴스화
            buttons[i] = mainObj.GetChild(i+7).GetComponent<ButtonClass2>();

        // 버튼 객체의 UI 세팅 및 버튼 리스너 연결
        for(int i =0; i<buttons.Length; i++)
        {
            int num = i;
            // 클릭 리스너 할당
            buttons[num].GetButton().onClick.RemoveAllListeners();
            ButtonClass2_Reset(buttons[num]);
            buttons[num].GetButton().onClick.AddListener(() => WhenEquipPrint_SelectEquipMover(num));

            buttons[i].GetBackGroundImg().enabled = false;
            buttons[i].GetSymbolImg().color = Color.white;
            // 버튼 UI 수정
            if (i == index)
            {
                buttons[i].GetBackGroundImg().enabled|= true;
                buttons[i].GetSymbolImg().color = ItemSpritesSaver.Instance.GetDarkColor();
            }
        }

        // 스크롤뷰 콘텐츠 객체 인스턴스화
        Transform scrollContent = mainObj.GetChild(1).GetChild(0).GetComponent<Transform>();
        
        // 정렬 선택 버튼 객체 인스턴스화 및 관련 함수와 데이터 연결
        InventorySortSelectButton btnListObj = mainObj.GetChild(4).GetComponent<InventorySortSelectButton>();
        btnListObj.gameObject.SetActive(false);
        ButtonClass2 selectOrderBtn = mainObj.GetChild(2).GetComponent<ButtonClass2>();
        ButtonClass2 selectAscendingBtn = mainObj.GetChild(3).GetComponent<ButtonClass2>();

        // 데이터 출력, 후 태그를 비교하여, 일치하는 데이터를 제외한 나머지는 오브젝트 풀 리턴
        EquipPrintAtScroll(scrollContent, scrollPrintEquipOrder, isScrollPrintEquipAscending);
        string[] tags = { "꽃", "깃털", "모래", "성배", "왕관" };

        var equipPool = GameManager.Instance.EquipItemPool;
        foreach(var obj in equipPool.GetPoolList())
        {
            if (obj.gameObject.activeSelf == false) continue;
            obj.GetButton().onClick.RemoveAllListeners();
            if (obj.GetItemcls().GetTag().Equals(tags[index]))
            {
                obj.GetButton().onClick.AddListener(() => EquipmentChangeFunction(obj, index));
                continue;
            }

            obj.SetIsActive(false);
            equipPool.ReturnToPool(obj);
        }

        // 선택한 성유물의 데이터 출력
        var itemClass = GameManager.Instance.GetUserClass().GetUserEquippedEquipment()[index] as WeaponAndEquipCls;
        EquipmentUI_Print(itemClass, mainObj);
    }

    // 리스트 출력 했을 때, 선택한 성유물 객체를 이동함 
    void WhenEquipPrint_SelectEquipMover(int index)
    {
        nSelected_EquipIndex = index;
        // 초기 세팅
        CloseButtonSpriteToBack();
        InfoPrintTypeButtonUnActive();
        isEquipmentPrintScrollOpen = true;
        printInfoDataField[2].transform.GetChild(1).gameObject.SetActive(false);
        //printInfoDataField[2].transform.GetChild(6).gameObject.SetActive(true);
        touchPad.gameObject.SetActive(false);
        rotateBtnObj.SetAniControl_Play(index);         // 성유물 선택 애니메이션 호출


        var mainObj = printInfoDataField[2].transform.GetChild(6).GetComponent<Transform>();
        mainObj.gameObject.SetActive(true);
        InventorySortSelectButton btnListObj = mainObj.GetChild(4).GetComponent<InventorySortSelectButton>();
        btnListObj.gameObject.SetActive(false);

        // 스크롤뷰 출력 함수
        //ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Equip);
        PrintHadEquipmentAtScroll(index);

        // 정렬 선택 버튼 리스너 연결
        ButtonClass2 selectOrderBtn = mainObj.GetChild(2).GetComponent<ButtonClass2>();
        Button selectBtn = selectOrderBtn.GetButton();
        selectBtn.onClick.RemoveAllListeners();
        ButtonClass2_Reset(selectOrderBtn);
        selectBtn.onClick.AddListener(() => EquipScroll_SortOrderBtnClick(btnListObj, selectOrderBtn, index));

        // 오름차/내림차순 버튼 리스너 연결
        ButtonClass2 selectAscendingBtn = mainObj.GetChild(3).GetComponent<ButtonClass2>();
        selectAscendingBtn.GetButton().onClick.RemoveAllListeners();
        ButtonClass2_Reset(selectAscendingBtn);
        selectAscendingBtn.GetButton().onClick.AddListener(() => AscendingValueReversalFunction(ref isScrollPrintEquipAscending, e_PoolItemType.Equip));
        selectAscendingBtn.GetButton().onClick.AddListener(() => PrintHadEquipmentAtScroll(index));

        // 세트 효과 출력
        var item = GameManager.Instance.GetUserClass().GetUserEquippedEquipment()[index] as WeaponAndEquipCls;
        EquipUI_SetSynergyPrint(item);
    }
    // 성유물 출력 정렬 선택 버튼 이벤트 리스너
    void EquipScroll_SortOrderBtnClick(InventorySortSelectButton selectBtnListObj, ButtonClass2 selectBtn, int index)
    {
        selectBtnListObj.gameObject.SetActive(true);

        Button[] buttons = selectBtnListObj.GetButtons();

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.RemoveAllListeners();
        }

        // 리스트 버튼 객체에 이벤트 리스너 
        buttons[0].onClick.AddListener(() => SortOrderForChange(selectBtnListObj, "이름", selectBtn, e_PoolItemType.Equip, ref scrollPrintEquipOrder)); // 희귀도
        buttons[0].onClick.AddListener(() => selectBtnListObj.HideButtonBackGround(0));
        buttons[0].onClick.AddListener(() => PrintHadEquipmentAtScroll(index));                              

        buttons[1].onClick.AddListener(() => SortOrderForChange(selectBtnListObj, "레벨", selectBtn, e_PoolItemType.Equip, ref scrollPrintEquipOrder)); // 레벨
        buttons[1].onClick.AddListener(() => selectBtnListObj.HideButtonBackGround(1));
        buttons[1].onClick.AddListener(() => PrintHadEquipmentAtScroll(index));                             

        buttons[2].onClick.AddListener(() => SortOrderForChange(selectBtnListObj, "희귀도", selectBtn, e_PoolItemType.Equip, ref scrollPrintEquipOrder)); // 기초경험치
        buttons[2].onClick.AddListener(() => selectBtnListObj.HideButtonBackGround(2));
        buttons[2].onClick.AddListener(() => PrintHadEquipmentAtScroll(index));                                  
    }
    #endregion

    #region 성유물 교체

    // 성유물 교체 함수
    private void EquipmentChangeFunction(InvenItemObjClass obj, int index)
    {
        var cls = obj.GetItemcls();     //선택한 아이템 클래스
        var reCls = cls as WeaponAndEquipCls;
        var inImage = rotateBtnObj.GetInsideImages()[index];
        var outImage = rotateBtnObj.GetImages()[index];
        string[] tags = { "꽃", "깃털", "모래", "성배", "왕관" };
        if (cls.GetTag() != tags[index])
            return;

        var equipList = GameManager.Instance.GetUserClass().GetHadEquipmentList(); // 보유하고 있는 아이템 리스트 인스턴스화
        var equipedEquipList = GameManager.Instance.GetUserClass().GetUserEquippedEquipment();  // 착용하고 있는 아이템 리스트 인스턴스화

        // 선택한 아이템과 장착한 아이템이 같지 않을 경우,
        if(equipedEquipList[index]!= null && obj.GetItemcls().Equals(equipedEquipList[index]) == false)
        {
            CharacterDataReviseWhenEquipmentTakeOff(index);
            ItemClass ExistingItem = equipedEquipList[index];                                               // 기존 장착중인 아이템 가져오기
            ItemClass tmp = equipList.Find(item => item.Equals(ExistingItem));                              // 기존 장착중인 아이템 리스트에서 찾기
            tmp.SetActive(false);                                                                           // 장착 중이던 아이템을 활성화 X

            ItemClass SelectItem = equipList.Find(item => item.Equals(cls));                        // 선택한 아이템 찾기
            SelectItem.SetActive(true);                                                             // 장착할 아이템 활성화 O
            GameManager.Instance.GetUserClass().SetEquipedEquipmentList(SelectItem, index);         // 선택한 아이템 장착
            CharacterDataReviseToEquipment(index);                                                  // 착용한 장비 데이터 수정

            // 회전 선택 버튼에 이미지 부여
            EquipmentKindDivider(reCls, inImage);
            ItemUISetterByItemGrade(reCls, outImage);

            // 선택한 아이템의 세트 효과 출력
            EquipUI_SetSynergyPrint(reCls);

            // 세트 효과 반영 갱신
            EquipmentSetSynergyUnApplyer();
            EquipmentSetSynergyApplyer();
        }
        else if(equipedEquipList[index] == null)
        {
            // 선택한 성유물 파츠의 착용 여부가 null일 경우, 바로 장착
            ItemClass SelectItem = equipList.Find(item => item.Equals(cls));                        // 선택한 아이템 찾기
            SelectItem.SetActive(true);                                                             // 장착할 아이템 활성화 O
            GameManager.Instance.GetUserClass().SetEquipedEquipmentList(SelectItem, index);         // 선택한 아이템 장착
            CharacterDataReviseToEquipment(index);                                                  // 착용한 장비 데이터 수정

            // 회전 선택 버튼에 이미지 부여
            EquipmentKindDivider(reCls, inImage);
            ItemUISetterByItemGrade(reCls, outImage);

            // inImage의 알파 값을 최대 (1.0)로 설정
            Color inImageColor = inImage.color;
            inImageColor.a = 1.0f;
            inImage.color = inImageColor;

            // 선택한 아이템의 세트 효과 출력
            EquipUI_SetSynergyPrint(reCls);

            // 세트 효과 반영 갱신
            EquipmentSetSynergyUnApplyer();
            EquipmentSetSynergyApplyer();
        }
        else
        {
            //조건문을 모두 통과했다면, 장착했던 장비==선택한 장비이기에, 장착했던 장비를 해제한다.
            CharacterDataReviseWhenEquipmentTakeOff(index);
            ItemClass ExistingItem = equipedEquipList[index]; // 기존 장착중인 아이템 가져오기
            ItemClass tmp = equipList.Find(item => item.Equals(ExistingItem));                              // 기존 장착중인 아이템 리스트에서 찾기
            tmp.SetActive(false);                                                                           // 장착 중이던 아이템을 활성화 X
            equipedEquipList[index] = null;                                                                            // 장착 해제

            // 회전 선택 버튼에 이미지 부여
            outImage.color = Color.white;

            // inImage의 알파 값을 최저(0.0f)로 설정
            Color inImageColor = inImage.color;
            inImageColor.a = 0f;
            inImage.color = inImageColor;

            // 세트 효과 반영 갱신
            EquipmentSetSynergyUnApplyer();
            // 아이템 세트 효과 지우기
            EquipUI_SetSynergyPrint(null);
        }
        // 새로 변경된 UI 사항을 ScrollView에 출력
        PrintHadEquipmentAtScroll(index);
    }
    #endregion

    #region 성유물 _ 데이터 출력 함수

    // 장착 한 성유물 정보 출력
    void EquipmentUI_Print(ItemClass itemCls, Transform mainObj)
    {
        TextMeshProUGUI txt_Name = mainObj.GetChild(12).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txt_Type = mainObj.GetChild(13).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txt_MainStatLabel = mainObj.GetChild(14).GetChild(1).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txt_MainStatValue = mainObj.GetChild(14).GetChild(2).GetComponent<TextMeshProUGUI>();
        Image[] img_Stars = mainObj.GetChild(15).GetComponentsInChildren<Image>();
        TextMeshProUGUI txt_Level = mainObj.GetChild(16).GetChild(0).GetComponent<TextMeshProUGUI>();
        if(itemCls!= null)
        {
            // 데이터 출력
            var reCls = itemCls as WeaponAndEquipCls;
            txt_Name.text = reCls.GetName();
            txt_Type.text = reCls.GetTag();
            EquipmentKindDivider(reCls, null, txt_MainStatLabel, txt_MainStatValue);
            foreach (var tmp in img_Stars) tmp.enabled = false;
            for (int i = 0; i < reCls.GetGrade(); i++)
                img_Stars[i].enabled = true;
            txt_Level.text = "LV." + reCls.GetLevel().ToString();
        }
        else
        {
            txt_Name.text = "";
            txt_Type.text = "";
            txt_MainStatLabel.text = "";
            txt_MainStatValue.text = "";
            foreach (var tmp in img_Stars) tmp.enabled = false;
            txt_Level.text = "";
        }


    }
    // 선택 아이템 세트 효과 출력 함수
    void EquipUI_SetSynergyPrint(WeaponAndEquipCls itemCls)
    {
        Color originColor = new Color(163f / 255f, 210f / 255f, 113f / 255f);
        TextMeshProUGUI set1 = printInfoDataField[2].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI set2 = printInfoDataField[2].transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        set1.color = originColor;
        set2.color = originColor;

        if(itemCls!= null)
        {
            string set = itemCls.GetSet();
            var db = GameManager.Instance.GetList_EquipmentSetSynergyData();
            var dbItem1 = db.Find(tmp => tmp.EQUIPMENT_SET_NAME.Equals(set) && tmp.EQUIPMENT_SET_NUMBER == 2);
            var dbItem2 = db.Find(tmp => tmp.EQUIPMENT_SET_NAME.Equals(set) && tmp.EQUIPMENT_SET_NUMBER == 4);

            set1.text = set + "\n" + "2세트 " + dbItem1.EQUIPMENT_SET_EFFECT_TEXT;
            set2.text = set + "\n" + "4세트 " + dbItem2.EQUIPMENT_SET_EFFECT_TEXT;

            int cnt = 0;
            foreach (var tmp in GameManager.Instance.GetUserClass().GetUserEquippedEquipment())
            {
                if (tmp != null && tmp.GetSet() == set)
                    cnt++;
            }

            if (cnt < 4)
                set2.color = Color.gray;
            if (cnt < 2)
                set1.color = Color.gray;
        }
        else
        {
            set1.text = "";
            set2.text = ""; 
        }
    }
    void EquipUI_SetSynergyPrint(WeaponAndEquipCls itemCls, TextMeshProUGUI setTxt1, TextMeshProUGUI setTxt2)
    {
        Color originColor = new Color(163f / 255f, 210f / 255f, 113f / 255f);
        TextMeshProUGUI set1 = printInfoDataField[2].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI set2 = printInfoDataField[2].transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        set1.color = originColor;
        set2.color = originColor;

        if (itemCls != null)
        {
            string set = itemCls.GetSet();
            var db = GameManager.Instance.GetList_EquipmentSetSynergyData();
            var dbItem1 = db.Find(tmp => tmp.EQUIPMENT_SET_NAME.Equals(set) && tmp.EQUIPMENT_SET_NUMBER == 2);
            var dbItem2 = db.Find(tmp => tmp.EQUIPMENT_SET_NAME.Equals(set) && tmp.EQUIPMENT_SET_NUMBER == 4);

            setTxt1.text = set + "\n" + "2세트 " + dbItem1.EQUIPMENT_SET_EFFECT_TEXT;
            setTxt2.text = set + "\n" + "4세트 " + dbItem2.EQUIPMENT_SET_EFFECT_TEXT;

            int cnt = 0;
            foreach (var tmp in GameManager.Instance.GetUserClass().GetUserEquippedEquipment())
            {
                if (tmp != null && tmp.GetSet() == set)
                    cnt++;
            }

            if (cnt < 4)
                setTxt2.color = Color.gray;
            if (cnt < 2)
                setTxt1.color = Color.gray;
        }
        else
        {
            setTxt1.text = "";
            setTxt2.text = "";
        }
    }
    #endregion

    #region 성유물 _ 상세정보 및 업그레이드 출력
    /* 강화 버튼 클릭
     * 상세정보, 강화 창 출력
     * 관련 함수 구현
    **/
    // 강화 버튼 클릭 이벤트 리스너
    void EquipmentUpgradeButtonClickEvent(int index)
    {
        // 장비 중이 아이템이 없을 때, 강화 버튼을 누르면 리턴
        if (GameManager.Instance.GetUserClass().GetUserEquippedEquipment()[nSelected_EquipIndex] == null)
            return;

        isUpgrade_Info_Open = true;
        touchPad.gameObject.SetActive(false);

        InfoPrintTypeButtonUnActive();
        CloseButtonSpriteToBack();
        infoSelectButtons[2].OnOffSpriteSetting();

        printInfoDataField[2].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        printInfoDataField[2].transform.GetChild(2).gameObject.SetActive(false);
        printInfoDataField[2].transform.GetChild(3).gameObject.SetActive(false);
        printInfoDataField[2].transform.GetChild(6).gameObject.SetActive(false);

        // 강화/정보 출력을 담당하는 UI 객체 인스턴스화 및 활성화
        var mainObj = printInfoDataField[2].transform.GetChild(7).GetComponent<Transform>();
        mainObj.gameObject.SetActive(true);
        mainObj.GetChild(3).gameObject.SetActive(false);    // 상세정보 창은 우선 SetFalse
        mainObj.GetChild(4).gameObject.SetActive(false);    // 강화 창은 우선 SetFalse

        isEquipUpgradeDetailObjClicked = new bool[2] { false,false };
        string[] texts = { "상세", "강화" };

        PlayerInfoUI_Button[] btnObjs = mainObj.GetComponentsInChildren<PlayerInfoUI_Button>();

        for(int i=0; i<btnObjs.Length; i++)
        {
            var btnObj = btnObjs[i];

            btnObj.SetText(texts[i]);

            // 버튼에 연결되어 있는 기존 리스너 이벤트 제거
            Button btn = btnObj.GetButton();
            btn.onClick.RemoveAllListeners();

            switch(btnObj.GetText())
            {
                case "상세":
                    btnObj.GetButton().onClick.AddListener(() => EquipmentDetailPrint(btnObj,mainObj));
                    break;
                case "강화":
                    btnObj.GetButton().onClick.AddListener(() => EquipmentUpgradePrint(btnObj, mainObj));
                    break;
            }
        }

        // index에 따른 UI 출력 함수 연결
        // 디폴트 출력은 상세 정보 출력
        PlayerInfoUI_Button button = btnObjs[index];
        if(index == 0)
            EquipmentDetailPrint(button, mainObj);
        else if(index ==1)
            EquipmentUpgradePrint(button, mainObj);
    }
    // 상세 정보 출력
    void EquipmentDetailPrint(PlayerInfoUI_Button typeBtnObj, Transform mainObj)
    {
        if (isEquipUpgradeDetailObjClicked[0] == true)
            return;
        typeBtnObj.OnOffSpriteSetting();
        ClickedButtonsSetActiveReset(typeBtnObj, mainObj, 3, new int[2] { 3, 5 });

        ButtonClass2 useButton = mainObj.GetChild(2).GetComponent<ButtonClass2>();
        useButton.gameObject.SetActive(false);


        // 객체 인스턴스화 및 데이터 출력
        Transform content = mainObj.GetChild(3).GetChild(0).GetChild(0).GetComponent<Transform>();
        
        WeaponAndEquipCls itemCls = GameManager.Instance.GetUserClass().GetUserEquippedEquipment()[nSelected_EquipIndex] as WeaponAndEquipCls;

        // 라벨 프레임
        content.gameObject.SetActive(true);
        Image LabelFrame = content.GetChild(0).GetComponent<Image>();
        TextMeshProUGUI labelText = LabelFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        ItemUISetterByItemGrade(itemCls, LabelFrame);
        labelText.text = itemCls.GetName();

        // 탑 프레임
        Image TopFrame = content.GetChild(1).GetComponent<Image>();
        Image EquipImg = TopFrame.transform.GetChild(0).GetComponent<Image>();
        TextMeshProUGUI txt_Tag = TopFrame.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txt_StatLabel = TopFrame.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txt_StatValue = TopFrame.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
        Image[] img_Stars = TopFrame.transform.GetChild(1).GetChild(3).GetComponentsInChildren<Image>();

        ItemUISetterByItemGrade(itemCls, null, TopFrame);
        txt_Tag.text = itemCls.GetTag();
        EquipmentKindDivider(itemCls,EquipImg,txt_StatLabel,txt_StatValue);
        foreach(var img in img_Stars) img.enabled= false;
        for(int i=0; i<itemCls.GetGrade(); i++)
            img_Stars[i].enabled= true;

        // 바텀 프레임
        Image BottomFrame = content.GetChild(2).GetComponent<Image>();
        TextMeshProUGUI levelText = BottomFrame.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI setText = BottomFrame.transform.GetChild(1).GetComponent<TextMeshProUGUI>();    
        TextMeshProUGUI setSynergy_1_text = BottomFrame.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI setSynergy_2_text = BottomFrame.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Content_text = BottomFrame.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        Button btn_FindGetItem = BottomFrame.transform.GetChild(6).GetComponent<Button>();

        levelText.text = "Lv. "+itemCls.GetLevel().ToString();
        setText.text = itemCls.GetSet();
        EquipUI_SetSynergyPrint(itemCls,setSynergy_1_text,setSynergy_2_text);
        Content_text.text = itemCls.GetContent();

        isEquipUpgradeDetailObjClicked[0] = true;
        isEquipUpgradeDetailObjClicked[1] = false;
    }
    // 업그레이드 출력
    void EquipmentUpgradePrint(PlayerInfoUI_Button typeBtnObj, Transform parent)
    {
        if (isEquipUpgradeDetailObjClicked[1] == true)
            return;
        typeBtnObj.OnOffSpriteSetting();
        ClickedButtonsSetActiveReset(typeBtnObj, parent, 4, new int[2] { 3, 5 });

        Transform mainObj = parent.GetChild(4).GetComponent<Transform>();

        // 강화 버튼 이벤트 리스너 연결
        ButtonClass2 useButton = parent.GetChild(2).GetComponent<ButtonClass2>();
        useButton.gameObject.SetActive(true);
        useButton.GetButton().onClick.RemoveAllListeners();
        useButton.GetButton().onClick.AddListener(() => EquipLevelUpClickEventListener(mainObj));

        var itemCls = GameManager.Instance.GetUserClass().GetUserEquippedEquipment()[nSelected_EquipIndex] as WeaponAndEquipCls;

        // 일괄 처리 객체 인스턴스화 및 이벤트 연결
        ButtonClass2 all_FillBtn = mainObj.GetChild(0).GetComponent<ButtonClass2>();
        ButtonClass2 sortSelectBtn = mainObj.GetChild(1).GetComponent<ButtonClass2>();   
        InventorySortSelectButton listBtnObject = mainObj.GetChild(2).GetComponent<InventorySortSelectButton>();

        // 정렬 선택 버튼 클릭 이벤트 리스너 연결
        sortSelectBtn.GetButton().onClick.RemoveAllListeners();
        ButtonClass2_Reset(sortSelectBtn);
        sortSelectBtn.GetButton().onClick.AddListener(() => SelectSortButtonClickEventListener(listBtnObject,sortSelectBtn));
        sortSelectBtn.SetButtonTextInputter("★3 재료");
        // 일괄처리 버튼 클릭 이벤트 리스너 연결
        all_FillBtn.GetButton().onClick.RemoveAllListeners();
        ButtonClass2_Reset(all_FillBtn);
        all_FillBtn.GetButton().onClick.AddListener(() => AllClickWeaponButtonForUpgradeClickEventLeistener(e_PoolItemType.Equip));
        allPull_OrderValue = e_SortingOrder.NameOrder;


        // 변수 관리 및 초기화
        selectBtn_ForUpgrade = null;
        scrollForUpgradeMaterials = null;
        nWeaponUpgradeCost = 0;
        nWeaponUpgradeExp = 0;
        dic_ItemClsForUpgrade = new Dictionary<ItemClass, SelectButtonScript>();
        txt_NeedMora = mainObj.GetChild(11).GetChild(1).GetComponent<TextMeshProUGUI>();
        txt_CurrentHaveMora = mainObj.GetChild(12).GetChild(1).GetComponent<TextMeshProUGUI>();
        popUpForUpgrade = mainObj.GetChild(13).GetComponent<Transform>();
        popUpForUpgrade.gameObject.SetActive(false);
        list_ForItemUpgradeResourceSortOrder = e_SortingOrder.ExpOrder;
        isList_ForUpgradeResourceSortOrderAscending= false;

        // 리스트 버튼_For 일괄처리 선택 기준
        listBtnObject.gameObject.SetActive(false);  // 리스트 버튼은 기본 비활성화
        string[] strings = { "★3 재료", "★4 재료", "★5 재료" };
        ListButtonObject_UI_Initer(listBtnObject, strings, e_SortingOrder.NameOrder);   // 디폴트 3성
        

        // 재료로써 사용할 장비 선택 스크롤뷰 객체
        Transform selectBtnScrollView = mainObj.GetChild(3).GetChild(0).GetChild(0).GetComponent<Transform>();

        // 보유한 성유물 출력 스크롤뷰 객체
        Transform HaveEquipPrintScrollViewObject = mainObj.GetChild(8).GetComponent<Transform>();
        HaveEquipPrintScrollViewObject.gameObject.SetActive(false);     // 보유 성유물 출력 스크롤뷰 객체는 기본 비활성화


        // 선택 버튼 오브젝트 풀 Get 후, 함수 연결
        WeaponPrintAtScroll_BySelectButtonOnDefualtValue(selectBtnScrollView, 10);
        List<SelectButtonScript> selectButtonScripts = GameManager.Instance.SelectButtonScriptPool.GetPoolList();
        foreach(var tmp in selectButtonScripts)
        {
            Button btn = tmp.GetButton();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => tmp.OnClickEventListener());
            btn.onClick.AddListener(() => ScrollViewOpenButtonClickEvent(tmp, HaveEquipPrintScrollViewObject, e_PoolItemType.Equip));
            btn.onClick.AddListener(() => WeaponSelectForUpgradeBtnClickEventListener(tmp, HaveEquipPrintScrollViewObject, e_PoolItemType.Equip));
        }


        // 데이터 출력
        EquipStatusPrint(mainObj, itemCls);


        isEquipUpgradeDetailObjClicked[0] = false;
        isEquipUpgradeDetailObjClicked[1] = true;
    }
    // 기본 데이터 출력
    void EquipStatusPrint(Transform mainObj, WeaponAndEquipCls itemCls)
    {
        TextMeshProUGUI txt_Level = mainObj.GetChild(4).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txt_StatLabel = mainObj.GetChild(9).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txt_StatValue = mainObj.GetChild(10).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txt_ExpValue = mainObj.GetChild(5).GetComponent<TextMeshProUGUI>();
        img_ExpBar = mainObj.GetChild(7).GetComponent<Image>();
        

        txt_Level.text = "LV. "+itemCls.GetLevel().ToString();
        EquipmentKindDivider(itemCls, null, txt_StatLabel, txt_StatValue);
        txt_ExpValue.text = itemCls.GetCurrentExp().ToString() + "/" + itemCls.GetMaxExp().ToString();
        ExpBarAlphaRevisePerpect(img_ExpBar, itemCls);
    }
    // 재료 선택용 스크롤뷰 오브젝트 객체 클릭
    void ClickMaterialItemForEquipUpgrade(InvenItemObjClass cls, SelectButtonScript selectBtnCls, List<InvenItemObjClass> listObj = null)
    {
        ItemClass tmp = cls.GetItemcls();
        WeaponAndEquipCls selecWeapon = tmp as WeaponAndEquipCls;

        // 처음 선택하는 객체일 경우,
        if (dic_ItemClsForUpgrade.ContainsKey(tmp) == false)
        {

            var equipItem = GameManager.Instance.GetUserClass().GetUserEquippedEquipment()[nSelected_EquipIndex] as WeaponAndEquipCls;
            int needExp = 0;
            if (equipItem != null)
                needExp = equipItem.GetMaxExp() - equipItem.GetCurrentExp();

            if (nWeaponUpgradeExp >= needExp)   // 현재 누적 경험치가 요구 경험치를 초과 시, 리턴
            {
                PopUpScreenFunc(popUpForUpgrade, "최대 경험치에 도달했습니다!");
                return;
            }

            if (selectBtnCls.GetIsActive() == false)
            {

                // 리스트에 데이터 추가 및 exp 증가
                dic_ItemClsForUpgrade.Add(tmp, selectBtnCls);
                nWeaponUpgradeExp += selecWeapon.GetGrade() * 50;
                nWeaponUpgradeExp += selecWeapon.GetCurrentExp();
                cls.SetSelectBtnSpriteOn(true); // 선택했음을 UI로 표현
                NeededCostForUpgrade(nWeaponUpgradeExp, needExp, ref nWeaponUpgradeCost, txt_NeedMora, txt_CurrentHaveMora, img_ExpBar);

                // 버튼 객체의 UI 작업
                selectBtnCls.SetItemCls(tmp);
                selectBtnCls.SetIsActive(true);
                selectBtnCls.GetItemImage().enabled = true;
                EquipmentKindDivider(selecWeapon, selectBtnCls.GetItemImage());
                selectBtnCls.SetItemText("LV. " + selecWeapon.GetLevel().ToString());
                ItemUISetterByItemGrade(tmp, selectBtnCls.GetTopBgr(), null);
            }
            else
            {
                // 오브젝트 풀의 리스트를 순회하며 빈 객체에 데이터 추가.
                var list = GameManager.Instance.SelectButtonScriptPool.GetPoolList();
                foreach (var data in list)
                {
                    if (data.GetIsActive()) // 이미 데이터를 가지고 있는 버튼의 경우, 넘어간다
                        continue;

                    // 선택한 데이터 추가
                    dic_ItemClsForUpgrade.Add(tmp, data);
                    nWeaponUpgradeExp += selecWeapon.GetGrade() * 50;
                    nWeaponUpgradeExp += selecWeapon.GetCurrentExp();
                    cls.SetSelectBtnSpriteOn(true);
                    NeededCostForUpgrade(nWeaponUpgradeExp, needExp, ref nWeaponUpgradeCost, txt_NeedMora, txt_CurrentHaveMora, img_ExpBar);

                    // 버튼 객체의 UI 작업
                    data.SetItemCls(tmp);
                    data.SetIsActive(true);
                    data.GetItemImage().enabled = true;
                    EquipmentKindDivider(selecWeapon, data.GetItemImage());
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
                nWeaponUpgradeExp -= existCls.GetGrade() * 50;
                nWeaponUpgradeExp -= existCls.GetCurrentExp();
                dic_ItemClsForUpgrade.Remove(selectBtnCls.GetItemClass());

                SelectButtonToDefault(selectBtnCls);

                cls.SetSelectBtnSpriteOn(false);
            }
            else
            {
                List<InvenItemObjClass> datas;
                if (listObj != null)
                    datas = listObj;
                else
                    datas = GameManager.Instance.EquipItemPool.GetPoolList();
                foreach (var item in datas)
                {
                    if (item.GetItemcls() == cls.GetItemcls())
                    {
                        WeaponAndEquipCls existCls = item.GetItemcls() as WeaponAndEquipCls;
                        nWeaponUpgradeExp -= existCls.GetGrade() * 50;
                        nWeaponUpgradeExp -= existCls.GetCurrentExp();
                        var btnObj = dic_ItemClsForUpgrade[item.GetItemcls()];
                        SelectButtonToDefault(btnObj);
                        dic_ItemClsForUpgrade.Remove(item.GetItemcls());


                        item.SetSelectBtnSpriteOn(false);
                        break;
                    }
                }
            }

            if (dic_ItemClsForUpgrade.Count <= 0)
            {
                WeaponAndEquipCls originItem = GameManager.Instance.GetUserClass().GetUserEquippedWeapon() as WeaponAndEquipCls;
                ExpBarAlphaRevisePerpect(img_ExpBar, originItem);
            }
        }

        //Debug.Log(nameof(nWeaponUpgradeExp) + ":" + nWeaponUpgradeExp);
        //Debug.Log(nameof(dic_ItemClsForUpgrade.Count) + ":" + dic_ItemClsForUpgrade.Count);
    }
    // 무기 강화 - 사용 버튼 이벤트 리스너
    void EquipLevelUpClickEventListener(Transform mainObj)
    {
        int mora = GameManager.Instance.GetUserClass().GetMora();
        if (mora >= nWeaponUpgradeCost)
        {
            var equipCls = GameManager.Instance.GetUserClass().GetUserEquippedEquipment()[nSelected_EquipIndex] as WeaponAndEquipCls;
            // 돌파 요구 단계에 도달 시, true를 반환 받는다.
            EquipmentExpUp_Upgrade(equipCls, nWeaponUpgradeExp);   // 무기 경험치 업그레이드 클래스 호출하기

            // 유저 보유 모라 가져오기
            int haveMora = GameManager.Instance.GetUserClass().GetMora();
            haveMora -= nWeaponUpgradeCost;
            // 모라 최신화
            GameManager.Instance.GetUserClass().SetMora(haveMora);

            // 누적 경험치 초기화
            nWeaponUpgradeExp = 0;

            // 무기 데이터 삭제
            foreach (var tmp in dic_ItemClsForUpgrade)
            {
                ItemClass item = tmp.Key;
                GameManager.Instance.GetUserClass().GetHadEquipmentList().Remove(item);
                SelectButtonToDefault(tmp.Value);
            }

            // 선택버튼에서 무기 UI 삭제
            dic_ItemClsForUpgrade.Clear();

            EquipStatusPrint(mainObj, GameManager.Instance.GetUserClass().GetUserEquippedEquipment()[nSelected_EquipIndex] as WeaponAndEquipCls);
            NeededCostForUpgrade(nWeaponUpgradeExp, 0, ref nWeaponUpgradeCost, txt_NeedMora, txt_CurrentHaveMora);
            ExpBarAlphaRevisePerpect(img_ExpBar, equipCls);
        }
    }
    #endregion

    #endregion


    #region 운명의 별자리 UI 관리


    #endregion


    #region 특성 UI 관리

    #endregion

    #endregion

    #region 기타 및 툴

    private void ResetToSelectButtonScriptPoolDatas()
    {
        // 오브젝트 풀로 가져온 각 버튼에 Button이벤트를 Remove함
        var objList = GameManager.Instance.SelectButtonScriptPool.GetPoolList();
        foreach (var obj in objList)
        {
            //if (obj.gameObject.activeSelf == false) continue;
            SelectButtonToDefault(obj);
            Button btn = obj.GetButton();
            if (btn != null)
            {
                // 모든 이벤트 리스너를 제거하고, 기존의 이벤트 리스너를 다시 부착한다.
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => obj.OnClickEventListener());
            }
        }
        //GameManager.Instance.WeaponItemPool.AllReturnToPool();  // 웨폰 UI 오브젝트풀 리턴
    }
    private void ScrollViewOpenButtonClickEvent(SelectButtonScript clickedBtn, Transform mainobj, e_PoolItemType type)
    {
        mainobj.gameObject.SetActive(true);
        ResetToWeaponItemObjectPoolDatas(type);
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

    // 정렬_오름차/내림차순 버튼 이벤트 리스너
    private void AscendingValueReversalFunction(ref bool isAscending, GameManager.e_PoolItemType poolType)
    {
        isAscending = !isAscending;

        ResetToWeaponItemObjectPoolDatas(poolType);
    }


    // 씬 전환 간 컨트롤러 함수 연결 초기화
    public void UI_Manager_ControllerSet()
    {
        var parents = GameObject.FindGameObjectWithTag("Controller").transform;
        if (parents != null)
        {
            ButtonClass bagOpenBtn = parents.GetChild(7).GetComponent<ButtonClass>();
            var bagOpenBtnObj = bagOpenBtn.GetButton();
            bagOpenBtnObj.onClick.RemoveAllListeners();
            ButtonClass_Reset(bagOpenBtn);
            bagOpenBtnObj.onClick.AddListener(() => BagOpenButtonClick());

            ButtonClass infoOpenBtn = parents.GetChild(8).GetComponent<ButtonClass>();
            var infoOpenBtnObj = infoOpenBtn.GetButton();
            infoOpenBtnObj.onClick.RemoveAllListeners();
            ButtonClass_Reset(infoOpenBtn);
            infoOpenBtnObj.onClick.AddListener(() => OpenPlayerInfoScreenButton());
        }
    }


    public void WorldMapOpenButtonClick()
    {
        wolrdMap_manager.WorldMapOpenClick();
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


    #region 게터세터
    public e_InventoryTypeSelected GetnSelectedInvenIdx(){return invenType_Index; }
    
    public WorldMap_Manager GetWorldMap_Manager{ get { return wolrdMap_manager; } }

    #endregion

    #endregion




    #region 오브젝트 제어 클래스

    public class Synthesis 
    {
        GameObject SynthesisObjectScreen = instance.SynthesisObjectScreen;
        InventoryButton[] SynthesisTypeButtons = instance.SynthesisTypeButtons;
        private e_InventoryTypeSelected synthesisType_Index;                        // 인벤토리 타입 인덱스
        private Transform sythesisPrintScroll;                                      // 아이템 출력용 스크롤뷰 콘텐트
        private TextMeshProUGUI moraText;                                           // 모라 텍스트
        private ItemClass selectedItem_sythesis;                                             // 선택된 아이템 객체
        private SYNTHESIS_DATA_BASE selected_synthesisData;                                  // 선택된 아이템의 DB 데이터
        // ETC
        SliderController etc_fillingBar;
        private int etc_currentNum;
        private int etc_maxNum;
        TextMeshProUGUI etc_curNumText;
        TextMeshProUGUI etc_maxNumText;
        TextMeshProUGUI etc_resourceText_1;
        TextMeshProUGUI etc_resourceText_2;
        // Equipment and Weapons
        TextMeshProUGUI equips_contentText;
        TextMeshProUGUI equips_needGemText;
        int equips_currentNum;
        int equips_maxNum;

        public void SynthesisObjectFunc_UI_Print()
        {
            GameManager.Instance.PauseManager();

            // 변수 초기화
            instance.dic_ItemClsForUpgrade = new Dictionary<ItemClass, SelectButtonScript>();
            instance.openUI_ItemList = new List<InvenItemObjClass>();

            // 취소버튼 함수 연결
            ButtonClass closeButtn = SynthesisObjectScreen.transform.GetChild(3).GetComponent<ButtonClass>();
            closeButtn.GetButton().onClick.RemoveAllListeners();
            closeButtn.GetButton().onClick.AddListener(SynthesisObjectFunc_UI_PrintOff);

            // 사용버튼 함수 연결
            ButtonClass2 userButton = SynthesisObjectScreen.transform.GetChild(4).GetComponent<ButtonClass2>();
            userButton.GetButton().onClick.RemoveAllListeners();
            ButtonClass2_Reset(userButton);
            userButton.GetButton().onClick.AddListener(UseButtonFunc);

            // 데이터 세팅
            instance.SynthesisObjectScreen.gameObject.SetActive(true);
            synthesisType_Index = e_InventoryTypeSelected.Weapon;
            sythesisPrintScroll = SynthesisObjectScreen.transform.GetChild(5).GetChild(0).GetChild(0);
            selectedItem_sythesis = null;
            selected_synthesisData = default;
            // 모라 텍스트 인스턴스화 및 데이터 출력
            moraText = SynthesisObjectScreen.transform.GetChild(6).GetComponentInChildren<TextMeshProUGUI>();
            moraText.text = GameManager.Instance.GetUserClass().GetMora().ToString();
            // 기타 합성 필요 변수 인스턴스화
            etc_fillingBar = SynthesisObjectScreen.transform.GetChild(7).GetChild(1).GetChild(1).GetComponent<SliderController>();
            etc_currentNum = 0;
            etc_maxNum = 0;
            etc_curNumText = SynthesisObjectScreen.transform.GetChild(7).GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>();  // 현재 텍스트
            etc_maxNumText = SynthesisObjectScreen.transform.GetChild(7).GetChild(1).GetChild(4).GetComponent<TextMeshProUGUI>();  // 최대치 텍스트
            // 장비 합성 필요 변수 인스턴스화
            equips_contentText = SynthesisObjectScreen.transform.GetChild(8).GetChild(0).GetChild(3).GetChild(3).GetComponent<TextMeshProUGUI>();
            equips_needGemText = SynthesisObjectScreen.transform.GetChild(8).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();

            foreach (var tmp in SynthesisTypeButtons)
            {
                tmp.GetButton().onClick.RemoveAllListeners();
                tmp.GetButton().onClick.AddListener(() => tmp.ButtonUIColorSet());
                tmp.GetButton().onClick.AddListener(() => SynthesisIndexChange(tmp));
            }

            SynthesisTypeButtons[0].SetClickActive(false);
            SynthesisTypeButtons[0].ButtonUIColorSet();
            SynthesisPrintDataDevider();
        }
        public void SynthesisObjectFunc_UI_PrintOff()
        {
            GameManager.Instance.PauseManager();
            ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Weapon);
            ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Equip);
            ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Gem);
            ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Food);
            SynthesisObjectScreen.gameObject.SetActive(false);

            // 아이템 타입 버튼 객체 UI OFF
            foreach (var tmp in SynthesisTypeButtons)
            {
                if (tmp.GetClickActive() == true)
                    tmp.ButtonUIColorSet();
            }
        }

        private void SynthesisIndexChange(InventoryButton btn)
        {
            // 선택된 버튼을 제외한 나머지 객체들의 UI 수정
            foreach (var tmp in SynthesisTypeButtons)
            {
                if (tmp == btn)
                    continue;
                if (tmp.GetClickActive() == true)
                    tmp.ButtonUIColorSet();
            }

            selectedItem_sythesis = null;
            selected_synthesisData = default;

            // 인덱스 수정
            synthesisType_Index = btn.GetSelectType();

            // 데이터 출력
            SynthesisPrintDataDevider();
        }

        void SynthesisPrintDataDevider()
        {
            ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Weapon);
            ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Equip);
            ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Gem);
            ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Food);
            etc_currentNum = 0;
            equips_currentNum = 0;
            switch (synthesisType_Index)
            {
                case e_InventoryTypeSelected.Weapon:
                    SynthesisObjectScreen.transform.GetChild(8).gameObject.SetActive(true);
                    SynthesisObjectScreen.transform.GetChild(7).gameObject.SetActive(false);
                    PrintSynthesisWeapon();
                    SynthesisPrintEquipsObjects();
                    break;
                case e_InventoryTypeSelected.Equipment:
                    SynthesisObjectScreen.transform.GetChild(8).gameObject.SetActive(true);
                    SynthesisObjectScreen.transform.GetChild(7).gameObject.SetActive(false);
                    PrintSynthesisEquip();
                    SynthesisPrintEquipsObjects();
                    break;
                case e_InventoryTypeSelected.Etc:
                    SynthesisObjectScreen.transform.GetChild(8).gameObject.SetActive(false);
                    SynthesisObjectScreen.transform.GetChild(7).gameObject.SetActive(true);
                    PrintSynthesisEtc();
                    SynthesisPrintEtcObjects();
                    break;
            }
        }

        // 무기 프린트 출력
        void PrintSynthesisWeapon()
        {

            var datas = GameManager.Instance.GetWeaponSynthesisData();
            GameManager.Instance.ItemToObjPool(datas.Count, e_PoolItemType.Weapon, sythesisPrintScroll);
            var poolUIs = GameManager.Instance.WeaponItemPool.GetPoolList();

            HaveItemCompareToDataBase_ForSynthesis(datas, e_PoolItemType.Weapon, poolUIs);
        }
        void PrintSynthesisEquip()
        {
            var datas = GameManager.Instance.GetEquipSynthesisData();
            GameManager.Instance.ItemToObjPool(datas.Count, e_PoolItemType.Equip, sythesisPrintScroll);
            var poolUIs = GameManager.Instance.EquipItemPool.GetPoolList();

            HaveItemCompareToDataBase_ForSynthesis(datas, e_PoolItemType.Equip, poolUIs);
        }
        void PrintSynthesisEtc()
        {
            var datas = GameManager.Instance.GetEtcSynthesisData();
            GameManager.Instance.ItemToObjPool(datas.Count, e_PoolItemType.Gem, sythesisPrintScroll);
            var poolUIs = GameManager.Instance.GemItemPool.GetPoolList();

            HaveItemCompareToDataBase_ForSynthesis(datas, e_PoolItemType.Gem, poolUIs);
        }

        void HaveItemCompareToDataBase_ForSynthesis(List<SYNTHESIS_DATA_BASE> dbList, e_PoolItemType dataType, List<InvenItemObjClass> poolDatas)
        {
            List<ItemClass> haveItems_1 = new List<ItemClass>();
            List<ItemClass> haveItems_2 = new List<ItemClass>();
            if (dataType == e_PoolItemType.Weapon)
            {
                haveItems_1 = GameManager.Instance.GetUserClass().GetHadWeaponList();   // 무기
                haveItems_2 = GameManager.Instance.GetUserClass().GetHadGemList();      // 광석
            }
            if (dataType == e_PoolItemType.Equip)
            {
                haveItems_1 = GameManager.Instance.GetUserClass().GetHadEquipmentList();    // 성유물
                haveItems_2 = GameManager.Instance.GetUserClass().GetHadGemList();          // 광석
            }
            if (dataType == e_PoolItemType.Gem)
            {
                haveItems_1 = GameManager.Instance.GetUserClass().GetHadGrowMaterialList(); // 성장재료
            }
            // DB 순회하며, 데이터 탐색 및 UI 수정
            for (int i = 0; i < dbList.Count; i++)
            {
                string material_1 = dbList[i].MATERIAL_1;
                string material_2 = dbList[i].MATERIAL_2;
                int material_1_num = dbList[i].MATERIAL_1_NUM;
                int material_2_num = dbList[i].MATERIAL_2_NUM;
                string completeItem = dbList[i].COMPLETE_iTEM;

                bool isPossible = true;

                if (material_1 != "")
                {
                    if (dataType == e_PoolItemType.Weapon || dataType == e_PoolItemType.Equip)
                    {
                        var finds_1 = haveItems_1.Where(item => item.GetName().Equals(material_1)).ToList();

                        if (finds_1 == null)
                            isPossible = false;

                        if (finds_1 != null && finds_1.Count < material_1_num)
                            isPossible = false;
                    }
                    else if (dataType == e_PoolItemType.Gem)
                    {
                        var find_1 = haveItems_1.Find(item => item.GetName().Equals(material_1));

                        if (find_1 == null)
                            isPossible = false;

                        if (find_1 != null && find_1.GetNumber() < material_1_num)
                            isPossible = false;
                    }
                }
                if (material_2 != "")
                {
                    if (material_2 == "모라")
                    {
                        // 모라 확인
                        if (GameManager.Instance.GetUserClass().GetMora() < material_2_num)
                            isPossible = false;
                    }
                    else
                    {
                        var find_2 = haveItems_2.Find(tmp => tmp.GetName().Equals(material_2));

                        if (find_2 == null)
                            isPossible = false;

                        if (find_2 != null && find_2.GetNumber() < material_2_num)
                            isPossible = false;
                    }
                }

                // 플레이어가 보유한 데이터가 조건을 모두 만족할 경우
                if (isPossible == true)
                {
                    if (dataType == e_PoolItemType.Weapon)
                    {
                        var defaultDatas = GameManager.Instance.GetWeaponAndEquipmentDataList();
                        var target = defaultDatas.Find(tmp => tmp.GetName().Equals(completeItem));

                        WeaponKindDivider(target, poolDatas[i].GetTopItemImage());
                        poolDatas[i].SetItemColor(target.GetGrade());
                        poolDatas[i].SetItemcls(target);

                        // 버튼 세팅
                        var btn = poolDatas[i].GetButton();
                        int index = i; // i 값을 별도의 변수에 할당
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() => poolDatas[index].ClickedUIApply());
                        btn.onClick.AddListener(() => SynthesisButtonSelect(target, dataType, dbList[index]));
                    }
                    if (dataType == e_PoolItemType.Equip)
                    {
                        var defaultDatas = GameManager.Instance.GetWeaponAndEquipmentDataList();
                        var target = defaultDatas.Find(tmp => tmp.GetName().Equals(completeItem));

                        EquipmentKindDivider(target, poolDatas[i].GetTopItemImage());
                        poolDatas[i].SetItemColor(target.GetGrade());
                        poolDatas[i].SetItemcls(target);


                        // 버튼 세팅
                        var btn = poolDatas[i].GetButton();
                        int index = i; // i 값을 별도의 변수에 할당
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() => poolDatas[index].ClickedUIApply());
                        btn.onClick.AddListener(() => SynthesisButtonSelect(target, dataType, dbList[index]));
                    }
                    if (dataType == e_PoolItemType.Gem)
                    {
                        var defaultDatas = GameManager.Instance.GetItemDataList();
                        var target = defaultDatas.Find(tmp => tmp.GetName().Equals(completeItem));

                        poolDatas[i].SetItemSprite(WeaponAndEquipLimitBreak_UI_Dvider(target));
                        poolDatas[i].SetItemColor(target.GetGrade());
                        poolDatas[i].SetItemcls(target);


                        // 버튼 세팅
                        var btn = poolDatas[i].GetButton();
                        int index = i; // i 값을 별도의 변수에 할당
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() => poolDatas[index].ClickedUIApply());
                        btn.onClick.AddListener(() => SynthesisButtonSelect(target, dataType, dbList[index]));
                    }
                    poolDatas[i].SetIsActive(true);
                    poolDatas[i].SetItemText(completeItem);
                }
                // 만족하지 못 할 경우
                else
                {
                    if (dataType == e_PoolItemType.Weapon)
                    {
                        var defaultDatas = GameManager.Instance.GetWeaponAndEquipmentDataList();
                        var target = defaultDatas.Find(tmp => tmp.GetName().Equals(completeItem));

                        WeaponKindDivider(target, poolDatas[i].GetTopItemImage());

                        // 버튼 세팅
                        var btn = poolDatas[i].GetButton();
                        int index = i; // i 값을 별도의 변수에 할당
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() => poolDatas[index].ClickedUIApply());
                        btn.onClick.AddListener(() => SynthesisButtonSelect_DontHaveItem(target));

                    }
                    if (dataType == e_PoolItemType.Equip)
                    {
                        var defaultDatas = GameManager.Instance.GetWeaponAndEquipmentDataList();
                        var target = defaultDatas.Find(tmp => tmp.GetName().Equals(completeItem));

                        EquipmentKindDivider(target, poolDatas[i].GetTopItemImage());

                        // 버튼 세팅
                        var btn = poolDatas[i].GetButton();
                        int index = i; // i 값을 별도의 변수에 할당
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() => poolDatas[index].ClickedUIApply());
                        btn.onClick.AddListener(() => SynthesisButtonSelect_DontHaveItem(target));
                    }
                    if (dataType == e_PoolItemType.Gem)
                    {
                        var defaultDatas = GameManager.Instance.GetItemDataList();
                        var target = defaultDatas.Find(tmp => tmp.GetName().Equals(completeItem));
                        poolDatas[i].SetItemSprite(WeaponAndEquipLimitBreak_UI_Dvider(target));

                        // 버튼 세팅
                        var btn = poolDatas[i].GetButton();
                        int index = i; // i 값을 별도의 변수에 할당
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() => poolDatas[index].ClickedUIApply());
                        btn.onClick.AddListener(() => SynthesisButtonSelect_DontHaveItem(target));
                    }

                    poolDatas[i].SetItemColor(0);
                    poolDatas[i].SetIsActive(false);
                    poolDatas[i].SetItemText(completeItem);
                    Color aa = poolDatas[i].GetTopItemImage().color;
                    aa.a = 0.6f;
                    poolDatas[i].GetTopItemImage().color = aa;  // 알파값 수정

                    // 스크롤뷰 상에서 뒤쪽으로 보내기
                    poolDatas[i].transform.SetParent(sythesisPrintScroll);
                    poolDatas[i].transform.SetAsLastSibling();

                }
            }
        }

        // 1번 : 하위 아이템, 2번 모라
        void SynthesisPrintEtcObjects()
        {
            Transform parents = SynthesisObjectScreen.transform.GetChild(7).transform;
            Transform[] resourceSet_img = new Transform[3]; // 재료 이미지 출력
            for (int i = 0; i < 3; i++)
                resourceSet_img[i] = parents.GetChild(0).GetChild(0).GetChild(1 + i).GetComponent<Transform>();
            Transform makeTargetPos = parents.GetChild(1).GetChild(2).GetComponent<Transform>();        // 합성할 객체 출력 위치
            ButtonClass plusBtn = parents.GetChild(1).GetChild(5).GetComponent<ButtonClass>();        // 생성 개수 추가 버튼
            ButtonClass minusBtn = parents.GetChild(1).GetChild(6).GetComponent<ButtonClass>();       // 생성 개수 감소 버튼

            instance.ResetToSelectButtonScriptPoolDatas();
            // 선택한 버튼 객체가 있을 경우,
            if (selectedItem_sythesis == null)
            {
                etc_fillingBar.SetFillProgress(0);
                plusBtn.GetButton().onClick.RemoveAllListeners();
                minusBtn.GetButton().onClick.RemoveAllListeners();
                ButtonClass_Reset(plusBtn);
                ButtonClass_Reset(minusBtn);
                etc_curNumText.text = "";
                etc_maxNumText.text = "";
                GameManager.Instance.SelectButtonScriptPool.AllReturnToPool();
            }
            else
            {
                // 합성할 객체의 재료 찾기
                int needNum_1 = selected_synthesisData.MATERIAL_1_NUM;
                int haveNum_1 = -1;
                var haveItem = GameManager.Instance.GetUserClass().GetHadGrowMaterialList().Find(tmp => tmp.GetName().Equals(selected_synthesisData.MATERIAL_1));
                if (haveItem != null)
                    haveNum_1 = haveItem.GetNumber();

                int needNum_2 = selected_synthesisData.MATERIAL_2_NUM;
                int haveNum_2 = 0;
                if (selected_synthesisData.MATERIAL_2.Equals("모라"))
                    haveNum_2 = GameManager.Instance.GetUserClass().GetMora();

                // 최대 생성 가능 횟수 계산
                etc_maxNum = Math.Min((haveNum_1 / needNum_1), (haveNum_2 / needNum_2));


                etc_curNumText.text = etc_currentNum.ToString();
                etc_maxNumText.text = etc_maxNum.ToString();
                // 버튼 초기화 후 이벤트 리스너 연결
                plusBtn.GetButton().onClick.RemoveAllListeners();
                minusBtn.GetButton().onClick.RemoveAllListeners();
                ButtonClass_Reset(plusBtn);
                ButtonClass_Reset(minusBtn);
                plusBtn.GetButton().onClick.AddListener(() => SynthesisEtc_PlusButtonEvent());
                minusBtn.GetButton().onClick.AddListener(() => SynthesisEtc_MinusButtonEvent());

                // 슬라이더 함수 연결
                etc_fillingBar.GetSlider().onValueChanged.RemoveAllListeners();
                etc_fillingBar.GetSlider().onValueChanged.AddListener(SynthesisSlideValueGetNotify);

                // 합성할 객체를 출력
                var targetItemUI = GameManager.Instance.SelectButtonScriptPool.GetFromPool(Vector3.zero, Quaternion.identity, makeTargetPos);
                targetItemUI.transform.SetParent(makeTargetPos); 
                targetItemUI.transform.localPosition = Vector3.zero; 

                targetItemUI.SetItemColor(selectedItem_sythesis.GetGrade());
                targetItemUI.SetItemSprite(WeaponAndEquipLimitBreak_UI_Dvider(selectedItem_sythesis));
                targetItemUI.SetItemText(selectedItem_sythesis.GetName());
                targetItemUI.GetItemImage().enabled = true;
                targetItemUI.SetIsActive(true);

                // 합성할 객체의 재료 출력
                if(selected_synthesisData.MATERIAL_2.Equals("모라"))
                {
                    // 1번 재료
                    var resource_1 = GameManager.Instance.SelectButtonScriptPool.GetFromPool(Vector2.zero, Quaternion.identity, resourceSet_img[0]);
                    ResetToSelectButton(resource_1);
                    
                    resource_1.SetIsActive(true);
                    resource_1.transform.SetParent(resourceSet_img[0]);
                    resource_1.transform.localPosition = Vector3.zero;

                    resource_1.SetItemColor(haveItem.GetGrade());
                    resource_1.SetItemSprite(WeaponAndEquipLimitBreak_UI_Dvider(haveItem));
                    resource_1.SetItemText(needNum_1.ToString());
                    resource_1.GetItemImage().enabled = true;
                    etc_resourceText_1 = resource_1.GetItemTxt();
                    etc_resourceText_1.text = "0";

                    // 재료 파밍 위치 구하기
                    var data = GameManager.Instance.GetItemDataList().Find(tmp => tmp.GetName().Equals(selected_synthesisData.MATERIAL_1));
                    resource_1.GetButton().onClick.AddListener(() => NavigateResourceClickEvent(data));

                    // 2번 재료 - 모라
                    var resource_2 = GameManager.Instance.SelectButtonScriptPool.GetFromPool(Vector2.zero, Quaternion.identity, resourceSet_img[1]);
                    ResetToSelectButton(resource_2);
                    
                    resource_2.SetIsActive(true);
                    resource_2.transform.SetParent(resourceSet_img[1]);
                    resource_2.transform.localPosition = Vector3.zero;

                    resource_2.SetItemColor(3);
                    resource_2.SetItemSprite(ItemSpritesSaver.Instance.SpritesSet[3]);
                    resource_2.SetItemText(needNum_2.ToString());
                    resource_2.GetItemImage().enabled = true;
                    etc_resourceText_2 = resource_2.GetItemTxt();
                    etc_resourceText_2.text = "0";

                }
                else
                {

                }
            }
        }
        // 재료 파밍위치 안내 함수
        void NavigateResourceClickEvent(ItemClass item)
        {
            instance.getResourcePrintUI.Scroll_Start(item);
        }

        // 슬라이드 바, 값 Notifyed Getter
        void SynthesisSlideValueGetNotify(float value)
        {

            float closestValue = float.MaxValue; // 초기값 설정
            int closestNum = 0;

            for (int i = 0; i <= etc_maxNum; i++)
            {
                float currentValue = (float)i / etc_maxNum;
                float diff = Mathf.Abs(currentValue - value);

                if (diff < closestValue)
                {
                    closestValue = diff;
                    closestNum = i;
                }
            }

            float finalValue = (float)closestNum / etc_maxNum;

            etc_fillingBar.SetFillProgress(finalValue);
            etc_currentNum = closestNum;
            etc_curNumText.text = etc_currentNum.ToString();

            etc_resourceText_1.text = (selected_synthesisData.MATERIAL_1_NUM * etc_currentNum).ToString();
            etc_resourceText_2.text = (selected_synthesisData.MATERIAL_2_NUM * etc_currentNum).ToString();
        }
        // 기타 합성 _ 합성 횟수 증가 버튼이벤트
        void SynthesisEtc_PlusButtonEvent()
        {
            if (etc_currentNum + 1 > etc_maxNum) return;
            etc_currentNum += 1;
            float divideValue = (float)etc_currentNum / (float)etc_maxNum;
            etc_fillingBar.SetFillProgress(divideValue);

            etc_curNumText.text = etc_currentNum.ToString();
            SynthesisPrintEtcObjects();

            etc_resourceText_1.text = (selected_synthesisData.MATERIAL_1_NUM * etc_currentNum).ToString();
            etc_resourceText_2.text = (selected_synthesisData.MATERIAL_2_NUM * etc_currentNum).ToString();
        }
        // 기타 합성 _ 합성 횟수 감소 버튼이벤트
        void SynthesisEtc_MinusButtonEvent()
        {
            if (etc_currentNum - 1 < 0) return;
            etc_currentNum -= 1;
            float divideValue = (float)etc_currentNum / (float)etc_maxNum;
            etc_fillingBar.SetFillProgress(divideValue);

            etc_curNumText.text = etc_currentNum.ToString();
            SynthesisPrintEtcObjects();


            etc_resourceText_1.text = (selected_synthesisData.MATERIAL_1_NUM * etc_currentNum).ToString();
            etc_resourceText_2.text = (selected_synthesisData.MATERIAL_2_NUM * etc_currentNum).ToString();
        }

        // 장비 합성 출력
        void SynthesisPrintEquipsObjects()
        {
            // 무기 누적 경험치 -값 할당 => 기존의 함수를 이용하기에, 경험치 누적을 무시하기 위함.
            instance.nWeaponUpgradeExp = -987654321;            

            Transform parents = SynthesisObjectScreen.transform.GetChild(8).transform;
            Transform scrollTransform = parents.GetChild(0).GetChild(0).GetChild(0).GetComponent<Transform>();
            Transform targetPos = parents.GetChild(1).GetChild(1).GetComponent<Transform>();
            TextMeshProUGUI targetNumeText = parents.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI synthesisContentText = parents.GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>();
            Image img_needGem = parents.GetChild(2).GetChild(0).GetComponent<Image>();
            Transform materialsPrintObj = parents.GetChild(3).GetComponent<Transform>();


            // 선택 버튼 오브젝트풀 초기화
            instance.ResetToSelectButtonScriptPoolDatas();
            GameManager.Instance.SelectButtonScriptPool.AllReturnToPool();


            // 재료 선택 스크롤뷰 출력용 오브젝트 초기화
            ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Food);
            materialsPrintObj.gameObject.SetActive(false);  // 재료 출력용 오브젝트는 기본 SetFalse
            
            // 선택한 버튼 객체가 있을 경우,
            if (selectedItem_sythesis == null)
            {
                img_needGem.enabled = false;
                equips_needGemText.text = "";
                equips_contentText.text = "0/3";
                synthesisContentText.text = "장비 합성을 통해서 N개의 장비 제조";
                targetNumeText.text = "합성";
                // 스크롤뷰에 선택 버튼 객체 세팅
                GameManager.Instance.ItemToObjPool_SelectButton(15, scrollTransform);
                SelectButtonToDefault(GameManager.Instance.SelectButtonScriptPool.GetPoolList());
            }
            else
            {
                // 스크롤뷰에 선택 버튼 객체 세팅
                GameManager.Instance.ItemToObjPool_SelectButton(15, scrollTransform);

                // 목표 객체 출력 및 데이터 초기화
                var forMakeEquip = GameManager.Instance.GetWeaponAndEquipmentDataList().Find(tmp => tmp.GetName().Equals(selected_synthesisData.COMPLETE_iTEM));
                var targetItemUI = GameManager.Instance.SelectButtonScriptPool.GetFromPool(Vector2.zero, Quaternion.identity, targetPos);
                targetItemUI.transform.SetParent(targetPos);
                List<SelectButtonScript> selectBtnPool_list = GameManager.Instance.SelectButtonScriptPool.GetPoolList();
                SelectButtonToDefault(selectBtnPool_list);
                
                targetItemUI.transform.localPosition = Vector3.zero;
                targetItemUI.SetItemText(forMakeEquip.GetName());
                targetItemUI.SetItemColor(forMakeEquip.GetGrade());
                targetItemUI.GetItemImage().enabled = true;
                targetItemUI.SetIsActive(true);

                // 필요 광물 이미지 세팅
                if (selected_synthesisData.MATERIAL_2 == "철광석")
                    img_needGem.sprite = ItemSpritesSaver.Instance.GemSprites[0];
                else if (selected_synthesisData.MATERIAL_2 == "백철")
                    img_needGem.sprite = ItemSpritesSaver.Instance.GemSprites[1];
                else if(selected_synthesisData.MATERIAL_2 == "수정덩이")
                    img_needGem.sprite = ItemSpritesSaver.Instance.GemSprites[2];
                img_needGem.enabled = true;
                // 필요 광물 수치 표시
                equips_needGemText.text = (equips_currentNum * selected_synthesisData.MATERIAL_2_NUM).ToString();


                // 현재 선택된 재료 개수와 필요한 재료 객체 개수 출력
                equips_contentText.text = equips_currentNum.ToString() + "/" + CalculateMaxValue(equips_currentNum).ToString();

                // 생성할 장비의 개수 텍스트 출력
                synthesisContentText.text = "장비 합성을 통해서 " + (equips_currentNum/3).ToString() + "개의 장비 제조";

                // 선택한 객체가 무기일 때,
                if (selectedItem_sythesis.GetTag() == "무기")
                {
                    WeaponKindDivider(forMakeEquip, targetItemUI.GetItemImage());   // 목표 객체 이미지 세팅
                    targetNumeText.text = "무기 합성 : " + forMakeEquip.GetName();

                    foreach(var tmp in selectBtnPool_list)
                    {
                        if (tmp == targetItemUI)
                            continue;
                        Button btn = tmp.GetButton();
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(tmp.OnClickEventListener);
                        btn.onClick.AddListener(() => instance.ScrollViewOpenButtonClickEvent(tmp, materialsPrintObj, e_PoolItemType.Equip));
                        btn.onClick.AddListener(() => SynthesisSelectButtonClick_ForMaterial(e_PoolItemType.Weapon, tmp));
                    }
                }
                else    // 무기가 아닐때,
                {
                    EquipmentKindDivider(forMakeEquip,targetItemUI.GetItemImage()); // 목표 객체 이미지 세팅
                    targetNumeText.text = "성유물 합성 : " + forMakeEquip.GetName();

                    foreach (var tmp in selectBtnPool_list)
                    {
                        if (tmp == targetItemUI)
                            continue;
                        Button btn = tmp.GetButton();
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() => tmp.OnClickEventListener());
                        btn.onClick.AddListener(() => instance.ScrollViewOpenButtonClickEvent(tmp, materialsPrintObj, e_PoolItemType.Weapon));
                        btn.onClick.AddListener(() => SynthesisSelectButtonClick_ForMaterial(e_PoolItemType.Equip, tmp));
                    }
                }
            }
        }

        int CalculateMaxValue(int currentNumber)
        {
            // 현재 수를 3으로 나눈 나머지를 계산
            int remainder = currentNumber % 3;

            // 현재 수가 0인 경우 3을 반환
            if (currentNumber == 0)
                return 3;

            // 현재 수가 3의 배수라면 맥스값은 현재 수 그대로
            if (remainder == 0)
                return currentNumber;

            // 현재 수가 3의 배수가 아니라면, 다음 3의 배수를 계산
            int value = (currentNumber / 3 + 1) * 3;

            return value;
        } 
        // 선택 UI 오브젝트 클릭시, 호출되는 함수.(스크롤뷰 프린트 연결)
        void SynthesisSelectButtonClick_ForMaterial(e_PoolItemType type, SelectButtonScript selectBtn)
        {
            Transform materialsPrintObj = SynthesisObjectScreen.transform.GetChild(8).GetChild(3).GetComponent<Transform>();
            materialsPrintObj.gameObject.SetActive(true);
            Transform scrollContent_transform = materialsPrintObj.GetChild(1).GetChild(0).GetComponent<Transform>();

            if (type == e_PoolItemType.Weapon)
            {
                // 무기 => 성유물 pool로 스크롤뷰 채우기 구현해야함.

                // 무기 출력 UI pool은 이미, 사용 중이므로, 성유물 출력 UI Pool을 사용.
                ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Equip);
                int len = GameManager.Instance.GetUserClass().GetHadWeaponList().Count;                 // 보유한 무기 만큼 객체 생성.
                GameManager.Instance.ItemToObjPool(len, e_PoolItemType.Equip, scrollContent_transform);
                var poolList = GameManager.Instance.EquipItemPool.GetPoolList();                        // Poolling 한 객체를 저장하는 리스트를 인스턴스화.

                // 스크롤뷰 데이터 출력 함수 호출
                instance.WeaponSelectForUpgradeBtnClickEventListener(selectBtn, materialsPrintObj, e_PoolItemType.Weapon, poolList);
                Synthesis_RelatedDataRemoveAtScrollView(e_PoolItemType.Weapon);  // 연관 없는 재료 데이터는 제거
            }
            else if(type == e_PoolItemType.Equip)
            {
                ResetToWeaponItemObjectPoolDatas(e_PoolItemType.Weapon);

                int len = GameManager.Instance.GetUserClass().GetHadEquipmentList().Count;                 // 보유한 무기 만큼 객체 생성.
                GameManager.Instance.ItemToObjPool(len, e_PoolItemType.Weapon, scrollContent_transform);
                var poolList = GameManager.Instance.WeaponItemPool.GetPoolList();                        // Poolling 한 객체를 저장하는 리스트를 인스턴스화.

                // 스크롤뷰 데이터 출력 함수 호출
                instance.WeaponSelectForUpgradeBtnClickEventListener(selectBtn, materialsPrintObj, e_PoolItemType.Equip, poolList);
                Synthesis_RelatedDataRemoveAtScrollView(e_PoolItemType.Equip);  // 연관 없는 재료 데이터는 제거
            }
        }
        // 재료 출력용 스크롤뷰 상에서 연관 없는 데이터를 제거하는 함수
        void Synthesis_RelatedDataRemoveAtScrollView(e_PoolItemType type)
        {
            ObjectPool<InvenItemObjClass> pool_obj_List = null;
            if (type == e_PoolItemType.Weapon)
                pool_obj_List = GameManager.Instance.EquipItemPool;
            else if (type == e_PoolItemType.Equip)
                pool_obj_List = GameManager.Instance.WeaponItemPool;

            // 삭제할 아이템을 저장하는 임시 리스트
            List<InvenItemObjClass> itemsToRemove = new List<InvenItemObjClass>();

            foreach (var tmp in instance.openUI_ItemList)
            {
                if (tmp.GetItemcls() == null || tmp.GetItemcls().GetName() != selected_synthesisData.MATERIAL_1)
                {
                    tmp.SetIsActive(false);
                    itemsToRemove.Add(tmp);
                }
                else    // 버튼 리스너 연결, 기존 함수 Remove는 Obejct Print 단계에서 모두 제거되기에, Remove할 필요 없음.
                    tmp.GetButton().onClick.AddListener(() => Synthesis_Material_UI_Clicked());
            }

            // 반복문 외부에서 아이템 삭제
            foreach (var item in itemsToRemove)
            {
                pool_obj_List.ReturnToPool(item);
                instance.openUI_ItemList.Remove(item);
            }
        }
        // 재료 소모 아이템 클릭 시, 호출 함수
        void Synthesis_Material_UI_Clicked()
        {
            int cnt = instance.dic_ItemClsForUpgrade.Count();
            equips_currentNum = cnt;
            etc_currentNum = cnt/3; // 합성해야하는 장비 갯수로 나누어, 합성 가능한 수치를 변경.

            int minerals = etc_currentNum * selected_synthesisData.MATERIAL_2_NUM;
            equips_needGemText.text = minerals.ToString();
            var haveMineral = GameManager.Instance.GetUserClass().GetHadGemList().Find(tmp => tmp.GetName().Equals(selected_synthesisData.MATERIAL_2));
            if (haveMineral.GetNumber() < minerals)     // 광물 보유 여부 판단 후, 미달 시 색상 변경
                equips_needGemText.color = Color.red;
            else
                equips_needGemText.color = Color.white;

            equips_contentText.text = equips_currentNum.ToString() + "/" + CalculateMaxValue(equips_currentNum).ToString();
            TextMeshProUGUI targetNumeText = SynthesisObjectScreen.transform.GetChild(8).GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>();
            targetNumeText.text = "장비 합성을 통해서 " + etc_currentNum.ToString() + "개의 장비 제조";
        }
        // 합성할 개체를 선택하여 호출되는 이벤트 리스너
        void SynthesisButtonSelect(ItemClass selected, e_PoolItemType type, SYNTHESIS_DATA_BASE selectData)
        {

            instance.dic_ItemClsForUpgrade.Clear();
            instance.openUI_ItemList.Clear();
            selectedItem_sythesis = selected;
            selected_synthesisData = selectData;
            Debug.Log(selectedItem_sythesis.GetName());
            etc_currentNum = 0;
            etc_fillingBar.SetFillProgress(0);
            List<InvenItemObjClass> pools = new List<InvenItemObjClass>();

            switch (type)
            {
                case e_PoolItemType.Weapon:
                    pools = GameManager.Instance.WeaponItemPool.GetPoolList();
                    SynthesisPrintEquipsObjects();
                    break;
                case e_PoolItemType.Equip:
                    EquipIndexSetter();
                    pools = GameManager.Instance.EquipItemPool.GetPoolList();
                    SynthesisPrintEquipsObjects();
                    break;
                case e_PoolItemType.Gem:
                    pools = GameManager.Instance.GemItemPool.GetPoolList();
                    SynthesisPrintEtcObjects();
                    break;
            }

            // 다른 버튼 활성화 취소
            foreach (var tmp in pools)
            {
                if (tmp.GetIsActive() == true && tmp.GetItemcls() != selected && tmp.GetButtonClicked() == true)
                    tmp.ClickedUIApply();
            }
        }
        // 성유물 선택시 호출함수(보유 객체가 아닐때.)
        void SynthesisButtonSelect_DontHaveItem(ItemClass target)
        {
            instance.getResourcePrintUI.Scroll_Start(target);
        }

        void EquipIndexSetter() //성유물의 인덱스 설정
        {
            switch(selectedItem_sythesis.GetTag())
            {
                case "꽃":
                    instance.nSelected_EquipIndex = 0;
                    break;
                case "깃털":
                    instance.nSelected_EquipIndex = 1;
                    break;
                case "모래":
                    instance.nSelected_EquipIndex = 2;
                    break;
                case "성배":
                    instance.nSelected_EquipIndex = 3;
                    break;
                case "왕관":
                    instance.nSelected_EquipIndex = 4;
                    break;
            }
        }

        // 사용버튼 클릭 이벤트 리스너
        void UseButtonFunc()
        {
            Debug.Log(nameof(instance.dic_ItemClsForUpgrade.Count)+instance.dic_ItemClsForUpgrade.Count);
            Debug.Log(nameof(etc_currentNum)+ etc_currentNum);

            if (selectedItem_sythesis == null || synthesisType_Index == e_InventoryTypeSelected.None)
                return;
            if (etc_currentNum == 0)
                return;


            if(synthesisType_Index == e_InventoryTypeSelected.Weapon || synthesisType_Index == e_InventoryTypeSelected.Equipment)
            {
                int minerals = etc_currentNum * selected_synthesisData.MATERIAL_2_NUM;
                var haveMineral = GameManager.Instance.GetUserClass().GetHadGemList().Find(tmp => tmp.GetName().Equals(selected_synthesisData.MATERIAL_2));
                if (haveMineral.GetNumber() < minerals) // 요구 미네랄을 충족하지 못활 경우에는 리턴
                    return;

                List<ItemClass> userItems = null;
                if (synthesisType_Index == e_InventoryTypeSelected.Weapon)
                    userItems = GameManager.Instance.GetUserClass().GetHadWeaponList();
                else
                    userItems = GameManager.Instance.GetUserClass().GetHadEquipmentList();

                // 제거 대상인 아이템을 임시리스트에 저장.
                List<ItemClass> itemsToRemove = new List<ItemClass>();

                int count = 0;
                foreach (var tmp in instance.dic_ItemClsForUpgrade)
                {
                    itemsToRemove.Add(tmp.Key);
                    count++;

                    if (count >= etc_currentNum * 3)
                        break;
                }

                // 유저의 보유 아이템을 제거
                foreach (var tmp in itemsToRemove)
                {
                    userItems.Remove(tmp);
                }

                // 광물 제거
                int currentMineral = haveMineral.GetNumber();
                if (currentMineral == minerals)     // 개수가 같을 경우에는 광물 제거
                {
                    GameManager.Instance.GetUserClass().GetHadGemList().Remove(haveMineral);
                }
                else    // 그렇지 않다면, 광물을 감산하여 Set
                {
                    haveMineral.SetNumber(currentMineral - minerals);
                }


                // 합성하여 얻을 아이템을 초기화
                List<ItemClass> getItems = new List<ItemClass>();
                getItems.AddRange(Enumerable.Repeat(selectedItem_sythesis, etc_currentNum));


                // 아이디값 생성기 호출
                List<WeaponAndEquipCls> childList = new List<WeaponAndEquipCls>();
                foreach(var tmp in getItems)
                {
                    WeaponAndEquipCls data = new WeaponAndEquipCls(); // 새로운 인스턴스 생성
                    data.CopyFrom(tmp);                                 // 참조 복사로 인한, 데이터 중복 수정을 방지
                    if (synthesisType_Index == e_InventoryTypeSelected.Equipment)
                        GameManager.Instance.EquipStatusRandomSelector(data);
                    childList.Add(data);
                }


                List<WeaponAndEquipCls> updatedItems = GameManager.Instance.Item_Id_Generator_Copied(childList);
                getItems = new List<ItemClass>(updatedItems);

                foreach (var tmp in getItems)
                    Debug.Log(tmp.GetId() + "<= id값 ");

                if (synthesisType_Index == e_InventoryTypeSelected.Equipment)
                    GameManager.Instance.EquipItemStatusSet(getItems);

                // 합성한 아이템을 합성 갯수 만큼 유저 보유 아이템에 추가.
                userItems.AddRange(getItems);

            }
            else if(synthesisType_Index == e_InventoryTypeSelected.Etc)   
            {
                var items = GameManager.Instance.GetUserClass().GetHadGrowMaterialList();
                ItemClass removeItem = items.Find(tmp=>tmp.GetName().Equals(selected_synthesisData.MATERIAL_1));
                int removeNums = etc_currentNum * selected_synthesisData.MATERIAL_1_NUM;
                int removeMora = etc_currentNum * selected_synthesisData.MATERIAL_2_NUM;

                // 소모할 개수가 
                if(removeItem.GetNumber() <= removeNums)
                    items.Remove(removeItem);
                else
                    removeItem.SetNumber(removeItem.GetNumber() - removeNums);

                int curMora= GameManager.Instance.GetUserClass().GetMora();
                GameManager.Instance.GetUserClass().SetMora(curMora - removeMora);
                moraText.text = (curMora - removeMora).ToString();

                var db = GameManager.Instance.GetItemDataList();
                ItemClass target = db.Find(tmp => tmp.GetName().Equals(selected_synthesisData.COMPLETE_iTEM));
                target.SetNumber(etc_currentNum);
                GameManager.Instance.Item_Id_Generator(target);                                     // 아이디값 생성기 호출
                GameManager.Instance.GetUserClass().GetHadGrowMaterialList().Add(target);
            }
            etc_currentNum = 0;
            selectedItem_sythesis = null;
            selected_synthesisData = default;
            SynthesisPrintDataDevider();
        }

    }







    #endregion
}
