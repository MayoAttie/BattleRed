/*
Copyright (c) 2014, Marié Paulino
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

(This is the Simplified BSD Licence - http://opensource.org/licenses/BSD-2-Clause)
 */

using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using static TouchPadController;

public class CameraController : MonoBehaviour, Observer
{
    #region Public fields

    //[Space(10.0f)]
    //[Tooltip("Point aimed by the camera")]
    private Transform Target;

    [Tooltip("Maximum distance between the camera and Target")]
    public float Distance = 2;

    [Tooltip("Distance lerp factor")]
    [Range(.0f, 1.0f)]
    public float LerpSpeed = .1f;

    public float FixDuration = 1.0f;  // 고정 기간 (1초)

    [Space(10.0f)]
    [Tooltip("Collision parameters")]
    public TraceInfo RayTrace = new TraceInfo { Thickness = .2f };

    [Tooltip("Camera pitch limitations")]
    public LimitsInfo PitchLimits = new LimitsInfo { Minimum = -60.0f, Maximum = 60.0f };

    [Tooltip("캐릭터 위에서의 원하는 높이")]
    public float DesiredHeight = 2.0f;


    // 조명 오브젝트
    public Light LightObject;
    // 미니맵 카메라
    public Camera MiniMapCamera;


    // 플레이어 시야 상하 회전
    [Header("플레이어 시야 관련")]
    public float touchSensitivity = 2.0f;
    public float minRotationX = -90.0f;
    public float maxRotationX = 90.0f;
    private float rotationX = 0.0f;
    public TouchPadController touchPad;

    [Tooltip("카메라 컨트롤러 상태")]
    CameraControllerState cameraState;


    // 맵의 중앙값 좌표
    public Transform mainMapPos;

    // 타겟 지점을 거치는 좌표. (종점은 목표 타겟)
    public List<Vector3> moveToTarget;    


    #endregion

    #region Structs

    [System.Serializable]
    public struct LimitsInfo
    {
        [Tooltip("Minimum pitch angle, in the range [-90, Maximum]")]
        public float Minimum;

        [Tooltip("Maximum pitch angle, in the range [Minimum, 90]")]
        public float Maximum;
    }

    [System.Serializable]
    public struct TraceInfo
    {
        [Tooltip("Ray thickness")]
        public float Thickness;

        [Tooltip("Layers the camera collide with")]
        public LayerMask CollisionMask;
    }


    // 카메라 컨트롤러 상태
    public enum CameraControllerState
    {
        ChasePlayer,
        WorldMap,
        ToTarget
    }

    #endregion
    
    private float miniMapSize; // 월드맵 크기를 나타내는 변수
    private float _pitch;
    private float _distance;
    private Vector3 _jumpStartPos;
    private float _fixEndTime = 0f;
    bool isFindTarget = false;

    public void Awake()
    {
        moveToTarget = new List<Vector3>();
        cameraState = CameraControllerState.ChasePlayer;
        miniMapSize = 8f;
    }

    public void Start()
    {


        UI_Manager.Instance.GetWorldMap_Manager.Attach(this);
        _pitch = Mathf.DeltaAngle(0, -transform.localEulerAngles.x);
        _distance = Distance;

    }
    private void OnDestroy()
    {
        UI_Manager.Instance.GetWorldMap_Manager.Detach(this);
    }

    public void Update()
    {
        switch(cameraState)
        {
            case CameraControllerState.ChasePlayer:
                SetTarget();
                RotateCamera();
                break;
            case CameraControllerState.WorldMap:
                break;
            case CameraControllerState.ToTarget:
                break;
        }

    }


    public void LateUpdate()
    {
        switch(cameraState)
        {
            case CameraControllerState.ChasePlayer:
                ToTargetChase();
                break;
            case CameraControllerState.WorldMap:
                break;
            case CameraControllerState.ToTarget:
                break;
        }
    }

    // 타겟 추적 함수
    private void ToTargetChase()
    {

        if (Target == null) return;

        float characterYaw = Target.eulerAngles.y;
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, characterYaw, transform.eulerAngles.z);

        var startPos = Target.position;
        var endPos = startPos - transform.forward * Distance;
        var result = Vector3.zero;

        RayCast(startPos, endPos, ref result, RayTrace.Thickness);
        var resultDistance = Vector3.Distance(Target.position, result);

        // 캐릭터 위에 원하는 높이를 계산.
        var desiredPosition = result + Vector3.up * DesiredHeight;

        // 점프 감지
        if (CharacterManager.Instance.GetCharacterClass().GetState() == CharacterClass.eCharactgerState.e_JUMP)
        {
            // 점프 상태일 때
            _jumpStartPos = Target.position;  // 점프 시작 위치 업데이트
            _fixEndTime = Time.time + FixDuration;  // 고정 종료 시간 설정
        }

        if (Time.time < _fixEndTime)
        {
            // 고정 기간 중
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
        else if (resultDistance <= _distance)    // closest collision
        {
            transform.position = desiredPosition;
            _distance = resultDistance;
        }
        else
        {
            _distance = Mathf.Lerp(_distance, resultDistance, LerpSpeed);
            // 카메라의 위치를 지면 위에서 원하는 높이만큼 보간하여 조정.
            transform.position = startPos - transform.forward * _distance + Vector3.up * DesiredHeight;
        }

        // 미니맵 카메라 위치 설정
        if (MiniMapCamera != null)
        {
            Vector3 miniMapPosition = new Vector3(Target.transform.position.x, MiniMapCamera.transform.position.y, Target.transform.position.z);
            MiniMapCamera.transform.position = miniMapPosition;

            FixMiniMapSize();
        }
    }


    // 시작점(startPos)과 끝점(endPos) 사이에 레이를 발사하여 충돌 감지를 수행하는 메서드
    private bool RayCast(Vector3 start, Vector3 end, ref Vector3 result, float thickness)
    {
        // 레이의 방향과 거리를 계산합니다.
        var direction = end - start;
        var distance = Vector3.Distance(start, end);

        // 충돌 정보를 저장할 변수
        RaycastHit hit;

        // SphereCast를 사용하여 레이를 발사하고 충돌 검사를 수행.
        // SphereCast는 레이가 구체 모양으로 발사되며, 두께(thickness)를 가진 구체가 레이와 교차하는지 검사.
        // 만약 충돌이 발생하면 hit 변수에 충돌 정보가 저장.
        // 레이의 시작점과 끝점 사이에서 레이가 특정 레이어 마스크(CollisionMask)에 해당하는 콜라이더와 충돌하는지 검사.
        if (Physics.SphereCast(new Ray(start, direction), thickness, out hit, distance, RayTrace.CollisionMask.value))
        {
            // 충돌이 발생한 경우, 충돌 지점과 충돌 법선(normal)을 이용하여 카메라가 장애물과 충돌하지 않도록 위치를 조정.
            // 카메라의 위치를 충돌 지점에서 충돌 법선 방향으로 thickness만큼 떨어진 곳으로 이동.
            result = hit.point + hit.normal * RayTrace.Thickness;

            // 충돌이 발생한 경우 true를 반환합니다.
            return true;
        }
        else
        {
            // 충돌이 발생하지 않은 경우, 카메라를 끝점(endPos) 위치로 이동시킵니다.
            result = end;

            // 충돌이 발생하지 않은 경우 false를 반환합니다.
            return false;
        }
    }

    // 미니맵 사이즈 조절
    private void FixMiniMapSize()
    {
        if (MiniMapCamera != null)
        {
            MiniMapCamera.orthographicSize = miniMapSize; 
        }
    }

    // 카메라 회전 함수() (타겟 추적 중),
    private void RotateCamera()
    {
        e_TouchSlideDic touchDic;
        touchDic = touchPad.GetDirection();

        // 만약 수평 방향 입력이 위쪽(Up)으로 감지된 경우
        if (touchDic == e_TouchSlideDic.Up)
        {
            // 터치의 이동량에 따라 카메라의 상 회전 조절
            float deltaY = -1 * touchSensitivity;
            rotationX += deltaY;
            rotationX = Mathf.Clamp(rotationX, minRotationX, maxRotationX);

            // 카메라의 상하 회전 적용
            transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
        // 만약 수평 방향 입력이 아래쪽(Down)으로 감지된 경우
        else if (touchDic == e_TouchSlideDic.Down)
        {
            // 터치의 이동량에 따라 카메라의 하 회전 조절
            float deltaY = 1 * touchSensitivity;
            rotationX += deltaY;
            rotationX = Mathf.Clamp(rotationX, minRotationX, maxRotationX);

            // 카메라의 상하 회전 적용
            transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
    }



    // 플레이어 타겟 세팅
    void SetTarget()
    {
        if (!isFindTarget && Target == null)
        {
            isFindTarget = true;
            // 옵저버 패턴 부착
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                CharacterAttackMng characMng = playerObject.GetComponent<CharacterAttackMng>();
                characMng.Attach(this);
                // Player 레이어에 속한 객체 찾기
                Target = playerObject.transform;


                Vector3 dir = Target.position - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(dir);

                // x축 각도를 8로 보정
                targetRotation *= Quaternion.Euler(16, 0, 0);
                transform.rotation = targetRotation;
            }
            isFindTarget = false;
        }
    }

    // 리스트 내의 좌표들을 순회하며 부드럽게 움직임
    IEnumerator MoveToTargetByLerp()
    {
        if (cameraState != CameraControllerState.ToTarget)
            yield break;

        int currentIndex = 0;

        //Vector3 lastTarget = moveToTarget[moveToTarget.Count - 1];  // 마지막 목표 지점(항상 바라봄.)
        while (currentIndex < moveToTarget.Count)
        {
            Vector3 nextTarget = moveToTarget[currentIndex];

            float totalDistance = Vector3.Distance(transform.position, nextTarget);
            float segmentDistance = totalDistance / 13f;

            int i = 1;

            while (i <=13)
            {
                float segmentStart = (i - 1) * segmentDistance;
                float segmentEnd = i * segmentDistance;

                // 중간 지점 좌표 계산
                Vector3 middlePoint = Vector3.Lerp(transform.position, nextTarget, (segmentStart + segmentEnd) / (2 * totalDistance));

                // 현재 위치에서 중간 지점으로 부드럽게 이동
                while (Vector3.Distance(transform.position, middlePoint) > 4f)
                {
                    float t = Mathf.SmoothStep(0f, 1f, Vector3.Distance(transform.position, nextTarget) / 1);

                    // 거리에 따라 속도 동적 조절
                    float speed = Mathf.Lerp(2.0f, 10.0f, t); 
                    if (i == 1 || i == 13)
                        speed = 0.1f;

                    float step = speed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, middlePoint, step);

                    // 항상 다음 위치 방향을 향하도록 회전
                    Vector3 direction = middlePoint - transform.position;
                    Quaternion toRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 60 * Time.deltaTime);

                    yield return null;
                }

                i++;
            }

            currentIndex++;
        }

        yield return new WaitForSeconds(0.1f);

        Vector3 dir = Target.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(dir);

        // x축 각도를 8로 보정
        targetRotation *= Quaternion.Euler(8, 0, 0);
        transform.rotation = targetRotation;

        // 모든 위치를 돌았으면 움직임을 중지
        cameraState = CameraControllerState.ChasePlayer;
        yield break;
    }




    #region 옵저버
    public void AtkLevelNotify(CharacterAttackMng.e_AttackLevel level){}

    public void BlinkValueNotify(CharacterControlMng.e_BlinkPos value){}

    public void GetEnemyFindNotify(List<Transform> findList){}

    public void AttackSkillStartNotify()
    {
        if(LightObject!=null)
            LightObject.gameObject.SetActive(false);
    }
    public void AttackSkillEndNotify()
    {
        if (LightObject != null)
            LightObject.gameObject.SetActive(true);
    }

    public void CheckPoint_PlayerPassNotify(int num){}

    public void WorldMapOpenNotify()
    {

        cameraState = CameraControllerState.WorldMap;
        miniMapSize = 35f;
        MiniMapCamera.transform.position = mainMapPos.position;
        FixMiniMapSize();
    }

    public void WorldMapCloseNotify()
    {
        cameraState = CameraControllerState.ChasePlayer;
        miniMapSize = 8f;

    }

    public void ConvertToTargetStateNotify(List<Vector3> listTarget)
    {
        moveToTarget = listTarget;

        cameraState = CameraControllerState.ToTarget;

        StartCoroutine(MoveToTargetByLerp());
    }


    #endregion


    #region 게터세터
    public float MiniMapSize
    {
        get { return miniMapSize; }
        set { miniMapSize = value; }
    }
    #endregion
}
