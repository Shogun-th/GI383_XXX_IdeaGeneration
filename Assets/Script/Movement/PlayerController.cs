using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public float dashDamage = 50f;
    public float dashRadius = 0.5f;
    public LayerMask enemyLayer;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public Image dashCooldownImage;
    public TextMeshProUGUI dashCooldownText;

    private Rigidbody2D rb;
    //private Animator animator; // ตัวแปร Animator <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    private bool isGrounded;
    private bool isDashing;
    private float dashCooldownTime;
    private bool isFacingRight = true;

    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // animator = GetComponent<Animator>(); // ดึง Animator Component <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    }

    void Update()
    {
        // หากมีการเปิด Dialogue ให้หยุดเวลา
        if (DialogueManager.Instance.isDialogueActive)
        {
            if (Time.timeScale != 0f) // ตรวจสอบว่าเวลายังไม่ได้หยุด
            {
                Time.timeScale = 0f; // หยุดเวลา
            }
            return;
        }
        else if (!DialogueManager.Instance.isDialogueActive && Time.timeScale == 0f) // หาก Dialogue ปิดและเวลายังหยุดอยู่
        {
            Time.timeScale = 1f; // คืนค่าเวลา
        }
        
        
        if (isDashing) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // การเคลื่อนไหว
        moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // อัปเดตสถานะ Walking ใน Animator
        //animator.SetBool("IsWalking", moveInput != 0 && isGrounded); <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

        // การกระโดด
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            //animator.SetBool("IsJumping", true); // เปิดสถานะ Jumping <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        }

        if (isGrounded)
        {
            //animator.SetBool("IsJumping", false); // ปิดสถานะ Jumping <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        }

        // ระบบ Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > dashCooldownTime)
        {
            PerformDash(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        UpdateDashCooldownUI();
        FlipTowardsMouse();
    }

    private void PerformDash(Vector3 targetPosition)
    {
        //animator.SetBool("IsDashing", true); // เปิดสถานะ Dashing <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        StartCoroutine(DashCoroutine(targetPosition));
    }

    private System.Collections.IEnumerator DashCoroutine(Vector3 targetPosition)
    {
        isDashing = true;
        dashCooldownTime = Time.time + dashCooldown;

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);

        Vector2 dashDirection = ((Vector2)targetPosition - (Vector2)transform.position).normalized;
        rb.velocity = dashDirection * dashSpeed;

        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, dashRadius, enemyLayer);
            foreach (Collider2D enemy in hitEnemies)
            {
                EnemyAi enemyScript = enemy.GetComponent<EnemyAi>();
                if (enemyScript != null)
                {
                    enemyScript.TakeDamage(dashDamage);
                    Debug.Log($"Enemy hit by Dash: {enemy.name}");
                }
            }

            yield return null;
        }

        rb.velocity = Vector2.zero;

        //animator.SetBool("IsDashing", false); // ปิดสถานะ Dashing <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        isDashing = false;

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), false);
    }

    private void FlipTowardsMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mousePosition.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (mousePosition.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void UpdateDashCooldownUI()
    {
        if (dashCooldownImage != null)
        {
            float remainingCooldown = Mathf.Clamp(dashCooldownTime - Time.time, 0, dashCooldown);
            dashCooldownImage.fillAmount = remainingCooldown / dashCooldown;

            if (dashCooldownText != null)
            {
                dashCooldownText.text = remainingCooldown > 0 ? remainingCooldown.ToString("F1") : "";
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dashRadius);
    }
}
