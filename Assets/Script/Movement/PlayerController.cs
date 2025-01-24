using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // ตัวแปรสำหรับการเคลื่อนไหว
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private float originalMoveSpeed;

    // ตัวแปรสำหรับการตรวจจับพื้น
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    // UI Cooldown
    public Image dashCooldownImage;
    public TextMeshProUGUI dashCooldownText;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isDashing;
    private float dashTime;
    private float dashCooldownTime;

    private float moveInput;
    private bool isFacingRight = true; // ตัวแปรตรวจสอบทิศทางที่ผู้เล่นหันหน้า

    void Start()
    {
        // ดึง Component Rigidbody2D มาใช้งาน
        rb = GetComponent<Rigidbody2D>();

        // ตั้งค่าเริ่มต้นของ HP
        GameManager.Instance.ResetHealth();
    }

    void Update()
    {
        // หากมีการเปิด Dialogue ให้หยุดการเคลื่อนไหว
        if (DialogueManager.Instance.isDialogueActive)
        {
            if (moveSpeed != 0f) // เก็บค่าความเร็วเดิมก่อนหยุด
            {
                originalMoveSpeed = moveSpeed;
                moveSpeed = 0f;
            }
            return;
        }
        else if (moveSpeed == 0f && !DialogueManager.Instance.isDialogueActive) // หาก Dialogue ปิดและ moveSpeed ยังเป็น 0
        {
            moveSpeed = originalMoveSpeed; // คืนค่าความเร็วเดิม
        }
        // การตรวจจับว่าผู้เล่นอยู่บนพื้นหรือไม่
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // การเคลื่อนไหวซ้าย-ขวา
        if (!isDashing) // หยุดการเคลื่อนไหวปกติขณะ Dash
        {
            moveInput = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }

        // การกระโดด
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // การ Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > dashCooldownTime && moveInput != 0)
        {
            StartDash();
        }

        UpdateDashCooldownUI();

        // หันหน้าตามตำแหน่งเมาส์ทางซ้ายหรือขวา
        FlipTowardsMouse();

        // การลด HP ด้วยการทดสอบ
        if (Input.GetKeyDown(KeyCode.G))
        {
            TakeDamage(10);
        }

        // การเพิ่ม HP ด้วยการทดสอบ
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(10);
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashTime = Time.time + dashDuration;
        dashCooldownTime = Time.time + dashCooldown;

        // เพิ่มความเร็ว Dash ในทิศทางการเคลื่อนไหวปัจจุบัน
        rb.velocity = new Vector2(moveInput * dashSpeed, rb.velocity.y);

        // เรียก StopDash หลังจากเวลาที่กำหนด
        Invoke(nameof(StopDash), dashDuration);
    }

    void StopDash()
    {
        isDashing = false;
        rb.velocity = new Vector2(0, rb.velocity.y); // หยุดความเร็ว Dash
    }

    void FlipTowardsMouse()
    {
        // รับตำแหน่งเมาส์ในโลก 2D
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // เช็คว่าตำแหน่งเมาส์อยู่ทางซ้ายหรือขวาของตัวละคร
        if (mousePosition.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (mousePosition.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        // เปลี่ยนทิศทางการหันหน้าของตัวผู้เล่น
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    void UpdateDashCooldownUI()
    {
        if (dashCooldownImage != null)
        {
            // อัปเดต Fill ของ Image
            float remainingTime = Mathf.Clamp(dashCooldownTime - Time.time, 0, dashCooldown);
            dashCooldownImage.fillAmount = remainingTime / dashCooldown;

            // อัปเดตตัวเลขนับถอยหลัง
            if (dashCooldownText != null)
            {
                if (remainingTime > 0)
                {
                    dashCooldownText.text = remainingTime.ToString("F1"); // แสดงเวลาเป็นทศนิยม 1 ตำแหน่ง
                }
                else
                {
                    dashCooldownText.text = ""; // ซ่อนข้อความเมื่อ Dash พร้อมใช้งาน
                }
            }
        }
    }

    private void TakeDamage(int damage)
    {
        GameManager.Instance.DecreaseHealth(damage);
        Debug.Log("Current HP: " + GameManager.Instance.GetHealth());

        if (GameManager.Instance.GetHealth() <= 0)
        {
            Die();
        }
    }

    private void Heal(int amount)
    {
        GameManager.Instance.IncreaseHealth(amount);
        Debug.Log("Current HP after healing: " + GameManager.Instance.GetHealth());
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        // ใส่ logic เมื่อตัวละครตาย เช่นหยุดการเคลื่อนไหวหรือรีเซ็ตเกม
    }

    private void OnDrawGizmosSelected()
    {
        // แสดง Gizmo สำหรับ groundCheck ใน Editor
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
