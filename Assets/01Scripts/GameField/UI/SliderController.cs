using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    Slider slider;
    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetFillProgress(float progress)
    {
        slider.value = progress;
    }
    public Slider GetSlider() { return slider; }     
}
