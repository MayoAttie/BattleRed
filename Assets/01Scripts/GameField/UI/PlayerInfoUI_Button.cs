using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerInfoUI_Button : MonoBehaviour
{
    public Sprite originSprite;
    public Sprite selectSprite;
    public UI_Manager.e_InfoButtonSelected index;

    Image frameImage;
    Image insideImage;
    TextMeshProUGUI text;
    Button button;

    private Vector3 originalFrameSize;
    bool isClicked;

    private void Awake()
    {
        // 각 변수 초기화
        isClicked = false;
        frameImage = gameObject.transform.GetChild(0).GetComponent<Image>();
        insideImage = gameObject.transform.GetChild(1).GetComponent<Image>();
        text = gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        button = gameObject.transform.GetChild(3).GetComponent<Button>();
        originalFrameSize = frameImage.rectTransform.localScale;

        // 스프라이트 초기 설정
        insideImage.gameObject.SetActive(false);
        frameImage.sprite = originSprite;

        // 버튼 이벤트 부착
        button.onClick.AddListener(() => OnButtonClick());
    }

    void OnButtonClick()
    {
        // UI 매니저에 선택한 버튼 객체의 인덱스를 방송하고, 함수 호출
        UI_Manager.Instance.InfoSelectButton(index);
    }

    // 클릭된 버튼 UI 수정 함수
    public void OnOffSpriteSetting()
    {
        isClicked = !isClicked;

        if (isClicked)
        {
            // 클릭 시 프레임 크기 키우고 bold 설정 및 텍스트 크기 변경
            frameImage.rectTransform.localScale = originalFrameSize * 1.2f;
            frameImage.sprite = selectSprite;
            insideImage.gameObject.SetActive(true);
            text.fontStyle |= FontStyles.Bold;
            text.fontSize = 60;
        }
        else
        {
            // 클릭 해제 시 프레임 크기 원래대로 복구하고 bold 해제 및 텍스트 크기 변경
            frameImage.rectTransform.localScale = originalFrameSize;
            frameImage.sprite = originSprite;
            insideImage.gameObject.SetActive(false);
            text.fontStyle &= ~FontStyles.Bold;
            text.fontSize = 40;
        }

    }

    public bool GetClickedActive() { return isClicked; }
    public void SetText(string txt) { this.text.text = txt; }
    public bool GetIsCicked() { return isClicked; }
    public string GetText() { return text.text;}
    public Button GetButton() { return button; }
}
