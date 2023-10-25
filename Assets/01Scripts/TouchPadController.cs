using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
public class TouchPadController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public enum e_TouchSlideDic
    {
        None,
        Right,
        Left,
        Up,
        Down,
        Max
    }

    public float smoothing;  // 터치 패드 입력의 부드러움 정도를 제어하는 변수

    private Vector2 origin;  // 터치 시작 위치
    private Vector2 direction;  // 현재 터치 방향
    private Vector2 smoothDirection;  // 부드러운 방향
    private bool touched;  // 터치 여부를 나타내는 플래그
    private int pointerID;  // 터치의 고유 ID
    private e_TouchSlideDic slideDic;  // 터치의 슬라이드 방향을 나타내는 열거형 변수





    void Awake()
    {
        direction = Vector2.zero;  // 초기 방향을 제로 벡터로 설정
        touched = false;  // 터치 여부 초기화
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (!touched)
        {
            touched = true;  // 터치 시작
            pointerID = data.pointerId;  // 터치의 고유 ID 저장
            origin = data.position;  // 터치 시작 위치 저장
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (data.pointerId == pointerID)
        {
            Vector2 currentPosition = data.position;  // 현재 터치 위치
            Vector2 directionRaw = currentPosition - origin;  // 터치 이동 방향 (상대적 위치)
            direction = directionRaw.normalized;  // 터치 이동 방향을 정규화하여 저장
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (data.pointerId == pointerID)
        {
            direction = Vector3.zero;  // 터치 방향 초기화
            touched = false;  // 터치 종료
        }
    }

    public e_TouchSlideDic GetDirectionHorizontal()
    {
        // 현재 방향을 부드럽게 보정하여 저장
        smoothDirection = Vector2.MoveTowards(smoothDirection, direction, smoothing);

        // 보정된 방향을 기준으로 슬라이드 방향 판별
        if (smoothDirection.x > 0.9)
            slideDic = e_TouchSlideDic.Right;
        else if (smoothDirection.x < -0.9)
            slideDic = e_TouchSlideDic.Left;
        else
            slideDic = e_TouchSlideDic.None;

        return slideDic;
    }
    public e_TouchSlideDic GetDirection()
    {
        // 현재 방향을 부드럽게 보정하여 저장
        smoothDirection = Vector2.MoveTowards(smoothDirection, direction, smoothing);

        // 보정된 방향을 기준으로 슬라이드 방향 판별
        if (smoothDirection.x > 0.9f)
            slideDic = e_TouchSlideDic.Right;
        else if (smoothDirection.x < -0.9f)
            slideDic = e_TouchSlideDic.Left;
        else if (smoothDirection.y > 0.9f)
            slideDic = e_TouchSlideDic.Up;
        else if (smoothDirection.y < -0.9f)
            slideDic = e_TouchSlideDic.Down;
        else
            slideDic = e_TouchSlideDic.None;

        return slideDic;
    }
}
