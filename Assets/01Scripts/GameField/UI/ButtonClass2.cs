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
    [Range(0,1)] public float RightAlpha;           // 오른쪽 백그라운드 알파값
    [Range(0,1)] public float LeftAlpha;            // 왼쪽 백그라운드 알파값
    [Range(0, 1)] public float ButtonImageAlpha;    // 버튼 이미지 알파값

    private float elapsedTime = 0f;                 // 시간 계산용 변수
    private bool hasInvoked = true;                // 시간 계산용 변수

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

        background = transform.GetChild(0).GetComponent<Image>();
        leftImg = transform.GetChild(1).GetComponent<Image>();
        rightImg = transform.GetChild(2).GetComponent<Image>();
        symbolImg = transform.GetChild(3).GetComponent<Image>();

        // 버튼 텍스트 초기화
        inText = transform.GetChild(4).GetComponent<TextMeshProUGUI>();


        // 백그라운드 이미지 설정
        if (btnBackgroundSprite != null)
        {
            if (background != null)
                background.sprite = btnBackgroundSprite;
        }
        else
        { background.gameObject.SetActive(false); }

        // 왼쪽 이미지 설정
        if (leftSprite != null)
        {
            if (leftImg != null)
                leftImg.sprite = leftSprite;
        }
        else
        { leftImg.gameObject.SetActive(false); }

        // 오른쪽 이미지 설정
        if (rightSprite != null)
        {
            if (rightImg != null)
                rightImg.sprite = rightSprite;
        }
        else
        { rightImg.gameObject.SetActive(false);}

        // 심볼 이미지 설정
        if (symbolSprite != null)
        {
            if (symbolImg != null)
                symbolImg.sprite = symbolSprite;
        }
        else
        { symbolImg.gameObject.SetActive(false); }


        //왼쪽 오른쪽 이미지가 활성화 되지 않았다면, 버튼 크기 조절
        if(!leftImg.gameObject.activeSelf && !rightImg.gameObject.activeSelf)
        {
            // 심볼 이미지의 위치를 백그라운드 이미지의 정중앙으로 설정
            symbolImg.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // 백그라운드 이미지의 정중앙
            // 버튼 사이즈 조절
            button.GetComponent<RectTransform>().sizeDelta = background.GetComponent<RectTransform>().sizeDelta;
        }

        // 버튼 이미지 설정
        if (btnInsideSprite != null)
        {
            inside = button.GetComponent<Image>();
            if (inside != null)
                inside.sprite = btnInsideSprite;
        }

        // 백그라운드 알파값 설정
        if (background != null)
        {
            Color backgroundColor = background.color;
            backgroundColor.a = BackgroundAlpha;
            background.color = backgroundColor;
        }

        // 오른쪽 알파값 설정
        if (rightImg != null)
        {
            Color backgroundColor = rightImg.color;
            backgroundColor.a = RightAlpha;
            rightImg.color = backgroundColor;
        }
        // 왼쪽 알파값 설정
        if (leftImg != null)
        {
            Color backgroundColor = leftImg.color;
            backgroundColor.a = LeftAlpha;
            leftImg.color = backgroundColor;
        }

        // 버튼 이미지 알파값 설정
        if (inside != null)
        {
            Color insideColor = background.color;
            insideColor.a = ButtonImageAlpha;
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
            symbolImg.type = Image.Type.Filled;
        }
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
    }

    private void ResetElapsedTime()
    {
        // 함수가 호출된 후 경과 시간을 초기화하고 플래그도 초기화
        elapsedTime = 0f;
        hasInvoked = false;
    }

    public void OnClick()
    {
        AlphaValueChangeing();

        // 버튼이 클릭되면 함수 호출을 요청하고, 경과 시간 초기화
        ResetElapsedTime();

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
        if (symbolImg != null && symbolImg.type == Image.Type.Filled)
        {
            symbolImg.fillAmount = fillAmount;
        }
    }

    

    public void AlphaValueChangeing()
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

    public void SetSymbolSprite(Sprite sprite){symbolImg.sprite = sprite;}

    public Button GetButton(){return button;}
    public Image GetBackGroundImg() { return background; }
    public Image GetSymbolImg() { return symbolImg; }   
    public void SetButtonTextInputter(string text) { inText.text = text; }

}
