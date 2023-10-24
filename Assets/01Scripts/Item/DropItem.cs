using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    ItemClass itemCls;

    public ItemClass GetItemCls() { return itemCls; }
    public void SetItemCls(ItemClass data) { this.itemCls = data; }
}
