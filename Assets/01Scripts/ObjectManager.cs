using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UI_Manager;

public class ObjectManager : Singleton<ObjectManager>
{
    /** 0번 - SynthesisObject
     * 
     */
    public InteractionObject[] objectArray;

    public void FunctionConnecter(InteractionObject obj)
    {
        // 합성대 오브젝트 함수 연결
        switch(obj.name)
        {
            case "합성대":
                objectArray[0].ObjectClickEventSet().onClick.AddListener(() => SynthesisObjectFunction());
                break;
        }
    }

    void SynthesisObjectFunction()
    {
        Synthesis synthesisInstance = UI_Manager.Instance.synthesis; // Synthesis 클래스의 인스턴스 생성
        synthesisInstance.SynthesisObjectFunc_UI_Print();           // Synthesis 클래스의 메서드 호출
    }

}
