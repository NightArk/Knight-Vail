using System.Collections;
using UnityEngine;
using TMPro;
using Invector.vCharacterController;

public class TildaDialogue : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        [TextArea] public string line;
    }

    public DialogueLine[] dialogueLines;

    public DialogueLine[] randomLines = new DialogueLine[]
{
    new DialogueLine { speaker = "Tilda", line = "Muffin’s been wondering when you'd stop by again!" },
    new DialogueLine { speaker = "Tilda", line = "Well, look who’s back—Muffin's been keeping an eye out for you." },
    new DialogueLine { speaker = "Tilda", line = "I swear, Muffin knows more about you than I do at this point." },
    new DialogueLine { speaker = "Tilda", line = "Ah, you’re back! Muffin’s been asking if you’d finally bring snacks." }
};
    public DialogueLine[] firstInteractionLines = new DialogueLine[]
{
    new DialogueLine { speaker = "Tilda", line = "Muffin’s waiting for you to come say hello!" },
    new DialogueLine { speaker = "Tilda", line = "You should go pet Muffin, she’s got the softest fur!" },
    new DialogueLine { speaker = "Tilda", line = "Muffin’s waiting for a new friend. You should meet her, you’ll love her!" },
    new DialogueLine { speaker = "Tilda", line = "She’s just sitting there, waiting for you. Go on, give her a pet!" },
    new DialogueLine { speaker = "Tilda", line = "Muffin’s been waiting for someone like you to come by. You should go meet her!" },
    new DialogueLine { speaker = "Tilda", line = "You should give Muffin a visit. She’s the sweetest when she gets some attention." },
    new DialogueLine { speaker = "Tilda", line = "I think Muffin’s waiting for you to come and say hi!" },
    new DialogueLine { speaker = "Tilda", line = "Muffin’s just over there, waiting for someone to give her some love. Go meet her!" },
    new DialogueLine { speaker = "Tilda", line = "Muffin will really appreciate it if you go say hi! She’s waiting for you." }
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

    private Animator animator;

    void Start()
    {
        
        dialoguePanel.SetActive(false);
        playerInput = player.GetComponent<vThirdPersonInput>();
        animator = GetComponentInParent<Animator>();

        UpdateDialogueByState();
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
    void UpdateDialogueByState()
    {
        int randomIndex = Random.Range(0, randomLines.Length);
        int randomIndex1 = Random.Range(0, firstInteractionLines.Length);
        

        switch (QuestManagerState.Instance.tildaDialogueState)
        {
            case QuestManagerState.TildaState.Initial:
                dialogueLines = new DialogueLine[]
        {
            new DialogueLine { speaker = "Axel", line = "Don’t look behind you." },
            new DialogueLine { speaker = "Tilda", line = "What? Why? What’s wrong? Is it a bandit? A ghost?" },
            new DialogueLine { speaker = "Axel", line = "No... there's a sheep standing right there, staring into my soul." },
            new DialogueLine { speaker = "Tilda", line = "*laughs* Oh, that’s just Muffin, my darling sheep! She follows me everywhere." },
            new DialogueLine { speaker = "Axel", line = "She’s... oddly majestic. And mildly threatening." },
            new DialogueLine { speaker = "Tilda", line = "She’s got a strong gaze, yes. I think she used to be a noblewoman in a past life. Or a tax collector." },
            new DialogueLine { speaker = "Axel", line = "Do all your animals have intense eye contact, or just this one?" },
            new DialogueLine { speaker = "Tilda", line = "Just Muffin. The chickens are too busy panicking about literally everything." },
            new DialogueLine { speaker = "Axel", line = "Can I pet her? Or is she... sacred?" },
            new DialogueLine { speaker = "Tilda", line = "Of course! But speak softly. She once kicked a bard in the knee for whistling too loud." },
            new DialogueLine { speaker = "Axel", line = "Fair. I’ve wanted to do that to bards myself." },
            new DialogueLine { speaker = "Tilda", line = "She’s really just a fluffy ball of love. A slightly judgmental one, but aren’t we all?" },
            new DialogueLine { speaker = "Axel", line = "You two make quite the duo. Do you travel together much?" },
            new DialogueLine { speaker = "Tilda", line = "Everywhere. Market, temple, you name it. Muffin’s even been in the mayor’s office. She left a little ‘gift’ on the carpet." },
            new DialogueLine { speaker = "Axel", line = "Oh no..." },
            new DialogueLine { speaker = "Tilda", line = "Oh yes. I don’t think he’s ever fully recovered." },
            new DialogueLine { speaker = "Axel", line = "You're honestly the most interesting person I’ve met here so far." },
            new DialogueLine { speaker = "Tilda", line = "Stick around, stranger. You haven’t even heard the story of how I outran a pack of wolves with only a jar of honey and Muffin’s angry bleats." },
            new DialogueLine { speaker = "Axel", line = "I feel like this town just got a lot more entertaining." },
            new DialogueLine { speaker = "Tilda", line = "And it’s better with friends. Come visit any time. Muffin always remembers kind faces... and snacks." }
        };
                
                break;

            case QuestManagerState.TildaState.Second:
                dialogueLines = new DialogueLine[]
            {
                firstInteractionLines[randomIndex1], // Random first interaction line
            };
                break;

            case QuestManagerState.TildaState.VisitedMuffin:
                dialogueLines = new DialogueLine[]
                {
                new DialogueLine { speaker = "Axel", line = "I pet Muffin. She did not consume me." },
                new DialogueLine { speaker = "Tilda", line = "A rare honor! Muffin approves." },
                new DialogueLine { speaker = "Axel", line = "She might be the boss of this village." },
                new DialogueLine { speaker = "Tilda", line = "Don’t let her hear that. She’ll demand a crown." }
                };
                QuestManagerState.Instance.tildaDialogueState = QuestManagerState.TildaState.Final;
                break;

            case QuestManagerState.TildaState.Final:
                dialogueLines = new DialogueLine[]
            {
                randomLines[randomIndex], // Random first interaction line
            };
                break;
        }
    }


    void StartDialogue()
    {
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        currentLine = 0;
        UpdateDialogueByState();
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
        if (QuestManagerState.Instance.tildaDialogueState == QuestManagerState.TildaState.Initial)
        {
            QuestManagerState.Instance.tildaDialogueState = QuestManagerState.TildaState.Second;
        }
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
