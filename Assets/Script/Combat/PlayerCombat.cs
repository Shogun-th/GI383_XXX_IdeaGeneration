using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public GameObject projectilePrefab; // Prefab ของ Projectile (ลูกธนู)
    public Transform shootPoint;        // จุดที่ยิง Projectile ออกมา
    public float shootForce = 20f;      // ความแรงที่ใช้ยิง Projectile
    public float shootCooldown = 1f;    // Cooldown ระหว่างการยิง
    public float projectileLifetime = 4f; // ระยะเวลาที่ลูกธนูจะหายไป

    private float nextShootTime = 0f;   // เวลาในการยิงครั้งต่อไป
    private const float arrowOffsetAngle = 45f; // มุมเริ่มต้นของลูกธนู (ตะวันออกเฉียงเหนือ)

    void Update()
    {
        // ตรวจสอบการคลิกซ้ายและเวลา Cooldown
        if (Input.GetMouseButtonDown(0) && Time.time >= nextShootTime)
        {
            ShootProjectile();
            nextShootTime = Time.time + shootCooldown; // ตั้งเวลา Cooldown ครั้งถัดไป
        }
    }

    void ShootProjectile()
    {
        // คำนวณตำแหน่งของเมาส์ในโลก 2D
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // ตั้งค่า Z เป็น 0 เนื่องจากเป็น 2D

        // คำนวณทิศทางจากจุดยิงไปยังตำแหน่งเมาส์
        Vector2 shootDirection = (mousePosition - shootPoint.position).normalized;

        // คำนวณมุมของการหมุนจากทิศทาง
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        // ปรับมุมหมุนโดยลบ Offset มุมเริ่มต้น (45°)
        angle -= arrowOffsetAngle;

        // สร้าง Projectile ที่ตำแหน่งของ shootPoint พร้อมการหมุน
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.Euler(0f, 0f, angle));

        // ทำลายลูกธนูหลังจาก 6 วินาที
        Destroy(projectile, projectileLifetime);

        // เพิ่มแรงให้ Projectile ด้วย Rigidbody2D
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(shootDirection * shootForce, ForceMode2D.Impulse); // ยิงไปในทิศทางที่เมาส์คลิก
        }
    }
}