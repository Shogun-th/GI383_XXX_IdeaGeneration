using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneBoss : MonoBehaviour
{
    public Boss boss; // 🔥 ลิงก์ไปที่บอส

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // ถ้าผู้เล่นเข้าโซน
        {
            if (boss != null)
            {
                boss.ActivateBoss(); // 🔥 เปิด UI และเริ่มการต่อสู้
            }
        }
    }
}
