using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public static HealthBar instance;
    public Slider slider;
    public Image fill;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;

        /*fill.color = gradient.Evaluate(1f);*/
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        /*fill.color = gradient.Evaluate(slider.normalizedValue);*/

    }

}

