using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BossHealthUI : MonoBehaviour
{
    public Slider healthSlider; // ตัวเลื่อนแสดงหลอดเลือดบอส

    public void SetMaxHealth(float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    public void UpdateHealth(float currentHealth)
    {
        healthSlider.value = currentHealth;
    }

    public void ShowUI(bool show)
    {
        gameObject.SetActive(show); // ซ่อน/แสดง UI
    }
}
