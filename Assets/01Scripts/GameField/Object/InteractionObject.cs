using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionObject : MonoBehaviour
{
    public bool isActive;
    public string Name;
    DropItem_UI item;

    private void Awake()
    {
        isActive = false;
    }

    public void ObjectSetInit(Transform scrollViewTransform)
    {   // UI 객체인 DropItem_UI를 오브젝트 풀로 활성화. 이후, 데이터 초기화.
        var tmp = GameManager.Instance.InterectionObjUI_Pool.GetFromPool(Vector3.zero, Quaternion.identity, scrollViewTransform);
        tmp.ImgSymbol.sprite = ItemSpritesSaver.Instance.SpritesSet[2];
        tmp.Text.text = Name;
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

    private void ObjectUICldickEvent()
    {
        CharacterManager.Instance.InteractionObjReturnCall(this);
        GameManager.Instance.InterectionObjUI_Pool.ReturnToPool(item);
    }
}
