using UnityEngine;
using TMPro;
using static HandlePauseTool;
using UnityEngine.UIElements;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    private Animator animator;
    private TextMeshProUGUI damageText;
    private RectTransform rectTransform;
    Transform MngParents;
    RectTransform rectParent;
    Camera _camera;
    Canvas canvas;
    Vector3 monsterPos;
    bool isAnimation;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        damageText = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        _camera = canvas.worldCamera;
        rectParent = canvas.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        // 게임매니저의 이벤트에 구독
        GameManager.OnPauseStateChanged += HandlePauseStateChanged;
    }

    private void OnDisable()
    {
        // 게임매니저의 이벤트 구독 해제
        GameManager.OnPauseStateChanged -= HandlePauseStateChanged;
    }

    private void LateUpdate()
    {
        Vector3 newPos = monsterPos + new Vector3(0, 1.5f, 0);
        var screenPos = Camera.main.WorldToScreenPoint(newPos); // 몬스터의 월드 3D 좌표를 스크린 좌표로 변환

        // 스크린 좌표가 화면 경계를 벗어나는지 확인
        if (screenPos.x < 0)
            screenPos.x = 0;
        else if (screenPos.x > Screen.width)
            screenPos.x = Screen.width;

        if (screenPos.y < 0)
            screenPos.y = 0;
        else if (screenPos.y > Screen.height)
            screenPos.y = Screen.height;

        if (screenPos.z < 0.0f)
        {
            screenPos *= -1.0f;
        }

        var localPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, _camera, out localPos); // 스크린 좌표를 다시 객체 UI 캔버스 좌표로 변환

        // Y좌표를 70에서 90 사이로 보정
        localPos.y = Mathf.Clamp(localPos.y, 70, 90);

        rectTransform.localPosition = localPos; // 객체 위치 조정
                                                // 스케일을 항상 (1, 1, 1)로 설정
        rectTransform.localScale = Vector3.one;

        if (!isAnimation)
        {
            isAnimation = true;
            animator.SetTrigger("Start");
            StartCoroutine(AnimationPlayTime());
        }
    }

    public void SetText(string text)
    {
        damageText.text = text;
    }

    public void SetColor(Color color)
    {
        damageText.color = color;
    }

    IEnumerator AnimationPlayTime()
    {
        float aniTime = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(aniTime);
        MngParents.GetComponent<DamageTextManager>().GetAnimationEnd(this);
        Destroy(gameObject);
    }

    public void SetPosition(Vector3 localPos, Camera camera, Transform MngParents)
    {
        //_camera = camera;
        monsterPos = localPos;
        this.MngParents = MngParents;
    }
}
