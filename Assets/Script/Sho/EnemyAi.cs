using System.Collections;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    public float speed = 2f;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public float maxHealth = 100f;
    public float damageAmount = 10f;
    public LayerMask groundLayer;
    public LayerMask enemyLayer;
    private float avoidCooldown = 0f;  // ดีเลย์ระหว่างการหลบศัตรู
    public float avoidCooldownTime = 1f; // กำหนดเวลา cooldown

    private Transform player;
    public Transform enemyCheckPoint;
    private float attackTimer = 0f;
    private float currentHealth;
    private bool isPatrolling = true;
    private bool facingRight = true;
    private bool isStunned = false;
    private bool isDead = false;  // เพิ่มตัวแปรเพื่อตรวจสอบสถานะการตาย
    private bool isAttacking = false;


    public Transform groundCheck;
    public float groundCheckDistance = 0.5f;
    public float enemyCheckRadius = 0.5f;

    public Rigidbody2D rb;
    public float knockbackForce = 5f;
    public float stunDuration = 1f;
    private Animator animator;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;  // หยุดการทำงานใน Update ถ้าศัตรูตายแล้ว
        if (isStunned) return;

        // นับเวลาคูลดาวน์ก่อนเช็คการหลบศัตรู
        if (avoidCooldown <= 0)
        {
            AvoidOtherEnemies();
            avoidCooldown = avoidCooldownTime;  // ตั้งคูลดาวน์ใหม่
        }
        else
        {
            avoidCooldown -= Time.deltaTime;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            isPatrolling = false;

            if (distanceToPlayer <= attackRange && IsPlayerInFront())
            {
                if (!isAttacking)
                {
                    animator.SetBool("IsWalking", false);
                    animator.SetBool("Attack", true);
                    isAttacking = true;
                }
                AttackPlayer();
            }
            else
            {
                isAttacking = false;
                animator.SetBool("Attack", false);
                animator.SetBool("IsWalking", true);
                ChasePlayer();
            }
        }
        else
        {
            isPatrolling = true;
            animator.SetBool("IsWalking", true);
            Patrol();
        }

        DrawDetectionVisuals();
    }

    private void AvoidOtherEnemies()
    {
        Collider2D enemyNearby = Physics2D.OverlapCircle(enemyCheckPoint.position, enemyCheckRadius, enemyLayer);

        if (enemyNearby != null)  // ถ้ามีศัตรูอยู่ข้างหน้า
        {
            Vector3 directionToEnemy = enemyNearby.transform.position - transform.position;

            // เช็คว่าศัตรูอยู่ด้านหน้าจริงๆ ก่อนจะ Flip()
            if ((facingRight && directionToEnemy.x > 0) || (!facingRight && directionToEnemy.x < 0))
            {
                Flip();
            }
        }
    }
    private void Patrol()
    {
        if (IsAtEdge() || !IsGrounded())
        {
            Flip();
        }

        transform.Translate(Vector2.right * speed * Time.deltaTime * (facingRight ? 1 : -1));
    }

    private void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if ((direction.x > 0 && !facingRight) || (direction.x < 0 && facingRight))
        {
            Flip();
        }
    }
    
    private void AttackPlayer()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            return;
        }

        // ตรวจสอบว่าผู้เล่นอยู่ในระยะและอยู่ด้านหน้า
        if (Vector3.Distance(transform.position, player.position) <= attackRange && IsPlayerInFront())
        {
            Debug.Log("Enemy Attacking!");

            // เล่น Animation โจมตี
            animator.SetBool("Attack", true);

            // ตรวจสอบว่าผู้เล่นยังไม่ตายก่อนลด HP
            if (PlayerStats.Instance != null)
            {
                Debug.Log("Player took damage!");
                PlayerStats.Instance.TakeDamage(damageAmount);
            }
            else
            {
                Debug.LogWarning("PlayerStats Instance is null!");
            }
        }

        // ตั้งค่า cooldown
        attackTimer = attackCooldown;

        // ปิด Animation หลังจากโจมตีเสร็จ
        Invoke("ResetAttackAnimation", 0.5f);
    }


    public void TakeDamage(float damage)
    {
        if (isDead) return;  // ถ้าศัตรูตายแล้ว หยุดการทำงานของ TakeDamage

        currentHealth -= damage;
        Debug.Log("Enemy Health: " + currentHealth);

        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(ApplyKnockbackAndStun());
        }
    }

    private IEnumerator ApplyKnockbackAndStun()
    {
        isStunned = true;

        Vector2 knockbackDirection = (transform.position.x > player.position.x ? Vector2.right : Vector2.left);
        knockbackDirection.Normalize();

        rb.velocity = Vector2.zero;
        rb.AddForce((knockbackDirection + Vector2.up) * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
    }

    private void Die()
    {
        if (isDead) return;  // ป้องกันการเรียก Die ซ้ำ

        isDead = true;

        // ปิดแอนิเมชันอื่น ๆ ก่อนเล่นแอนิเมชัน Die
        animator.SetBool("Attack", false);
        animator.ResetTrigger("Hurt");

        animator.SetTrigger("Die");  // เรียกแอนิเมชัน Die
        Debug.Log("Enemy died!");

        StartCoroutine(WaitAndDestroy());
    }

    private IEnumerator WaitAndDestroy()
    {
        // รอให้แอนิเมชัน Die เล่นจบ
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        Destroy(gameObject);  // ทำลายวัตถุ
    }

    private bool IsGrounded()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
    }

    private bool IsAtEdge()
    {
        return !Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    private void ResetAttackAnimation()
    {
        animator.SetBool("Attack", false);
    }
    private bool IsPlayerInFront()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        return (facingRight && directionToPlayer.x > 0) || (!facingRight && directionToPlayer.x < 0);
    }

    private void DrawDetectionVisuals()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.right * detectionRange * (facingRight ? 1 : -1), Color.red);
        Debug.DrawLine(transform.position, transform.position + Vector3.right * attackRange * (facingRight ? 1 : -1), Color.yellow);
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.green);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(enemyCheckPoint.position, enemyCheckRadius);
    }
}

