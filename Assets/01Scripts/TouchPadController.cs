using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
public class TouchPadController : Singleton<TouchPadController>, IPointerDownHandler, IDragHandler, IPointerUpHandler
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

    public float smoothing;

    private Vector2 origin;
    private Vector2 direction;
    private Vector2 smoothDirection;
    private bool touched;
    private int pointerID;
    private e_TouchSlideDic slideDic;

    void Awake()
    {
        direction = Vector2.zero;
        touched = false;
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (!touched)
        {
            touched = true;
            pointerID = data.pointerId;
            origin = data.position;
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (data.pointerId == pointerID)
        {
            Vector2 currentPosition = data.position;
            Vector2 directionRaw = currentPosition - origin;
            direction = directionRaw.normalized;
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (data.pointerId == pointerID)
        {
            direction = Vector3.zero;
            touched = false;
        }
    }

    public e_TouchSlideDic GetDirectionHorizontal()
    {
        smoothDirection = Vector2.MoveTowards(smoothDirection, direction, smoothing);
        if (smoothDirection.x > 0.9)
            slideDic = e_TouchSlideDic.Right;
        else if (smoothDirection.x < -0.9)
            slideDic = e_TouchSlideDic.Left;
        else
            slideDic = e_TouchSlideDic.None;

        return slideDic;
    }
}
