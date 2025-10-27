using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueLine
{
    public string characterName;
    [TextArea(2, 6)]
    public string text;
    public Sprite portrait;
}



public class DialogueSystem : MonoBehaviour
{
    private Player playerController;
    private Boss boss;

    [Header("UI refs")]
    public GameObject dialoguePanel;
    public Text nameText;
    public Text dialogText;
    public Image portraitImage;

    [Header("Typing")]
    public float typingSpeed = 0.02f;
    public bool allowSkipTyping = true;

    private DialogueLine[] lines;
    private int index;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    public NpcBasicControllerW2 npc;
    public List<SoldierAlien> soldierAliens = new List<SoldierAlien>();

    public bool activateNpcWalk = false;
    public bool activateEnemies = false;
    public bool activateNextScene = false;
    public bool activateValthar = false;
    public bool activateMonster = false;

    public ValThar valthar;
    public SpawnMonsterAlien monster;

    public GameObject SceneNew;

    private void Awake()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    private void Start()
    {
        playerController = FindObjectOfType<Player>();
        boss = FindObjectOfType<Boss>();
    }



    private void Update()
    {
        if (dialoguePanel == null || !dialoguePanel.activeInHierarchy) return;


        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (isTyping && allowSkipTyping)
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                dialogText.text = lines[index].text;
                isTyping = false;
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    public void StartDialogue(DialogueLine[] dialogueLines, bool npcWalk = false, bool enemiesAttack = false, bool NextScene = false, bool valthar = false, bool monster= false)
    {
        lines = dialogueLines;
        index = 0;
        dialoguePanel.SetActive(true);

        activateNpcWalk = npcWalk;
        activateEnemies = enemiesAttack;
        activateNextScene = NextScene;
        activateValthar = valthar;
        activateMonster = monster;

        if (playerController != null) playerController.EnableMovement(false);
        if (boss != null) boss.EnableBoss(false);

        ShowLine(index);
    }

    void ShowNextLine()
    {
        index++;
        if (index >= lines.Length)
        {
            EndDialogue();
            return;
        }
        ShowLine(index);
    }

    void ShowLine(int i)
    {
        if (i < 0 || i >= lines.Length) return;
        nameText.text = lines[i].characterName;
        if (portraitImage != null)
        {
            if (lines[i].portrait != null)
            {
                portraitImage.sprite = lines[i].portrait;
                portraitImage.gameObject.SetActive(true);
            }
            else portraitImage.gameObject.SetActive(false);
        }
        dialogText.text = "";
        typingCoroutine = StartCoroutine(TypeText(lines[i].text));
    }

    IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        dialogText.text = "";
        for (int k = 0; k < fullText.Length; k++)
        {
            dialogText.text += fullText[k];
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);

        if (playerController != null) playerController.EnableMovement(true);
        if (boss != null) boss.EnableBoss(true);

        if (activateNpcWalk && npc != null)
            npc.StartWalking();

        if (activateEnemies && soldierAliens != null && soldierAliens.Count > 0)
        {
            foreach (SoldierAlien e in soldierAliens)
            {
                if (e != null)
                    e.EnableAttack(true);
            }
        }

        if (activateValthar)
        {
            valthar.EnableAttack(true);
        }

        if (activateMonster)
        {
            monster.enableAttack = true;
        }


        if (activateNextScene && SceneNew != null)
        {
            SceneNew.SetActive(true);
        }
    }
}