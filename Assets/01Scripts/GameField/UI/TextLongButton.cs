using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TextLongButton : MonoBehaviour
{
    Image mySymbolImage;
    TextMeshProUGUI myText;
    Button myButton;
    private void Awake()
    {
        myText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        mySymbolImage = transform.GetChild(1).GetComponent<Image>();
        myButton = transform.GetChild(2).GetComponent<Button>();
    }

    public string MyText
    {
        get { return myText.text; }
        set { myText.text = value; }
    }

    public Sprite MySymbolImage
    {
        get { return mySymbolImage.sprite; }
        set { mySymbolImage.sprite = value; }
    }

    public Button MyButton
    {
        get { return myButton; }
    }

}
