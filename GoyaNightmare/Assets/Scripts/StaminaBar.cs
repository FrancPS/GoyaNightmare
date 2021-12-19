using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 
 * This script is responsible for controlling the player stamina bar.
 * Works as a Singleton, to be able to acces script members and functions calling "StaminaBar.instance".
 * 'UseStamina()' must be called when performing any action that consumes stamina.
 * 'HasStamina()' can be called to check if there is any stamina left.
 */

public class StaminaBar : MonoBehaviour
{
    public static StaminaBar instance; // Singleton pattern

    public float regenDelayTime;
    public float regenSpeedRatio;
    public Image background;
    public Image fill;
    public float fadeTime;

    private Slider staminaBar;
    private float maxStamina;
    private float currentStamina;
    private Coroutine regen;
    private WaitForSeconds regenTick;
    private YieldInstruction fadeInstruction = new YieldInstruction();

    void Awake()
    {
        staminaBar = gameObject.GetComponent<Slider>();
        instance = this;
        regenTick = new WaitForSeconds(regenSpeedRatio);
    }

    void Start()
    {
        // Initialise bar
        maxStamina = staminaBar.maxValue;
        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = maxStamina;

        // Set bar to invisible
        Color bgColor = background.color;
        Color fillColor = fill.color;
        bgColor.a = 0f;
        fillColor.a = 0f;
        background.color = bgColor;
        fill.color = fillColor;
    }

    public bool HasStamina()
    {
        return currentStamina > 0f;
    }

    public void UseStamina(float amount)
    {
        StartCoroutine(HideShowBar(true));
        currentStamina -= amount;
        if (currentStamina < 0) currentStamina = 0;
        staminaBar.value = currentStamina;

        if (regen != null) StopCoroutine(regen);
        regen = StartCoroutine(RegenStamina());
    }

    private IEnumerator RegenStamina()
    {
        yield return new WaitForSeconds(regenDelayTime);

        while (currentStamina < maxStamina)
        {
            currentStamina += maxStamina / 100;
            staminaBar.value = currentStamina;

            yield return regenTick;
        }
        yield return new WaitForSeconds(regenDelayTime);
        StartCoroutine(HideShowBar(false));
        regen = null;
    }

    private IEnumerator HideShowBar(bool mustShow)
    {
        float elapsedTime = 0f;
        Color bgColor = background.color;
        Color fillColor = fill.color;

        // If must show and it is already shown, return.
        if (bgColor.a >= 1f && mustShow) yield break;

        while (elapsedTime < fadeTime)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime;
            
            if (mustShow)
            {
                bgColor.a = Mathf.Clamp01(elapsedTime / fadeTime);
                fillColor.a = Mathf.Clamp01(elapsedTime / fadeTime);
            } else
            {
                bgColor.a = 1f - Mathf.Clamp01(elapsedTime / fadeTime);
                fillColor.a = 1f - Mathf.Clamp01(elapsedTime / fadeTime);
            }
            background.color = bgColor;
            fill.color = fillColor;
        }
    }
}
