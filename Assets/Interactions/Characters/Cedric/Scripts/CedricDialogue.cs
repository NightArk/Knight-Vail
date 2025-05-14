using System.Collections;
using UnityEngine;
using TMPro;
using Invector.vCharacterController;

public class CedricDialogue : MonoBehaviour
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

    private Animator animator;

    void Start()
    {
        dialogueLines = new DialogueLine[]
    {
        new DialogueLine { speaker = "Axel", line = "Hi there. I just arrived in town and wanted to introduce myself." },
        new DialogueLine { speaker = "Cedric", line = "Another outsider, huh? We've had our fair share of those lately." },
        new DialogueLine { speaker = "Axel", line = "I don’t mean any trouble. Just trying to get settled in." },
        new DialogueLine { speaker = "Cedric", line = "Settling in is easy. Earning trust... not so much." },
        new DialogueLine { speaker = "Axel", line = "I understand. Maybe you can tell me more about this place?" },
        new DialogueLine { speaker = "Cedric", line = "What’s said and what’s true don’t always match. You’ll learn that soon enough." },
        new DialogueLine { speaker = "Axel", line = "Fair enough. I’ll keep my eyes open." },
        new DialogueLine { speaker = "Cedric", line = "Good. Eyes open, ears sharp. That's how you survive here." },
        new DialogueLine { speaker = "Axel", line = "Thanks for the warning. I’ll stay cautious." },
        new DialogueLine { speaker = "Cedric", line = "You do that, stranger. Time will tell what you’re really after." }
    };


        dialoguePanel.SetActive(false);
        playerInput = player.GetComponent<vThirdPersonInput>();

        animator = GetComponentInParent<Animator>();
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
        StopPlayerMovement();

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
        ResumePlayerMovement();

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

    void StopPlayerMovement()
    {
        playerInput.enabled = false;  // Disable player input to stop movement
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;  // Stop any existing momentum
        player.GetComponent<Rigidbody>().isKinematic = true;  // Disable physics to prevent movement
    }

    // Resume player movement and input
    void ResumePlayerMovement()
    {
        playerInput.enabled = true;  // Re-enable player input to allow movement
        player.GetComponent<Rigidbody>().isKinematic = false;  // Enable physics again
    }
}
