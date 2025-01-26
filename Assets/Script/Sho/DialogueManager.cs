using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;

    private Queue<Dialogueline> lines;

    public bool isDialogueActive = false;

    public float typingSpeed = 0.2f;

    public Animator animator;

    private Coroutine typingCoroutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<Dialogueline>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;

        // แสดง UI และเริ่ม Dialogue
        animator.Play("show");
        Time.timeScale = 0f; // หยุดเวลาในเกม
        lines.Clear();

        foreach (Dialogueline dialogueLine in dialogue.dialoguelines)
        {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        Dialogueline currentLine = lines.Dequeue();

        characterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;

        // หยุด Coroutine ก่อนเริ่มใหม่ (ป้องกันปัญหาหลาย Coroutine ทำงานซ้อนกัน)
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(Dialogueline dialogueLine)
    {
        dialogueArea.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed); // ใช้ WaitForSecondsRealtime เพื่อไม่ให้หยุดเมื่อ Time.timeScale = 0
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;

        // ซ่อน UI และคืนค่าเวลาในเกม
        animator.Play("hide");
        Time.timeScale = 1f; // คืนค่าเวลาในเกม
    }
}
