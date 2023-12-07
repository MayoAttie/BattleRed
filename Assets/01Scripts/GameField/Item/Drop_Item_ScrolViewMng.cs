using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop_Item_ScrolViewMng : MonoBehaviour
{
    Transform mainObject;
    Transform scrollObject;
    private void Awake()
    {
        mainObject = transform.GetChild(0);
        scrollObject = mainObject.GetChild(0).GetChild(0);
    }

    public Transform GetMainObject() { return mainObject; }
    public Transform GetScrollObject() { return scrollObject; }
}
