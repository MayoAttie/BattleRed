using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementRocksObject : Subject
{
    Dictionary<InteractionObject, bool> elementObjs;
    ParticleSystem[] effectParticles;
    [SerializeField] GameObject ClearActive_circle;
    private void Awake()
    {
        elementObjs = new Dictionary<InteractionObject, bool>();
        InteractionObject[] objects = transform.GetComponentsInChildren<InteractionObject>();
        foreach(var tmp in  objects)
        {
            elementObjs[tmp] = false;
        }
        effectParticles = transform.GetComponentsInChildren<ParticleSystem>();

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
            ClearActive_circle.gameObject.SetActive(true);
            List<Vector3> newList = new List<Vector3>();
            newList.Add(ClearActive_circle.transform.position + new Vector3(0,6,0));
            newList.Add(ClearActive_circle.transform.position);
            
            // 카메라 제어 알림
            ConvertToTargetStateNotify(newList);
        }
        else
        {
            Debug.Log("비활성화 객체 있음");
        }
    }
}
