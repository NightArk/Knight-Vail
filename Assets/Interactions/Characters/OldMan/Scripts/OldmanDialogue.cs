using System.Collections;
using UnityEngine;
using TMPro;
using Invector.vCharacterController;

public class OldmanDialogue : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        [TextArea] public string line;
    }

    public DialogueLine[] initialDialogueLines;
    public DialogueLine[] questOngoingDialogue;
    public DialogueLine[] questCompleteDialogue;
    public DialogueLine[] questComplete1Dialogue;

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

    private DialogueLine[] currentDialogue;
    private bool hasReward = false;
    private Animator animator; // auto-find on parent
    public AudioSource typingAudioSource;



    void Start()
    {
        animator = GetComponentInParent<Animator>();
        dialoguePanel.SetActive(false);
        playerInput = player.GetComponent<vThirdPersonInput>();

        // Initial quest dialogue
        initialDialogueLines = new DialogueLine[]
        {
            new DialogueLine { speaker = "Old Man", line = "Ah... young one. May I trouble you for a moment?" },
            new DialogueLine { speaker = "Axel", line = "Of course. What’s wrong?" },
            new DialogueLine { speaker = "Old Man", line = "I was on the road back here when I was ambushed." },
            new DialogueLine { speaker = "Old Man", line = "Creatures—dark, snarling things—chased me through the woods. I barely escaped." },
            new DialogueLine { speaker = "Old Man", line = "And there were signs... of elves, twisted by something foul. Their eyes glowed with hate." },
            new DialogueLine { speaker = "Old Man", line = "Stay sharp if you go into those woods. Monsters aren't the only things lurking there." },
            new DialogueLine { speaker = "Old Man", line = "As I ran, I stumbled near a cluster of rocks along the roadside." },
            new DialogueLine { speaker = "Old Man", line = "In the fall... I must've dropped something. My ring." },
            new DialogueLine { speaker = "Axel", line = "What kind of ring?" },
            new DialogueLine { speaker = "Old Man", line = "A silver band, worn with time. It belonged to my wife. She's been gone many years now." },
            new DialogueLine { speaker = "Old Man", line = "It’s all I have left of her." },
            new DialogueLine { speaker = "Axel", line = "Where exactly did you lose it?" },
            new DialogueLine { speaker = "Old Man", line = "Somewhere along the road that winds through the woods—near those rocks I mentioned." },
            new DialogueLine { speaker = "Old Man", line = "I was too afraid to turn back." },
            new DialogueLine { speaker = "Axel", line = "I'll find your ring. You have my word." },
            new DialogueLine { speaker = "Old Man", line = "Thank you... truly. May fate be kinder to you than it was to me." }
        };




        // Dialogue after quest is active but not complete
        questOngoingDialogue = new DialogueLine[]
        {
            new DialogueLine { speaker = "Old Man", line = "Still searching? Bless you. I tripped near some rocks off the road—I think that’s where it slipped away." }
        };



        // Dialogue after quest is complete
        questCompleteDialogue = new DialogueLine[]
        {
            new DialogueLine { speaker = "Axel", line = "I found your ring. It was lying on top of some rocks off the road." },
            new DialogueLine { speaker = "Old Man", line = "...You found it? I... I can’t believe it." },
            new DialogueLine { speaker = "Old Man", line = "Thank you. You’ve returned a part of me I thought lost forever." }
        };


        questComplete1Dialogue = new DialogueLine[]
        {
            new DialogueLine { speaker = "Old Man", line = "Every time I look at the ring, I remember her smile. Thank you again. You’ve done more than you know." }
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

        // Show reward dialogue only once
        if (OldmanTaskTracker.Instance.IsOldManQuestComplete)
        {
            if (!hasReward)
            {
                currentDialogue = questCompleteDialogue;
                hasReward = true;

                animator.SetBool("gotRing", true);
                StartCoroutine(SwitchToHappyIdle());

                OldmanTaskTracker.Instance.taskPanel.SetActive(false);
                OldmanTaskTracker.Instance.MarkOldManQuestAsFinished();
            }
            else
            {
                currentDialogue = questComplete1Dialogue;
            }
        }
        else if (OldmanTaskTracker.Instance.IsOldManQuestActive)
        {
            currentDialogue = questOngoingDialogue;
        }
        else if (OldmanTaskTracker.Instance.IsOldManQuestFinished)
        {
            currentDialogue = questComplete1Dialogue;
        }
        else
        {
            currentDialogue = initialDialogueLines;
        }


        ShowLine();
        StopPlayerMovement();
    }


    void AdvanceDialogue()
    {
        currentLine++;
        if (currentLine < currentDialogue.Length)
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

        var line = currentDialogue[currentLine];
        speakerNameText.text = line.speaker;
        typingCoroutine = StartCoroutine(TypeLine(line.line));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        typingAudioSource.Play(); // Start the typing sound

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        typingAudioSource.Stop(); // stop after typing ends

        isTyping = false;
    }
    IEnumerator SwitchToHappyIdle()
{
    yield return new WaitForSeconds(2f); // wait for ThankYou animation to play

    animator.SetBool("gotRing", false); // stop thank you animation
    animator.SetBool("isHappy", true);  // permanently switch to happy idle
}


    void FinishTyping()
    {
        StopCoroutine(typingCoroutine);
        typingAudioSource.Stop();
        dialogueText.text = currentDialogue[currentLine].line;
        isTyping = false;
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        ResumePlayerMovement();


        // Only start the quest if it hasn't already been finished
        if (!OldmanTaskTracker.Instance.IsOldManQuestActive && !OldmanTaskTracker.Instance.IsOldManQuestFinished)
        {
            OldmanTaskTracker.Instance.StartTracking("Find the Old Man’s ring", 1);
        }
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
