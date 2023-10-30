﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionObject : MonoBehaviour
{
    public string name;
    DropItem_UI item;
    
    public void ObjectSetInit(Transform scrollViewTransform)
    {
        var tmp = GameManager.Instance.InterectionObjUI_Pool.GetFromPool(Vector3.zero, Quaternion.identity, scrollViewTransform);
        tmp.ImgSymbol.sprite = ItemSpritesSaver.Instance.SpritesSet[2];
        tmp.Text.text = name;
        tmp.ItemCls = null;
        tmp.Button.onClick.RemoveAllListeners();
        tmp.Button.onClick.AddListener(() => ObjectUICldickEvent());
        item = tmp;
    }
    public Button ObjectClickEventSet()
    {
        return item.Button;
    }
    public DropItem_UI GetDropItem_UI() { return item; }
    public string GetObjName() { return name; }

    private void ObjectUICldickEvent()
    {
        CharacterManager.Instance.InteractionObjReturnCall(this);
        GameManager.Instance.InterectionObjUI_Pool.ReturnToPool(item);
    }
}
