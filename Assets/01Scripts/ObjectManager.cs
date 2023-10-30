using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        UI_Manager.Instance.SynthesisObjectFunc_UI_Print();
    }

}
