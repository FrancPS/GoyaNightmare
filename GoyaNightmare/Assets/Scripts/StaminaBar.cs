using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public static StaminaBar instance; // Singleton pattern

    public Slider staminaBar;
    public int maxStamina;
    public float regenDelayTime;
    public float regenSpeedRatio;

    private int currentStamina;
    private Coroutine regen;
    private WaitForSeconds regenTick;

    void Awake()
    {
        instance = this;
        regenTick = new WaitForSeconds(regenSpeedRatio);
    }

    void Start()
    {
        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = maxStamina;
    }

    public void UseStamina(int amount)
    {
        if (currentStamina -amount >= 0)
        {
            currentStamina -= amount;
            staminaBar.value = currentStamina;

            if (regen != null) StopCoroutine(regen); 
            regen = StartCoroutine(RegenStamina());
        }
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
    }
}
