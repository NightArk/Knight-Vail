using System.Collections;
using UnityEngine;
using TMPro;
using Invector.vCharacterController;

public class AlessandraDialogue : MonoBehaviour
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
    public AudioSource typingAudioSource;

    private int currentLine = 0;
    private bool isPlayerInRange = false;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private Animator animator; // Animator reference

    void Start()
    {
        dialogueLines = new DialogueLine[]
        {
            new DialogueLine { speaker = "Player", line = "Hi, you must be Alessandra." },
            new DialogueLine { speaker = "Alessandra", line = "That's right! You must be the new face everyone’s been talking about." },
            new DialogueLine { speaker = "Player", line = "Yeah, I just arrived in town. Looks like a great place." },
            new DialogueLine { speaker = "Alessandra", line = "Welcome then! We don’t get too many newcomers around here, but it’s always nice to see fresh faces." },
            new DialogueLine { speaker = "Player", line = "Thanks! I'm still getting to know the area." },
            new DialogueLine { speaker = "Alessandra", line = "You’re in the right place. People here are kind, and we stick together. You’ll feel at home in no time." },
            new DialogueLine { speaker = "Player", line = "That sounds great. I’m looking forward to exploring." },
            new DialogueLine { speaker = "Alessandra", line = "If you need anything, don’t hesitate to ask. Oh, and watch out for Cedric." },
            new DialogueLine { speaker = "Player", line = "Cedric? Who’s that?" },
            new DialogueLine { speaker = "Alessandra", line = "He’s a bit of a mystery, that one. Keeps to himself mostly. But don’t worry, he’s harmless—just a little... skeptical of newcomers." },
            new DialogueLine { speaker = "Player", line = "I’ll keep that in mind." },
            new DialogueLine { speaker = "Alessandra", line = "Good. Anyway, don’t let me keep you. Enjoy your time here, and don’t be a stranger!" }
        };

        dialoguePanel.SetActive(false);
        playerInput = player.GetComponent<vThirdPersonInput>();

        animator = GetComponentInParent<Animator>(); // Auto-find animator on parent object
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isDialogueActive)
        {
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

        if (animator != null)
            animator.SetBool("isTalking", true);
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
        typingCoroutine = StartCoroutine(TypeLine(line.line));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        typingAudioSource.Play();

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        typingAudioSource.Stop();
        isTyping = false;
    }

    void FinishTyping()
    {
        StopCoroutine(typingCoroutine);
        typingAudioSource.Stop();
        dialogueText.text = dialogueLines[currentLine].line;
        isTyping = false;
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        playerInput.lockInput = false;

        if (animator != null)
            animator.SetBool("isTalking", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = false;
    }
}
