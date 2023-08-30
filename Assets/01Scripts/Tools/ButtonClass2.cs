using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClass2 : MonoBehaviour
{
    #region 변수
    Button button;
    Image background;
    Image rightImg;
    Image leftImg;
    Image symbolImg;
    Image inside;
    TextMeshProUGUI inText;

    public Sprite btnBackgroundSprite;              // 버튼 백그라운드 이미지
    public Sprite btnInsideSprite;                  // 버튼 이미지
    public Sprite rightSprite;                      // 버튼셋 오른쪽 이미지
    public Sprite leftSprite;                       // 버튼셋 왼쪽 이미지
    public Sprite symbolSprite;                     // 버튼셋 심볼
    [Range(0,1)] public float BackgroundAlpha;      // 버튼 백그라운드 알파값

    // 이미지들의 알파값
    private Color originalBackgroundColor;
    private Color originalRightImgColor;
    private Color originalLeftImgColor;
    private Color originalSymbolImgColor;
    private bool isClicked = false;

    [Tooltip("onPressed이벤트는, Time Pause의 영향을 받지 않음.")]
    public UnityEngine.Events.UnityEvent onPressed;         // 프레스 이벤트
    public UnityEngine.Events.UnityEvent onButtonDown;      // 프레스다운 이벤트
    public UnityEngine.Events.UnityEvent onButtonUp;        // 프레스업 이벤트

    [Tooltip("이미지를 Filled 상태로 변경할지 선택")]
    public bool ImageFilledSet;                             // 버튼을 FillAmount할지 판단하는 부울형

    #endregion

    private void Awake()
    {
        // 버튼에 클릭 이벤트 리스너 등록
        button = this.gameObject.GetComponentInChildren<Button>();

        // 버튼 클릭 이벤트에 대한 메서드 등록
        button.onClick.AddListener(OnClick);
        button.onClick.AddListener(OnButtonDown);
        button.onClick.AddListener(OnButtonUp);



        // 백그라운드 이미지 설정
        if (btnBackgroundSprite != null)
        {
            background = transform.GetChild(0).GetComponent<Image>();
            if (background != null)
                background.sprite = btnBackgroundSprite;
        }

        // 왼쪽 이미지 설정
        if (leftSprite != null)
        {
            leftImg = transform.GetChild(1).GetComponent<Image>();
            if (leftImg != null)
                leftImg.sprite = leftSprite;
        }

        // 오른쪽 이미지 설정
        if (rightSprite != null)
        {
            rightImg = transform.GetChild(2).GetComponent<Image>();
            if (rightImg != null)
                rightImg.sprite = rightSprite;
        }

        // 심볼 이미지 설정
        if (symbolSprite != null)
        {
            symbolImg = transform.GetChild(3).GetComponent<Image>();
            if (symbolImg != null)
                symbolImg.sprite = symbolSprite;
        }

        // 버튼 텍스트 초기화
        inText = transform.GetChild(4).GetComponent<TextMeshProUGUI>();


        // 버튼 이미지 설정
        if (btnInsideSprite != null)
        {
            inside = button.GetComponent<Image>();
            if (inside != null)
                inside.sprite = btnInsideSprite;
        }

        // 버튼 이미지 알파값 설정
        if (inside != null)
        {
            Color insideColor = background.color;
            insideColor.a = BackgroundAlpha;
            inside.color = insideColor;
        }

        // 초기 이미지 알파값 저장
        originalBackgroundColor = background.color;
        originalRightImgColor = rightImg.color;
        originalLeftImgColor = leftImg.color;
        originalSymbolImgColor = symbolImg.color;
    }

    private void Start()
    {
        if(ImageFilledSet == true)
        {
            // 이미지 타입을 Filled로 변경
            inside.type = Image.Type.Filled;
        }
    }

    private void OnClick()
    {
        AlphaValueChangeing();
        Invoke("AlphaValueChangeing", 0.1f);

        // 버튼이 눌릴 때 호출되는 이벤트
        onPressed.Invoke();
    }

    public void OnButtonDown()
    {
        // 일시정지일 경우에는 리턴.
        if (GameManager.Instance.GetPauseActive())
            return;

        AlphaValueChangeing();
        Invoke("AlphaValueChangeing", 0.1f);

        // 버튼이 눌렸을 때 호출되는 이벤트
        onButtonDown.Invoke();
    }

    public void OnButtonUp()
    {
        // 일시정지일 경우에는 리턴.
        if (GameManager.Instance.GetPauseActive())
            return;

        AlphaValueChangeing();
        Invoke("AlphaValueChangeing", 0.1f);

        // 버튼이 눌려있던 상태에서 뗄 때 호출되는 이벤트
        onButtonUp.Invoke();
    }

    // 버튼의 내부 이미지의 FillAmount 값을 조정하는 함수
    public void SetInsideImageFillAmount(float fillAmount)
    {
        if (inside != null && inside.type == Image.Type.Filled)
        {
            inside.fillAmount = fillAmount;
        }
    }

    public void SetButtonTextInputter(string text)
    {
        inText.text = text;
    }

    private void AlphaValueChangeing()
    {
        // 버튼이 눌릴 때 호출되는 이벤트

        if (!isClicked)
        {
            // 이미지 알파값을 80%로 변경
            SetImageAlpha(background, 0.8f);
            SetImageAlpha(rightImg, 0.8f);
            SetImageAlpha(leftImg, 0.8f);
            SetImageAlpha(symbolImg, 0.8f);
        }
        else
        {
            // 이미지 알파값을 원래 값으로 복원
            SetImageAlpha(background, originalBackgroundColor.a);
            SetImageAlpha(rightImg, originalRightImgColor.a);
            SetImageAlpha(leftImg, originalLeftImgColor.a);
            SetImageAlpha(symbolImg, originalSymbolImgColor.a);
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


}
