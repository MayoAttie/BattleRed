using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoyStickController : Singleton<JoyStickController>, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image joystickBgImage;  // 조이스틱 배경 이미지
    private Image joystickImage;    // 조이스틱 이미지

    private Vector3 inputVector;    // 입력 벡터

    private void Start()
    {
        joystickBgImage = transform.GetChild(0).GetComponent<Image>();
        joystickImage = joystickBgImage.transform.GetChild(0).GetComponent<Image>();
    }

    // 드래그시 실행되는 함수
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBgImage.rectTransform,
            eventData.position, eventData.pressEventCamera, out pos))
        {
            // 조이스틱 배경의 반지름 계산
            pos.x = (pos.x / joystickBgImage.rectTransform.sizeDelta.x);
            pos.y = (pos.y / joystickBgImage.rectTransform.sizeDelta.y);

            float x = (joystickBgImage.rectTransform.pivot.x == 1) ? pos.x * 2 + 1 : pos.x * 2 - 1;
            float y = (joystickBgImage.rectTransform.pivot.y == 1) ? pos.y * 2 + 1 : pos.y * 2 - 1;

            inputVector = new Vector3(x, 0f, y); // y 값을 0으로 수정하여 3D 공간에서의 이동을 위한 벡터 생성

            // 입력 벡터 정규화
            if (inputVector.magnitude > 1.0f)
                inputVector = inputVector.normalized;

            // 조이스틱 이미지 이동
            joystickImage.rectTransform.anchoredPosition =
                new Vector2(inputVector.x * (joystickBgImage.rectTransform.sizeDelta.x / 3),
                            inputVector.z * (joystickBgImage.rectTransform.sizeDelta.y / 3)); // z 값을 사용하여 조이스틱 이미지 이동 (수정)
        }
    }

    // 클릭시 실행되는 함수
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    // 클릭 해제시 실행되는 함수
    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector3.zero; // Vector2에서 Vector3로 수정

        // 조이스틱 이미지의 위치 초기화
        joystickImage.rectTransform.anchoredPosition = Vector2.zero;
    }

    // 입력 벡터 값 반환 함수
    public float GetHorizontalValue()
    {
        return inputVector.x; // x 값을 반환
    }

    public float GetVerticalValue()
    {
        return inputVector.z; // z 값을 반환
    }
}
