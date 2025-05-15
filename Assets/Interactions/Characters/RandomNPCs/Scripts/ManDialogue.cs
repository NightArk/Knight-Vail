using System.Collections;
using UnityEngine;
using TMPro;
using Invector.vCharacterController;

public class ManDialogue : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        [TextArea] public string line;
    }

    public DialogueLine[] firstDialogue;

    public DialogueLine finalRepeatLine = new DialogueLine
    {
        speaker = "Man",
        line = "Stay safe out there, friend."
    };

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

    private bool hasHadFirstDialogue = false;
    private Animator animator;

    void Start()
    {
        dialoguePanel.SetActive(false);
        playerInput = player.GetComponent<vThirdPersonInput>();
        animator = GetComponentInParent<Animator>();

        firstDialogue = new DialogueLine[]
        {
            new DialogueLine { speaker = "Man", line = "Are you safe? Are you in any danger?" },
            new DialogueLine { speaker = "Axel", line = "Yeah, I'm okay for now." },
            new DialogueLine { speaker = "Man", line = "Good to hear. There are monsters everywhere these days." },
            new DialogueLine { speaker = "Man", line = "Do you need shelter or anything to keep safe?" },
            new DialogueLine { speaker = "Axel", line = "No, I'm here to fight and save everyone." },
            new DialogueLine { speaker = "Man", line = "I admire your courage. May luck be with you on your journey." },
            new DialogueLine { speaker = "Man", line = "If you need shelter or rest, my home is open to you. You're not alone in this." }
        };
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
        StopPlayerMovement();

        if (animator != null)
            animator.SetBool("isTalking", true);

        if (!hasHadFirstDialogue)
        {
            ShowLine();
        }
        else
        {
            ShowFinalLine();
        }
    }

    void ShowLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        DialogueLine lineToShow = firstDialogue[currentLine];

        speakerNameText.text = lineToShow.speaker;
        typingCoroutine = StartCoroutine(TypeLine(lineToShow.line));
    }

    void ShowFinalLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        speakerNameText.text = finalRepeatLine.speaker;
        typingCoroutine = StartCoroutine(TypeLine(finalRepeatLine.line));
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

        if (!hasHadFirstDialogue)
            dialogueText.text = firstDialogue[currentLine].line;
        else
            dialogueText.text = finalRepeatLine.line;

        isTyping = false;
    }

    void AdvanceDialogue()
    {
        if (!hasHadFirstDialogue)
        {
            currentLine++;
            if (currentLine < firstDialogue.Length)
            {
                ShowLine();
            }
            else
            {
                hasHadFirstDialogue = true;
                EndDialogue();
            }
        }
        else
        {
            EndDialogue();
        }
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

    void ResumePlayerMovement()
    {
        playerInput.enabled = true;  // Re-enable player input to allow movement
        player.GetComponent<Rigidbody>().isKinematic = false;  // Enable physics again
    }
}
