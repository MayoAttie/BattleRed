using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    #region 변수
    [SerializeField]
    Image[] img_SelectBgr;
    [SerializeField]
    Image[] img_symbolAndBgr;   //0번 - bgr, 1번 - 심볼 이미지
    Button button;

    // 원래 색상
    Color nomalColor_Symbol = Color.white;
    // 클릭시 색상
    Color selectColor_Symbol = new Color(51f / 255f, 58f / 255f, 71/255f);

    bool isclicked;
    #endregion


    public UnityEngine.Events.UnityEvent onPressed;         // 프레스 이벤트

    private void Awake()
    {
        isclicked = true;
        button = gameObject.GetComponentInChildren<Button>();
        // 버튼 클릭 이벤트에 대한 메서드 등록
        button.onClick.AddListener(OnClick);

        ButtonUIColorSet();
    }

    private void OnClick()
    {
        ButtonUIColorSet();
        // 버튼이 눌릴 때 호출되는 이벤트
        onPressed.Invoke();
    }


    // 버튼 UI를 수정하는 코드
    public void ButtonUIColorSet()
    {
        isclicked = !isclicked; 

        switch (isclicked)
        {
            case true:
                for (int i = 0; i < img_SelectBgr.Length; i++)
                {
                    img_SelectBgr[i].enabled = true;
                }
                img_symbolAndBgr[0].enabled = false;
                img_symbolAndBgr[1].color = selectColor_Symbol;
                break;
            case false:
                for (int i = 0; i < img_SelectBgr.Length; i++)
                {
                    img_SelectBgr[i].enabled = false;
                }
                img_symbolAndBgr[0].enabled = true;
                img_symbolAndBgr[1].color = nomalColor_Symbol;
                break;
        }
    }

    public bool GetClickActive(){return isclicked;}
}
