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

    public DialogueLine[] firstDialogue;

    public DialogueLine finalRepeatLine = new DialogueLine
    {
        speaker = "Alessandra",
        line = "Don’t be a stranger, Axel. The town’s always better with company like yours."
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
    private bool hasHadFirstDialogue = false;
    private Coroutine typingCoroutine;

    private Animator animator;

    void Start()
    {
        dialoguePanel.SetActive(false);
        playerInput = player.GetComponent<vThirdPersonInput>();
        animator = GetComponentInParent<Animator>();

        firstDialogue = new DialogueLine[]
        {
            new DialogueLine { speaker = "Axel", line = "Hi, I’m new in town. My name’s Axel." },
            new DialogueLine { speaker = "Alessandra", line = "Nice to meet you, Axel. I’m Alessandra." },
            new DialogueLine { speaker = "Axel", line = "It’s a pleasure, Alessandra. This place looks great so far." },
            new DialogueLine { speaker = "Alessandra", line = "Welcome then! We don’t get too many newcomers around here, but it’s always nice to see fresh faces." },
            new DialogueLine { speaker = "Axel", line = "Thanks! I'm still getting to know the area." },
            new DialogueLine { speaker = "Alessandra", line = "You’re in the right place. People here are kind, and we stick together. You’ll feel at home in no time." },
            new DialogueLine { speaker = "Axel", line = "That sounds great. I’m looking forward to exploring." },
            new DialogueLine { speaker = "Alessandra", line = "If you need anything, don’t hesitate to ask. Oh, and watch out for Cedric." },
            new DialogueLine { speaker = "Axel", line = "Cedric? Who’s that?" },
            new DialogueLine { speaker = "Alessandra", line = "He’s a bit of a mystery, that one. Keeps to himself mostly. But don’t worry, he’s harmless—just a little... skeptical of newcomers." },
            new DialogueLine { speaker = "Axel", line = "I’ll keep that in mind." },
            new DialogueLine { speaker = "Alessandra", line = "Good. Anyway, don’t let me keep you. Enjoy your time here, and don’t be a stranger!" }
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
            ShowLine();
        else
            ShowFinalLine();
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

        dialogueText.text = !hasHadFirstDialogue
            ? firstDialogue[currentLine].line
            : finalRepeatLine.line;

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
        playerInput.enabled = false;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.GetComponent<Rigidbody>().isKinematic = true;
    }

    void ResumePlayerMovement()
    {
        playerInput.enabled = true;
        player.GetComponent<Rigidbody>().isKinematic = false;
    }
}
