using System.Collections;
using UnityEngine;
using TMPro;
using Invector.vCharacterController;

public class AgnesDialogue : MonoBehaviour
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
        speaker = "Agnes",
        line = "Please, stay safe out there..."
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
            new DialogueLine { speaker = "Agnes", line = "Oh, thank the stars you're here." },
            new DialogueLine { speaker = "Agnes", line = "The animals... the sheep... they're all gone." },
            new DialogueLine { speaker = "Agnes", line = "The dark elves came in the night while we were asleep. They killed them all." },
            new DialogueLine { speaker = "Player", line = "That’s terrible. Are you alright?" },
            new DialogueLine { speaker = "Agnes", line = "I’m scared, truly. I worry about my father... he fought them off, but what if he doesn’t come back?" },
            new DialogueLine { speaker = "Agnes", line = "And my little sister Lily… I can’t bear the thought of something happening to her." },
            new DialogueLine { speaker = "Player", line = "Don't worry, Agnes. I’ll protect you and your family. No harm will come to you." },
            new DialogueLine { speaker = "Agnes", line = "Thank you. Your words give me hope in these dark times." }
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
