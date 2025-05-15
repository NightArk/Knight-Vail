using System.Collections;
using UnityEngine;
using TMPro;
using Invector.vCharacterController;

public class WalterDialogue : MonoBehaviour
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
        speaker = "Walter Strider",
        line = "Thank You Again for Sparing my life."
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
            new DialogueLine { speaker = "Axel", line = "Awfully, meek for a Dark elf are you? Hiding in the bushes and all?" },
            new DialogueLine { speaker = "Walter Strider", line = "...huh?" },
            new DialogueLine { speaker = "Walter Strider", line = "You aren't gonna slash me like you do my bretheren?" },
            new DialogueLine { speaker = "Axel", line = "Really could, but It feels sad to slash you when you are hiding all by your lonesome." },
            new DialogueLine { speaker = "Axel", line = "Plus I can't be Arsed this far into this quest gotta save my strength for what is to come." },
            new DialogueLine { speaker = "Walter Strider", line = "Appreciate it, I am a craftsman not much of a fighter." },
            new DialogueLine { speaker = "Walter Strider", line = "I escaped the grasp of this lunatic lich ahead now, and before that escaped my clan to venture with two other travelers" },
            new DialogueLine { speaker = "Walter Strider", line = "a smaller priest with a massive afro and a curious souls-and-knives wielding fella." },
            new DialogueLine { speaker = "Axel", line = "Never heard of either, but sounds like a fun circus-in-the-making." },
            new DialogueLine { speaker = "Walter Strider", line = "Anyways... I appreciate you not murdering me on the spot, so you should know that the lich \"Nightveil\" ahead moves rather fast compared to most enemies." },
            new DialogueLine { speaker = "Walter Strider", line = "heals just as much as you hero, and is hardier to boot. Perserve your stamina and take caution." },
            new DialogueLine { speaker = "Axel", line = "Thank you, best of luck traveller!" },
            new DialogueLine { speaker = "Walter Strider", line = "May Shadows guide you friend." }
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

        // Disable the prompt script
        GetComponent<NPCInteraction>().enabled = false;

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
        GetComponent<NPCInteraction>().enabled = true;

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
