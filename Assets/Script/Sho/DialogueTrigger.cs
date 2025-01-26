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

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
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
}

