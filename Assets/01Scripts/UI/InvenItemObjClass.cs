using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvenItemObjClass : MonoBehaviour
{
    #region 변수
    Image img_TopBgrImg;                // 백그라운드 이미지
    Image img_TopSpriteImg;             // 아이템 이미지
    Image img_SelectExpress;            // 선택된 아이템 표시
    Image img_EquippedImg;              // 장비중인 아이템 표시
    TextMeshProUGUI txt_BottomText;     // 하단 텍스트
    Button button;

    ItemClass itemCls;                  // 오브젝트가 저장하는 아이템
    bool isActive;                      // obj가 엑티브 상태인지 체크
    bool isClicked;                     // 클릭 상태인지 체크



    #endregion



    public UnityEngine.Events.UnityEvent onPressed;

    private void Awake()
    {
        img_TopBgrImg = gameObject.transform.GetChild(0).GetComponent<Image>();
        img_TopSpriteImg = img_TopBgrImg.transform.GetChild(1).GetComponent<Image>();
        img_EquippedImg = img_TopBgrImg.transform.GetChild(2).GetComponent<Image>();
        txt_BottomText = gameObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        img_SelectExpress = gameObject.transform.GetChild(2).GetComponent<Image>();

        button = gameObject.GetComponentInChildren<Button>();
        // 버튼 클릭 이벤트에 대한 메서드 등록
        button.onClick.AddListener(OnClick);
    }

    private void OnEnable()
    {
        // 활성화 될 때마다, 변수 초기화
        isActive = false;
        isClicked = false;
        img_SelectExpress.enabled = false;
        img_EquippedImg.enabled = false;
    }

    private void OnDisable()
    {
        itemCls = null;
    }

    private void OnClick()
    {

        OnClickEventer();
        ClickedUIApply();
        onPressed.Invoke();
    }

    public void OnClickEventer()
    {
        if (!isClicked)  // 클릭된 상태가 아닐 경우.
        {
            // 클릭된 객체의 데이터를 전송
            UI_Manager.Instance.ClickedItemNotifyed(itemCls, this);
        }
        else            // 클릭된 상태일 경우.
        {
            // 프레임 리셋.
            UI_Manager.Instance.ExpressFrameReset();
        }

    }

    public void ClickedUIApply()
    {
        isClicked = !isClicked;

        switch(isClicked)
        {
            case true:
                img_SelectExpress.enabled= true;
                break;
            case false:
                img_SelectExpress.enabled = false;
                break;
        }
    }

    public void EquippedItemUIPrint(bool isActive)
    {
        if (img_EquippedImg != null)
        {
            img_EquippedImg.enabled = isActive;

            // 부모 객체의 RectTransform을 가져옵니다.
            RectTransform parentRect = transform.parent.GetComponent<RectTransform>();

            if (isActive)
                img_EquippedImg.rectTransform.localScale = new Vector3(0.3f, 0.3f, 1.0f);
        }
    }

    // 아이템 등급에 따라서 색상 변경
    public void SetItemColor(int grade)
    {
        switch(grade)
        {
            case 5:
                img_TopBgrImg.color = ItemSpritesSaver.Instance.GetFiveStarColor();
                break;
            case 4:
                img_TopBgrImg.color = ItemSpritesSaver.Instance.GetFourStarColor();
                break;
            case 3:
                img_TopBgrImg.color = ItemSpritesSaver.Instance.GetThreeStarColor();
                break;
            default:
                img_TopBgrImg.color = ItemSpritesSaver.Instance.GetOneStarColor();
                break;
        }
    }

    // 아이템 스프라이트 설정
    public void SetItemSprite(Sprite sprite){ img_TopSpriteImg.sprite = sprite; }
    // 아이템 텍스트 설정
    public void SetItemText(string sText) { txt_BottomText.text = sText; }
    public void SetItemcls(ItemClass itemCls) { this.itemCls = itemCls; }
    public void SetIsActive(bool isActive) { this.isActive = isActive; }
    public bool GetButtonClicked() { return isClicked; }    // 버튼 클릭 여부값 반환
    public bool GetIsActive() { return isActive; }
    public Button GetButton() { return button; }
    public ItemClass GetItemcls() { return itemCls; }
    public Sprite GetItemSprite() { return img_TopSpriteImg.sprite; }
    public Image GetTopItemImage() { return img_TopSpriteImg; }
    public InvenItemObjClass GetInvenItemObjClass() { return this; }
    public void SetSelectBtnSpriteOn(bool isActive){ img_SelectExpress.enabled=isActive;}

}   
