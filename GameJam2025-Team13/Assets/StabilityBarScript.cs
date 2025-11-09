using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StabilityBarScript : MonoBehaviour
{

    public Slider slider;

    
    public void SetSlider(float ammount)
    {
        slider.value = ammount;
    }
    
}
