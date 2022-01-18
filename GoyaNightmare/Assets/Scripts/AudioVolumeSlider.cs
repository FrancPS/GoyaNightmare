using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Basic functionality for the Volume Slider in the UI canvas.
 * It initialises the slider value according to the master volume, and
 * includes the functionality to modify the volume when moving the slider handle.
 */

public class AudioVolumeSlider : MonoBehaviour
{
    Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Start()
    {
        slider.value = GameController.Instance.GetMasterVolume();
    }

    public void OnSliderChange()
    {
        if (!slider) return;
        GameController.Instance.SetMasterVolume(slider.value);
    }
}
