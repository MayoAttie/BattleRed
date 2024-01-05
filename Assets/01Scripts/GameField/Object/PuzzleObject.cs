using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleObject : MonoBehaviour
{
    // 보유한 퍼즐 다이스 자식 객체
    Rigidbody[] puzzleDice;

    [SerializeField]
    float[] offset;
    List<bool> puzzleMovingFlag;


    private void Awake()
    {
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

        GameObject puzzle1 = puzzleDice[index2].gameObject;
        GameObject puzzle2 = puzzleDice[index3].gameObject;


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
            while (currentAngle < 359.9 && currentAngle !=0)
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
            Debug.Log("정답 맞춤");
        }
        else
        {
            Debug.Log("정답 아님");
        }
    }
}
