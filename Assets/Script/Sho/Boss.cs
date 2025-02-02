using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharState
{
    Idle,
    Walk,
    Attack,
    Hit,
    Die
}

public class Boss : MonoBehaviour
{
    public float speed = 2f;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public float maxHealth = 100f;
    public float damageAmount = 10f;
    public LayerMask groundLayer;

    private Transform player;
    private float attackTimer = 0f;
    private float currentHealth;
    private bool isStunned = false;
    private bool facingRight = true;

    public Transform groundCheck;
    public float groundCheckDistance = 0.5f;
    public Rigidbody2D rb;
    public Animator animator;
    private CharState currentState = CharState.Idle;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isStunned) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer <= attackRange)
            {
                AttackPlayer();
            }
            else
            {
                ChasePlayer();
            }
        }
        else
        {
            Patrol();
        }

        DrawDetectionVisuals();
    }

    private void Patrol()
    {
        if (!IsGrounded() || IsAtEdge())
        {
            Flip();
        }

        rb.velocity = new Vector2(speed * (facingRight ? 1 : -1), rb.velocity.y);
        ChangeState(CharState.Walk);
    }

    private void ChasePlayer()
    {
        float directionX = player.position.x - transform.position.x;
        if (directionX > 0 && !facingRight) Flip();
        if (directionX < 0 && facingRight) Flip();

        rb.velocity = new Vector2(speed * Mathf.Sign(directionX), rb.velocity.y);
        ChangeState(CharState.Walk);
    }

    private void AttackPlayer()
    {
        if (attackTimer <= 0)
        {
            ChangeState(CharState.Attack);
            PlayerStats.Instance.TakeDamage(damageAmount);
            attackTimer = attackCooldown;
            Invoke("ResetAttack", 0.5f); // รีเซ็ต Animation หลังจาก 0.5 วินาที
        }
        attackTimer -= Time.deltaTime;
    }

    private void ResetAttack()
    {
        ChangeState(CharState.Idle);
    }

    private bool IsGrounded()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
    }

    private bool IsAtEdge()
    {
        return !Physics2D.Raycast(groundCheck.position + new Vector3(facingRight ? 0.5f : -0.5f, 0, 0), Vector2.down, groundCheckDistance, groundLayer);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void DrawDetectionVisuals()
    {
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.green);
    }

    private void ChangeState(CharState newState)
    {
        if (currentState == newState) return;

        // Reset Animator ทุกครั้งที่เปลี่ยน State
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isDead", false);

        currentState = newState;
        switch (currentState)
        {
            case CharState.Idle:
                rb.velocity = Vector2.zero;
                break;
            case CharState.Walk:
                animator.SetBool("isWalking", true);
                break;
            case CharState.Attack:
                animator.SetBool("isAttacking", true);
                rb.velocity = Vector2.zero; // หยุดเคลื่อนที่ตอนโจมตี
                break;
            case CharState.Hit:
                animator.SetTrigger("isHit");
                break;
            case CharState.Die:
                animator.SetBool("isDead", true);
                rb.velocity = Vector2.zero;
                break;
        }
    }
}
