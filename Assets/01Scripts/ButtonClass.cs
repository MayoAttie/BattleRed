using UnityEngine;
using UnityEngine.UI;

public class ButtonClass : MonoBehaviour
{
    Button button;
    Image background;
    Image inside;
    public Sprite btnBackgroundSprite;
    public Sprite btnInsideSprite;
    [Range(0,1)] public float BackgroundAlpha;

    public UnityEngine.Events.UnityEvent onPressed;
    public UnityEngine.Events.UnityEvent onButtonDown;
    public UnityEngine.Events.UnityEvent onButtonUp;

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
            background = button.gameObject.GetComponent<Image>();
            if (background != null)
                background.sprite = btnBackgroundSprite;
        }

        // 내부 이미지 설정
        if (btnInsideSprite != null)
        {
            inside = transform.GetChild(0).GetComponent<Image>();
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
    }

    private void OnClick()
    {
        // 버튼이 눌릴 때 호출되는 이벤트
        onPressed.Invoke();
    }

    public void OnButtonDown()
    {
        // 버튼이 눌렸을 때 호출되는 이벤트
        onButtonDown.Invoke();
    }

    public void OnButtonUp()
    {
        // 버튼이 눌려있던 상태에서 뗄 때 호출되는 이벤트
        onButtonUp.Invoke();
    }
}
