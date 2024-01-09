using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleObject : MonoBehaviour
{
    // 보유한 퍼즐 다이스 자식 객체
    Rigidbody[] puzzleDice;                     // 퍼즐큐브(자식 객체)

    [SerializeField]
    float[] offset;                             // 퍼즐 큐브의 각도를 보정하는 오프셋
    List<bool> puzzleMovingFlag;                // 퍼즐 기믹을 푸는 데 사용되는 플래그

    [SerializeField]
    Transform rewardPos;                        // 보상용 보물상자 생성 위치

    [SerializeField]
    string rewardBoxSetName;                    // 보상용 보물상자 세팅 이름

    bool isAnswerCorrect;

    private void Awake()
    {
        isAnswerCorrect = false;
        puzzleDice = transform.GetComponentsInChildren<Rigidbody>();
        puzzleMovingFlag = new List<bool>();
    }

    private void Start()
    {
        for (int i = 0; i < offset.Length; i++)
        {
            puzzleMovingFlag.Add(false);
        }
    }


    public void ObjectRotation(Transform obj)
    {
        int index = 0;
        int index2 = 0;
        int index3 = 0;
        for (int i = 0; i < puzzleDice.Length; i++)  // 객체 찾기
        {
            if (puzzleDice[i].gameObject.Equals(obj.gameObject))
            {
                index = i;
                break;
            }
        }

        if (index == puzzleDice.Length - 1)
            index2 = 0;
        else
            index2 = index + 1;

        if (index == 0)
            index3 = puzzleDice.Length - 1;
        else
            index3 = index - 1;

        // 인덱스값에 해당하는 퍼즐 객체를 저장
        GameObject puzzle1 = puzzleDice[index2].gameObject;
        GameObject puzzle2 = puzzleDice[index3].gameObject;

        // 제어 플래그 확인 후, 큐브 오브젝트가 이동 중이 아니라면. 코루틴 함수를 호출합
        if (puzzleMovingFlag[index] == false && puzzleMovingFlag[index2] == false && puzzleMovingFlag[index3] == false)
        {
            puzzleMovingFlag[index] = true;
            puzzleMovingFlag[index2] = true;
            puzzleMovingFlag[index3] = true;
            StartCoroutine(RotateSmoothly(obj.gameObject, index));
            StartCoroutine(RotateSmoothly(puzzle1, index2));
            StartCoroutine(RotateSmoothly(puzzle2, index3));
        }

    }
     
    IEnumerator RotateSmoothly(GameObject obj,int index)
    {
        // 초기 각도 설정
        float currentAngle = obj.transform.eulerAngles.y % 360;

        float targetY = (currentAngle + 90) % 360;
        float epsilon = 0.01f; // 근사치 보정에 사용할 값

        if(targetY != 0)
        {
            while (currentAngle < targetY - epsilon)
            {
                // 부드러운 회전 계산
                Quaternion newRotation = Quaternion.RotateTowards(obj.transform.rotation, Quaternion.Euler(0f, targetY, 0f), 30f * Time.deltaTime);

                // 부드러운 회전 적용
                obj.transform.rotation = newRotation;

                // 현재 각도 업데이트
                currentAngle = obj.transform.eulerAngles.y;

                yield return null;
            }
        }
        else
        {
            while (currentAngle < 359.9 && currentAngle !=0)        // 디버깅으로 확인 후, 회전 예외를 조건문으로 설정.
            {
                Quaternion newRotation = Quaternion.RotateTowards(obj.transform.rotation, Quaternion.Euler(0f, targetY, 0f), 30f * Time.deltaTime);

                obj.transform.rotation = newRotation;

                currentAngle = obj.transform.eulerAngles.y;

                yield return null;
            }
        }

        // 정확한 각도로 맞추기
        obj.transform.rotation = Quaternion.Euler(0f, targetY, 0f);
        puzzleMovingFlag[index] = false;
        AnswerCheck();
        yield break;
    }


    void AnswerCheck()
    {
        bool isAnswer = true;
        for (int i = 1; i < puzzleDice.Length; i++)
        {
            float currentAngle1 = puzzleDice[i - 1].transform.eulerAngles.y % 360 + offset[i - 1];
            float currentAngle2 = puzzleDice[i].transform.eulerAngles.y % 360 + offset[i];

            currentAngle1 %= 360;
            currentAngle2 %= 360;

            if (currentAngle1 != currentAngle2)
            {
                isAnswer = false;
                break;
            }
        }



        if (isAnswer)
        {
            if(!isAnswerCorrect)
            {
                Debug.Log("정답!");

                // 보물상자 생성
                isAnswerCorrect = true;
                var box = Instantiate(ObjectManager.Instance.TreasureBox, rewardPos);
                box.transform.localScale *= 8f;

                // 오브젝트 매니저에 생성한 객체를 추가
                InteractionObject cls = box.GetComponent<InteractionObject>();
                cls.Name = rewardBoxSetName;    // 보물상자에 설정된 이름을 초기화. (오브젝트 매니저에서 보상처리할 때 사용)
                ObjectManager.Instance.IsOpenChecker[cls] = false;
            }
        }
        else
        {
            Debug.Log("정답 아님");
        }
    }
}
