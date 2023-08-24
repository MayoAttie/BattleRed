
using TMPro;
using UnityEngine;

public class DamageTextManager : Singleton<DamageTextManager>
{
    [SerializeField]
    private GameObject popupText;
    [SerializeField]
    private GameObject canvas;


    public void CreateFloatingText(string text, Vector3 position, Color textColor)
    {
        var instance = Instantiate(popupText);

        // 월드 좌표를 스크린 좌표로 변환합니다.
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(position);
        Debug.Log("_screenPosition  : " + screenPosition);

        instance.transform.SetParent(canvas.transform, false);
        instance.transform.position = screenPosition;

        var floatingTextComponent = instance.GetComponent<FloatingText>();
        floatingTextComponent.SetText(text);
        floatingTextComponent.SetColor(textColor);
    }
}
