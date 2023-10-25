using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropItem_UI : MonoBehaviour
{
    Image img_Symbol;
    TextMeshProUGUI text;
    Button button;
    ItemClass itemCls;
    int id;
    private void Awake()
    {
        img_Symbol = transform.GetChild(2).GetComponent<Image>();
        text = transform.GetComponentInChildren<TextMeshProUGUI>();
        button = transform.GetComponentInChildren<Button>();
    }
    public int Id
    {
        get { return id; }
        set { id = value; }
    }

    public Image ImgSymbol
    {
        get { return img_Symbol; }
        set { img_Symbol = value; }
    }
    public TextMeshProUGUI Text
    {
        get { return text; }
        set { text = value; }
    }
    public Button Button
    {
        get { return button; }
        set { button = value; }
    }
    public ItemClass ItemCls
    {
        get { return itemCls; }
        set { itemCls = value; }
    }

}
