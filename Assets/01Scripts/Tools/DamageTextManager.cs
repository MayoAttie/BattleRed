
using TMPro;
using UnityEngine;

public class DamageTextManager : Singleton<DamageTextManager>
{
    [SerializeField]
    private GameObject popupText;
    [SerializeField]
    private Canvas canvas;
    private Camera _camera;

    RectTransform rectParent;

    private void OnEnable()
    {
        rectParent = canvas.GetComponent<RectTransform>();
        _camera = canvas.worldCamera;
    }

    public void CreateFloatingText(string text, Vector3 position, Color textColor)
    {
        var instance = Instantiate(popupText);
        RectTransform rectHp = instance.GetComponent<RectTransform>();

        // 부모 객체를 canvas로 설정
        rectHp.SetParent(canvas.transform, false);

        var screenPos = Camera.main.WorldToScreenPoint(position); // 몬스터의 월드 3D 좌표를 스크린 좌표로 변환
        if (screenPos.z < 0.0f)
        {
            screenPos *= -1.0f;
        }
        var localPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, _camera, out localPos); // 스크린 좌표를 다시 캔버스 좌표로 변환

        rectHp.localPosition = localPos; // 체력바 위치 조정

        // 스케일을 항상 (1, 1, 1)로 설정
        rectHp.localScale = Vector3.one;

        var floatingTextComponent = instance.GetComponent<FloatingText>();
        floatingTextComponent.SetText(text);
        floatingTextComponent.SetColor(textColor);
    }

    #region 레거시
    //Vector3 screenPos = _camera.WorldToScreenPoint(position);
    // 월드 좌표를 스크린 좌표로 변환합니다.
    //Vector3 screenPosition = _camera.WorldToScreenPoint(position);
    //screenPosition.z = (canvas.transform.position - popupText.transform.position).magnitude;
    //Vector3 flippedPosition = new Vector3(screenPosition.x, Screen.height - screenPosition.y, screenPosition.z);

    // get the screen position
    //Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(_camera, position);
    // convert the screen position to the local anchored position
    //Vector2 anchoredPosition = transform.InverseTransformPoint(screenPoint);

    //Debug.Log(nameof(anchoredPosition) + " : " + anchoredPosition);
    //instance.transform.SetParent(canvas.transform, false);

    #endregion

}
