using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
        gameObject.SetActive(false);
        slider.value = GameController.masterVolume;

    }

    // Update is called once per frame
    public void OnSliderChange()
    {
        if (!slider) return;

        GameController.SetGeneralVolume(slider.value);
    }
}
