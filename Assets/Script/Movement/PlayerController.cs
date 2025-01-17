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

    void Start()
    {
        // ดึง Component Rigidbody2D มาใช้งาน
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
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
