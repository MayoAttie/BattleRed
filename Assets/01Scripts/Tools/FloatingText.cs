using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    Animator animator;
    private TextMeshProUGUI damageText;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        Debug.Log(clipInfo.Length);
        Destroy(gameObject, clipInfo[0].clip.length);
        damageText = animator.GetComponent<TextMeshProUGUI>();
    }

    public void SetText(string text)
    {
        damageText.text = text;
    }

    public void SetColor(Color color)
    {
        damageText.color = color;
    }
}
