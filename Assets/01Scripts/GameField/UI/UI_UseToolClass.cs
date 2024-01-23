using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UI_Manager;
using static CharacterUpgradeManager;
using System.Linq;
using static GameManager;

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

        List<WeaponAndEquipCls> weaponActiveList = new List<WeaponAndEquipCls>();
        List<WeaponAndEquipCls> weaponInActiveList = new List<WeaponAndEquipCls>();

        if (selected_SortOrder == e_SortingOrder.ExpOrder)
        {
            weaponActiveList = activeItems
                            .Where(item => item is WeaponAndEquipCls)
                            .Select(item => (WeaponAndEquipCls)item)
                            .ToList();

            weaponInActiveList = inactiveItems
                            .Where(item => item is WeaponAndEquipCls)
                            .Select(item => (WeaponAndEquipCls)item)
                            .ToList();
        }



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
            case e_SortingOrder.ExpOrder:
                if (isAscending)
                    weaponActiveList.Sort((item1, item2) => item1.GetCurrentExp().CompareTo(item2.GetCurrentExp()));
                else
                    weaponActiveList.Sort((item1, item2) => item2.GetCurrentExp().CompareTo(item1.GetCurrentExp()));
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
            case e_SortingOrder.ExpOrder:
                if (isAscending)
                    weaponInActiveList.Sort((item1, item2) => item1.GetCurrentExp().CompareTo(item2.GetCurrentExp()));
                else
                    weaponInActiveList.Sort((item1, item2) => item2.GetCurrentExp().CompareTo(item1.GetCurrentExp()));
                break;
            default:
                break;
        }

        // 두 그룹을 병합합니다.
        if(selected_SortOrder != e_SortingOrder.ExpOrder)
        {
            clsList.Clear();
            clsList.AddRange(activeItems);
            clsList.AddRange(inactiveItems);
        }
        else
        {
            clsList.Clear();
            clsList.AddRange(weaponActiveList);
            clsList.AddRange(weaponInActiveList);
        }
    }
    // 타입에 따라 분기하여, 오브젝트 풀 데이터를 리셋해주는 함수
    public static void ResetToWeaponItemObjectPoolDatas(e_PoolItemType type)
    {
        List<InvenItemObjClass> objList = new List<InvenItemObjClass>();
        // 오브젝트 풀로 가져온 각 버튼에 Button이벤트를 Remove함
        switch (type)
        {
            case e_PoolItemType.Weapon:
                objList = GameManager.Instance.WeaponItemPool.GetPoolList();
                break;
            case e_PoolItemType.Equip:
                objList = GameManager.Instance.EquipItemPool.GetPoolList();
                break;
            case e_PoolItemType.Gem:
                objList = GameManager.Instance.GemItemPool.GetPoolList();
                break;
            case e_PoolItemType.Food:
                objList = GameManager.Instance.FoodItemPool.GetPoolList();
                break;

        }
        foreach (var obj in objList)
        {
            obj.SetIsActive(false);
            
            // 알파값 최대
            Color aa = obj.GetTopItemImage().color;
            aa.a = 1.0f;
            obj.GetTopItemImage().color = aa;
            
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
        // UI 오브젝트풀 리턴
        switch (type)
        {
            case e_PoolItemType.Weapon:
                GameManager.Instance.WeaponItemPool.AllReturnToPool();
                break;
            case e_PoolItemType.Equip:
                GameManager.Instance.EquipItemPool.AllReturnToPool();
                break;
            case e_PoolItemType.Gem:
                GameManager.Instance.GemItemPool.AllReturnToPool();
                break;
            case e_PoolItemType.Food:
                GameManager.Instance.FoodItemPool.AllReturnToPool();
                break;
        }
    }
    public static void ResetIndividualObject(InvenItemObjClass obj)
    {
        if (obj != null)
        {
            obj.SetIsActive(false);

            // 알파값 최대
            Color aa = obj.GetTopItemImage().color;
            aa.a = 1.0f;
            obj.GetTopItemImage().color = aa;

            Button btn = obj.GetButton();
            if (btn != null)
            {
                // 모든 이벤트 리스너를 제거하고, 기존의 이벤트 리스너를 다시 부착한다.
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => obj.OnClickEventer());
                btn.onClick.AddListener(() => obj.ClickedUIApply());
            }
        }
    }
    public static void ResetToSelectButton(SelectButtonScript btnObj)
    {
        Button btn = btnObj.GetButton();
        // 액티브는 비활성화
        SelectButtonToDefault(btnObj);
        if (btn != null)
        {
            // 모든 이벤트 리스너를 제거하고, 기존의 이벤트 리스너를 다시 부착한다.
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => btnObj.OnClickEventListener());
        }
        
    }

    // 게임매니저의 데이터를 참조하여, 무기들을 스크롤뷰 콘텐츠에 출력 <ObjectPool<InvenItemObjClass> WeaponItemPool <- 웨폰 데이터 출력>
    public static void WeaponPrintAtScroll(Transform content, e_SortingOrder selected_SortOrder, bool isAscending, List<InvenItemObjClass> openUI_ItemList, List<InvenItemObjClass> listObj = null)
    {
        var itemClses = GameManager.Instance.GetUserClass().GetHadWeaponList();      // 저장된 아이템 목록
        openUI_ItemList.Clear();
        SortingItemList(itemClses, selected_SortOrder, isAscending);

        List<InvenItemObjClass> datas = null;
        // 오브젝트 풀로, UI객체 생성
        if (listObj == null)
        {
            GameManager.Instance.ItemToObjPool(itemClses.Count, GameManager.e_PoolItemType.Weapon, content);
            // 오브젝트 풀에 저장된 리스트 인스턴스화
            datas = GameManager.Instance.WeaponItemPool.GetPoolList();
        }
        else
            datas = listObj;



        foreach (ItemClass data in itemClses)
        {
            foreach (InvenItemObjClass obj in datas)
            {
                if (obj.gameObject.activeSelf == false || obj.GetIsActive() == true)
                    continue;

                var reData = data as WeaponAndEquipCls;
                WeaponKindDivider(reData, obj.GetTopItemImage());

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
    public static void EquipPrintAtScroll(Transform content, e_SortingOrder selected_SortOrder, bool isAscending, List<InvenItemObjClass> openUI_ItemList = null, List<InvenItemObjClass> listObj = null)
    {
        var itemClses = GameManager.Instance.GetUserClass().GetHadEquipmentList();      // 저장된 아이템 목록

        SortingItemList(itemClses, selected_SortOrder, isAscending);


        List<InvenItemObjClass> datas = null;
        // 오브젝트 풀로, UI객체 생성
        if (listObj == null)
        {
            GameManager.Instance.ItemToObjPool(itemClses.Count, GameManager.e_PoolItemType.Equip, content);
            // 오브젝트 풀에 저장된 리스트 인스턴스화
            datas = GameManager.Instance.EquipItemPool.GetPoolList();
        }
        else
            datas = listObj;


        foreach (ItemClass data in itemClses)
        {
            foreach (InvenItemObjClass obj in datas)
            {
                if (obj.gameObject.activeSelf == false || obj.GetIsActive() == true)
                    continue;

                var reData = data as WeaponAndEquipCls;

                EquipmentKindDivider(reData, obj.GetTopItemImage());

                #region 레거시

                //if (data.GetName() == "이국의 술잔")
                //    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[0]);
                //else if (data.GetName() == "귀향의 깃털")
                //    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[1]);
                //else if (data.GetName() == "이별의 모자")
                //    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[2]);
                //else if (data.GetName() == "옛 벗의 마음")
                //    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[3]);
                //else if (data.GetName() == "빛을 좆는 돌")
                //    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[4]);   

                //else if (data.GetName() == "전투광의 해골잔")
                //    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[5]);
                //else if (data.GetName() == "전투광의 깃털")
                //    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[6]);
                //else if (data.GetName() == "전투광의 귀면")
                //    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[7]);
                //else if (data.GetName() == "전투광의 장미")
                //    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[8]);
                //else if (data.GetName() == "전투광의 시계")
                //    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[9]);

                //else if (data.GetName() == "피에 물든 기사의 술잔")
                //    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[10]);
                //else if (data.GetName() == "피에 물든 검은 깃털")
                //    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[11]);
                //else if (data.GetName() == "피에 물든 철가면")
                //    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[12]);
                //else if (data.GetName() == "피에 물든 강철 심장")
                //    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[13]);
                //else if (data.GetName() == "피에 물든 기사의 시계")
                //    obj.SetItemSprite(ItemSpritesSaver.Instance.EquipSprites[14]);
                //else { break; }
                #endregion

                if (data.GetIsActive() == true) // 아이템이 활성 상태라면, 사용중임을 알림.
                    obj.EquippedItemUIPrint(true);

                obj.SetItemColor(data.GetGrade());
                obj.SetItemText("LV : " + data.GetLevel().ToString());
                obj.SetIsActive(true);
                obj.SetItemcls(data);
                if(openUI_ItemList!= null)
                    openUI_ItemList.Add(obj);
                break;
            }
        }
    }

    public static void EtcPrintAtScroll(Transform content, e_SortingOrder selected_SortOrder, bool isAscending, List<InvenItemObjClass> openUI_ItemList = null, List<InvenItemObjClass> listObj = null)
    {
        var itemClses = GameManager.Instance.GetUserClass().GetHadEtcItemList();  // 저장된 아이템 목록
        List<ItemClass> fullList = new List<ItemClass>(itemClses);  // itemClses의 복사본 생성
        fullList.AddRange(GameManager.Instance.GetUserClass().GetHadGrowMaterialList());  // 성장재료 추가

        SortingItemList(fullList, selected_SortOrder, isAscending);

        List<InvenItemObjClass> datas = null;
        // 오브젝트 풀로, UI객체 생성
        if (listObj == null)
        {
            GameManager.Instance.ItemToObjPool(fullList.Count, GameManager.e_PoolItemType.Food, content);      // Food의 UI, Pool을 사용
            // 오브젝트 풀에 저장된 리스트 인스턴스화
            datas = GameManager.Instance.FoodItemPool.GetPoolList();
        }
        else
            datas = listObj;


        foreach (ItemClass data in fullList)
        {
            foreach (InvenItemObjClass obj in datas)
            {
                if (obj.gameObject.activeSelf == false || obj.GetIsActive() == true)
                    continue;

                if (data.GetTag() == "육성 아이템")
                    obj.GetTopItemImage().sprite = WeaponAndEquipLimitBreak_UI_Dvider(data);
                else
                    EtcKindDivider(data, obj.GetTopItemImage());


                obj.SetItemColor(data.GetGrade());
                obj.SetItemText(data.GetName());
                obj.SetIsActive(true);
                obj.SetItemcls(data);
                if (openUI_ItemList != null)
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

                var reData = data as WeaponAndEquipCls;
                WeaponKindDivider(reData, obj.GetItemImage());


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
    // 성유물에 따라 달라지는 장비 데이터 출력을 담당하는 함수
    public static void EquipmentKindDivider(WeaponAndEquipCls weaponCls, Image equipImage = null, TextMeshProUGUI equipStatusLabelTxt = null, TextMeshProUGUI equipStatusTxt = null)
    {
        string reDataText = weaponCls.GetEffectText(); // reData.GetEffectText()로부터 문자열 가져오기
        string pivot = ""; // pivot 문자열 초기화
        if (reDataText.Length > 7) // 8번째 값부터 끝까지의 부분을 pivot에 저장
            pivot = reDataText.Substring(7);

        // 장비 이름에 따라 데이터 분기

        // 아이템 이미지 출력
        if(equipImage!= null)
        {
            switch (weaponCls.GetName())
            {
                case "이국의 술잔":
                    equipImage.sprite = ItemSpritesSaver.Instance.EquipSprites[0];
                    break;
                case "귀향의 깃털":
                    equipImage.sprite = ItemSpritesSaver.Instance.EquipSprites[1];
                    break;
                case "이별의 모자":
                    equipImage.sprite = ItemSpritesSaver.Instance.EquipSprites[2];
                    break;
                case "옛 벗의 마음":
                    equipImage.sprite = ItemSpritesSaver.Instance.EquipSprites[3];
                    break;
                case "빛을 좆는 돌":
                    equipImage.sprite = ItemSpritesSaver.Instance.EquipSprites[4];
                    break;
                case "전투광의 해골잔":
                    equipImage.sprite = ItemSpritesSaver.Instance.EquipSprites[5];
                    break;
                case "전투광의 깃털":
                    equipImage.sprite = ItemSpritesSaver.Instance.EquipSprites[6];
                    break;
                case "전투광의 귀면":
                    equipImage.sprite = ItemSpritesSaver.Instance.EquipSprites[7];
                    break;
                case "전투광의 장미":
                    equipImage.sprite = ItemSpritesSaver.Instance.EquipSprites[8];
                    break;
                case "전투광의 시계":
                    equipImage.sprite = ItemSpritesSaver.Instance.EquipSprites[9];
                    break;
                case "피에 물든 기사의 술잔":
                    equipImage.sprite = ItemSpritesSaver.Instance.EquipSprites[10];
                    break;
                case "피에 물든 검은 깃털":
                    equipImage.sprite = ItemSpritesSaver.Instance.EquipSprites[11];
                    break;
                case "피에 물든 철가면":
                    equipImage.sprite = ItemSpritesSaver.Instance.EquipSprites[12];
                    break;
                case "피에 물든 강철 심장":
                    equipImage.sprite = ItemSpritesSaver.Instance.EquipSprites[13];
                    break;
                case "피에 물든 기사의 시계":
                    equipImage.sprite = ItemSpritesSaver.Instance.EquipSprites[14];
                    break;
                default:break;
            }
        }

        // 스탯 라벨 출력
        if (equipStatusLabelTxt != null)
        {
            if (weaponCls.GetTag() == "꽃")
                equipStatusLabelTxt.text = "체력";
            else if (weaponCls.GetTag() == "깃털")
                equipStatusLabelTxt.text = "공격력";
            else
                equipStatusLabelTxt.text = pivot;
        }

        if(equipStatusTxt!= null)   // 스탯 표기 텍스트 출력
        {
            if(weaponCls.GetTag()=="꽃" || weaponCls.GetTag() == "깃털")
                equipStatusTxt.text = weaponCls.GetMainStat().ToString();
            else
            {
                switch (pivot)
                {
                    case "공격력":
                        equipStatusTxt.text = weaponCls.GetMainStat().ToString()+"%";
                        break;
                    case "방어력":
                        equipStatusTxt.text = weaponCls.GetMainStat().ToString() + "%";
                        break;
                    case "체력":
                        equipStatusTxt.text = weaponCls.GetMainStat().ToString() + "%";
                        break;
                    case "원소 마스터리":
                        equipStatusTxt.text = weaponCls.GetMainStat().ToString();
                        break;
                    case "치명타 확률":
                        equipStatusTxt.text = weaponCls.GetMainStat().ToString() + " %";
                        break;
                    case "치명타 피해":
                        equipStatusTxt.text = weaponCls.GetMainStat().ToString();
                        break;
                    default: break;
                }
            }

        }
    }

    public static void GemKindDivider(ItemClass itemCls, Image image)
    {
        // 아이템 이름과 해당하는 이미지의 인덱스 매핑
        Dictionary<string, int> itemNameToSpriteIndex = new Dictionary<string, int>
        {
            { "철광석", 0 },
            { "백철", 1 },
            { "수정덩이", 2 },
            { "정제용 하급 광물", 3 },
            { "정제용 광물", 4 },
            { "정제용 마법 광물", 5 },
            { "야박석", 6 },
            { "전기수정", 7 },
            { "콜라피스", 8 },
        };

        string itemName = itemCls.GetName();

        // 매핑된 아이템이 있다면 해당 이미지를 할당하고 종료
        if (itemNameToSpriteIndex.TryGetValue(itemName, out int spriteIndex))
        {
            image.sprite = ItemSpritesSaver.Instance.GemSprites[spriteIndex];
            return;
        }

        Debug.LogWarning($"Unhandled item name: {itemName}");
    }

    public static void FoodKindDivider(ItemClass itemCls, Image image)
    {
        // 아이템 이름과 해당하는 이미지의 인덱스 매핑
        Dictionary<string, int> itemNameToSpriteIndex = new Dictionary<string, int>
        {
            { "어부 토스트", 0 },
            { "버섯닭꼬치", 1 },
            { "달콤달콤 닭고기 스튜", 2 },
            { "냉채수육", 3 },
            { "허니캐럿그릴", 4 },
            { "스테이크", 5 },
            { "무스프", 6 },
            { "몬드 감자전", 7 },
            { "방랑자의 경험", 8 },
            { "모험자의 경험", 9 },
            { "영웅의 경험", 10 },
        };

        string itemName = itemCls.GetName();

        // 매핑된 아이템이 있다면 해당 이미지를 할당하고 종료
        if (itemNameToSpriteIndex.TryGetValue(itemName, out int spriteIndex))
        {
            image.sprite = ItemSpritesSaver.Instance.FoodSprites[spriteIndex];
            return;
        }

        Debug.LogWarning($"Unhandled item name: {itemName}");
    }

    public static void EtcKindDivider(ItemClass itemCls, Image image)
    {
        // 아이템 이름과 해당하는 이미지의 인덱스 매핑
        Dictionary<string, int> itemNameToSpriteIndex = new Dictionary<string, int>
        {
            { "열쇠", 0 },
        };

        string itemName = itemCls.GetName();

        // 매핑된 아이템이 있다면 해당 이미지를 할당하고 종료
        if (itemNameToSpriteIndex.TryGetValue(itemName, out int spriteIndex))
        {
            image.sprite = ItemSpritesSaver.Instance.EtcSprite[spriteIndex];
            return;
        }

    }

    // 개발자가 입력한 데이터에 따라, 스탯 출력
    public static void WeaponSubStatDivider(WeaponAndEquipCls weaponCls, float subStat, TextMeshProUGUI weaponStatusLabelTxt = null, TextMeshProUGUI weaponStatusTxt = null)
    {
        // 장비 이름에 따라 데이터 분기
        switch (weaponCls.GetName())
        {
            case "천공의 검":
                {

                    if (weaponStatusLabelTxt != null)
                        weaponStatusLabelTxt.text = "원소 충전 효율";

                    if (weaponStatusTxt != null)
                        weaponStatusTxt.text = subStat.ToString() + "%";
                }
                break;
            case "제례검":
                {

                    if (weaponStatusLabelTxt != null)
                        weaponStatusLabelTxt.text = "원소 충전 효율";

                    if (weaponStatusTxt != null)
                        weaponStatusTxt.text = subStat.ToString() + "%";
                }
                break;
            case "여명신검":
                {

                    if (weaponStatusLabelTxt != null)
                        weaponStatusLabelTxt.text = "치명타 피해";

                    if (weaponStatusTxt != null)
                        weaponStatusTxt.text = subStat.ToString() + "%";
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
    public static void SelectButtonToDefault(List<SelectButtonScript>cls)
    {
        foreach(var tmp in cls)
        {
            tmp.SetItemSprite(null);
            tmp.GetItemTxt().color = Color.black;
            tmp.GetItemImage().enabled = false;
            tmp.SetIsActive(false);
            tmp.SetItemColor(1);
            tmp.SetItemText("-");
            tmp.SetItemCls(null);
        }
    }

    public static void SelectButtonToDefault(SelectButtonScript cls)
    {
        cls.SetItemSprite(null);
        cls.GetItemTxt().color = Color.black;
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
        if (needCostTextUI == null || needCostTextUI.gameObject.activeSelf == false) // needCostTextUI 객체 확인 후, null이거나 비활성일 경우 불필요한 함수로 판단 리턴.
            return;

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
            float nextExp = (float)(weaponCls.GetCurrentExp() + nExp) / (float)weaponCls.GetMaxExp();
            fillAmountImage.fillAmount = nextExp;

            //Debug.Log((weaponCls.GetCurrentExp() + nExp) + "weaponCls.GetMaxExp() : " + weaponCls.GetMaxExp());
            //Debug.Log("fillamount : "+ nextExp);
        }

        int mora = GameManager.Instance.GetUserClass().GetMora();
        if (mora <= needCost)
            needCostTextUI.color = Color.red;
        else
            needCostTextUI.color = baseColor == default ? Color.white : baseColor;
    }
    public static void ExpBarAlphaRevisePerpect(Image img_ExpBar, WeaponAndEquipCls weaponCls)
    {
        if (img_ExpBar == null || img_ExpBar.gameObject.activeSelf == false)
            return;

        img_ExpBar.fillAmount = (float)weaponCls.GetCurrentExp() / (float)weaponCls.GetMaxExp();
        Color colorTmp = img_ExpBar.color;
        colorTmp.a = 1f;
        img_ExpBar.color = colorTmp;
    }


    // statImages 0 - 메인스텟, 1 - 서브스텟  __ levelTexts 0 - 레벨, 1 - 경험치
    public static void WeaponLevelUp_UI_Applyer(WeaponAndEquipCls weaponCls, Image[] statImages, TextMeshProUGUI[] levelTexts, Image arrowImg = null)
    {
        // arrowImg가 널일 경우, 레벨업. 아니면, 돌파
        if (arrowImg == null)
        {
            // 메인 스탯 UI 출력
            statImages[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = weaponCls.GetMainStat().ToString(); // 주스탯 표기
            statImages[0].transform.GetChild(2).gameObject.SetActive(false);
            statImages[0].transform.GetChild(3).gameObject.SetActive(false);

            // 서브 스탯 UI 출력
            var subStatLabel = statImages[1].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            var subStatText = statImages[1].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            WeaponKindDivider(weaponCls, null, subStatLabel, subStatText);
            statImages[1].transform.GetChild(2).gameObject.SetActive(false);
            statImages[1].transform.GetChild(3).gameObject.SetActive(false);

            // 레벨 및 exp 출력
            levelTexts[0].text = "LV. " + weaponCls.GetLevel().ToString();
            levelTexts[1].text = Mathf.Floor(weaponCls.GetCurrentExp()).ToString() + "/" + Mathf.Floor(weaponCls.GetMaxExp()).ToString();
        }
        else
        {
            Color nextSpotColor = ItemSpritesSaver.Instance.GetColorAtGarade(weaponCls.GetGrade());
            float[] nextDatas = WeapomLimitBreakStateUp(weaponCls);
            // 메인 스탯 UI 출력
            statImages[0].transform.GetChild(2).gameObject.SetActive(true);
            statImages[0].transform.GetChild(3).gameObject.SetActive(true);
            statImages[0].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = weaponCls.GetMainStat().ToString(); // 기존 주스탯 표기
            statImages[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = nextDatas[0].ToString(); // 다음 주스탯 표기
            statImages[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = nextSpotColor;

            // 서브 스탯 UI 출력
            statImages[1].transform.GetChild(2).gameObject.SetActive(true);
            statImages[1].transform.GetChild(3).gameObject.SetActive(true);
            var subStatLabel = statImages[1].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            var subStatText = statImages[1].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            WeaponKindDivider(weaponCls, null, subStatLabel, subStatText);
            
            var subStatText_2 = statImages[1].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            WeaponSubStatDivider(weaponCls, nextDatas[1], null, subStatText_2); // 돌파 시 다음 데이터

            //레벨 데이터 출력
            levelTexts[0].text = weaponCls.GetLevel().ToString()+"/"+weaponCls.GetLimitLevel().ToString();
            int nextLimitLevel = GameManager.Instance.NextLimitLevelFinder(weaponCls);
            if(nextLimitLevel != -1)    // -1을 반환받을 경우, 최대 돌파임을 나타냄
            {
                levelTexts[1].text = weaponCls.GetLimitLevel().ToString() +"/";
                levelTexts[2].text = nextLimitLevel.ToString();
                levelTexts[2].color = nextSpotColor;
                levelTexts[2].fontStyle = FontStyles.Bold;
            }
            else
            {
                levelTexts[1].text = weaponCls.GetLimitLevel().ToString() + "/"; 
                levelTexts[2].text = weaponCls.GetLimitLevel().ToString();
                levelTexts[2].color = ItemSpritesSaver.Instance.GetColorAtGarade(weaponCls.GetGrade());
                levelTexts[2].fontStyle = FontStyles.Bold;
            }

            arrowImg.enabled = true;

        }
    }
    // 리스트 버튼에 초기 데이터 세팅
    public static void ListButtonObject_UI_Initer(InventorySortSelectButton listBtnObj, string[] strings, e_SortingOrder initOrder)
    {
        listBtnObj.gameObject.SetActive(false);
        listBtnObj.SetButtonsText(strings);                      // 버튼 텍스트 적용
        listBtnObj.StringUIApplyer();                            // 버튼 텍스트 적용
        listBtnObj.SetSortingOrder(initOrder);  
        listBtnObj.HideButtonBackGround();
    }


    public static void PopUpScreenFunc(Transform popObj, string popText)
    {
        popObj.gameObject.SetActive(true);
        TextMeshProUGUI txt = popObj.GetChild(0).GetComponent<TextMeshProUGUI>();
        txt.text = popText;
        var aniMng = popObj.GetComponent<Animator>();
        aniMng.SetTrigger("Start");
    }

    public static Sprite WeaponAndEquipLimitBreak_UI_Dvider(ItemClass cls)
    {
        Sprite img = null;
        switch(cls.GetName())
        {
            case "칼바람 울프의 젖니":
                img = ItemSpritesSaver.Instance.GrowMaterialSprite[12];
                break;
            case "칼바람 울프의 이빨":
                img= ItemSpritesSaver.Instance.GrowMaterialSprite[13];
                break;
            case "칼바람 울프의 부서진 이빨":
                img = ItemSpritesSaver.Instance.GrowMaterialSprite[14];
                break;
            case "지맥의 낡은 가지":
                img = ItemSpritesSaver.Instance.GrowMaterialSprite[6];
                break;
            case "지맥의 마른 잎":
                img = ItemSpritesSaver.Instance.GrowMaterialSprite[7];
                break;
            case "지맥의 새싹":
                img = ItemSpritesSaver.Instance.GrowMaterialSprite[8];
                break;
            case "슬라임 응축액":
                img = ItemSpritesSaver.Instance.GrowMaterialSprite[0];
                break;
            case "슬라임청":
                img = ItemSpritesSaver.Instance.GrowMaterialSprite[1];
                break;
            case "슬라임 원액":
                img = ItemSpritesSaver.Instance.GrowMaterialSprite[2];
                break;
            case "라이언 투사의 족쇄":
                img = ItemSpritesSaver.Instance.GrowMaterialSprite[16];
                break;
            case "라이언 투사의 쇠사슬":
                img = ItemSpritesSaver.Instance.GrowMaterialSprite[17];
                break;
            case "라이언 투사의 수갑":
                img = ItemSpritesSaver.Instance.GrowMaterialSprite[18];
                break;
            case "혼돈의 장치":
                img = ItemSpritesSaver.Instance.GrowMaterialSprite[9];
                break;
            case "혼돈의 회로":
                img = ItemSpritesSaver.Instance.GrowMaterialSprite[10];
                break;
            case "혼돈의 노심":
                img = ItemSpritesSaver.Instance.GrowMaterialSprite[11];
                break;
            case "이능 두루마리":
                img = ItemSpritesSaver.Instance.GrowMaterialSprite[3];
                break;
            case "봉마의 두루마리":
                img = ItemSpritesSaver.Instance.GrowMaterialSprite[4];
                break;
            case "금주의 두루마리":
                img = ItemSpritesSaver.Instance.GrowMaterialSprite[5];
                break;
        }
        return img;
    }
    public static void ButtonClass2_Reset(ButtonClass2 btnCls)
    {
        btnCls.GetButton().onClick.AddListener(btnCls.OnClick);
        btnCls.GetButton().onClick.AddListener(btnCls.OnButtonDown);
        btnCls.GetButton().onClick.AddListener(btnCls.OnButtonUp);
    }
    public static void ButtonClass_Reset(ButtonClass btnCls)
    {
        btnCls.GetButton().onClick.AddListener(btnCls.OnClick);
        btnCls.GetButton().onClick.AddListener(btnCls.OnButtonDown);
        btnCls.GetButton().onClick.AddListener(btnCls.OnButtonUp);
    }

    /// <summary>
    /// 성유물 UI tools
    /// 
    /// 
    /// </summary>

    // 성유물 아이템들의 스탯을 리턴 - hp, atk, def, element, criRate, criDamage 순서로 반환
    public static float[] ReturnEquipmentStatSum(ItemClass[] EquipmentList, Dictionary<string, int> setFinder)
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
                            criticalRate += vlaue;
                        }
                        break;
                    case "치명타 피해":
                        {
                            float value = reData.GetMainStat();
                            criticalDamage += value;
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

        float[] answers = { hp, atk, def, element, criticalRate, criticalDamage };
        return answers;

    }

    // 성유물 데이터에 따라 성유물 스탯 수정
    public static void ApplyEquipmentEffect(GameManager.EQUIPMENT_SET_SYNERGY_DATA_BASE dbItem, ref int atk, ref int def, ref int hp, ref int element)
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
        //Debug.Log("pluser :: => " + pluser);
    }
    public static void ApplyEquipmentEffect(GameManager.EQUIPMENT_SET_SYNERGY_DATA_BASE dbItem, ref int atk, ref int def, ref int hp, ref int element, ref float citicalRate, ref float criticalDamage, ref float elementCharge, ref float normalAtkDamgage, ref float skillAtkDamage, ref float damageIncrease)
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
        else if (dbItem.EQUIPMENT_SET_EFFECT_ELEMENT.Equals("원소 충전"))
            elementCharge += dbItem.EQUIPMENT_SET_EFFECT_VALUE;
        else if (dbItem.EQUIPMENT_SET_EFFECT_ELEMENT.Equals("일반 공격"))
            normalAtkDamgage += dbItem.EQUIPMENT_SET_EFFECT_VALUE;
        else if (dbItem.EQUIPMENT_SET_EFFECT_ELEMENT.Equals("스킬 공격"))
            skillAtkDamage += dbItem.EQUIPMENT_SET_EFFECT_VALUE;
        else if (dbItem.EQUIPMENT_SET_EFFECT_ELEMENT.Equals("피해량"))
            damageIncrease += dbItem.EQUIPMENT_SET_EFFECT_VALUE;
    }

    // 정렬 리스트 버튼, 기능 함수 => (희귀도, 레벨, 기초경험치)
    public static void SortOrderForUpgradeResorce(InventorySortSelectButton selectBtnListObj, string index, ButtonClass2 selectBtn, e_PoolItemType type, ref e_SortingOrder order)
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
        order = tmp;
        Debug.Log("allPull_OrderValue" + " == " + order);

        selectBtnListObj.HideButtonBackGround(indexNum);
        selectBtnListObj.gameObject.SetActive(false);  // 비활성화
        selectBtn.SetButtonTextInputter(index);

        ResetToWeaponItemObjectPoolDatas(type);
    }
    public static void SortOrderForChange(InventorySortSelectButton selectBtnListObj, string index, ButtonClass2 selectBtn, e_PoolItemType type, ref e_SortingOrder order)
    {
        e_SortingOrder tmp = e_SortingOrder.ExpOrder;
        int indexNum = 0;
        if (index == "이름")
        {
            tmp = e_SortingOrder.NameOrder;
            indexNum = 0;
        }
        else if (index == "레벨")
        {
            tmp = e_SortingOrder.LevelOrder;
            indexNum = 1;
        }
        else if (index == "희귀도")
        {
            tmp = e_SortingOrder.GradeOrder;
            indexNum = 2;

        }
        selectBtnListObj.SetSortingOrder(tmp);
        order = tmp;
        Debug.Log("allPull_OrderValue" + " == " + order);

        selectBtnListObj.HideButtonBackGround(indexNum);
        selectBtnListObj.gameObject.SetActive(false);  // 비활성화
        selectBtn.SetButtonTextInputter(index);

        ResetToWeaponItemObjectPoolDatas(type);
    }

}

