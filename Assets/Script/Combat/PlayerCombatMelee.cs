using System.Collections;
using UnityEngine;

public class PlayerCombatMelee : MonoBehaviour
{
    public float[] attackDamages = { 10f, 20f, 30f, 40f }; // ดาเมจสำหรับแต่ละรูปแบบการโจมตี
    public Animator animator; // ใส่ Animator Component ที่ผูกกับตัวละคร
    public Transform attackPoint; // ตำแหน่งโจมตี
    public float attackRange = 0.5f; // ระยะโจมตี
    public LayerMask enemyLayer; // กำหนด Layer ของ Enemy

    private int attackIndex = 0; // ติดตามการโจมตีลำดับปัจจุบัน
    private bool isAttacking = false;
    
    private float attackResetTimer = 1f;
    private float timeSinceLastAttack = 0f;
    private bool isComboActive = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            // ตั้งค่าให้เริ่มโจมตีจากลำดับที่ 1
            if (attackIndex == 0)
            {
                attackIndex = 1;  // เริ่มการโจมตีที่ลำดับ 1
            }

            timeSinceLastAttack = 0f;
            StartCoroutine(PerformAttack());
        }

        if (!Input.GetMouseButton(0))
        {
            timeSinceLastAttack += Time.deltaTime;

            if (timeSinceLastAttack >= attackResetTimer && !isAttacking)
            {
                // รีเซ็ตค่าเมื่อไม่มีการโจมตีต่อเนื่อง
                animator.SetInteger("AttackIndex", 0);
                animator.SetBool("IsAttacking", false);
                attackIndex = 0;  // กลับไปสถานะ Idle
                isComboActive = false;
            }
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        isComboActive = true;

        // ตั้งค่า AttackIndex ใน Animator ให้ตรงกับลำดับปัจจุบัน
        animator.SetInteger("AttackIndex", attackIndex);
        animator.SetBool("IsAttacking", true);

        yield return new WaitForSeconds(0.1f);  // รอให้แอนิเมชันเข้าสู่เฟรมโจมตี

        // ตรวจจับศัตรูในระยะโจมตี
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyAi enemyAi = enemy.GetComponent<EnemyAi>();
            Boss boss = enemy.GetComponent<Boss>(); // เช็คว่าเป็นบอสรึเปล่า

            if (enemyAi != null) // ถ้าเป็นศัตรูทั่วไป
            {
                enemyAi.TakeDamage(attackDamages[attackIndex - 1]);
                Debug.Log("Hit enemy: " + enemy.name);
            }
            else if (boss != null) // ถ้าเป็นบอส
            {
                boss.TakeDamage(attackDamages[attackIndex - 1]);
                Debug.Log("Hit boss: " + enemy.name);
            }
            else
            {
                Debug.LogWarning("Hit an object without EnemyAi or Boss: " + enemy.name);
            }
        }


        yield return new WaitForSeconds(0.5f);  // รอจนแอนิเมชันจบ

        // วนลำดับการโจมตีต่อไป
        attackIndex = (attackIndex % attackDamages.Length) + 1;

        // ปิดสถานะการโจมตีใน Animator
        animator.SetBool("IsAttacking", false);
        isAttacking = false;
    }
    
    private void OnDrawGizmosSelected()
    {
        // วาดวงแสดงระยะโจมตีใน Scene View
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}