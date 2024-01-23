using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UseTool;
public class ElementRocksObject : Subject, IObjectTriggerCheckFunc
{
    Dictionary<InteractionObject, bool> elementObjs;
    [SerializeField] GameObject[] ClearActive_circle;   // 퍼즐 풀이 후, 표현되는 마법진 객체.

    [SerializeField] GameObject[] ActiveConvertObject_Off;  // 마법진에 플레이어 객체 충돌 후, 사용되는 객체.
    [SerializeField] GameObject[] ActiveConvertObject_On;   // 마법진에 플레이어 객체 충돌 후, 사용되는 객체.

    private void Awake()
    {
        elementObjs = new Dictionary<InteractionObject, bool>();
        InteractionObject[] objects = transform.GetComponentsInChildren<InteractionObject>();
        foreach(var tmp in  objects)
        {
            elementObjs[tmp] = false;
        }
        ClearActive_circle[0].gameObject.SetActive(false);
        ClearActive_circle[1].gameObject.SetActive(false);
    }

    public void ElementRockOn(InteractionObject mySelf)
    {
        elementObjs[mySelf] = true;
        CheckAllClear();
    }


    void CheckAllClear()
    {
        bool isClear = true;

        foreach (var pair in elementObjs)
        {
            if (!pair.Value)
            {
                // 딕셔너리에 false 값을 가진 원소가 있음
                isClear = false;
                break;
            }
        }

        if (isClear)
        {
            // 서클 객체 활성화
            ClearActive_circle[0].gameObject.SetActive(true);
            ClearActive_circle[1].gameObject.SetActive(true);
            List<Vector3> newList = new List<Vector3>();
            newList.Add(ClearActive_circle[1].transform.position + new Vector3(-4,1.5f,0));
            newList.Add(ClearActive_circle[0].transform.position + new Vector3(4,1.5f,0));
            //newList.Add(ClearActive_circle[0].transform.position);
            
            // 카메라 제어 알림
            ConvertToTargetStateNotify(newList);
        }
        else
        {
            Debug.Log("비활성화 객체 있음");
        }
    }

    public void EnterTriggerFunctionInit(ObjectTriggerEnterCheck other)
    {
        if(other.gameObject == ClearActive_circle[0])   // 매개변수로 받은 객체가 start일 경우,
        {
            if (CharacterManager.Instance.GetCharacterClass().GetState() != CharacterClass.eCharactgerState.e_JUMP)
            {
                other.IsActive = false;
                return;
            }

            StartCoroutine(Jump(CharacterManager.Instance.ControlMng.MyController, other));
        }
        else if(other.gameObject == ClearActive_circle[1])  // 매개변수로 받은 객체가 end일 경우,
        {
            foreach (var tmp in ActiveConvertObject_On)     // 엔드 포인트에 도달시, 지정된 객체 활성화
            {
                tmp.SetActive(true);
            }
            foreach (var tmp in ActiveConvertObject_Off)    // 엔드 포인트 도달시, 기존의 객체 비활성화
            {
                tmp.SetActive(false);
            }
            other.IsActive = false;
        }
    }

    IEnumerator Jump(CharacterController player, ObjectTriggerEnterCheck other)
    {
        IsControlLock(true);

        float elapsedTime = 0f;
        float maxJumpTime = 10f;  // 최대 점프 시간 (10초로 가정)

        Vector3 startPosition = player.transform.position;  // 현재 위치를 시작점으로 사용
        Vector3 endPosition = ClearActive_circle[1].transform.position;  // 목표 지점으로 설정

        // 중간 지점 계산
        Vector3 middlePoint = (startPosition + endPosition) / 2f;
        middlePoint.y += 100f;  // 중간 지점의 높이를 100으로 설정
        
        CharacterManager.Instance.GetCharacterClass().SetState(CharacterClass.eCharactgerState.e_Idle);

        while (elapsedTime < maxJumpTime)  // 최대 점프 시간까지만 점프
        {
            // 포물선 계산
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / maxJumpTime);
            Vector3 newPosition = CalculateParabola(startPosition, middlePoint, endPosition, t);

            // 이동
            player.gameObject.transform.position = newPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        player.gameObject.transform.position = ClearActive_circle[1].transform.position;
        IsControlLock(false);
        yield break;
    }


}
