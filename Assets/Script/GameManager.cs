using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int maxHealth = 100; // ปรับค่าได้ใน Inspector
    private int playerHealth;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResetHealth();
    }

    public int GetHealth()
    {
        return playerHealth;
    }

    public void IncreaseHealth(int amount)
    {
        playerHealth = Mathf.Clamp(playerHealth + amount, 0, maxHealth);
    }

    public void DecreaseHealth(int amount)
    {
        playerHealth = Mathf.Clamp(playerHealth - amount, 0, maxHealth);
        if (playerHealth <= 0)
        {
            OnPlayerDeath();
        }
    }

    public void ResetHealth()
    {
        playerHealth = maxHealth;
    }

    private void OnPlayerDeath()
    {
        Debug.Log("Player has died!");
        // Add additional logic for when the player dies, e.g., restart the game, show game over screen, etc.
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        playerHealth = Mathf.Clamp(playerHealth, 0, maxHealth);
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}