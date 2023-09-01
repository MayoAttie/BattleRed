using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvenItemObjClass : MonoBehaviour
{
    Image img_TopBgrImg;                // 백그라운드 이미지
    Image img_TopSpriteImg;             // 아이템 이미지
    Image img_SelectExpress;            // 선택된 아이템 표시
    TextMeshProUGUI txt_BottomText;     // 하단 텍스트
    Button button;

    Color FiveStarColor = new Color(177/255, 115/255, 43/255);
    Color FourStarColor = new Color(138/255, 107/255, 170/255);
    Color ThreeStarColor = new Color(86 / 255, 131 / 255, 164 / 255);
    Color OneStarColor = new Color(108 / 255, 105 / 255, 108 / 255);


    bool isClicked;

    public UnityEngine.Events.UnityEvent onPressed;

    private void Awake()
    {
        isClicked = true;
        img_TopBgrImg = gameObject.transform.GetChild(0).GetComponent<Image>();
        img_TopSpriteImg = img_TopBgrImg.transform.GetChild(1).GetComponent<Image>();
        txt_BottomText = gameObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        img_SelectExpress = gameObject.transform.GetChild(2).GetComponent<Image>();

        img_SelectExpress.enabled = false;

        button = gameObject.GetComponentInChildren<Button>();
        // 버튼 클릭 이벤트에 대한 메서드 등록
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        ClickedUIApply();
        onPressed.Invoke();
    }

    void Start()
    {
        
    }

    void ClickedUIApply()
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


}
