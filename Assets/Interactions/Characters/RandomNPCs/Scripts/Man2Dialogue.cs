using System.Collections;
using UnityEngine;
using TMPro;
using Invector.vCharacterController;

public class Man2Dialogue : MonoBehaviour
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
        line = "The road ahead is dark—go carefully, friend."
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
            new DialogueLine { speaker = "Man", line = "There you are, young man. I was hoping you'd come this way." },
            new DialogueLine { speaker = "Man", line = "This road leads into the old forest. It used to be peaceful once, but now..." },
            new DialogueLine { speaker = "Man", line = "Monsters crawl beneath the trees. They whisper in the fog. Many who enter don’t return." },
            new DialogueLine { speaker = "Man", line = "I’ve seen brave folk leave with torch and sword, but none have come back." },
            new DialogueLine { speaker = "Man", line = "If you truly mean to face what’s out there, promise me—stay on the road. Don’t follow shadows." },
            new DialogueLine { speaker = "Man", line = "I may be old, but I've seen darkness before. This one feels... deeper." },
            new DialogueLine { speaker = "Man", line = "If you ever need a place to rest, or someone to share a fire with, come back here. The village stands with you." },
            new DialogueLine { speaker = "Man", line = "Now go. And may the old spirits walk with you." }
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
