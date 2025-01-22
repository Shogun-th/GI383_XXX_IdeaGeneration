using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public BoxCollider2D trigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // ตั้งค่า Respawn Point
            RespawnController.instance.respawnPoint = transform;

            // รีเซ็ต Potion Heal ผ่าน PlayerStats
            PlayerStats.Instance.ResetPotion();

            // ปิดการทำงานของ Trigger
            trigger.enabled = false;
        }
    }
}