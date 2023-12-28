using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UseTool;
public class WorldMap_Manager : Subject,IPointerClickHandler
{
    [SerializeField]
    GameObject worldMapObject;

    Camera minimapCamera;
    bool isActive;

    void Start()
    {
        gameObject.SetActive(false);
        isActive = false;
    }

    void Update()
    {
        TouchToNavigation();
    }

    public void WorldMapOpenClick()
    {
        if (IsCurrentSceneNameCorrect("GameField") == false)
            return;

        GameManager.Instance.PauseManager();
        WorldMapOpenNotify();                   // 옵저버패턴으로 월드맵 켜짐을 알림
        isActive = true;
        gameObject.SetActive(true);
    }
    public void WorldMapCloseClick()
    {
        GameManager.Instance.PauseManager();
        WorldMapCloseNotify();                  // 옵저버패턴으로 월드맵 종료를 알림
        isActive = false;
        gameObject.SetActive(false);
    }


    void TouchToNavigation()
    {
        if (isActive == false)
            return;

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isActive == false)
            return;


        // 클릭된 위치를 스크린 좌표에서 월드 좌표로 변환
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 clickPosition;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out clickPosition))
        {
            Debug.Log("Clicked at clickPosition Position: " + clickPosition);

            // 월드 좌표를 미니맵 카메라의 뷰포트 좌표로 변환
            Vector3 miniMapViewportPosition = minimapCamera.WorldToViewportPoint(clickPosition);

            // 원하는 작업을 수행
            Debug.Log("Clicked at MiniMap Viewport Position: " + miniMapViewportPosition);

            CharacterManager.Instance._PathFinder.FindPathStart(miniMapViewportPosition);
        }
    }

    public Camera MinimapCamera
    {
        set { minimapCamera = value; }
    }

}
