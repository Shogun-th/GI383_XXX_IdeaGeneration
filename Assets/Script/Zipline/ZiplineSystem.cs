using UnityEngine;

public class ZiplineSystem : MonoBehaviour
{
    [SerializeField] private Transform startPoint; // ตำแหน่งเริ่มต้นของ Zipline
    [SerializeField] private Transform endPoint; // ตำแหน่งปลายทางของ Zipline
    [SerializeField] private float initialZiplineSpeed = 5f; // ความเร็วเริ่มต้นในการโหน
    [SerializeField] private float acceleration = 2f; // อัตราเร่งความเร็ว
    [SerializeField] private KeyCode activateKey = KeyCode.E; // ปุ่มสำหรับเริ่ม Zipline

    private bool isUsingZipline = false; // ตรวจสอบว่ากำลังโหนอยู่หรือไม่
    private bool hasUsedZipline = false; // ตรวจสอบว่าถูกใช้งานไปแล้วหรือยัง
    private Transform player; // อ้างอิงถึง Player

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isUsingZipline && !hasUsedZipline)
        {
            player = collision.transform; // เก็บข้อมูล Transform ของ Player
            Debug.Log("Player entered Zipline trigger!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isUsingZipline)
        {
            player = null; // ล้างข้อมูล Player เฉพาะเมื่อไม่ได้โหน
            Debug.Log("Player exited Zipline trigger!");
        }
    }

    private void Update()
    {
        if (player != null && Input.GetKeyDown(activateKey) && !isUsingZipline && !hasUsedZipline)
        {
            Debug.Log("Zipline activated!");
            StartCoroutine(UseZipline());
        }
    }

    private System.Collections.IEnumerator UseZipline()
    {
        isUsingZipline = true;

        // เคลื่อนที่ Player จาก startPoint ไป endPoint
        float time = 0;
        float currentSpeed = initialZiplineSpeed; // เริ่มต้นความเร็วด้วยค่าเริ่มต้น
        Vector3 initialPosition = startPoint.position;
        Vector3 finalPosition = endPoint.position;

        while (time < 1f)
        {
            time += Time.deltaTime * currentSpeed / Vector3.Distance(initialPosition, finalPosition);
            currentSpeed += acceleration * Time.deltaTime; // เพิ่มความเร็วตามอัตราเร่ง

            if (player != null)
            {
                player.position = Vector3.Lerp(initialPosition, finalPosition, time);
            }
            yield return null;
        }

        // ให้แน่ใจว่า Player อยู่ที่ปลายทาง
        if (player != null)
        {
            player.position = finalPosition;
        }

        isUsingZipline = false;
        hasUsedZipline = true; // ตั้งค่าว่า Zipline ถูกใช้งานแล้ว
        Debug.Log("Zipline finished!");
    }
}
