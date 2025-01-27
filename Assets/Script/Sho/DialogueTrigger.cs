using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DialogueCharacter
{
    public string name;
    public Sprite icon;
}


[System.Serializable]
public class Dialogueline
{
    public DialogueCharacter character;
    [TextArea(3, 10)]
    public string line;
}


[System.Serializable]
public class Dialogue
{
    public List<Dialogueline> dialoguelines = new List<Dialogueline>();
}


public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    private bool playerInRange = false; // ตรวจสอบว่าผู้เล่นอยู่ในระยะหรือไม่
    [Header("Scene Transition")]
    public string sceneToLoad; // ชื่อฉากที่ต้องการโหลด
    private bool isSubscribedToEvent = false; // ตัวแปรตรวจสอบว่ามีการสมัคร Event หรือยัง

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
        // ตรวจสอบว่ามีชื่อฉากระบุไว้หรือไม่
        if (!string.IsNullOrEmpty(sceneToLoad) && !isSubscribedToEvent)
        {
            DialogueManager.Instance.onDialogueEnd += LoadScene; // สมัครฟังก์ชัน LoadScene เมื่อสนทนาจบ
            isSubscribedToEvent = true; // ตั้งค่าว่ามีการสมัคร Event แล้ว
        }
    }

    private void Update()
    {
        // หากผู้เล่นอยู่ในระยะและกดปุ่ม E
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TriggerDialogue();

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true; // ตั้งค่าว่าผู้เล่นอยู่ในระยะ
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false; // ตั้งค่าว่าผู้เล่นออกจากระยะ
        }
    }
    private void LoadScene()
    {
        // ตรวจสอบว่าฉากที่ระบุมีชื่อถูกต้อง
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Scene name is not set in the Inspector!");
        }

        // ยกเลิกการสมัคร Event หลังจากเปลี่ยนฉาก
        DialogueManager.Instance.onDialogueEnd -= LoadScene;
        isSubscribedToEvent = false; // รีเซ็ตตัวแปรตรวจสอบ Event
    }
}

