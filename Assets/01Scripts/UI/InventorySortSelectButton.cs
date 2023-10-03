using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class InventorySortSelectButton : MonoBehaviour
{
    // 각 버튼 객체의 부모 오브젝트
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

    // 버튼 오브젝트
    Button btn1_button;
    Button btn2_button;
    Button btn3_button;
    


    // 선택된 정렬순위
    UI_Manager.e_SortingOrder order;

    private void Awake()
    {
        Button1 = gameObject.transform.GetChild(3).GetComponent<Transform>();
        Button2 = gameObject.transform.GetChild(4).GetComponent<Transform>();
        Button3 = gameObject.transform.GetChild(5).GetComponent<Transform>();

        btn1_Images = new Image[3];
        btn2_Images = new Image[3];
        btn3_Images = new Image[3];

        // 0부터 2까지의 자식 객체의 이미지 컴포넌트를 가져와 배열에 저장
        for (int i = 0; i < 3; i++)
        {
            btn1_Images[i] = Button1.GetChild(i).GetComponent<Image>();
            btn2_Images[i] = Button2.GetChild(i).GetComponent<Image>();
            btn3_Images[i] = Button3.GetChild(i).GetComponent<Image>();
        }

        btn1_Text = Button1.GetComponentInChildren<TextMeshProUGUI>();
        btn2_Text = Button2.GetComponentInChildren<TextMeshProUGUI>();
        btn3_Text = Button3.GetComponentInChildren<TextMeshProUGUI>();

        btn1_button = Button1.GetComponentInChildren<Button>();
        btn2_button = Button2.GetComponentInChildren<Button>();
        btn3_button = Button3.GetComponentInChildren<Button>();

        // 버튼 클릭 이벤트 핸들러 함수를 연결
        btn1_button.onClick.AddListener(() => OnButtonClick(1));
        btn2_button.onClick.AddListener(() => OnButtonClick(2));
        btn3_button.onClick.AddListener(() => OnButtonClick(3));

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

    private void Start()
    {

    }

    public void UI_TextSettingInit()
    {
        UI_Manager.e_InventoryTypeSelected idx = UI_Manager.Instance.GetnSelectedInvenIdx();
        if(idx == UI_Manager.e_InventoryTypeSelected.Weapon || idx == UI_Manager.e_InventoryTypeSelected.Equipment)    // 무기, 장비
        {
            btn3_InputTxt = "희귀도";
            btn2_InputTxt = "레벨";
            btn1_InputTxt = "이름";
        }
        if(idx == UI_Manager.e_InventoryTypeSelected.Gem || idx == UI_Manager.e_InventoryTypeSelected.Food)    // 광물, 음식
        {
            btn3_InputTxt = "희귀도";
            btn2_InputTxt = "갯수";
            btn1_InputTxt = "이름";
        }
        btn1_Text.text = btn1_InputTxt;
        btn2_Text.text = btn2_InputTxt;
        btn3_Text.text = btn3_InputTxt;
    }

    // 선택된 인덱스를 제외한 나머지의 색을 감춤.
    public void HideButtonBackGround()
    {

        // 선택된 값을 제외하고, 버튼의 백그라운드를 보이지 않도록 초기화.
        switch (order)
        {
            // 3번
            case UI_Manager.e_SortingOrder.GradeOrder:
                foreach (Image img in btn1_Images)
                    img.enabled = false; // SetActive 대신 enabled 사용
                foreach (Image img in btn2_Images)
                    img.enabled = false; // SetActive 대신 enabled 사용
                btn1_Text.color = Color.white;
                btn2_Text.color = Color.white;
                RevealButtonBackGround(3);
                break;
            // 2번
            case UI_Manager.e_SortingOrder.LevelOrder:
                foreach (Image img in btn1_Images)
                    img.enabled = false; // SetActive 대신 enabled 사용
                foreach (Image img in btn3_Images)
                    img.enabled = false; // SetActive 대신 enabled 사용
                btn1_Text.color = Color.white;
                btn3_Text.color = Color.white;
                RevealButtonBackGround(2);
                break;
            // 1번
            case UI_Manager.e_SortingOrder.NameOrder:
                foreach (Image img in btn2_Images)
                    img.enabled = false; // SetActive 대신 enabled 사용
                foreach (Image img in btn3_Images)
                    img.enabled = false; // SetActive 대신 enabled 사용
                btn2_Text.color = Color.white;
                btn3_Text.color = Color.white;
                RevealButtonBackGround(1);
                break;
            default: break;
        }
    }
    public void HideButtonBackGround(int index)
    {

        // 선택된 값을 제외하고, 버튼의 백그라운드를 보이지 않도록 초기화.
        switch (index)
        {
            // 0번 버튼 제외 나머지 숨김,
            case 0:
                foreach (Image img in btn2_Images)
                    img.enabled = false; // SetActive 대신 enabled 사용
                foreach (Image img in btn3_Images)
                    img.enabled = false; // SetActive 대신 enabled 사용
                btn2_Text.color = Color.white;
                btn3_Text.color = Color.white;
                RevealButtonBackGround(1);
                break;
            // 1번 버튼 제외 나머지 숨김,
            case 1:
                foreach (Image img in btn1_Images)
                    img.enabled = false; // SetActive 대신 enabled 사용
                foreach (Image img in btn3_Images)
                    img.enabled = false; // SetActive 대신 enabled 사용
                btn1_Text.color = Color.white;
                btn3_Text.color = Color.white;
                RevealButtonBackGround(2);
                break;
            // 2번 버튼 제외 나머지 숨김,
            case 2:
                foreach (Image img in btn1_Images)
                    img.enabled = false; // SetActive 대신 enabled 사용
                foreach (Image img in btn2_Images)
                    img.enabled = false; // SetActive 대신 enabled 사용
                btn1_Text.color = Color.white;
                btn2_Text.color = Color.white;
                RevealButtonBackGround(3);
                break;
            default: break;
        }
    }
    // 선택한 인덱스에 해당하는 UI를 활성화
    public void RevealButtonBackGround(int index)
    {
        // 버튼의 백그라운드를 보이도록 초기화.
        if (index == 1)
        {
            foreach (Image img in btn1_Images)
                img.enabled = true; // SetActive 대신 enabled 사용
            btn1_Text.color = ItemSpritesSaver.Instance.GetDarkColor();
            btn1_Text.text = btn1_InputTxt;
        }
        else if (index == 2)
        {
            foreach (Image img in btn2_Images)
                img.enabled = true; // SetActive 대신 enabled 사용
            btn2_Text.color = ItemSpritesSaver.Instance.GetDarkColor();
            btn2_Text.text = btn2_InputTxt;
        }
        else if (index == 3)
        {
            foreach (Image img in btn3_Images)
                img.enabled = true; // SetActive 대신 enabled 사용
            btn3_Text.color = ItemSpritesSaver.Instance.GetDarkColor();
            btn3_Text.text = btn3_InputTxt;
        }
        else { }
    }


    #region 버튼 클릭 이벤트

    // 버튼 클릭 이벤트 핸들러 함수
    public void OnButtonClick(int index)
    {
        // 클릭한 버튼의 인덱스를 받아와서 처리할 로직을 구현
        UI_Manager.Instance.GetSortIndex(index);
        order = (UI_Manager.e_SortingOrder)index;
        UI_TextSettingInit();
        HideButtonBackGround();
    }
    #endregion


    public Button[] GetButtons()
    {
        Button[] buttons = { btn1_button, btn2_button, btn3_button, };
        return buttons;
    }
    public void SetButtonsText(string[] texts)
    {
        if (texts.Length > 3) return;
        btn1_InputTxt = texts[0];
        btn2_InputTxt = texts[1];
        btn3_InputTxt = texts[2];
    }
    public void StringUIApplyer()
    {
        btn1_Text.text = btn1_InputTxt;
        btn2_Text.text = btn2_InputTxt;
        btn3_Text.text = btn3_InputTxt;
    }
    public void SetSortingOrder(UI_Manager.e_SortingOrder sortOrder)
    {
        order = sortOrder;
    }
}
