using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UI_Manager;

public class UI_UseToolClass
{

    /* 아이템 정렬 
    *  sortingOrder == GradeOrder : 등급 기준  // LevelOrder : 레벨 기준  //  NameOrder : 이름 기준
    *  ascending == true : 오름차순, false : 내림차순
    */

    public static void SortingItemList(List<ItemClass> clsList, e_SortingOrder selected_SortOrder, bool isAscending)
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

    // 게임매니저의 데이터를 참조하여, 무기들을 스크롤뷰 콘텐츠에 출력 <ObjectPool<InvenItemObjClass> WeaponItemPool <- 웨폰 데이터 출력>
    public static void WeaponPrintAtScroll(Transform content, e_SortingOrder selected_SortOrder, bool isAscending, List<InvenItemObjClass> openUI_ItemList)
    {
        var itemClses = GameManager.Instance.GetUserClass().GetHadWeaponList();      // 저장된 아이템 목록

        SortingItemList(itemClses, selected_SortOrder, isAscending);


        // 오브젝트 풀로, UI객체 생성
        GameManager.Instance.ItemToObjPool(itemClses.Count, GameManager.e_PoolItemType.Weapon, content);
        // 오브젝트 풀에 저장된 리스트 인스턴스화

        var datas = GameManager.Instance.WeaponItemPool.GetPoolList();


        foreach (ItemClass data in itemClses)
        {
            foreach (InvenItemObjClass obj in datas)
            {
                if (obj.gameObject.activeSelf == false || obj.GetIsActive() == true)
                    continue;

                if (data.GetName() == "천공의 검")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.WeaponSprites[0]);
                else if (data.GetName() == "제례검")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.WeaponSprites[1]);
                else if (data.GetName() == "여명신검")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.WeaponSprites[2]);
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

    // Select버튼 오브젝트 풀, 무기 출력 <ObjectPool<SelectButtonScript> SelectButtonScriptPool <- 웨폰 데이터 출력>
    public static void WeaponPrintAtScroll_BySelectButton(Transform content, UI_Manager.e_SortingOrder selected_SortOrder, bool isAscending)
    {
        var itemClses = GameManager.Instance.GetUserClass().GetHadWeaponList();      // 저장된 아이템 목록

        SortingItemList(itemClses, selected_SortOrder, isAscending);

        // 오브젝트 풀로, UI객체 생성
        GameManager.Instance.ItemToObjPool_SelectButton(itemClses.Count, content);

        // 오브젝트 풀에 저장된 리스트 인스턴스화
        var datas = GameManager.Instance.SelectButtonScriptPool.GetPoolList();


        foreach (ItemClass data in itemClses)
        {
            foreach (SelectButtonScript obj in datas)
            {
                if (obj.gameObject.activeSelf == false || obj.GetIsActive() == true)
                    continue;

                if (data.GetName() == "천공의 검")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.WeaponSprites[0]);
                else if (data.GetName() == "제례검")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.WeaponSprites[1]);
                else if (data.GetName() == "여명신검")
                    obj.SetItemSprite(ItemSpritesSaver.Instance.WeaponSprites[2]);
                else { break; }

                //if (data.GetIsActive() == true) // 아이템이 활성 상태라면, 사용중임을 알림.
                //    obj.EquippedItemUIPrint(true);

                obj.SetItemColor(data.GetGrade());
                obj.SetItemText("LV : " + data.GetLevel().ToString());
                obj.SetIsActive(true);
                obj.SetItemCls(data);
                break;
            }
        }
    }

    // Select버튼 오브젝트 풀 출력 [ObjectPool<SelectButtonScript> <- 디폴트 출력]
    public static void WeaponPrintAtScroll_BySelectButtonOnDefualtValue(Transform content, int num)
    {
        // 오브젝트 풀로, UI객체 생성
        GameManager.Instance.ItemToObjPool_SelectButton(num, content);

        // 오브젝트 풀에 저장된 리스트 인스턴스화
        var datas = GameManager.Instance.SelectButtonScriptPool.GetPoolList();

        foreach (SelectButtonScript obj in datas)
        {
            if (obj.gameObject.activeSelf == false)
                continue;
            obj.transform.GetChild(0).GetChild(0).GetComponent<Image>().enabled = false;
            obj.SetItemText("-");
        }
    }

    // 무기에 따라 달라지는 장비 데이터 출력을 담당하는 함수
    public static void WeaponKindDivider(WeaponAndEquipCls weaponCls, Image WeaponImage = null, TextMeshProUGUI weaponStatusLabelTxt = null, TextMeshProUGUI weaponStatusTxt = null)
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
            case "여명신검":
                {
                    if (WeaponImage != null)
                        WeaponImage.sprite = ItemSpritesSaver.Instance.WeaponSprites[2];

                    if (weaponStatusLabelTxt != null)
                        weaponStatusLabelTxt.text = "치명타 피해";

                    if (weaponStatusTxt != null)
                        weaponStatusTxt.text = weaponCls.GetSubStat().ToString() + "%";
                }
                break;
        }
    }

    public static void ItemUISetterByItemGrade(ItemClass itemCls, Image titleFrameColor = null, Image topFrameImage = null)
    {
        if (titleFrameColor != null)
        {
            // 등급에 따라서 색상 변경
            switch (itemCls.GetGrade())
            {
                case 5:
                    titleFrameColor.color = ItemSpritesSaver.Instance.GetFiveStarColor();
                    break;
                case 4:
                    titleFrameColor.color = ItemSpritesSaver.Instance.GetFourStarColor();
                    break;
                case 3:
                    titleFrameColor.color = ItemSpritesSaver.Instance.GetThreeStarColor();
                    break;
                default:
                    titleFrameColor.color = ItemSpritesSaver.Instance.GetOneStarColor();
                    break;
            }
        }

        if (topFrameImage != null)
        {
            // 등급에 따라서 이미지 변경
            switch (itemCls.GetGrade())
            {
                case 5:
                    topFrameImage.sprite = ItemSpritesSaver.Instance.GradationSprite[0];
                    break;
                case 4:
                    topFrameImage.sprite = ItemSpritesSaver.Instance.GradationSprite[1];
                    break;
                case 3:
                    topFrameImage.sprite = ItemSpritesSaver.Instance.GradationSprite[2];
                    break;
                default:
                    topFrameImage.sprite = ItemSpritesSaver.Instance.GradationSprite[3];
                    break;
            }
        }
    }
    public static void ItemUISetterByItemGrade(WeaponAndEquipCls itemCls, Image titleFrameColor = null, Image topFrameImage = null)
    {
        if (titleFrameColor != null)
        {
            // 등급에 따라서 색상 변경
            switch (itemCls.GetGrade())
            {
                case 5:
                    titleFrameColor.color = ItemSpritesSaver.Instance.GetFiveStarColor();
                    break;
                case 4:
                    titleFrameColor.color = ItemSpritesSaver.Instance.GetFourStarColor();
                    break;
                case 3:
                    titleFrameColor.color = ItemSpritesSaver.Instance.GetThreeStarColor();
                    break;
                default:
                    titleFrameColor.color = ItemSpritesSaver.Instance.GetOneStarColor();
                    break;
            }
        }

        if (topFrameImage != null)
        {
            // 등급에 따라서 이미지 변경
            switch (itemCls.GetGrade())
            {
                case 5:
                    topFrameImage.sprite = ItemSpritesSaver.Instance.GradationSprite[0];
                    break;
                case 4:
                    topFrameImage.sprite = ItemSpritesSaver.Instance.GradationSprite[1];
                    break;
                case 3:
                    topFrameImage.sprite = ItemSpritesSaver.Instance.GradationSprite[2];
                    break;
                default:
                    topFrameImage.sprite = ItemSpritesSaver.Instance.GradationSprite[3];
                    break;
            }
        }
    }

    public static void SelectButtonToDefault(SelectButtonScript cls)
    {
        cls.SetItemSprite(null);
        cls.GetItemImage().enabled = false;
        cls.SetIsActive(false);
        cls.SetItemColor(1);
        cls.SetItemText("-");
        cls.SetItemCls(null);
    }

    public static void ScrollObjectOffButton(GameObject obj, Button clickedButton)
    {
        clickedButton.gameObject.SetActive(false);
        obj.gameObject.SetActive(false);
    }

    public static void NeededCostForUpgrade(int nExp, int needExp, ref int needCost, TextMeshProUGUI needCostTextUI, TextMeshProUGUI haveCostTextUI, Image fillAmountImage=null, Color baseColor = default)
    {
        int offset = Mathf.Min(nExp, needExp);
        needCostTextUI.text = (offset * 100).ToString();
        needCost = offset * 100;
        haveCostTextUI.text = GameManager.Instance.GetUserClass().GetMora().ToString();

        var weaponCls = GameManager.Instance.GetUserClass().GetUserEquippedWeapon() as WeaponAndEquipCls;

        if(fillAmountImage !=null)  // 이미지가 있을 경우에,
        {
            Color color = fillAmountImage.color; // 현재 색상 정보 가져오기
            color.a = 0.5f; // 알파 값을 0.5로 설정
            fillAmountImage.color = color; // 변경된 색상 정보 설정
            Debug.Log((weaponCls.GetCurrentExp() + nExp) + "weaponCls.GetMaxExp() : " + weaponCls.GetMaxExp());
            float nextExp = (float)(weaponCls.GetCurrentExp() + nExp) / (float)weaponCls.GetMaxExp();
            fillAmountImage.fillAmount = nextExp;
            Debug.Log("fillamount : "+ nextExp);
        }

        int mora = GameManager.Instance.GetUserClass().GetMora();
        if (mora <= needCost)
            needCostTextUI.color = Color.red;
        else
            needCostTextUI.color = baseColor == default ? Color.white : baseColor;
    }
    public static void ExpBarAlphaRevisePerpect(Image img_ExpBar, WeaponAndEquipCls weaponCls)
    {
        img_ExpBar.fillAmount = (float)weaponCls.GetCurrentExp() / (float)weaponCls.GetMaxExp();
        Color colorTmp = img_ExpBar.color;
        colorTmp.a = 1f;
        img_ExpBar.color = colorTmp;
    }

}
