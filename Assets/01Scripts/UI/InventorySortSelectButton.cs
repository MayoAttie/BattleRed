using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class InventorySortSelectButton : MonoBehaviour
{
    Transform Button1;
    Transform Button2;
    Transform Button3;

    // 버튼의 백그라운드 이미지들
    Image[] btn1_Images;
    Image[] btn2_Images;
    Image[] btn3_Images;

    // 버튼 텍스트 객체
    TextMeshProUGUI btn1_Text;
    TextMeshProUGUI btn2_Text;
    TextMeshProUGUI btn3_Text;

    // 버튼에 적용될 문자열
    string btn1_InputTxt;
    string btn2_InputTxt;
    string btn3_InputTxt;


    // 선택된 정렬순위
    UI_Manager.e_SortingOrder order;

    private void Awake()
    {
        Button1 = gameObject.transform.GetChild(3).GetComponent<Transform>();
        Button2 = gameObject.transform.GetChild(4).GetComponent<Transform>();
        Button3 = gameObject.transform.GetChild(5).GetComponent<Transform>();

        btn1_Images = Button1.GetComponentsInChildren<Image>();
        btn2_Images = Button2.GetComponentsInChildren<Image>();
        btn3_Images = Button3.GetComponentsInChildren<Image>();

        btn1_Text = Button1.GetComponentInChildren<TextMeshProUGUI>();
        btn2_Text = Button2.GetComponentInChildren<TextMeshProUGUI>();
        btn3_Text = Button3.GetComponentInChildren<TextMeshProUGUI>();
        
        order = UI_Manager.e_SortingOrder.GradeOrder;
        btn3_InputTxt = "희귀도";
        btn2_InputTxt = "레벨";
        btn1_InputTxt = "이름";
            


    }
    private void OnEnable()
    {
        btn1_Text.text = btn1_InputTxt;
        btn2_Text.text = btn2_InputTxt;
        btn3_Text.text = btn3_InputTxt;
    }

    public void HideButtonBackGround()
    {
        // 선택된 값을 제외하고, 버튼의 백그라운드를 보이지 않도록 초기화.
        switch (order)
        {
            // 3번
            case UI_Manager.e_SortingOrder.GradeOrder:
                foreach (Image img in btn1_Images)
                    img.gameObject.SetActive(false);
                foreach (Image img in btn2_Images)
                    img.gameObject.SetActive(false);
                btn1_Text.color = Color.white;
                btn2_Text.color = Color.white;

                RevealButtonBackGround(3);
                break;
            // 2번
            case UI_Manager.e_SortingOrder.LevelOrder:
                foreach (Image img in btn1_Images)
                    img.gameObject.SetActive(false);
                foreach (Image img in btn3_Images)
                    img.gameObject.SetActive(false);
                btn1_Text.color = Color.white;
                btn3_Text.color = Color.white;

                RevealButtonBackGround(2);
                break;
            // 1번
            case UI_Manager.e_SortingOrder.NameOrder:
                foreach (Image img in btn2_Images)
                    img.gameObject.SetActive(false);
                foreach (Image img in btn3_Images)
                    img.gameObject.SetActive(false);
                btn2_Text.color = Color.white;
                btn3_Text.color = Color.white;

                RevealButtonBackGround(1);
                break;
            default: break;
        }
    }

    public void RevealButtonBackGround(int index)
    {
        // 버튼의 백그라운드를 보이도록 초기화.
        if (index == 1)
        {
            foreach (Image img in btn1_Images)
                img.gameObject.SetActive(true);
            btn1_Text.color = ItemSpritesSaver.Instance.GetDarkColor();
            btn1_Text.text = btn1_InputTxt;
        }
        else if (index == 2)
        {
            foreach (Image img in btn2_Images)
                img.gameObject.SetActive(true);
            btn2_Text.color = ItemSpritesSaver.Instance.GetDarkColor();
            btn2_Text.text = btn2_InputTxt;
        }
        else if (index == 3)
        {
            foreach (Image img in btn3_Images)
                img.gameObject.SetActive(true);
            btn3_Text.color = ItemSpritesSaver.Instance.GetDarkColor();
            btn3_Text.text = btn3_InputTxt;
        }
        else { }
    }

    public void SetButtonText(string text, int index)
    {
        if (index == 1)
            btn1_InputTxt = text;
        else if(index == 2)
            btn2_InputTxt = text;
        else if(index == 3)
            btn3_InputTxt = text;
    }

    public void SetButtonOrder(UI_Manager.e_SortingOrder order) { this.order= order; }


}
