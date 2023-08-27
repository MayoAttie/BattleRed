using UnityEngine;
using TMPro;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;

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
