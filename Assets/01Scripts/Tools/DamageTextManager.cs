
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class DamageTextManager : Singleton<DamageTextManager>
{
    [SerializeField]
    private GameObject popupText;
    [SerializeField]
    private Canvas canvas;
    public Camera _camera;


    private List<FloatingTextData> textList = new List<FloatingTextData>(); // 텍스트 데이터 리스트


    private void Start()
    {
        _camera = canvas.worldCamera;
    }

    private void Update()
    {
        if (textList.Count > 0)
        {
            foreach(var text in textList)
            {
                text.obj.SetPosition(text.position, _camera, this.transform);
            }
        }
    }

    public void CreateFloatingText(string text, Vector3 position, Color textColor)
    {
        var instance = Instantiate(popupText);
        instance.transform.SetParent(canvas.transform);

        var floatingTextComponent = instance.GetComponent<FloatingText>();
        floatingTextComponent.SetText(text);
        floatingTextComponent.SetColor(textColor);
        floatingTextComponent.SetPosition(position, _camera, transform);

        // 필요한 데이터를 리스트에 추가
        textList.Add(new FloatingTextData(instance.GetComponent<FloatingText>(),position));

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



    /*
        var instance = Instantiate(popupText);

        // 부모 객체를 canvas로 설정
        instance.transform.SetParent(canvas.transform, false);
        RectTransform rectHp = instance.GetComponent<RectTransform>();

        var screenPos = Camera.main.WorldToScreenPoint(position); // 몬스터의 월드 3D 좌표를 스크린 좌표로 변환
        if (screenPos.z < 0.0f)
        {
            screenPos *= -1.0f;
        }
        var localPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, _camera, out localPos); // 스크린 좌표를 다시 캔버스 좌표로 변환

        // 스케일을 항상 (1, 1, 1)로 설정
        rectHp.localScale = Vector3.one;

        var floatingTextComponent = instance.GetComponent<FloatingText>();
        floatingTextComponent.SetText(text);
        floatingTextComponent.SetColor(textColor);
        floatingTextComponent.SetPosition(localPos);
     
     */



    #endregion

    public void GetAnimationEnd(FloatingText floatingText)
    {
        // textList에서 해당 FloatingText 객체를 제거
        for (int i = 0; i < textList.Count; i++)
        {
            if (textList[i].obj == floatingText)
            {
                textList.RemoveAt(i);
                break;
            }
        }
    }


    public class FloatingTextData
    {
        public FloatingText obj;
        public Vector3 position;

        public FloatingTextData(FloatingText obj, Vector3 position)
        {
            this.obj = obj;
            this.position = position;
        }
    }
}
