using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* This script is responsible for controlling the player stamina bar.
 * It is a Singleton pattern, meant to be used (mostly) by the PlayerController.
 * Stamina will be consumed upon calling UseStamina(), and will be recovered over time
 * by the RegenStamina Coroutine.
 * The bar will fade in on usage, and fade out when its filled completely again.
 */

public class StaminaBar : MonoBehaviour
{
    // SINGLETON PATTERN
    public static StaminaBar Instance => instance;
    private static StaminaBar instance;

    // Regenerate Stamina Coroutine
    public float regenDelaySeconds;     // Time before starting the regeneration.
    public float regenTickSeconds;      // Time between coroutine ticks.
    private WaitForSeconds regenTick;
    public float fadeDelaySeconds;     // Time before fading out the stamina bar, when on full stamina.
    private Coroutine regen;

    // Hide Show Coroutine
    public float fadeDuration;

    // Slider refs
    private Slider staminaBar;
    private Image background;
    private Image fill;

    // Stamina values
    private float maxStamina;
    private float currentStamina;

    void Awake()
    {
        // Singleton!
        instance = this;

        // Slider refs
        staminaBar = gameObject.GetComponent<Slider>();
        fill = staminaBar.fillRect.GetComponent<Image>();
        background = staminaBar.transform.Find("Background").GetComponent<Image>();

        // Since we would be calling this WaitForSeconds in a while loop,
        // we instead store its reference so we don't have to create it every time.
        regenTick = new WaitForSeconds(regenTickSeconds);
    }

    void Start()
    {
        // Sync logic with the Slider component at 100%.
        maxStamina = staminaBar.maxValue;
        currentStamina = maxStamina;
        staminaBar.value = maxStamina;

        // Start the Level with a clean screen.
        HideShowBarInstantly(false);
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

        // Defense vs UseStamina spamming.
        // By storing the coroutine's reference, each time we UseStamina we can restart the regen process.
        if (regen != null) StopCoroutine(regen);
        regen = StartCoroutine(RegenStamina());
    }

    private IEnumerator RegenStamina()
    {
        yield return new WaitForSeconds(regenDelaySeconds);

        while (currentStamina < maxStamina)
        {
            currentStamina += maxStamina / 100;
            staminaBar.value = currentStamina;

            yield return regenTick;
        }

        yield return new WaitForSeconds(fadeDelaySeconds);

        regen = null;
        StartCoroutine(HideShowBar(false));
    }

    private IEnumerator HideShowBar(bool mustShow)
    {
        float elapsedTime = 0f;
        Color bgColor = background.color;
        Color fillColor = fill.color;

        // If must show but it is already there, return.
        if (bgColor.a >= 1f && mustShow) yield break;

        while (elapsedTime < fadeDuration)
        {
            yield return null;
            elapsedTime += Time.deltaTime;

            if (mustShow)
            {
                bgColor.a = Mathf.Clamp01(elapsedTime / fadeDuration);
                fillColor.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            }
            else
            {
                bgColor.a = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
                fillColor.a = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            }

            background.color = bgColor;
            fill.color = fillColor;
        }
    }

    private void HideShowBarInstantly(bool mustShow)
    {
        Color bgColor = background.color;
        Color fillColor = fill.color;
        bgColor.a = mustShow ? 1f : 0f;
        fillColor.a = mustShow ? 1f : 0f;
        background.color = bgColor;
        fill.color = fillColor;
    }
}
