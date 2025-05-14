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
    public AudioSource acceptSFX;
    public AudioSource declineSFX;
    public CoinManager coinManager;

    public CanvasGroup choicePanelCanvasGroup;


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
        if (isDialogueActive && awaitingChoice)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                acceptSFX.Play();
                AcceptQuest();
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                declineSFX.Play();
                DeclineQuest();
            }
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
            new DialogueLine { speaker = "Bob", line = "You really came through for me — those coins will go a long way. I owe you one!" }
            };

            animator?.SetBool("isTalking", true);
            animator?.SetBool("questCompleted", false);
        }
        // Quest complete, but reward not given yet
        else if (TaskTracker.Instance.IsBobQuestComplete)
        {
            dialogueLines = new DialogueLine[]
            {
                new DialogueLine { speaker = "Bob", line = "Well I'll be! You actually found all five coins!" },
                new DialogueLine { speaker = "Player", line = "Told you I’d come through." },
                new DialogueLine { speaker = "Bob", line = "I can’t thank you enough. That really means a lot." }
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
            new DialogueLine { speaker = "Bob", line = "Hey there, traveler! You’re new around here, ain’t ya?" },
            new DialogueLine { speaker = "Player", line = "Just arrived. Thought I’d check out the town." },
            new DialogueLine { speaker = "Bob", line = "Well, welcome! Name’s Bob. Folks here call me that ‘cause… well, it’s my name." },
            new DialogueLine { speaker = "Bob", line = "I usually help out the blacksmith, but things been slow lately." },
            new DialogueLine { speaker = "Bob", line = "Say, could you help me out? I need 5 coins to buy some tools." },
            new DialogueLine { speaker = "Bob", line = "I’d really appreciate it. What do you say?" },
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
        choicePanelCanvasGroup.alpha = 0f;
        choicePanelCanvasGroup.interactable = false;
        choicePanelCanvasGroup.blocksRaycasts = false;

        // Fade in
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            choicePanelCanvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        choicePanelCanvasGroup.interactable = true;
        choicePanelCanvasGroup.blocksRaycasts = true;

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(btnAccept.gameObject);
        btnAccept.onClick.RemoveAllListeners();
        btnDecline.onClick.RemoveAllListeners();

        btnAccept.onClick.AddListener(AcceptQuest);
        btnDecline.onClick.AddListener(DeclineQuest);
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
    void AcceptQuest()
    {
        if (!TaskTracker.Instance.IsBobQuestComplete)
        {
            dialogueLines = new DialogueLine[]
            {
            new DialogueLine { speaker = "Player", line = "Alright. I’ll find those coins for you." },
            new DialogueLine { speaker = "Bob", line = "Thank you kindly! You’ll be helpin’ more than you know." },
            new DialogueLine { speaker = "Bob", line = "Come back once you’ve got ‘em." }
            };

            TaskTracker.Instance.StartTracking("Collect 5 coins: ", 5);
            choicePanel.SetActive(false);
            awaitingChoice = false;
            if (coinManager != null)
                coinManager.SetCoinsActive(true);
            currentLine = 0; // Reset index
            ShowLine();      // Start new dialogue
        }
    }


    void DeclineQuest()
    {
        dialogueLines = new DialogueLine[]
        {
        new DialogueLine { speaker = "Player", line = "Sorry, I’ve got other things to do right now." },
        new DialogueLine { speaker = "Bob", line = "Ah, no worries. Just thought I’d ask. Maybe another time." }
        };

        choicePanel.SetActive(false);
        awaitingChoice = false;
        currentLine = 0; // Reset index
        ShowLine();      // Start new dialogue
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
