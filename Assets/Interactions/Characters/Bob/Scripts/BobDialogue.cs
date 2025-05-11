using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Invector.vCharacterController;

public class BobDialogue : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        [TextArea] public string line;
    }

    public DialogueLine[] dialogueLines;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerNameText;
    public vThirdPersonController player;
    private vThirdPersonInput playerInput;
    public float typingSpeed = 0.02f;

    public GameObject choicePanel;
    public Button btnAccept;
    public Button btnDecline;

    private int currentLine = 0;
    private bool isPlayerInRange = false;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private bool awaitingChoice = false;
    private Animator animator;

    public AudioSource typingAudioSource;

    void Start()
    {
        animator = GetComponentInParent<Animator>();
        dialoguePanel.SetActive(false);
        playerInput = player.GetComponent<vThirdPersonInput>();
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isDialogueActive)
        {
            StartDialogue();
        }

        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            if (awaitingChoice) return;

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

        // Quest already finished
        if (TaskTracker.Instance.IsBobQuestFinished)
        {
            dialogueLines = new DialogueLine[]
            {
            new DialogueLine { speaker = "Bob", line = "Thanks again for those coins!" }
            };
            animator?.SetBool("isTalking", true);
            animator?.SetBool("questCompleted", false);
        }
        // Quest complete, but reward not given yet
        else if (TaskTracker.Instance.IsBobQuestComplete)
        {
            dialogueLines = new DialogueLine[]
            {
            new DialogueLine { speaker = "Bob", line = "Wow, you actually got them! Thanks!" },
            new DialogueLine { speaker = "Player", line = "It was nothing." },
            new DialogueLine { speaker = "Bob", line = "Here’s your reward!" }
            };
            animator?.SetBool("questCompleted", true);
            animator?.SetBool("isTalking", true);

            // Give reward and mark quest as finished
            TaskTracker.Instance.MarkBobQuestAsFinished();
        }
        // Quest in progress
        else if (TaskTracker.Instance.HasActiveTask && TaskTracker.Instance.IsBobQuestActive)
        {
            dialogueLines = new DialogueLine[]
            {
            new DialogueLine { speaker = "Bob", line = "Still waiting on those 5 coins!" }
            };
            animator?.SetBool("isTalking", true);
            //animator?.SetBool("questCompleted", false);
        }
        // Quest not started yet
        else
        {
            dialogueLines = new DialogueLine[]
            {
            new DialogueLine { speaker = "Bob", line = "Hey! Got any spare change?" },
            new DialogueLine { speaker = "Player", line = "Nope, sorry!" },
            new DialogueLine { speaker = "Bob", line = "Actually... can you collect 5 coins for me?" },
            new DialogueLine { speaker = "", line = "[CHOICE]" }
            };
            animator?.SetBool("isTalking", true);
           //animator?.SetBool("questCompleted", false);
        }

        ShowLine();
        playerInput.lockInput = true;
    }


    void AdvanceDialogue()
    {
        currentLine++;
        if (currentLine < dialogueLines.Length)
            ShowLine();
        else
            EndDialogue();
    }

    void ShowLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        if (currentLine >= dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        var line = dialogueLines[currentLine];

        if (line.line.Contains("[CHOICE]"))
        {
            speakerNameText.text = line.speaker;
            typingCoroutine = StartCoroutine(TypeLine(line.line.Replace("[CHOICE]", "")));
            StartCoroutine(ShowChoicesAfterTyping());
            return;
        }

        speakerNameText.text = line.speaker;
        typingCoroutine = StartCoroutine(TypeLine(line.line));
    }

    IEnumerator ShowChoicesAfterTyping()
    {
        awaitingChoice = true;
        yield return new WaitUntil(() => isTyping == false);

        choicePanel.SetActive(true);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(btnAccept.gameObject);
        btnAccept.onClick.RemoveAllListeners();
        btnDecline.onClick.RemoveAllListeners();

        btnAccept.onClick.AddListener(() =>
        {
            // Only allow quest to start if it's not completed
            if (!TaskTracker.Instance.IsBobQuestComplete)
            {
                TaskTracker.Instance.StartTracking("Collect 5 coins: ", 5);
                choicePanel.SetActive(false);
                awaitingChoice = false;
                AdvanceDialogue();
            }
        });

        btnDecline.onClick.AddListener(() =>
        {
            choicePanel.SetActive(false);
            awaitingChoice = false;
            EndDialogue();
        });
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


    void FinishTyping()
    {
        StopCoroutine(typingCoroutine);
        typingAudioSource.Stop();
        dialogueText.text = dialogueLines[currentLine].line.Replace("[CHOICE]", "");
        isTyping = false;
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false;
        currentLine = 0;
        playerInput.lockInput = false;
        animator?.SetBool("isTalking", false);
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
