using System.Collections;
using UnityEngine;
using TMPro;
using Invector.vCharacterController;

public class GarethDialogue : MonoBehaviour
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
        speaker = "Gareth",
        line = "Take care out there, friend."
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
            new DialogueLine { speaker = "Gareth", line = "You there—traveler. You’ve arrived at a dangerous time." },
            new DialogueLine { speaker = "Gareth", line = "Strange beasts have been seen near the second forest. Eyes in the dark. Sounds no man can place." },
            new DialogueLine { speaker = "Gareth", line = "The village is on edge. Livestock gone. Children kept inside. We fear what might come next." },
            new DialogueLine { speaker = "Axel", line = "I’m here to help. Just tell me where to go." },
            new DialogueLine { speaker = "Gareth", line = "Follow the main road through the first forest. Keep going until the trees grow thick—that’s the second." },
            new DialogueLine { speaker = "Gareth", line = "Do not stray from the road. Whatever happens—stay on the path." },
            new DialogueLine { speaker = "Axel", line = "Understood. Second forest. Stay on the road." },
            new DialogueLine { speaker = "Gareth", line = "One more thing... if you come across a man named Barnaby—be cautious." },
            new DialogueLine { speaker = "Gareth", line = "He believes he’s a wizard. Talks to the wind. Tried to enchant a potato once." },
            new DialogueLine { speaker = "Axel", line = "...Alright then." },
            new DialogueLine { speaker = "Gareth", line = "Go carefully. Stay on the road, and don’t trust everything you see." }
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

    // Resume player movement and input
    void ResumePlayerMovement()
    {
        playerInput.enabled = true;  // Re-enable player input to allow movement
        player.GetComponent<Rigidbody>().isKinematic = false;  // Enable physics again
    }
}
