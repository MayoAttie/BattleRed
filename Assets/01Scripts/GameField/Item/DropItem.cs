using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    ItemClass itemCls;
    int id;

    public ItemClass GetItemCls() { return itemCls; }
    public void SetItemCls(ItemClass data) { this.itemCls = data; }

    public int Id
    {
        get { return id; }
        set { id = value; }
    }

}
