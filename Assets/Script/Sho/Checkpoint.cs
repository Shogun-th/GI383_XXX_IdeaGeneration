using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public BoxCollider2D trigger; // ตัวตรวจจับชน
    public SpriteRenderer spriteRenderer; // ตัวแสดงผล Sprite
    public Sprite beforeCheckpoint; // Sprite ก่อนชน
    public Sprite afterCheckpoint; // Sprite หลังชน

    private void Start()
    {
        // ตั้งค่า Sprite เริ่มต้น
        if (spriteRenderer != null && beforeCheckpoint != null)
        {
            spriteRenderer.sprite = beforeCheckpoint;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("✅ Player ชน Checkpoint!");
            Invoke("ChangeSprite", 0.1f); // หน่วงเวลา 0.1 วินาที
            trigger.enabled = false;

            // ตรวจสอบค่า SpriteRenderer
            if (spriteRenderer == null)
            {
                Debug.LogError("❌ spriteRenderer เป็น NULL! ตรวจสอบว่ามีการ Assign ใน Inspector");
                return;
            }

            // ตรวจสอบค่า afterCheckpoint
            if (afterCheckpoint == null)
            {
                Debug.LogError("❌ afterCheckpoint เป็น NULL! ตรวจสอบว่าตั้งค่า Sprite ใน Inspector");
                return;
            }

            // เปลี่ยนตำแหน่งจุด Respawn
            RespawnController.instance.respawnPoint = transform;

            // รีเซ็ต Potion และ HP
            PlayerStats.Instance.ResetPotion();
            PlayerStats.Instance.CurrentHealth = PlayerStats.Instance.maxHealth;

            // ลองบังคับเปลี่ยน Sprite อีกรอบ
            spriteRenderer.sprite = afterCheckpoint;
            spriteRenderer.enabled = false;
            spriteRenderer.enabled = true;

            Debug.Log("✅ เปลี่ยน Sprite เป็นหลังชนแล้ว!");

            // ปิดการทำงานของ Trigger
            trigger.enabled = false;
        }
    }
    void ChangeSprite()
    {
        if (spriteRenderer != null && afterCheckpoint != null)
        {
            spriteRenderer.sprite = afterCheckpoint;
            Debug.Log("✅ เปลี่ยน Sprite เป็นหลังชนแล้ว!");
        }
    }
}