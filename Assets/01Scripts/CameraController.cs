﻿/*
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

public class CameraController : MonoBehaviour, Observer
{
    #region Public fields

    [Space(10.0f)]
    [Tooltip("Point aimed by the camera")]
    public Transform Target;

    [Tooltip("Maximum distance between the camera and Target")]
    public float Distance = 2;

    [Tooltip("Distance lerp factor")]
    [Range(.0f, 1.0f)]
    public float LerpSpeed = .1f;

    [Space(10.0f)]
    [Tooltip("Collision parameters")]
    public TraceInfo RayTrace = new TraceInfo { Thickness = .2f };

    [Tooltip("Camera pitch limitations")]
    public LimitsInfo PitchLimits = new LimitsInfo { Minimum = -60.0f, Maximum = 60.0f };

    [Tooltip("캐릭터 위에서의 원하는 높이")]
    public float DesiredHeight = 2.0f;


    public Light LightObject;

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
    #endregion
    
    private float _pitch;
    private float _distance;

    public void Awake()
    {
        // 옵저버 패턴 부착
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        CharacterAttackMng characMng = playerObject.GetComponent<CharacterAttackMng>();
        characMng.Attach(this);
    }

    public void Start()
    {
        _pitch = Mathf.DeltaAngle(0, -transform.localEulerAngles.x);
        _distance = Distance;

    }


    public void LateUpdate()
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

        if (resultDistance <= _distance)    // closest collision
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

    #region 옵저버
    public void AtkLevelNotify(CharacterAttackMng.e_AttackLevel level){}

    public void BlinkValueNotify(CharacterControlMng.e_BlinkPos value){}

    public void GetEnemyFindNotify(List<Transform> findList){}

    public void AttackSkillStartNotify()
    {
        LightObject.gameObject.SetActive(false);
    }
    public void AttackSkillEndNotify()
    {
        LightObject.gameObject.SetActive(true);
    }
    #endregion
}
