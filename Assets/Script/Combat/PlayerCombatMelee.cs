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
        if (Input.GetMouseButtonDown(0) && !isAttacking)  // เริ่มโจมตี
        {
            //Debug.Log("Start Attack: AttackIndex = " + attackIndex);
            timeSinceLastAttack = 0f;
            StartCoroutine(PerformAttack());
        }

        if (!Input.GetMouseButton(0))
        {
            timeSinceLastAttack += Time.deltaTime;

            if (timeSinceLastAttack >= attackResetTimer && !isAttacking)
            {
                //Debug.Log("Resetting AttackIndex to -1");
                animator.SetInteger("AttackIndex", -1);
                animator.SetBool("IsAttacking", false);
                attackIndex = 0;
                isComboActive = false;
            }
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        isComboActive = true;

        /*// เล่นแอนิเมชันที่เกี่ยวข้อง
        string attackAnimation = "Attack" + (attackIndex + 1); // เช่น "Attack1", "Attack2"
        animator.SetInteger("AttackIndex", attackIndex);*/

        // ตั้งค่า AttackIndex ใน Animator ให้ตรงกับลำดับปัจจุบัน
        animator.SetInteger("AttackIndex", attackIndex);
        animator.SetBool("IsAttacking", true); // ตั้งค่าเป็น true เมื่อเริ่มโจมตี
        
        yield return new WaitForSeconds(0.1f); // รอช่วงเวลาที่แอนิเมชันถึงเฟรมโจมตี

        // ตรวจจับศัตรูในระยะโจมตี
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit " + enemy.name);
            enemy.GetComponent<EnemyAi>().TakeDamage(attackDamages[attackIndex]);
        }

        yield return new WaitForSeconds(0.5f); // รอจนแอนิเมชันจบ

        // เปลี่ยนรูปแบบการโจมตีไปยังลำดับถัดไป
        attackIndex = (attackIndex + 1) % attackDamages.Length;

        // ปิดสถานะการโจมตีใน Animator
        //animator.SetInteger("AttackIndex", -1);
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