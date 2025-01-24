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

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            TriggerDialogue();
        }
    }


}
