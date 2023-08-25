
using TMPro;
using UnityEngine;

public class DamageTextManager : Singleton<DamageTextManager>
{
    [SerializeField]
    private GameObject popupText;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private Camera _camera;
    

    public void CreateFloatingText(string text, Vector3 position, Color textColor)
    {
        //Debug.Log(nameof(position)+ " : " + position);

        var instance = Instantiate(popupText);
        // 월드 좌표를 스크린 좌표로 변환합니다.
        //Vector3 screenPosition = _camera.WorldToScreenPoint(position);
        //screenPosition.z = (canvas.transform.position - popupText.transform.position).magnitude;
        //Vector3 flippedPosition = new Vector3(screenPosition.x, Screen.height - screenPosition.y, screenPosition.z);

        // get the screen position
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(_camera, position);
        // convert the screen position to the local anchored position
        Vector2 anchoredPosition = transform.InverseTransformPoint(screenPoint);

        //Debug.Log(nameof(anchoredPosition) + " : " + anchoredPosition);
        instance.transform.SetParent(canvas.transform, false);
        instance.transform.position = anchoredPosition;

        var floatingTextComponent = instance.GetComponent<FloatingText>();
        floatingTextComponent.SetText(text);
        floatingTextComponent.SetColor(textColor);
    }

}
