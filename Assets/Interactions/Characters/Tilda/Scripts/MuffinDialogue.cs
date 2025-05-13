using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Invector.vCharacterController;
public class MuffinDialogue : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        [TextArea] public string line;
    }

    public DialogueLine[] dialogueLines;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerNameText;
    public vThirdPersonController player;
    private vThirdPersonInput playerInput;
    public float typingSpeed = 0.02f;

    private int currentLine = 0;
    private bool isPlayerInRange = false;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    public AudioSource audio;

    void Start()
    {
        dialogueLines = new DialogueLine[]
        {
            new DialogueLine { speaker = "Muffin", line = "Baaaaaaaaaa~" }
        };

        dialoguePanel.SetActive(false);
        playerInput = player.GetComponent<vThirdPersonInput>();

    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isDialogueActive)
        {
            audio.Play();
            StartDialogue();
        }

        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
                FinishTyping();
            else
                AdvanceDialogue();
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        currentLine = 0;
        ShowLine();
        playerInput.lockInput = true;
    }

    void AdvanceDialogue()
    {
        currentLine++;
        if (currentLine < dialogueLines.Length)
        {
            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }

    void ShowLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        var line = dialogueLines[currentLine];
        speakerNameText.text = line.speaker;
        typingCoroutine = StartCoroutine(TypeText(line.line));
    }

    IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    void FinishTyping()
    {
        StopCoroutine(typingCoroutine);
        dialogueText.text = dialogueLines[currentLine].line;
        isTyping = false;
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false;
        playerInput.lockInput = false;

        if (QuestManagerState.Instance.tildaDialogueState == QuestManagerState.TildaState.Second)
        {
            QuestManagerState.Instance.tildaDialogueState = QuestManagerState.TildaState.VisitedMuffin;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
