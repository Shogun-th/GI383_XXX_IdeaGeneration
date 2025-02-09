using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // ใช้สำหรับโหลดฉากใหม่

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // ใส่ Prefab ของศัตรู
    public Transform[] spawnPoints; // เก็บจุดเกิดของศัตรู
    public int enemyCount = 3; // จำนวนศัตรูที่เกิดต่อรอบ
    public int maxRounds = 5; // จำนวนรอบสูงสุดที่ศัตรูจะเกิด
    public string nextSceneName; // กำหนดชื่อฉากถัดไป

    private int currentRound = 0; // รอบปัจจุบัน
    private List<GameObject> activeEnemies = new List<GameObject>(); // เก็บศัตรูที่เกิดแล้ว

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (currentRound < maxRounds)
        {
            yield return new WaitForSeconds(2f); // รอ 2 วินาทีก่อนเช็คว่าศัตรูตายหมดหรือยัง

            if (AllEnemiesDead())
            {
                Debug.Log("All enemies defeated! Spawning new wave...");
                SpawnWave();
                currentRound++;
            }
        }

        Debug.Log("Max rounds reached. No more enemies will spawn.");

        // ตรวจสอบว่าศัตรูรอบสุดท้ายตายหมดแล้ว
        yield return new WaitUntil(AllEnemiesDead);

        Debug.Log("All enemies in the last wave are defeated! Changing scene...");
        SceneManager.LoadScene(nextSceneName); // โหลดฉากใหม่
    }

    private void SpawnWave()
    {
        activeEnemies.Clear(); // ล้างข้อมูลศัตรูเก่าที่ตายแล้ว

        for (int i = 0; i < enemyCount; i++)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Length); // เลือกจุดเกิดแบบสุ่ม
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity);
            activeEnemies.Add(newEnemy);
        }
    }

    private bool AllEnemiesDead()
    {
        activeEnemies.RemoveAll(enemy => enemy == null); // ลบศัตรูที่ถูกทำลายไปแล้ว
        return activeEnemies.Count == 0; // ถ้าศัตรูหมดให้คืนค่า true
    }
}




