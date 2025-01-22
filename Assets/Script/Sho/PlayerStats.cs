using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;
    
    [Header("===== PLAYER STATS =====")]
    public float maxHealth; // HP ที่เต็มหรือค่าปลาย
    public float CurrentHealth; // HP ที่เหลือตอนนี้
    public GameObject gameOverUI;
    
    [Header("===== HEAL POTION =====")]
    [SerializeField] private int healAmount = 20; // จำนวน HP ที่ Heal ได้ต่อครั้ง
    [SerializeField] private int maxPotionUses = 3; // จำนวนครั้งสูงสุดที่ใช้ได้
    private int remainingPotionUses;
    [SerializeField] private TextMeshProUGUI healthUIText; // UI Text สำหรับแสดง HP
    [SerializeField] private Image[] potionImages; // รูปภาพ Potion ที่แสดงสถานะ
    [SerializeField] private Sprite fullPotionSprite; // รูปภาพ Potion เต็ม
    [SerializeField] private Sprite potionUsedOnceSprite; // รูปภาพ Potion ดื่มครั้งที่ 1
    [SerializeField] private Sprite potionUsedTwiceSprite; // รูปภาพ Potion ดื่มครั้งที่ 2
    [SerializeField] private Sprite emptyPotionSprite; // รูปภาพ Potion ดื่มครั้งที่ 3
    [SerializeField] private TextMeshProUGUI potionCountText; // UI Text สำหรับแสดงจำนวน Potion ที่เหลือ
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        CurrentHealth = maxHealth;
        HealthBar.instance.SetMaxHealth(maxHealth);
        ResetPotion();
        UpdateHealthUI();
        UpdatePotionUI();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) // กดปุ่ม H เพื่อใช้ Potion
        {
            Instance.UsePotion();
        }
    }

    private void FixedUpdate()
    {
        HealthBar.instance.SetHealth(CurrentHealth);
        UpdateHealthUI();
    }

    public void TakeDamage(float damageAmount) // ดึงตัวแปรไปไว้ลด HP
    {
        CurrentHealth -= damageAmount;
        Debug.Log("Player HP: " + CurrentHealth);

        if (CurrentHealth <= 0 || CurrentHealth == 0)
        {
            Time.timeScale = 0f;
            gameOverUI.SetActive(true);
        }
        UpdateHealthUI();
    }

    public void ChangeValue(float value)  // เอาไว้ทำไอเทมเพิ่มเลือด
    {
        maxHealth += value;
        CurrentHealth = maxHealth;
        HealthBar.instance.SetMaxHealth(maxHealth);
        UpdateHealthUI();
    }

    // ใช้ Potion เพื่อ Heal
    public void UsePotion()
    {
        if (remainingPotionUses > 0)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth + healAmount, 0, maxHealth);
            remainingPotionUses--;
            HealthBar.instance.SetHealth(CurrentHealth);
            Debug.Log($"Potion used. Remaining uses: {remainingPotionUses}");
        }
        else
        {
            Debug.Log("No remaining potion uses!");
        }
        UpdateHealthUI();
        UpdatePotionUI();
    }

    // รีเซ็ตจำนวนครั้งที่สามารถใช้ได้ (เช่น หลังจาก Game Over หรือเริ่มด่านใหม่)
    public void ResetPotion()
    {
        remainingPotionUses = maxPotionUses;
        Debug.Log("Potion reset. Ready for use.");
        UpdatePotionUI();
    }

    // ตรวจสอบจำนวนครั้งที่เหลือ
    public int GetRemainingPotionUses()
    {
        return remainingPotionUses;
    }

    // อัปเดต UI สำหรับแสดง HP
    private void UpdateHealthUI()
    {
        if (healthUIText != null)
        {
            healthUIText.text = $"HP: {CurrentHealth} / {maxHealth}";
        }
    }

    // อัปเดต UI สำหรับแสดงสถานะ Potion
    private void UpdatePotionUI()
    {
        for (int i = 0; i < potionImages.Length; i++)
        {
            if (i < remainingPotionUses)
            {
                switch (remainingPotionUses - i)
                {
                    case 3:
                        potionImages[i].sprite = fullPotionSprite;
                        break;
                    case 2:
                        potionImages[i].sprite = potionUsedOnceSprite;
                        break;
                    case 1:
                        potionImages[i].sprite = potionUsedTwiceSprite;
                        break;
                }
            }
            else
            {
                potionImages[i].sprite = emptyPotionSprite;
            }
        }

        if (potionCountText != null)
        {
            potionCountText.text = $"{remainingPotionUses}";
        }
    }
}
