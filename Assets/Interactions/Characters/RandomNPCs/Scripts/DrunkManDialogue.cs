using System.Collections;
using UnityEngine;
using TMPro;
using Invector.vCharacterController;

public class DrunkManDialogue : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        [TextArea] public string line;
    }

    public DialogueLine[] firstDialogue;

    [TextArea]
    public string[] finalRepeatLines = new string[]
        {
            "WHO STOLE MY SHOES—BE HONEST, I WON’T BE MAD!",
            "I SWEAR THE MOON JUST BLINKED AT ME!",
            "AXEL! I CHALLENGE YOU TO A STUMBLE-OFF!",
            "IF I FALL DOWN, THAT’S THE EARTH’S FAULT!",
            "THE ALE TOLD ME TO INVEST IN CABBAGES!",
            "I’M NOT DRUNK, I’M DANCING WITH INVISIBLE FIRE!",
            "SOMEONE FETCH ME A DRAGON—I NEED A RIDE!",
            "THE TAVERN’S MOVIN’ AGAIN! HOLD STILL!",
            "WHO NEEDS SOBRIETY WHEN YOU’VE GOT VIBES?",
            "I’VE MADE PEACE WITH THE FLOOR. WE’RE MARRIED NOW!"
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

    private string currentRepeatLine;

    private bool hasHadFirstDialogue = false;
    private Animator animator;

    void Start()
    {
        dialoguePanel.SetActive(false);
        playerInput = player.GetComponent<vThirdPersonInput>();
        animator = GetComponentInParent<Animator>();

        // Example initialization for firstDialogue - replace with your actual lines or set them in inspector
        firstDialogue = new DialogueLine[]
        {
            new DialogueLine { speaker = "Drunk Townsman", line = "Looks like end of the world, care to join me in watching chickens fight with a drink?" },
            new DialogueLine { speaker = "Drunk Townsman", line = "No? Not even for a Scupi snack? Well suit yourself with all that honor and baloney." }
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

        ShowLine();
    }

    void ShowLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        DialogueLine lineToShow;

        if (!hasHadFirstDialogue)
        {
            lineToShow = firstDialogue[currentLine];
        }
        else
        {
            currentRepeatLine = finalRepeatLines[Random.Range(0, finalRepeatLines.Length)];
            lineToShow = new DialogueLine { speaker = "Drunk Townsman", line = currentRepeatLine };
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

        if (!hasHadFirstDialogue)
        {
            dialogueText.text = firstDialogue[currentLine].line;
        }
        else
        {
            dialogueText.text = currentRepeatLine; // show the one already selected
        }

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
