using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;
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

    private List<InvenItemObjClass> openUI_ItemList;    // 선택한 아이템 타입의 객체 리스트

    #endregion

    #region 구조체
    enum SortingOrder
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
        openUI_ItemList = new List<InvenItemObjClass>();
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
        for(int i=0; i<invenButtons.Length; i++)
        {
            if(invenButtons[i].GetClickActive() == true && i != index)
                invenButtons[i].ButtonUIColorSet();
        }
        ScrollViewReset(index);             // 스크롤뷰 리셋
        ItemPrintByObj_Index(index);        // 아이템 출력
        ExpressFrame.gameObject.SetActive(false);
    }

    // 인덱스 값에 따라서 아이템 출력
    private void ItemPrintByObj_Index(int index)
    {
        switch((GameManager.e_PoolItemType)index)
        {
            case GameManager.e_PoolItemType.Weapon:    //웨폰
                WeaponPrintAtScroll(SortingOrder.NameOrder,true);
                break;
            case GameManager.e_PoolItemType.Equip:     //장비
                
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
    private void ScrollViewReset(int index)
    {
        switch((GameManager.e_PoolItemType)index)
        {
            case GameManager.e_PoolItemType.Weapon:
                GameManager.Instance.EquipItemPool.AllReturnToPool();
                GameManager.Instance.GemItemPool.AllReturnToPool();
                GameManager.Instance.FoodItemPool.AllReturnToPool();
                break;
            case GameManager.e_PoolItemType.Equip:
                GameManager.Instance.WeaponItemPool.AllReturnToPool();
                GameManager.Instance.GemItemPool.AllReturnToPool();
                GameManager.Instance.FoodItemPool.AllReturnToPool();
                break;
            case GameManager.e_PoolItemType.Gem:
                GameManager.Instance.EquipItemPool.AllReturnToPool();
                GameManager.Instance.WeaponItemPool.AllReturnToPool();
                GameManager.Instance.FoodItemPool.AllReturnToPool();
                break;
            case GameManager.e_PoolItemType.Food:
                GameManager.Instance.EquipItemPool.AllReturnToPool();
                GameManager.Instance.GemItemPool.AllReturnToPool();
                GameManager.Instance.WeaponItemPool.AllReturnToPool();
                break;
            default: break;
        }
        openUI_ItemList.Clear();
    }
    
    // 게임매니저의 데이터를 참조하여, 무기들을 스크롤뷰 콘텐츠에 출력
    void WeaponPrintAtScroll(SortingOrder sortOrder, bool ascending)
    {
        var itemClses = GameManager.Instance.GetWeaponItemClass();      // 저장된 아이템 목록

        SortingItemList(itemClses, sortOrder, ascending);


        // 오브젝트 풀로, UI객체 생성
        GameManager.Instance.WeaponItemToObjPool(itemClses.Count, GameManager.e_PoolItemType.Weapon, scrollContent);
        // 오브젝트 풀에 저장된 리스트 인스턴스화

        var datas = GameManager.Instance.WeaponItemPool.GetPoolList();


        foreach (ItemClass data in itemClses)
        {
            foreach(InvenItemObjClass obj in datas)
            {
                if (obj.gameObject.activeSelf == false || obj.GetIsActive() == true)
                    continue;

                if(data.GetName() == "천공의 검")
                {
                    obj.SetItemSprite(ItemSpritesSaver.Instance.WeaponSprites[0]);
                    obj.SetItemColor(data.GetGrade());
                    obj.SetItemText("LV : " + data.GetLevel().ToString());
                    obj.SetIsActive(true);
                    obj.SetItemcls(data);
                    openUI_ItemList.Add(obj);
                    break;
                }
                else if(data.GetName() == "제례검")
                {
                    obj.SetItemSprite(ItemSpritesSaver.Instance.WeaponSprites[1]);
                    obj.SetItemColor(data.GetGrade());
                    obj.SetItemText("LV : " + data.GetLevel().ToString());
                    obj.SetIsActive(true);
                    obj.SetItemcls(data);
                    openUI_ItemList.Add(obj);
                    break;
                }
            }
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
        switch(itemCls.GetName())
        {
            case "천공의 검":
                topImage.sprite = ItemSpritesSaver.Instance.WeaponSprites[0];
                break;
            case "제례검":
                topImage.sprite = ItemSpritesSaver.Instance.WeaponSprites[1];
                break;
            default:
                break;
        }

        
    }
    #endregion


    #region 기타

    /* 아이템 정렬 
    *  sortingOrder == GradeOrder : 등급 기준  // LevelOrder : 레벨 기준  //  NameOrder : 이름 기준
    *  ascending == true : 오름차순, false : 내림차순
    */
    void SortingItemList(List<ItemClass> clsList, SortingOrder sortingOrder, bool ascending)
    {
        switch (sortingOrder)
        {
            case SortingOrder.GradeOrder: // 등급을 기준으로 정렬
                if (ascending)
                    clsList.Sort((item1, item2) => item1.GetGrade().CompareTo(item2.GetGrade()));
                else
                    clsList.Sort((item1, item2) => item2.GetGrade().CompareTo(item1.GetGrade()));
                break;
            case SortingOrder.LevelOrder: // 레벨을 기준으로 정렬
                if (ascending)
                    clsList.Sort((item1, item2) => item1.GetLevel().CompareTo(item2.GetLevel()));
                else
                    clsList.Sort((item1, item2) => item2.GetLevel().CompareTo(item1.GetLevel()));
                break;
            case SortingOrder.NameOrder: // 이름을 기준으로 정렬
                if (ascending)
                    clsList.Sort((item1, item2) => string.Compare(item1.GetName(), item2.GetName()));
                else
                    clsList.Sort((item1, item2) => string.Compare(item2.GetName(), item1.GetName()));
                break;
            default:
                break;
        }
    }
    #endregion

    #endregion

}
