 using System.Collections;
using System.Collections.Generic;
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

    private Transform player;
    private float attackTimer = 0f;
    private float currentHealth;
    private bool isPatrolling = true;
    private bool facingRight = true;

    public Transform groundCheck;
    public Transform detectionOrigin;
    public float groundCheckDistance = 0.5f;
    public float detectionVisualRadius = 0.2f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            isPatrolling = false;

            if (distanceToPlayer <= attackRange && IsPlayerInFront())
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
            isPatrolling = true;
            Patrol();
        }

        DrawDetectionVisuals();
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
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown)
        {
            Debug.Log("Enemy attacks the player!");
            PlayerStats.Instance.TakeDamage(damageAmount);
            attackTimer = 0f;
        }
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

    private bool IsPlayerInFront()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        return (facingRight && directionToPlayer.x > 0) || (!facingRight && directionToPlayer.x < 0);
    }

    private void DrawDetectionVisuals()
    {
        // วาดเส้นแสดงระยะตรวจจับ
        Debug.DrawLine(transform.position, transform.position + Vector3.right * detectionRange * (facingRight ? 1 : -1), Color.red);
        Debug.DrawLine(transform.position, transform.position + Vector3.right * attackRange * (facingRight ? 1 : -1), Color.yellow);

        // วาดวงกลมสำหรับ Ground Check
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.green);
    }

    public void TakeDamage(float damage) // ดึงไปถ้าอยากฆ่าศัตรู
    {
        currentHealth -= damage;
        Debug.Log("Enemy Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }
}
