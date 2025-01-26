using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleSystem : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public GameObject puzzlePanel;        // Panel ของ Puzzle
    public TextMeshProUGUI questionText;             // ข้อความแสดงคำถาม
    public TMP_InputField answerInputField;   // ช่องกรอกคำตอบ
    public Button submitButton;           // ปุ่มยืนยันคำตอบ
    public GameObject targetObject;       // Object ที่จะหายไปเมื่อแก้ Puzzle สำเร็จ
    public GameObject interactImage;      // UI Image ที่จะแสดงเมื่อเข้าใกล้
    public Vector3 imageOffset = new Vector3(0, 1.5f, 0); // Offset ของ Interact Image

    private int number1;                  // ตัวเลขที่ 1 ของคำถาม
    private int number2;                  // ตัวเลขที่ 2 ของคำถาม
    private int correctAnswer;            // คำตอบที่ถูกต้อง
    private bool puzzleSolved = false;    // สถานะของ Puzzle
    private bool playerInRange = false;   // ตรวจสอบว่า Player อยู่ในระยะหรือไม่

    private void Start()
    {
        // ซ่อน UI Panel Puzzle และ Interact Image ตอนเริ่มเกม
        puzzlePanel.SetActive(false);
        interactImage.SetActive(false);

        // สุ่มค่าตัวเลขสำหรับคำถาม
        GeneratePuzzle();

        // กำหนดการทำงานของปุ่มยืนยัน
        submitButton.onClick.AddListener(CheckAnswer);
    }

    private void Update()
    {
        if (playerInRange && !puzzleSolved && Input.GetKeyDown(KeyCode.E))
        {
            OpenPuzzlePanel();
        }

        // ตรวจสอบว่าผู้เล่นกดปุ่ม Escape เพื่อปิด Panel
        if (puzzlePanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePuzzlePanel();
        }

        // อัปเดตตำแหน่งของ Interact Image ให้ติดตาม Object
        if (interactImage.activeSelf)
        {
            UpdateInteractImagePosition();
        }
    }

    private void GeneratePuzzle()
    {
        number1 = Random.Range(0, 100);
        number2 = Random.Range(0, 100);
        correctAnswer = number1 + number2;

        // อัปเดตข้อความคำถาม
        questionText.text = $"What is {number1} + {number2}?";
    }

    private void CheckAnswer()
    {
        // อ่านคำตอบจาก Input Field
        if (int.TryParse(answerInputField.text, out int playerAnswer))
        {
            if (playerAnswer == correctAnswer)
            {
                Debug.Log("Correct Answer!");
                SolvePuzzle();
            }
            else
            {
                Debug.Log("Incorrect Answer! Try Again.");
            }
        }
        else
        {
            Debug.Log("Invalid Input! Please enter a number.");
        }
    }

    private void SolvePuzzle()
    {
        // ปิด Panel และเปลี่ยนสถานะ Puzzle
        puzzleSolved = true;
        puzzlePanel.SetActive(false);

        // ซ่อน Interact Image
        interactImage.SetActive(false);

        // คืนเวลา
        Time.timeScale = 1f;

        // ทำให้ Object เป้าหมายหายไป
        if (targetObject != null)
        {
            Destroy(targetObject);
        }
    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !puzzleSolved)
        {
            // แสดง Interact Image เมื่อ Player เข้าใกล้
            interactImage.SetActive(true);
            UpdateInteractImagePosition(); // อัปเดตตำแหน่งของ Interact Image
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // ซ่อน Interact Image และปิดสถานะเมื่อ Player ออกจากระยะ
            interactImage.SetActive(false);
            playerInRange = false;
        }
    }

    private void UpdateInteractImagePosition()
    {
        // แปลงตำแหน่งของ Puzzle Object จาก World Space เป็น Screen Space
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + imageOffset);

        // อัปเดตตำแหน่งของ Interact Image
        interactImage.transform.position = screenPosition;
    }
    
    private void OpenPuzzlePanel()
    {
        // แสดง Panel และหยุดเวลา
        puzzlePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ClosePuzzlePanel()
    {
        // ซ่อน Panel และคืนค่าเวลา
        puzzlePanel.SetActive(false);
        Time.timeScale = 1f;

        // เพิ่มการแจ้งเตือนใน Console
        Debug.Log("Puzzle Panel Closed");
    }
}
