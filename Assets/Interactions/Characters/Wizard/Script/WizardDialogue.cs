using System.Collections;
using UnityEngine;
using TMPro;
using Invector.vCharacterController;

public class WizardDialogue : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        [TextArea] public string line;
    }

    public DialogueLine[] firstDialogue;
    public DialogueLine[] secondDialogue;
    [TextArea]
    public string[] finalRepeatLines = new string[]
{
    "Beware the pastries... Always the pastries.",
    "The muffin speaks truths you dare not imagine!",
    "My spoon is too powerful!",
    "I enchanted a pigeon. Don’t ask why.",
    "The wind whispered secrets... or maybe that was Greg.",
    "Avoid the cookies. They know too much.",
    "OOOH! THEY’RE EVERYWHERE!",
    "BE CAREFUL! The strudel sees all.",
    "Help me defeat these cursed crumbs!",
    "The éclairs have formed a council. We are not invited.",
    "Don’t blink. The danishes move when you blink.",
    "They took my hat... and my dignity.",
    "HELP ME DEFEAT THESE FROSTED DEMONS!",
    "The croissants... they hiss now.",
    "One tart to rule them all... and it's missing.",
    "They said I was mad. They were correct, but that’s beside the point.",
    "BEWARE! The lemon squares have a new leader.",
    "Pastries in the shadows... plotting."
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
    private bool hasHadSecondDialogue = false;

    private Animator animator;

    void Start()
    {
        dialoguePanel.SetActive(false);
        playerInput = player.GetComponent<vThirdPersonInput>();
        animator = GetComponentInParent<Animator>();

        // First dialogue set
        firstDialogue = new DialogueLine[]
        {
        new DialogueLine { speaker = "Barnaby", line = "Ah-ha! You've arrived at last!" },
        new DialogueLine { speaker = "Axel", line = "...I'm literally just walking past." },
        new DialogueLine { speaker = "Barnaby", line = "We don't have time for modesty. The stars foretold your coming!" },
        new DialogueLine { speaker = "Axel", line = "Right. So... where *exactly* are we right now?" },
        new DialogueLine { speaker = "Barnaby", line = "This is the hidden realm of Thal’Zuur, veiled in shadow and illusion!" },
        new DialogueLine { speaker = "Axel", line = "Pretty sure this is the village square. Next to the well." },
        new DialogueLine { speaker = "Barnaby", line = "You’ve been deceived! The villagers—every one of them—are thralls! Puppets of the Dark Confectioner!" },
        new DialogueLine { speaker = "Axel", line = "Oh no, not the Dark Confectioner. Let me guess… he controls minds through pastries?" },
        new DialogueLine { speaker = "Barnaby", line = "YES! You *do* understand! The cursed scones! The mind-syrup! It’s all real!" },
        new DialogueLine { speaker = "Axel", line = "Yup. Terrifying. I ate a muffin earlier—I might already be doomed." },
        new DialogueLine { speaker = "Barnaby", line = "No! There’s still time. I MUST SAVE THEM!" },
        new DialogueLine { speaker = "Axel", line = "You do that. I’ll be... right behind you. Waaaaay behind you." },
        new DialogueLine { speaker = "Barnaby", line = "Stay vigilant, apprentice! The fate of Thal’Zuur rests on our shoulders!" }
        };

        // Second dialogue set
        secondDialogue = new DialogueLine[]
        {
        new DialogueLine { speaker = "Barnaby", line = "You're back! Excellent, excellent. I need an assistant for my potion of squirrel invisibility!" },
        new DialogueLine { speaker = "Axel", line = "Is the squirrel invisible... or is the potion invisible?" },
        new DialogueLine { speaker = "Barnaby", line = "Both! But only on Tuesdays during a waxing moon!" },
        new DialogueLine { speaker = "Axel", line = "That’s oddly specific. Do you even have a squirrel?" },
        new DialogueLine { speaker = "Barnaby", line = "No. But that hasn’t stopped me before!" },
        new DialogueLine { speaker = "Axel", line = "Yup. Same old Barnaby." }
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

        ShowLine();
    }

    void ShowLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        DialogueLine lineToShow;

        if (!hasHadFirstDialogue)
            lineToShow = firstDialogue[currentLine];
        else if (!hasHadSecondDialogue)
            lineToShow = secondDialogue[currentLine];
        else
        {
            string randomLine = finalRepeatLines[Random.Range(0, finalRepeatLines.Length)];
            lineToShow = new DialogueLine { speaker = "Barnaby", line = randomLine };
        }

        speakerNameText.text = lineToShow.speaker;
        typingCoroutine = StartCoroutine(TypeLine(lineToShow.line));
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

        DialogueLine lineToShow;

        if (!hasHadFirstDialogue)
            lineToShow = firstDialogue[currentLine];
        else if (!hasHadSecondDialogue)
            lineToShow = secondDialogue[currentLine];
        else { 
        string randomLine = finalRepeatLines[Random.Range(0, finalRepeatLines.Length)];
        lineToShow = new DialogueLine { speaker = "Barnaby", line = randomLine };
        }

        dialogueText.text = lineToShow.line;
        isTyping = false;
    }

    void AdvanceDialogue()
    {
        currentLine++;

        if (!hasHadFirstDialogue && currentLine < firstDialogue.Length)
        {
            ShowLine();
        }
        else if (!hasHadFirstDialogue)
        {
            hasHadFirstDialogue = true;
            EndDialogue();
        }
        else if (!hasHadSecondDialogue && currentLine < secondDialogue.Length)
        {
            ShowLine();
        }
        else if (!hasHadSecondDialogue)
        {
            hasHadSecondDialogue = true;
            EndDialogue();
        }
        else
        {
            EndDialogue(); // Final one-liner doesn't need multiple lines
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
