using UnityEngine;

public class ArrowTraps : MonoBehaviour
{
    [SerializeField] private float damage = 10f; // ความเสียหายที่ลูกธนูทำได้
    private bool hasHit = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        if (collision.CompareTag("Player"))
        {
            hasHit = true;

            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.TakeDamage(damage);
                Debug.Log("Damage dealt to Player");
            }
            else
            {
                Debug.LogWarning("PlayerStats.Instance is null!");
            }

            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall"))
        {
            hasHit = true;
            Debug.Log("Arrow hit Wall");
            Destroy(gameObject);
        }
    }

}