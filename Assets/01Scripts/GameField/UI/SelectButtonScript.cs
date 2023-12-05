using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectButtonScript : MonoBehaviour
{
    #region 변수

    Image topBgr;
    Image itemImage;
    Image plusSymbolImg;
    TextMeshProUGUI itemTxt;
    Button button;

    ItemClass itemCls;                  // 오브젝트가 저장하는 아이템
    bool isActive;                      // obj가 엑티브 상태인지 체크
    bool isClicked;                     // 클릭 상태인지 체크

    private float elapsedTime = 0f;                 // 시간 계산용 변수
    private bool hasInvoked = true;                // 시간 계산용 변수

    #endregion

    private void Awake()
    {
        topBgr = gameObject.transform.GetChild(0).GetComponent<Image>();
        itemImage = topBgr.transform.GetChild(0).GetComponent<Image>();
        plusSymbolImg = topBgr.transform.GetChild(1).GetComponent<Image>();
        itemTxt = gameObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        button = gameObject.transform.GetChild(2).GetComponent<Button>();
        button.onClick.AddListener(OnClickEventListener);

    }
    private void Start()
    {
    }
    private void OnEnable()
    {
        topBgr.color = ItemSpritesSaver.Instance.GetOneStarColor();
        isActive = false;
        isClicked = false;
    }

    private void Update()
    {
        // Time.unscaledDeltaTime를 사용하여 경과한 시간 누적
        elapsedTime += Time.unscaledDeltaTime;

        // 경과 시간이 0.1초 이상이고 아직 함수가 호출되지 않았다면 호출
        if (elapsedTime >= 0.2f && !hasInvoked)
        {
            AlphaValueChangeing();
            // 함수를 호출한 후 플래그를 설정하여 중복 호출을 방지
            hasInvoked = true;
        }

        if(isActive)
            plusSymbolImg.enabled = false;
        else
            plusSymbolImg.enabled = true;
    }

    // 버튼 클릭시 호출 함수
    public void OnClickEventListener()
    {
        AlphaValueChangeing();

        // 버튼이 클릭되면 함수 호출을 요청하고, 경과 시간 초기화
        ResetElapsedTime();

    }

    private void ResetElapsedTime()
    {
        // 함수가 호출된 후 경과 시간을 초기화하고 플래그도 초기화
        elapsedTime = 0f;
        hasInvoked = false;
    }
    public void AlphaValueChangeing()
    {
        // 버튼이 눌릴 때 호출되는 이벤트

        if (!isClicked)
        {
            // 이미지 알파값을 80%로 변경
            SetImageAlpha(topBgr, 0.7f);
            SetImageAlpha(itemImage, 0.7f);
            SetImageAlpha(plusSymbolImg, 0.2f);
        }
        else
        {
            // 이미지 알파값을 원래 값으로 복원
            SetImageAlpha(topBgr, 1f);
            SetImageAlpha(itemImage, 1f);
            SetImageAlpha(plusSymbolImg, 0.66f);
        }

        isClicked = !isClicked;

    }

    private void SetImageAlpha(Image image, float alpha)
    {
        if (image != null)
        {
            Color imageColor = image.color;
            imageColor.a = alpha;
            image.color = imageColor;
        }
    }

    public void SetItemColor(int grade)
    {
        switch (grade)
        {
            case 5:
                topBgr.color = ItemSpritesSaver.Instance.GetFiveStarColor();
                break;
            case 4:
                topBgr.color = ItemSpritesSaver.Instance.GetFourStarColor();
                break;
            case 3:
                topBgr.color = ItemSpritesSaver.Instance.GetThreeStarColor();
                break;
            default:
                topBgr.color = ItemSpritesSaver.Instance.GetOneStarColor();
                break;
        }
    }

    public Image GetTopBgr() { return topBgr; }
    public bool GetIsActive() { return isActive; }
    public Button GetButton() { return button; }
    public Image GetItemImage() { return itemImage; }
    public TextMeshProUGUI GetItemTxt() { return itemTxt; }
    public ItemClass GetItemClass() { return itemCls; }
    public void SetItemSprite(Sprite sprite) { itemImage.sprite = sprite; }
    public void SetItemText(string text) { itemTxt.text = text; }
    public void SetIsActive(bool isActive) { this.isActive= isActive; }
    public void SetItemCls(ItemClass itemCls) { this.itemCls = itemCls;}
}
