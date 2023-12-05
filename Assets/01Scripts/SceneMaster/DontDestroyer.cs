using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyer : MonoBehaviour
{
    // 이미 존재하는 객체들을 저장하기 위한 리스트
    private static List<string> dontDestroyObjects = new List<string>();

    void Awake()
    {
        // 자기 자신의 gameObject가 이미 리스트에 존재하면 파괴
        if (dontDestroyObjects.Contains(gameObject.name))
        {
            Destroy(gameObject);
            return;
        }

        // 자기 자신의 gameObject를 리스트에 추가하고 파괴되지 않도록 설정
        dontDestroyObjects.Add(gameObject.name);
        DontDestroyOnLoad(gameObject);
    }
}
