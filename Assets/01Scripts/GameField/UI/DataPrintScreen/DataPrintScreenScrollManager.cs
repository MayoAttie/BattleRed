using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UI_UseToolClass;
public class DataPrintScreenScrollManager : Singleton<DataPrintScreenScrollManager>
{
    GameObject mianObject;
    Transform contentTransform;
    TextMeshProUGUI label_text;
    Dictionary<ItemClass, SelectButtonScript> dic_uiAnditem = new Dictionary<ItemClass,SelectButtonScript>();
    List<ItemClass> itemList = new List<ItemClass>();
    bool isPrintSkip = false;

    private void Awake()
    {
        mianObject = gameObject.transform.GetChild(0).gameObject;
        contentTransform = mianObject.transform.GetChild(1).GetChild(0).GetComponent<Transform>();
        label_text = mianObject.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        mianObject.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => SkipButtonEvent());
    }
    private void Start()
    {
        mianObject.SetActive(false);
    }
    #region 획득 아이템 출력
    // 아이템 출력
    public void GetItemPrint(ItemClass item)
    {
        if (mianObject.activeSelf == false)
            mianObject.SetActive(true);

        var obj = GameManager.Instance.SelectButtonScriptPool.GetFromPool(Vector3.zero, Quaternion.identity, contentTransform);
        ResetToSelectButton(obj);

        float newZ = 10f; // 예시로 10으로 설정
        obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, newZ);

        if (!isPrintSkip)   // 스킵중이 아니라면, 이팩트 효과 부여
            EffectManager.Instance.UI_ParticleCreate(obj.transform, 2);

        string _text = "아이템 획득 !! " + item.GetName() + " " + item.GetNumber().ToString() + "개";
        label_text.text = _text;


        obj.SetItemCls(item);
        obj.SetItemColor(item.GetGrade());
        obj.SetItemText(item.GetName());

        if (item.GetTag() == "무기")
        {
            WeaponKindDivider(item as WeaponAndEquipCls, obj.GetItemImage());
        }
        else if (item.GetTag() == "꽃" || item.GetTag() == "성배" || item.GetTag() == "왕관" || item.GetTag() == "모래" || item.GetTag() == "깃털")
        {
            EquipmentKindDivider(item as WeaponAndEquipCls, obj.GetItemImage());
        }
        else if (item.GetTag() == "광물")
        {
            GemKindDivider(item, obj.GetItemImage());
        }
        else if (item.GetTag() == "음식")
        {
            FoodKindDivider(item, obj.GetItemImage());
        }
        else if (item.GetTag() == "육성 아이템")
        {
            obj.SetItemSprite(WeaponAndEquipLimitBreak_UI_Dvider(item));
        }
        else if(item.GetTag() == "기타")
        {
            EtcKindDivider(item, obj.GetItemImage());
        }
        obj.SetIsActive(true);
        obj.GetItemImage().enabled = true;
        dic_uiAnditem.Add(item, obj);

        if (itemList == null || itemList.Count <= 0)
            StartCoroutine(OffCanvasAndDataReset());
    }
    // 오버로딩을 통한 리스트 데이터 출력
    public void GetItemPrint(List<ItemClass> itemList)
    {
        this.itemList = itemList;
        mianObject.SetActive(true);
        StartCoroutine(SequentialOutput());
    }
    // 시간차를 두고, 아이템 데이터 출력
    IEnumerator SequentialOutput()
    {
        float interval = 1f;
        for (int i = 0; i < itemList.Count; i++)
        {
            if (isPrintSkip)
                interval = 0.05f;
            else
                interval = 1f;


            GetItemPrint(itemList[i]);
            yield return new WaitForSeconds(interval);
        }
        StartCoroutine(OffCanvasAndDataReset());
    }
    #endregion

    // 데이터 리셋 및 객체 Off
    IEnumerator OffCanvasAndDataReset()
    {
        float time = 0;
        if (isPrintSkip)
            time = 1f;
        else
            time = 2f;

        yield return new WaitForSeconds(time);
     
        if(dic_uiAnditem.Count>0)
        {
            foreach (var tmp in dic_uiAnditem)
            {
                ResetToSelectButton(tmp.Value);
                GameManager.Instance.SelectButtonScriptPool.ReturnToPool(tmp.Value);
            }
            dic_uiAnditem.Clear();
        }
        isPrintSkip = false;
        mianObject.SetActive(false);
    }
    // 스킵버튼 연결
    void SkipButtonEvent()
    {
        isPrintSkip = true;
    }


    public void JustTextPrint(string printText)
    {
        mianObject.SetActive(true);
        label_text.text = printText;
        StartCoroutine(OffCanvasAndDataReset());
    }

    public GameObject GetReturnToMainObject() { return mianObject; }
    public Transform GetReturnToContentScroll() { return contentTransform; }
}
