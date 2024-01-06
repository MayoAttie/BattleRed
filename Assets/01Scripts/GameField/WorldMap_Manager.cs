using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UseTool;
public class WorldMap_Manager : Subject,IPointerClickHandler, IDragHandler
{
    [SerializeField]
    GameObject worldMapObject;

    Camera minimapCamera;                       // 미니맵 카메라
    bool isActive;                              // 활성상태
    float cameraFarSize;                        // 미니맵 카메라의 촬영 사이즈
    CameraController cameraController;          // 카메라 컨트롤러
    public float moveSpeed;              // 카메라 이동량 조절할 스칼라 값

    float offsetX;                              // 미니맵 카메라 좌표 획득의 보정값
    float offsetY;                              // 미니맵 카메라 좌표 획득의 보정값

    private void Awake()
    {
        cameraFarSize = 35f;
        moveSpeed = 0.03f;
    }
    void Start()
    {
        gameObject.SetActive(false);
        isActive = false;
    }


    public void WorldMapOpenClick()
    {
        if (IsCurrentSceneNameCorrect("GameField") == false)
            return;

        GameManager.Instance.PauseManager();
        WorldMapOpenNotify();                   // 옵저버패턴으로 월드맵 켜짐을 알림
        isActive = true;
        gameObject.SetActive(true);

        // 미니맵 카메라의 촬영 사이즈 갱신
        cameraFarSize = cameraController.MiniMapSize;
        Screen_CurrentResolutionOffsetGet();
    }
    public void WorldMapCloseClick()
    {
        GameManager.Instance.PauseManager();
        WorldMapCloseNotify();                  // 옵저버패턴으로 월드맵 종료를 알림
        isActive = false;
        gameObject.SetActive(false);
    }


    // 짧은 터치를 감지.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isActive == false)
            return;

        // 클릭된 위치를 스크린 좌표에서 월드 좌표로 변환
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 clickPosition;
        Vector3 init_worldPos = minimapCamera.transform.position;

        // 미니맵 내 좌측하단 좌표값 보정
        init_worldPos -= new Vector3(cameraFarSize, 0, cameraFarSize);

        // ui 스크린 터치 좌표를 get
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out clickPosition))
        {
            // 클릭된 위치의 x, y 값을 0에서 1920, 0에서 1080 사이의 값으로 보정
            float correctedX = init_worldPos.x + (clickPosition.x / offsetX);
            float correctedY = init_worldPos.z + (clickPosition.y / offsetY);

            // 보정된 값으로 miniMapViewportPosition 업데이트
            Vector3 miniMapViewportPosition = new Vector3(correctedX, init_worldPos.y, correctedY);


            // Raycast를 통해 Ground 레이어와의 충돌 검사
            RaycastHit hit;
            if (Physics.Raycast(miniMapViewportPosition, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                // Ground 레이어와 충돌한 경우, y 좌표 업데이트
                miniMapViewportPosition.y = hit.point.y;
            }
            else
                miniMapViewportPosition -= new Vector3(0, 10, 0);


            CharacterManager.Instance._PathFinder.FindPathStart(miniMapViewportPosition);
        }
    }

    // 드래그를 감지
    public void OnDrag(PointerEventData eventData)
    {
        if (isActive == false)
            return;

        // 마우스 또는 터치 이동의 변화량
        Vector2 dragDelta = eventData.delta;

        // 이동시킬 카메라의 이동량 계산 및 속도 조절
        float translateX = -dragDelta.x * moveSpeed;
        float translateY = -dragDelta.y * moveSpeed;  // Y 축 이동 부호 변경

        if (!IsCollidingWithInaccessibleObject(translateX, translateY))
        {
            // 충돌이 없을 경우, 미니맵 카메라를 이동 방향으로 이동
            minimapCamera.transform.Translate(translateX, translateY, 0);
        }
    }
    // 충돌 체크
    bool IsCollidingWithInaccessibleObject(float translateX, float translateY)
    {
        if (minimapCamera == null)
            return false;

        // 미니맵 카메라의 현재 위치
        Vector3 currentPosition = minimapCamera.transform.position;

        // 이동 방향으로 레이를 쏘아 충돌 검사
        RaycastHit hit;
        if (Physics.Raycast(currentPosition, new Vector3(translateX, 0, translateY), out hit, Mathf.Sqrt(translateX * translateX + translateY * translateY), LayerMask.GetMask("InaccessibleObject")))
            return true;
        else
            return false;
    }


    // 해상도 및 좌표 보정값 계산
    void Screen_CurrentResolutionOffsetGet()
    {
        // 현재 디스플레이의 해상도를 얻음
        Resolution currentResolution = Screen.currentResolution;

        // x축과 y축에 대한 해상도 값을 사용하여 offset 계산
        offsetX = currentResolution.width / (cameraFarSize * 2);
        offsetY = currentResolution.height / (cameraFarSize * 2);
    }

    public Camera MinimapCamera
    {
        set { minimapCamera = value; }
    }

    public float CameraFarSize
    {
        set { cameraFarSize = value; }
    }
    public CameraController _cameraController
    {
        set { cameraController = value; }
    }
    public GameObject WorldMapObject
    {
        get { return worldMapObject; }
        set { worldMapObject = value; }
    }
}
