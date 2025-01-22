using UnityEngine;

public class TrapSystem : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab; // Prefab ของลูกธนู
    [SerializeField] private Transform arrowSpawnPoint; // ตำแหน่งที่ลูกธนูจะถูกยิงออกไป
    [SerializeField] private float arrowSpeed = 10f; // ความเร็วของลูกธนู
    [SerializeField] private float cooldownTime = 3f; // เวลาคูลดาวน์ (กำหนดได้ใน Inspector)

    private bool isOnCooldown = false; // ตรวจสอบสถานะการใช้งานกับดัก

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isOnCooldown)
        {
            ShootArrow(collision.transform); // ยิงลูกธนูไปยัง Player
            Debug.Log("HIT!");

            // เริ่มต้นคูลดาวน์
            StartCooldown();
        }
    }

    private void ShootArrow(Transform target)
    {
        // คำนวณทิศทางจากจุดยิงไปยัง Player
        Vector2 direction = (target.position - arrowSpawnPoint.position).normalized;

        // คำนวณมุมเพื่อหมุนลูกธนู
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // สร้างลูกธนูจาก Prefab และหมุนไปในมุมที่คำนวณได้
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.Euler(0, 0, angle - 45)); // ปรับแกนให้ตรงกับหัวลูกธนู

        // เพิ่มแรงให้ลูกธนูเคลื่อนที่เข้าหา Player
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * arrowSpeed;
        }

        // ทำลายลูกธนูหลังจากเวลาหนึ่ง (เช่น 5 วินาที)
        Destroy(arrow, 5f);
    }

    private void StartCooldown()
    {
        isOnCooldown = true; // ตั้งค่าสถานะเป็นคูลดาวน์
        Invoke(nameof(ResetCooldown), cooldownTime); // เรียก ResetCooldown หลังจาก cooldownTime วินาที
    }

    private void ResetCooldown()
    {
        isOnCooldown = false; // ตั้งค่าสถานะพร้อมใช้งานอีกครั้ง
    }
}