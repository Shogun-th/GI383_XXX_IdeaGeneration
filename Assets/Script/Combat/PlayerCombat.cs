using UnityEngine;
using UnityEngine.UI; // สำหรับ UI

public class PlayerCombat : MonoBehaviour
{
    public GameObject projectilePrefab; // Prefab ของ Projectile (ลูกธนู)
    public Transform shootPoint;        // จุดที่ยิง Projectile ออกมา
    public float shootForce = 20f;      // ความแรงที่ใช้ยิง Projectile
    public float shootCooldown = 1f;    // Cooldown ระหว่างการยิง
    public float projectileLifetime = 4f; // ระยะเวลาที่ลูกธนูจะหายไป
    public float skillKnockbackForce = 5f; // แรงกระแทกเมื่อใช้สกิล
    public float skillCooldown = 3f;   // Cooldown ของสกิล

    public float maxChargeTime = 2f;      // เวลาชาร์จสูงสุด
    public float minShootForce = 10f;     // ความแรงขั้นต่ำของการยิง
    public float maxShootForce = 40f;     // ความแรงสูงสุดของการยิง
    private float chargeTime = 0f;        // เวลาในการชาร์จ
    private bool isCharging = false;      // สถานะการชาร์จ

    public Image chargeUI;                // UI สำหรับแสดงสถานะการชาร์จ
    public Canvas canvas;                 // อ้างอิงถึง Canvas
    private RectTransform chargeUIRect;   // RectTransform ของ UI

    public Vector3 uiOffset = new Vector3(0, 1f, 0); // ตำแหน่ง Offset ของ UI เหนือ Player

    private float nextSkillTime = 0f;  // เวลาในการใช้สกิลครั้งถัดไป
    private const float arrowOffsetAngle = 45f; // มุมเริ่มต้นของลูกธนู (ตะวันออกเฉียงเหนือ)

    void Start()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("Projectile"));

        // ดึง RectTransform ของ chargeUI
        if (chargeUI != null)
        {
            chargeUIRect = chargeUI.GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        // อัปเดตตำแหน่ง UI ให้ตามตำแหน่งของ Player
        UpdateUIPosition();

        // กดค้างเพื่อชาร์จการยิง
        if (Input.GetMouseButton(0))
        {
            isCharging = true;
            chargeTime += Time.deltaTime; // เพิ่มเวลาในการชาร์จ
            chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime); // จำกัดเวลาไม่ให้เกิน maxChargeTime

            // อัปเดต UI การชาร์จ
            if (chargeUI != null)
            {
                chargeUI.fillAmount = chargeTime / maxChargeTime;
            }
        }

        // ปล่อยปุ่มเพื่อยิง
        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            if (chargeTime >= maxChargeTime / 2) // ตรวจสอบว่าชาร์จถึงครึ่งหรือไม่
            {
                ShootProjectile(); // ยิงลูกธนู
            }
            isCharging = false; // รีเซ็ตสถานะการชาร์จ
            chargeTime = 0f;    // รีเซ็ตเวลาในการชาร์จ

            // รีเซ็ต UI การชาร์จ
            if (chargeUI != null)
            {
                chargeUI.fillAmount = 0f;
            }
        }

        // ตรวจสอบการกดปุ่ม E และเวลา Cooldown ของสกิล
        if (Input.GetKeyDown(KeyCode.E) && Time.time >= nextSkillTime)
        {
            UseSkill();
            nextSkillTime = Time.time + skillCooldown; // ตั้งเวลา Cooldown ของสกิล
        }
    }

    void UpdateUIPosition()
    {
        if (chargeUI != null && canvas != null)
        {
            // แปลงตำแหน่ง World Space ของ Player (พร้อม Offset) ไปยัง Screen Space ของ Canvas
            Vector3 worldPosition = transform.position + uiOffset;
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPosition);

            // ตรวจสอบว่าตำแหน่งในหน้าจอยังคงอยู่ในพื้นที่มองเห็น
            if (screenPoint.z > 0) // ตรวจสอบว่าอยู่ด้านหน้ากล้อง
            {
                // ทำให้การอัปเดตตำแหน่ง UI นุ่มนวล
                chargeUIRect.position = Vector3.Lerp(chargeUIRect.position, screenPoint, Time.deltaTime * 10f);
            }
        }
    }



    void ShootProjectile()
    {
        // คำนวณตำแหน่งของเมาส์ในโลก 2D
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        // คำนวณทิศทางจากจุดยิงไปยังตำแหน่งเมาส์
        Vector2 shootDirection = (mousePosition - shootPoint.position).normalized;

        // คำนวณกำลังยิงตามเวลาในการชาร์จ
        float currentShootForce = Mathf.Lerp(minShootForce, maxShootForce, chargeTime / maxChargeTime);

        // คำนวณมุมของการหมุนจากทิศทาง
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        // ปรับมุมหมุนโดยลบ Offset มุมเริ่มต้น
        angle -= arrowOffsetAngle;

        // สร้าง Projectile ที่ตำแหน่งของ shootPoint พร้อมการหมุน
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.Euler(0f, 0f, angle));

        // เพิ่ม Collider2D และ Rigidbody2D ให้ลูกธนูถ้ายังไม่มี
        Collider2D projectileCollider = projectile.GetComponent<Collider2D>();
        if (projectileCollider == null)
        {
            projectileCollider = projectile.AddComponent<BoxCollider2D>();
            projectileCollider.isTrigger = true;
        }
        if (!projectile.TryGetComponent<Rigidbody2D>(out _))
        {
            Rigidbody2D rb = projectile.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }

        // เพิ่มแรงให้ Projectile ด้วย Rigidbody2D
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        if (projectileRb != null)
        {
            projectileRb.AddForce(shootDirection * currentShootForce, ForceMode2D.Impulse);
        }

        // ทำให้ลูกธนูหายไปหลังจากชนกับวัตถุ
        ProjectileBehavior projectileBehavior = projectile.AddComponent<ProjectileBehavior>();
        projectileBehavior.Lifetime = projectileLifetime;

        // ทำลายลูกธนูหลังจากระยะเวลาที่กำหนด
        Destroy(projectile, projectileLifetime);
    }

    void UseSkill()
    {
        // คำนวณตำแหน่งของเมาส์ในโลก 2D
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        // คำนวณทิศทางพื้นฐานจากตำแหน่งเมาส์
        Vector2 baseDirection = (mousePosition - shootPoint.position).normalized;

        // ยิงลูกดอก 3 ทิศทาง
        Vector2[] directions = {
            baseDirection,                                   // ทิศทางตรงไปที่เมาส์
            Quaternion.Euler(0, 0, 30) * baseDirection,     // ทิศทางเฉียงขึ้น 30 องศา
            Quaternion.Euler(0, 0, -30) * baseDirection     // ทิศทางเฉียงลง 30 องศา
        };

        foreach (Vector2 direction in directions)
        {
            // คำนวณมุมของการหมุนจากทิศทาง
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // ปรับมุมหมุนโดยลบ Offset มุมเริ่มต้น
            angle -= arrowOffsetAngle;

            // สร้าง Projectile พร้อมการหมุนในแต่ละทิศทาง
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.Euler(0f, 0f, angle));

            // เพิ่ม Collider2D และ Rigidbody2D ให้ลูกธนูถ้ายังไม่มี
            Collider2D projectileCollider = projectile.GetComponent<Collider2D>();
            if (projectileCollider == null)
            {
                projectileCollider = projectile.AddComponent<BoxCollider2D>();
                projectileCollider.isTrigger = true;
            }
            if (!projectile.TryGetComponent<Rigidbody2D>(out _))
            {
                Rigidbody2D rb = projectile.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0;
            }

            // เพิ่มแรงให้ Projectile ด้วย Rigidbody2D
            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
            if (projectileRb != null)
            {
                projectileRb.AddForce(direction * shootForce, ForceMode2D.Impulse);
            }

            // ทำให้ลูกธนูหายไปหลังจากชนกับวัตถุ
            ProjectileBehavior projectileBehavior = projectile.AddComponent<ProjectileBehavior>();
            projectileBehavior.Lifetime = projectileLifetime;

            // ทำลายลูกธนูหลังจากระยะเวลาที่กำหนด
            Destroy(projectile, projectileLifetime);
        }
    }
}

    public class ProjectileBehavior : MonoBehaviour
    {
        public float Lifetime { get; set; }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // ทำลายลูกธนูเมื่อชนกับวัตถุใด ๆ
            Destroy(gameObject);
        }
}