using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueLine[] dialogueLines;
    public bool triggerOnEnter = true;
    public bool autoStart = true; 

    private DialogueSystem dialogueSystem;

    public bool activateNpcWalk = false;
    public bool activateEnemiesAttack = false;
    public bool activateNextScene = false;
    private void Start()
    {
        dialogueSystem = FindObjectOfType<DialogueSystem>();
        if (dialogueSystem == null)
            Debug.LogError("No DialogueSystem found in scene.");
        if (autoStart && !triggerOnEnter)
        {
            StartDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggerOnEnter) return;
        if (other.CompareTag("Player"))
        {
            StartDialogue();
            gameObject.SetActive(false);
        }
    }

    public void StartDialogue()
    {
        if (dialogueSystem != null)
            dialogueSystem.StartDialogue(dialogueLines, activateNpcWalk, activateEnemiesAttack, activateNextScene);
    }
}
