using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Reflection;
using System.Data;
using JetBrains.Annotations;

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

    private e_InfoButtonSelected info_Index;                // 선택한 정보 인덱스
    private bool isDetailScreenOpen;                        // 캐릭터 상세정보 프리팹 On/Off 플래그 변수
    private bool isWeaponChangeBtnClicked;                  // 무기 변경 클릭 제어 플래그
    private bool isWeaponUpgradeBtnClicked;                 // 무기 업글 클릭 제어 플래그
    private ButtonClass2 weaponChangeButton;                // 무기 변경 버튼



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
        NameOrder =1,
        LevelOrder =2,
        GradeOrder =3
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
        isWeaponChangeBtnClicked = false;   // 무기 전환 제어 변수


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
                WeaponPrintAtScroll(scrollContent);
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
    
    // 게임매니저의 데이터를 참조하여, 무기들을 스크롤뷰 콘텐츠에 출력
    void WeaponPrintAtScroll(Transform content)
    {
        var itemClses = GameManager.Instance.GetUserClass().GetHadWeaponList();      // 저장된 아이템 목록

        SortingItemList(itemClses);


        // 오브젝트 풀로, UI객체 생성
        GameManager.Instance.ItemToObjPool(itemClses.Count, GameManager.e_PoolItemType.Weapon, content);
        // 오브젝트 풀에 저장된 리스트 인스턴스화

        var datas = GameManager.Instance.WeaponItemPool.GetPoolList();


        foreach (ItemClass data in itemClses)
        {
            foreach(InvenItemObjClass obj in datas)
            {
                if (obj.gameObject.activeSelf == false || obj.GetIsActive() == true)
                    continue;

                if(data.GetName() == "천공의 검")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.WeaponSprites[0]);
                else if(data.GetName() == "제례검")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.WeaponSprites[1]);
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

    // 게임매니저의 데이터를 참조하여, 장비들을 스크롤뷰 콘텐츠에 출력
    void EquipPrintAtScroll()
    {
        var itemClses = GameManager.Instance.GetUserClass().GetHadEquipmentList();      // 저장된 아이템 목록

        SortingItemList(itemClses);


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
                else if (data.GetName() == "피에 물든 기사의 술잔")
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

        SortingItemList(itemClses);


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

        SortingItemList(itemClses);


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


    void ItemUISetterByItemGrade(ItemClass itemCls, Image titleFrame, Image topFrame)
    {
        // 등급에 따라서 색상 변경
        switch (itemCls.GetGrade())
        {
            case 5:
                titleFrame.color = ItemSpritesSaver.Instance.GetFiveStarColor();
                topFrame.sprite = ItemSpritesSaver.Instance.GradationSprite[0];
                break;
            case 4:
                titleFrame.color = ItemSpritesSaver.Instance.GetFourStarColor();
                topFrame.sprite = ItemSpritesSaver.Instance.GradationSprite[1];
                break;
            case 3:
                titleFrame.color = ItemSpritesSaver.Instance.GetThreeStarColor();
                topFrame.sprite = ItemSpritesSaver.Instance.GradationSprite[2];
                break;
            default:
                titleFrame.color = ItemSpritesSaver.Instance.GetOneStarColor();
                topFrame.sprite = ItemSpritesSaver.Instance.GradationSprite[3];
                break;
        }
    }
    void ItemUISetterByItemGrade(WeaponAndEquipCls itemCls, Image titleFrame, Image topFrame)
    {
        // 등급에 따라서 색상 변경
        switch (itemCls.GetGrade())
        {
            case 5:
                titleFrame.color = ItemSpritesSaver.Instance.GetFiveStarColor();
                topFrame.sprite = ItemSpritesSaver.Instance.GradationSprite[0];
                break;
            case 4:
                titleFrame.color = ItemSpritesSaver.Instance.GetFourStarColor();
                topFrame.sprite = ItemSpritesSaver.Instance.GradationSprite[1];
                break;
            case 3:
                titleFrame.color = ItemSpritesSaver.Instance.GetThreeStarColor();
                topFrame.sprite = ItemSpritesSaver.Instance.GradationSprite[2];
                break;
            default:
                titleFrame.color = ItemSpritesSaver.Instance.GetOneStarColor();
                topFrame.sprite = ItemSpritesSaver.Instance.GradationSprite[3];
                break;
        }
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

            InfoPrintTypeButtonActive();
        }
        // 무기 교체 버튼 클릭 시, 기능 변경
        else if(isWeaponChangeBtnClicked)
        {
            InfoPrintTypeButtonActive();
            CloseButtonSpriteToClose();
            infoSelectButtons[1].OnOffSpriteSetting();              // 웨폰 인포 버튼 UI 활성화
            weaponChangeButton.AlphaValueChangeing();               // 무기 변경 버튼 알파값 변경

            ResetToWeaponItemObjectPoolDatas();
            isWeaponChangeBtnClicked = false;
        }
        // 무기 업글 버튼 클릭 시, 기능 변경
        else if(isWeaponUpgradeBtnClicked)
        {
            InfoPrintTypeButtonActive();
            CloseButtonSpriteToClose();

            Transform screen = printInfoDataField[1].transform.GetChild(13).GetComponent<Transform>();    // 무기 업글 UI 오브젝트 인스턴스화
            screen.gameObject.SetActive(false);

            ResetToWeaponItemObjectPoolDatas();
            isWeaponUpgradeBtnClicked = false;
        }
        else    // 종료
        {
            
            GameManager.Instance.PauseManager();
            PlayerInfoScreen.SetActive(false);
        }
    }

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

    private void CloseButtonSpriteToClose()
    {
        // 스프라이트 이미지(ToClose) 변경
        InfoObj_CloseButtion.GetComponent<ButtonClass2>().SetSymbolSprite(ItemSpritesSaver.Instance.SpritesSet[1]);
        infoSelectButtons[1].OnOffSpriteSetting();              // 웨폰 인포 버튼 UI 활성화
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

    // 무기에 따라 달라지는 장비 데이터 출력을 담당하는 함수
    void WeaponKindDivider(WeaponAndEquipCls weaponCls, Image WeaponImage = null, TextMeshProUGUI weaponStatusLabelTxt = null, TextMeshProUGUI weaponStatusTxt = null)
    {
        // 장비 이름에 따라 데이터 분기
        switch (weaponCls.GetName())
        {
            case "천공의 검":
            {
                if (WeaponImage != null)
                    WeaponImage.sprite = ItemSpritesSaver.Instance.WeaponSprites[0];

                if (weaponStatusLabelTxt != null)
                    weaponStatusLabelTxt.text = "원소 충전 효율";

                if (weaponStatusTxt != null)
                    weaponStatusTxt.text = weaponCls.GetSubStat().ToString() + "%";
            }
            break;
            case "제례검":
            {
                if (WeaponImage != null)
                    WeaponImage.sprite = ItemSpritesSaver.Instance.WeaponSprites[1];

                if (weaponStatusLabelTxt != null)
                    weaponStatusLabelTxt.text = "원소 충전 효율";

                if (weaponStatusTxt != null)
                    weaponStatusTxt.text = weaponCls.GetSubStat().ToString() + "%";
            }
            break;
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
        WeaponPrintAtScroll(scrollViewContentObj);              // 콘텐트에 UI 출력

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
        WeaponPrintAtScroll(scrollViewContentObj);              // 콘텐트에 UI 출력
        EquippedWeaponPrint();
    }
    #endregion

    #region 무기 업그레이드_상세정보

    //  무기 업그레이드 버튼 클릭 시 호출 함수
    public void WeaponUpgeadeButtonClickEvent()
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
        PlayerInfoUI_Button cls = buttons[0];
        WeaponUpgrade_DetailScreenButton(cls, equippedWeapon, tmp);
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
                continue;

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

    }

    #endregion

    #region 무기 업그레이드_레벨업

    // 무기 레벨업 출력 함수
    private void WeaponUpgrade_LevelUpButton(PlayerInfoUI_Button my, WeaponAndEquipCls equippedWeapon, Transform parents)
    {
        my.OnOffSpriteSetting();
        ClickedButtonsSetActiveReset(my, parents, 5);
        var mainObject = parents.GetChild(5).GetComponent<Transform>();

        // 스텟 표기 UI 오브젝트
        Image MainStatBgr = mainObject.GetChild(0).gameObject.GetComponent<Image>();
        Image SubStatBgr = mainObject.GetChild(1).gameObject.GetComponent<Image>();

        // 각 속성 별 UI 오브젝트
        Transform LevelLimitBreak_Obj = mainObject.GetChild(6).GetComponent<Transform>();
        Transform LevelUpBreak_Obj = mainObject.GetChild(7).GetComponent<Transform>();
        
        // 사용버튼 오브젝트
        Transform useBtn = mainObject.GetChild(2).GetComponent<Transform>();
        var useButtonObj = useBtn.GetComponentInChildren<Button>();
        // 모라 오브젝트
        Transform moraObj = mainObject.GetChild(3).GetComponent<Transform>();

        int curLevelData = equippedWeapon.GetLevel();
        int maxLevelData = equippedWeapon.GetLimitLevel();
        int WeaponGrade = equippedWeapon.GetGrade();

        // 상세 정보는 레벨에 따라 분기하여 출력하므로, 우선 SetFalse함
        LevelLimitBreak_Obj.gameObject.SetActive(false);
        LevelUpBreak_Obj.gameObject.SetActive(false);

        // 현재레벨 == 한계레벨 (돌파요구)
        if (curLevelData == maxLevelData)
        {
            LevelLimitBreak_Obj.gameObject.SetActive(true);
        }
        // 현재레벨 != 한계레벨 (레벨업 가능)
        else
        {
            LevelUpBreak_Obj.gameObject.SetActive(true);

            // 메인 스탯 UI 출력
            MainStatBgr.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = equippedWeapon.GetMainStat().ToString(); // 주스탯 표기
            MainStatBgr.transform.GetChild(2).gameObject.SetActive(false);
            MainStatBgr.transform.GetChild(3).gameObject.SetActive(false);

            // 서브 스탯 UI 출력
            var subStatLabel = SubStatBgr.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            var subStatText = SubStatBgr.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            WeaponKindDivider(equippedWeapon,null,subStatLabel,subStatText);
            SubStatBgr.transform.GetChild(2).gameObject.SetActive(false);
            SubStatBgr.transform.GetChild(3).gameObject.SetActive(false);

            // 사용 버튼 UI 수정 및 이벤트 리스너 연결
            useButtonObj.onClick.AddListener(() => WeaponLevelUpClickEventListener());
            useBtn.GetChild(4).GetComponent<TextMeshProUGUI>().text = "강화";

            // 모라 UI 출력
            moraObj.GetChild(1).GetComponent<TextMeshProUGUI>().text = ((curLevelData*1000)* WeaponGrade*3).ToString();

            // 상세 정보 UI 인스턴스화
            ButtonClass2 AllSelectButton = LevelUpBreak_Obj.GetChild(0).GetComponent<ButtonClass2>();
            ButtonClass2 SelectSortButton = LevelUpBreak_Obj.GetChild(1).GetComponent<ButtonClass2>();
            InventorySortSelectButton SelectionButtonOnList = LevelUpBreak_Obj.GetChild(2).GetComponent<InventorySortSelectButton>();
            Transform Select_Item_Scroll_Content = LevelUpBreak_Obj.GetChild(3).GetChild(0).GetChild(0).GetComponent<Transform>();
            TextMeshProUGUI LevelText = LevelUpBreak_Obj.GetChild(4).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI LevelExpText = LevelUpBreak_Obj.GetChild(5).GetComponent<TextMeshProUGUI>();
            Image ExpProgressBar_Outline = LevelUpBreak_Obj.GetChild(6).GetComponent<Image>();
            Image ExpProgressBar_Inline = LevelUpBreak_Obj.GetChild(7).GetComponent<Image>();

            // SelectSortButton 클릭 이벤트 연결
            SelectSortButton.GetButton().onClick.AddListener(() => SelectSortButtonClickEventListener(SelectionButtonOnList, SelectSortButton));
            SelectSortButton.SetButtonTextInputter("★3 재료");

            // exp바 출력
            float currentExp = equippedWeapon.GetCurrentExp();
            float maxExp = equippedWeapon.GetMaxExp();
            ExpProgressBar_Inline.fillAmount = currentExp / maxExp;

            // 레벨 출력
            LevelText.text = "LV. "+curLevelData.ToString();
            LevelExpText.text = Mathf.Floor(currentExp).ToString() + "/" + Mathf.Floor(maxExp).ToString();

            // 리스트 버튼은 기본 비활성화
            SelectionButtonOnList.gameObject.SetActive(false);
            string[] strings = { "★3 재료", "★4 재료", "★5 재료" };
            SelectionButtonOnList.SetButtonsText(strings);                      // 버튼 텍스트 적용
            SelectionButtonOnList.StringUIApplyer();                            // 버튼 텍스트 적용
            SelectionButtonOnList.SetSortingOrder(e_SortingOrder.NameOrder);    // 디폴트는 3성 재료
            SelectionButtonOnList.HideButtonBackGround();


            // 스크롤뷰 콘텐츠 채우기
            WeaponPrintAtScroll_BySelectButton(Select_Item_Scroll_Content);

        }


        Debug.Log("돌파 버튼 클릭");
    }

    // Select버튼 오브젝트 풀, 무기 출력
    void WeaponPrintAtScroll_BySelectButton(Transform content)
    {
        var itemClses = GameManager.Instance.GetUserClass().GetHadWeaponList();      // 저장된 아이템 목록

        SortingItemList(itemClses);
        
        // 오브젝트 풀로, UI객체 생성
        GameManager.Instance.ItemToObjPool_SelectButton(itemClses.Count, content);
        
        // 오브젝트 풀에 저장된 리스트 인스턴스화
        var datas = GameManager.Instance.SelectButtonScriptPool.GetPoolList();


        foreach (ItemClass data in itemClses)
        {
            foreach(SelectButtonScript obj in datas)
            {
                if (obj.gameObject.activeSelf == false || obj.GetIsActive() == true)
                    continue;

                if(data.GetName() == "천공의 검")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.WeaponSprites[0]);
                else if(data.GetName() == "제례검")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.WeaponSprites[1]);
                else { break; }

                //if (data.GetIsActive() == true) // 아이템이 활성 상태라면, 사용중임을 알림.
                //    obj.EquippedItemUIPrint(true);

                obj.SetItemColor(data.GetGrade());
                obj.SetItemText("LV : " + data.GetLevel().ToString());
                obj.SetIsActive(true);
                obj.SetItemcls(data);
                break;
            }
        }
    }

    // SelectSortButton 버튼의 이벤트 리스너
    void SelectSortButtonClickEventListener(InventorySortSelectButton SelectionButtonOnList, ButtonClass2 SelectSortButton)
    {
        // 리스트 버튼은 기본 활성화
        SelectionButtonOnList.gameObject.SetActive(true);
        Button[] buttons = SelectionButtonOnList.GetButtons();

        for (int i=0; i<buttons.Length; i++)
        {
            buttons[i].onClick.RemoveAllListeners();
        }
        buttons[0].onClick.AddListener(() => StarOrderButtonClickListener(SelectionButtonOnList, 3, SelectSortButton));
        buttons[1].onClick.AddListener(() => StarOrderButtonClickListener(SelectionButtonOnList, 4, SelectSortButton));
        buttons[2].onClick.AddListener(() => StarOrderButtonClickListener(SelectionButtonOnList, 5, SelectSortButton));


    }
    // 리스트 버튼 객체의 선택한 버튼 객체의 함수.
    void StarOrderButtonClickListener(InventorySortSelectButton SelectionButtonOnList, int selectNum, ButtonClass2 SelectSortButton)
    {
        e_SortingOrder tmp = e_SortingOrder.GradeOrder;
        if (selectNum == 3) tmp = e_SortingOrder.NameOrder;
        else if (selectNum == 4) tmp = e_SortingOrder.LevelOrder;
        else if (selectNum == 5) tmp = e_SortingOrder.GradeOrder;

        SelectionButtonOnList.SetSortingOrder(tmp);
        SelectionButtonOnList.HideButtonBackGround();
        SelectionButtonOnList.gameObject.SetActive(false);  // 비활성화
        NotifySelectStarOrderEventListener(selectNum, SelectSortButton);
    }
    int NotifySelectStarOrderEventListener(int selectNum, ButtonClass2 SelectSortButton)
    {
        Debug.Log("회귀도 : " + selectNum);
        SelectSortButton.SetButtonTextInputter("★"+ selectNum + " 재료");
        return selectNum;   // 선택한 객체의 번호를 리턴함
    }


    // 레벨업(강화)_사용 버튼 이벤트 리스너
    void WeaponLevelUpClickEventListener()
    {

    }
    // 레벨업(강화)_돌파 버튼 이벤트 리스너
    void WeaponLimitBreakClickEventListener()
    {

    }


    // 스크롤뷰 콘텐츠 아이템 선택 함수
    void ContentWeaponSelectClickEventListener(Button btn)
    {

    }

    #endregion

    #region 무기 업그레이드_재련

    // 무기 재련 출력 함수
    private void WeaponUpgrade_ReforgeButton(PlayerInfoUI_Button my, WeaponAndEquipCls equippedWeapon, Transform parents)
    {
        my.OnOffSpriteSetting();
        ClickedButtonsSetActiveReset(my,parents,6);
        var mainObject = parents.GetChild(6).GetComponent<Transform>();


        Debug.Log("재련 버튼 클릭");
    }
    #endregion

    #endregion



    #region 성유물 UI 관리


    #endregion


    #region 운명의 별자리 UI 관리


    #endregion


    #region 특성 UI 관리

    #endregion


    #endregion

    #region 기타

    /* 아이템 정렬 
    *  sortingOrder == GradeOrder : 등급 기준  // LevelOrder : 레벨 기준  //  NameOrder : 이름 기준
    *  ascending == true : 오름차순, false : 내림차순
    */
    void SortingItemList(List<ItemClass> clsList)
    {
        // isActive가 true인 아이템과 false인 아이템을 나눕니다.
        // isActive가 true인 아이템과 false인 아이템을 나눕니다.
        List<ItemClass> activeItems = clsList.FindAll(item => item.GetIsActive());
        List<ItemClass> inactiveItems = clsList.FindAll(item => !item.GetIsActive());


        // isActive가 true인 아이템을 먼저 정렬합니다.
        switch (selected_SortOrder)
        {
            case e_SortingOrder.GradeOrder: // 등급을 기준으로 정렬
                if (isAscending)
                    activeItems.Sort((item1, item2) => item1.GetGrade().CompareTo(item2.GetGrade()));
                else
                    activeItems.Sort((item1, item2) => item2.GetGrade().CompareTo(item1.GetGrade()));
                break;
            case e_SortingOrder.LevelOrder: // 레벨을 기준으로 정렬
                if (isAscending)
                    activeItems.Sort((item1, item2) => item1.GetLevel().CompareTo(item2.GetLevel()));
                else
                    activeItems.Sort((item1, item2) => item2.GetLevel().CompareTo(item1.GetLevel()));
                break;
            case e_SortingOrder.NameOrder: // 이름을 기준으로 정렬
                if (isAscending)
                    activeItems.Sort((item1, item2) => string.Compare(item1.GetName(), item2.GetName()));
                else
                    activeItems.Sort((item1, item2) => string.Compare(item2.GetName(), item1.GetName()));
                break;
            default:
                break;
        }

        // 선택된 정렬 기준에 따라 inactiveItems를 정렬합니다.
        switch (selected_SortOrder)
        {
            case e_SortingOrder.GradeOrder: // 등급을 기준으로 정렬
                if (isAscending)
                    inactiveItems.Sort((item1, item2) => item1.GetGrade().CompareTo(item2.GetGrade()));
                else
                    inactiveItems.Sort((item1, item2) => item2.GetGrade().CompareTo(item1.GetGrade()));
                break;
            case e_SortingOrder.LevelOrder: // 레벨을 기준으로 정렬
                if (isAscending)
                    inactiveItems.Sort((item1, item2) => item1.GetLevel().CompareTo(item2.GetLevel()));
                else
                    inactiveItems.Sort((item1, item2) => item2.GetLevel().CompareTo(item1.GetLevel()));
                break;
            case e_SortingOrder.NameOrder: // 이름을 기준으로 정렬
                if (isAscending)
                    inactiveItems.Sort((item1, item2) => string.Compare(item1.GetName(), item2.GetName()));
                else
                    inactiveItems.Sort((item1, item2) => string.Compare(item2.GetName(), item1.GetName()));
                break;
            default:
                break;
        }

        // 두 그룹을 병합합니다.
        clsList.Clear();
        clsList.AddRange(activeItems);
        clsList.AddRange(inactiveItems);
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
