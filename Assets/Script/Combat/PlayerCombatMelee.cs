using System.Collections;
using UnityEngine;

public class PlayerCombatMelee : MonoBehaviour
{
    public float[] attackDamages = { 10f, 20f, 30f, 40f }; // ดาเมจสำหรับแต่ละรูปแบบการโจมตี
    //public Animator animator; // ใส่ Animator Component ที่ผูกกับตัวละคร
    public Transform attackPoint; // ตำแหน่งโจมตี
    public float attackRange = 0.5f; // ระยะโจมตี
    public LayerMask enemyLayer; // กำหนด Layer ของ Enemy

    private int attackIndex = 0; // ติดตามการโจมตีลำดับปัจจุบัน
    private bool isAttacking = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking) // คลิกซ้ายเพื่อโจมตี
        {
            Debug.Log("Attck!!");
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        // เล่นแอนิเมชันที่เกี่ยวข้อง
        string attackAnimation = "Attack" + (attackIndex + 1); // เช่น "Attack1", "Attack2"
        //animator.Play(attackAnimation); <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

        yield return new WaitForSeconds(0.1f); // รอช่วงเวลาที่แอนิเมชันถึงเฟรมโจมตี

        // ตรวจจับศัตรูในระยะโจมตี
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit " + enemy.name);
            enemy.GetComponent<EnemyAi>().TakeDamage(attackDamages[attackIndex]);
        }

        yield return new WaitForSeconds(0.5f); // รอจนกว่าแอนิเมชันจะจบ

        // เปลี่ยนรูปแบบการโจมตี (วนกลับมาเริ่มที่ 0 เมื่อจบรูปแบบทั้งหมด)
        attackIndex = (attackIndex + 1) % attackDamages.Length;

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