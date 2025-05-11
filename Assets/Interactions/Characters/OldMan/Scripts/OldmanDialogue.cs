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
            new DialogueLine { speaker = "Old Man", line = "Ah, young one. I need your help." },
            new DialogueLine { speaker = "Player", line = "What do you need help with?" },
            new DialogueLine { speaker = "Old Man", line = "I lost my ring. It’s very precious to me. Can you find it?" },
            new DialogueLine { speaker = "Player", line = "Sure, I'll find your ring." },
            new DialogueLine { speaker = "Old Man", line = "It fell near the old oak tree, to the north of here." }
        };

        // Dialogue after quest is active but not complete
        questOngoingDialogue = new DialogueLine[]
        {
            new DialogueLine { speaker = "Old Man", line = "Did you find my ring near the old oak tree?" }
        };

        // Dialogue after quest is complete
        questCompleteDialogue = new DialogueLine[]
        {
            new DialogueLine { speaker = "Player", line = "I found your ring!" },
            new DialogueLine { speaker = "Old Man", line = "Thank you, young one. I am forever grateful." }
        };
        questComplete1Dialogue = new DialogueLine[]
        {
            new DialogueLine { speaker = "Old Man", line = "Thank you again, you saved my life!" }
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
        playerInput.lockInput = true;
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
        playerInput.lockInput=false;


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
}
