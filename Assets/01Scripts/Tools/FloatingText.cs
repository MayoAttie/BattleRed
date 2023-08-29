using UnityEngine;
using TMPro;
using static HandlePauseTool;

public class FloatingText : MonoBehaviour
{
    private Animator animator;
    private TextMeshProUGUI damageText;
    private RectTransform rectTransform;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        damageText = GetComponent<TextMeshProUGUI>();
        rectTransform = gameObject.GetComponent<RectTransform>();
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

    public void SetText(string text)
    {
        damageText.text = text;
    }

    public void SetColor(Color color)
    {
        damageText.color = color;
    }

    public void SetPosition(Vector2 localPos)
    {
        rectTransform.localPosition = localPos;
        Debug.Log(nameof(rectTransform.localPosition)+ ":" +rectTransform.localPosition);
        Debug.Log(nameof(transform.position)+":"+transform.position);
        
        animator.SetTrigger("Start");
        float aniTime = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, aniTime);

    }
}
