using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

public class UI_Manager : EnergyBarManager
{
    // 싱글턴 인스턴스
    private static UI_Manager instance;
    public static UI_Manager Instance { get { return instance; } }

    #region 변수
    // 인벤토리 관련
    public GameObject Inventory;                // 인벤토리 전체 객체
    
    [SerializeField]                        
    private InventoryButton[] invenButtons;             // 인벤토리 아이템 타입 선택 버튼들(0-웨폰,1-장비,2-광물,3-음식)
    [SerializeField]
    private Transform scrollContent;                    //인벤토리 스크롤뷰 콘텐츠의 Transform
    [SerializeField]
    private GameObject scrollViewObj;                   // 스크롤뷰 콘텐츠에 부착될 컴포넌트 프리팹
    [SerializeField]
    private GameObject ExpressFrame;                    // 특정 아이템 선택시 아이템 정보 출력창
    [SerializeField]
    private GameObject SortSelectionButtonOnList;       // Sort 선택 버튼 클릭 시, 띄우는 리스트 버튼 객체


    private List<InvenItemObjClass> openUI_ItemList;    // 선택한 아이템 타입의 객체 리스트
    private bool[] invenButtonIsClicked;                // 인벤토리 아이템 타입선택 버튼 클릭 여부
    private bool isAscending;                           // 오름차순 정렬 기준인지 판단
    private e_SortingOrder selected_SortOrder;            // 현재 선택된 정렬 기준
    private int nSelectedInvenIdx;


    #endregion

    #region 구조체
    public enum e_SortingOrder
    {
        GradeOrder,
        LevelOrder,
        NameOrder
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
        Inventory.SetActive(false); // 인벤토리는 기본적으로 Off
        openUI_ItemList = new List<InvenItemObjClass>();                    // 선택한 아이템 리스트
        invenButtonIsClicked = new bool[] { false, false, false, false };   // 배열 초기화
        isAscending = false;
        selected_SortOrder = e_SortingOrder.GradeOrder;
        nSelectedInvenIdx = 0;                                              // 선택한 정렬 버튼 인덱스 무기로 초기화
        SortSelectionButtonOnList.SetActive(false);                         // 정렬 리스트 목록 숨김
    }

    #region 인벤토리 UI 관리
    
    //인벤토리 오픈 버튼
    public void BagOpenButtonClick()
    {
        GameManager.Instance.PauseManager();
        Inventory.SetActive(true);
        ExpressFrame.SetActive(false);
        invenButtons[0].ButtonUIColorSet();
        InventoryViewItemTypeNotify(0);
        Debug.Log("일시정지");
    }
    // 인벤토리 종료 버튼
    public void BagCloseButtonClick()
    {
        GameManager.Instance.PauseManager();
        Inventory.SetActive(false);

    }
    // 사용하기 버튼 클릭시 호출
    public void UsingButtonClick()
    {
        Debug.Log("아이템 사용");
    }

    #region 스크롤뷰 아이템 출력

    // 아이템 타입 버튼 클릭시 인덱스값 송신 함수
    public void InventoryViewItemTypeNotify(int index)
    {
        nSelectedInvenIdx = index;
        // 만약 선택한 타입이 이미 열람 중인 객체라면, 이벤트 리턴.
        if (invenButtonIsClicked[nSelectedInvenIdx] == true)
        {
            invenButtons[nSelectedInvenIdx].ButtonUIColorSet(); // UI 상태는 유지
            return;
        }
        // 타입 선택 버튼들을 순회하며, 해당 버튼 UI의 Active 상태를 Off하고 부울 초기화
        for (int i = 0; i < invenButtons.Length; i++)
        {
            if (invenButtons[i].GetClickActive() == true && i != nSelectedInvenIdx)
                invenButtons[i].ButtonUIColorSet();
            invenButtonIsClicked[i] = false;
        }

        invenButtonIsClicked[nSelectedInvenIdx] = true; // 클릭한 버튼 객체의 인덱스를 true.

        ViewProcess();
    }

    private void ViewProcess()
    {
        ScrollViewReset();                              // 스크롤뷰 리셋
        ItemPrintByObj_Index(nSelectedInvenIdx);        // 아이템 출력
        ExpressFrame.gameObject.SetActive(false);
    }

    // 인덱스 값에 따라서 아이템 출력
    // ascending == true : 오름차순, false : 내림차순
    private void ItemPrintByObj_Index(int index)
    {
        switch((GameManager.e_PoolItemType)index)
        {
            case GameManager.e_PoolItemType.Weapon:    //웨폰
                WeaponPrintAtScroll(selected_SortOrder);
                break;
            case GameManager.e_PoolItemType.Equip:     //장비
                EquipPrintAtScroll(selected_SortOrder);
                break;
            case GameManager.e_PoolItemType.Gem:       //광물
                
                break;
            case GameManager.e_PoolItemType.Food:      //음식
                
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
    void WeaponPrintAtScroll(e_SortingOrder sortOrder)
    {
        var itemClses = GameManager.Instance.GetWeaponItemClass();      // 저장된 아이템 목록

        SortingItemList(itemClses, sortOrder);


        // 오브젝트 풀로, UI객체 생성
        GameManager.Instance.ItemToObjPool(itemClses.Count, GameManager.e_PoolItemType.Weapon, scrollContent);
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
    void EquipPrintAtScroll(e_SortingOrder sortOrder)
    {
        var itemClses = GameManager.Instance.GetEquipItemClass();      // 저장된 아이템 목록

        SortingItemList(itemClses, sortOrder);


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

                obj.SetItemColor(data.GetGrade());
                obj.SetItemText("LV : " + data.GetLevel().ToString());
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
            mng.HideButtonBackGround();
            SortSelectionButtonOnList.SetActive(true);
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

        // 아이템 이미지 출력
        topImage.sprite = clickedObj.GetItemSprite();

    }

    public void ExpressFrameReset()
    {
        ExpressFrame.gameObject.SetActive(false);
    }
    #endregion



    #endregion

    #region 기타

    /* 아이템 정렬 
    *  sortingOrder == GradeOrder : 등급 기준  // LevelOrder : 레벨 기준  //  NameOrder : 이름 기준
    *  ascending == true : 오름차순, false : 내림차순
    */
    void SortingItemList(List<ItemClass> clsList, e_SortingOrder sortingOrder)
    {
        switch (sortingOrder)
        {
            case e_SortingOrder.GradeOrder: // 등급을 기준으로 정렬
                if (isAscending)
                    clsList.Sort((item1, item2) => item1.GetGrade().CompareTo(item2.GetGrade()));
                else
                    clsList.Sort((item1, item2) => item2.GetGrade().CompareTo(item1.GetGrade()));
                break;
            case e_SortingOrder.LevelOrder: // 레벨을 기준으로 정렬
                if (isAscending)
                    clsList.Sort((item1, item2) => item1.GetLevel().CompareTo(item2.GetLevel()));
                else
                    clsList.Sort((item1, item2) => item2.GetLevel().CompareTo(item1.GetLevel()));
                break;
            case e_SortingOrder.NameOrder: // 이름을 기준으로 정렬
                if (isAscending)
                    clsList.Sort((item1, item2) => string.Compare(item1.GetName(), item2.GetName()));
                else
                    clsList.Sort((item1, item2) => string.Compare(item2.GetName(), item1.GetName()));
                break;
            default:
                break;
        }
    }
    #endregion
}
