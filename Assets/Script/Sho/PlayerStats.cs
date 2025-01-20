using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;
    public float maxHealth; //HPที่เต็มหรือค่าปลาย
    public float CurrentHealth; //HPที่เหลือตอนนี้
    public GameObject gameOverUI;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        
    }
    private void Start()
    {
        CurrentHealth = maxHealth;
        HealthBar.instance.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        HealthBar.instance.SetHealth(CurrentHealth);
    }

    public void TakeDamage(float damageAmount) //ดึงตัวแปรไปไว้ลดHP
    {
        CurrentHealth -= damageAmount;
        Debug.Log("Player HP: " + CurrentHealth);

        if (CurrentHealth <= 0 || CurrentHealth == 0)
        {
            Time.timeScale = 0f;
            gameOverUI.SetActive(true);
        }
    }
    public void ChangeValue(float value)  //เอาไว้ทำไอเทมเพิ่มเลือด
    {
        maxHealth += value;
        CurrentHealth = maxHealth;
        HealthBar.instance.SetMaxHealth(maxHealth);
    }
}
