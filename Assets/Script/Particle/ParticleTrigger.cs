using UnityEngine;

public class ParticleTrigger : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem; // Particle System ที่จะหยุดทำงาน

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (particleSystem != null)
            {
                particleSystem.Stop(); // หยุดการทำงานของ Particle System
                Debug.Log("Particle System Stopped!");
            }
        }
    }
}