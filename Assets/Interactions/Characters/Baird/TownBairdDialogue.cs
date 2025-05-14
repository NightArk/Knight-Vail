using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Invector.vCharacterController;

public class TownBairdDialogue : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        [TextArea] public string line;
    }
    public CanvasGroup choicePanelCanvasGroup;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerNameText;
    public GameObject choicePanel;
    public Button btnAccept;
    public Button btnDecline;

    public vThirdPersonController player;
    private vThirdPersonInput playerInput;
    private Animator animator;

    private bool isPlayerInRange = false;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private bool awaitingChoice = false;
    private Coroutine typingCoroutine;


    public AudioSource typingAudioSource;
    public AudioSource music;

    public AudioSource btnY;
    public AudioSource btnN;


    private int currentLine = 0;
    private int currentQuestion = 0;

    private DialogueLine[] dialogueSequence;
    private List<DialogueLine> poemLines = new();


    // Question answers as bools
    private bool isJoyful;         // Q1
    private bool usesStrength;     // Q2
    private bool hasHorse;         // Q3
    private bool endsWithFame;     // Q4

    private DialogueLine[] introLines = new DialogueLine[]
    {
        new DialogueLine { speaker = "Town Baird", line = "Ah, greetings, traveler! I am the town's Baird, spinner of tales and songs." },
        new DialogueLine { speaker = "Town Baird", line = "For but a few words, I shall craft thee a poem, shaped by thy soul." },
        new DialogueLine { speaker = "Town Baird", line = "Wouldst thou like a poem made just for thee?" },
        new DialogueLine { speaker = "", line = "[CHOICE]" }
    };

    private DialogueLine[] declineLines = new DialogueLine[]
    {
        new DialogueLine { speaker = "Axel", line = "Not right now, thanks." },
        new DialogueLine { speaker = "Town Baird", line = "Ah, no worries. Mayhap another day, then." }
    };

    private DialogueLine[] acceptIntroLines = new DialogueLine[]
    {
        new DialogueLine { speaker = "Axel", line = "Sure! I’d love a poem." },
        new DialogueLine { speaker = "Town Baird", line = "Splendid! First, a few questions, so I may know thee better." }
    };

    private string[] questionPrompts = new string[]
    {
        "Shall thy tale be light with joy, or heavy with heart?",
        "Do you fight with strength or with smarts?",
        "What curious friend walks beside thee?",
        "And when your tale is done… what remains?"
    };

    private string[,] questionChoices = new string[4, 2]
    {
        { "Let it be bright and full of cheer! [Y]", "Let it be deep, with a touch of sorrow… [N]" },
        { "I swing my sword with pride! [Y]", "I win with clever words and plans! [N]" },
        { "A brave and loyal horse. [Y]", "A cheeky frog who talks too much! [N]" },
        { "Songs sung of me for years to come! [Y]", "Only whispers… and a mystery. [N]" }
    };



    void Start()
    {
        animator = GetComponentInParent<Animator>();
        playerInput = player.GetComponent<vThirdPersonInput>();
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
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

        if (awaitingChoice)
        {
            if (Input.GetKeyDown(KeyCode.Y)) btnAccept.onClick.Invoke();
            else if (Input.GetKeyDown(KeyCode.N)) btnDecline.onClick.Invoke();
        }
    }

    void StartDialogue()
    {
        music.Play();
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        currentLine = 0;
        currentQuestion = 0;

        isJoyful = false;
        usesStrength = false;
        hasHorse = false;
        endsWithFame = false;

        animator.SetTrigger("isBowing");
        animator.SetBool("isTalking", true); // Start talking animation
        StopPlayerMovement();
        ShowLines(introLines);
    }

    void ShowLines(DialogueLine[] lines)
    {
        StopAllCoroutines();
        dialogueSequence = lines;
        currentLine = 0;
        ShowLine();
    }

    void ShowLine()
    {
        if (currentLine >= dialogueSequence.Length)
        {
            if (dialogueSequence == acceptIntroLines)
            {
                AskNextQuestion(); // continue to questions
                return;
            }
            else if (dialogueSequence == declineLines || dialogueSequence == introLines)
            {
                EndDialogue();
                return;
            }
            else if (dialogueSequence == poemLines.ToArray())
            {
                EndDialogue();
                return;
            }

            // If we just finished a question, go to the next one
            if (dialogueSequence.Length == 2 && dialogueSequence[1].line == "[CHOICE]")
            {
                AskNextQuestion();
                return;
            }

            return;
        }

        DialogueLine line = dialogueSequence[currentLine];

        if (line.line == "[CHOICE]")
        {
            speakerNameText.text = ""; // No speaker
            dialogueText.text = "";    // No text
            StartCoroutine(ShowChoicePanel());
            return;
        }

        speakerNameText.text = line.speaker;
        typingCoroutine = StartCoroutine(TypeLine(line.line));

    }



    IEnumerator ShowChoicePanel()
    {
        awaitingChoice = true;
        yield return new WaitUntil(() => isTyping == false);

        btnAccept.onClick.RemoveAllListeners();
        btnDecline.onClick.RemoveAllListeners();

        if (dialogueSequence == introLines)
        {
            btnAccept.GetComponentInChildren<TextMeshProUGUI>().text = "Yes (Y)";
            btnDecline.GetComponentInChildren<TextMeshProUGUI>().text = "No (N)";
            btnAccept.onClick.AddListener(() => AcceptPoem());
            btnDecline.onClick.AddListener(() => DeclinePoem());
        }
        else
        {
            // Question Choice
            btnAccept.GetComponentInChildren<TextMeshProUGUI>().text = questionChoices[currentQuestion, 0];
            btnDecline.GetComponentInChildren<TextMeshProUGUI>().text = questionChoices[currentQuestion, 1];
            btnAccept.onClick.AddListener(() => ChooseAnswer(true));
            btnDecline.onClick.AddListener(() => ChooseAnswer(false));
        }

        // Now that the correct text is set, show and fade the panel
        choicePanel.SetActive(true);
        choicePanelCanvasGroup.alpha = 0f;
        choicePanelCanvasGroup.interactable = false;
        choicePanelCanvasGroup.blocksRaycasts = false;

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            choicePanelCanvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        choicePanelCanvasGroup.alpha = 1f;
        choicePanelCanvasGroup.interactable = true;
        choicePanelCanvasGroup.blocksRaycasts = true;

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(btnAccept.gameObject);
    }


    void AcceptPoem()
    {
        btnY.Play();
        awaitingChoice = false;
        choicePanel.SetActive(false);
        ShowLines(acceptIntroLines);
    }

    void DeclinePoem()
    {
        btnN.Play();
        awaitingChoice = false;
        choicePanel.SetActive(false);
        ShowLines(declineLines);
        animator.SetBool("isTalking", false); // Stop talking animation when poem is declined
    }

    void AdvanceDialogue()
    {
        currentLine++;
        if (currentLine < dialogueSequence.Length)
        {
            ShowLine();
        }
        else
        {
            if (dialogueSequence == acceptIntroLines)
                AskNextQuestion();
            else
                EndDialogue();
        }
    }

    void AskNextQuestion()
    {
        if (currentQuestion >= questionPrompts.Length)
        {
            GeneratePoem();
            return;
        }

        // Keep talking animation while asking questions
        animator.SetBool("isTalking", true); // Ensure talking animation stays true during questions

        DialogueLine[] qLines = new DialogueLine[]
        {
        new DialogueLine { speaker = "Town Baird", line = questionPrompts[currentQuestion] },
        new DialogueLine { speaker = "Axel", line = "[CHOICE]" }
        };

        ShowLines(qLines);
    }

    void ChooseAnswer(bool choseFirstOption)
    {
        if (choseFirstOption)
            btnY.Play();
        else
            btnN.Play();
        awaitingChoice = false;
        choicePanel.SetActive(false);

        // Save to bools
        switch (currentQuestion)
        {
            case 0: isJoyful = choseFirstOption; break;
            case 1: usesStrength = choseFirstOption; break;
            case 2: hasHorse = choseFirstOption; break;
            case 3: endsWithFame = choseFirstOption; break;
        }

        currentQuestion++;
        AskNextQuestion();
    }

    void GeneratePoem()
    {
        poemLines = new List<DialogueLine>();
        poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Ahem... Allow me to begin thy tale..." });

        animator.SetBool("isYelling", true);
        animator.SetBool("isTalking", true);

        if (isJoyful && usesStrength && hasHorse && endsWithFame)
        {
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "In lands where laughter kissed the sky," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "A hero marched with head held high," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Their steed a blur through meadows wide," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Their tale now echoes far and wide." });
        }
        else if (isJoyful && usesStrength && hasHorse && !endsWithFame)
        {
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "When dawnlight spilled o'er blooming fields," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "A steadfast knight refused to yield," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "With hooves that thundered down the trail," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Their fate now cloaked in moonlight’s veil." });
        }
        else if (isJoyful && usesStrength && !hasHorse && !endsWithFame)
        {
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "In lands where the sun forever gleams," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "A hero stood, as bold as dreams," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "A froggy friend with endless cheer," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Their story fades, but still we hear." });
        }
        else if (isJoyful && !usesStrength && !hasHorse && !endsWithFame)
        {
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "In bright fields, laughter fills the air," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "With clever words, they showed no fear." });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "A frog beside them, chatty and bold," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Now whispers of their tale are all we're told." });
        }
        else if (!isJoyful && !usesStrength && !hasHorse && !endsWithFame)
        {
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Beneath the sky where shadows lay," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "With cunning mind and words to sway," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "A frog that croaked with secrets deep," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Now only whispers through the trees do creep." });
        }
        else if (!isJoyful && usesStrength && hasHorse && endsWithFame)
        {
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "In shadows deep, where sorrow lies," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "A hero stands, with steady eyes," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "With faithful steed, they ride the storm," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Their name a tale, forever warm." });
        }
        else if (!isJoyful && usesStrength && hasHorse && !endsWithFame)
        {
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Through shadowed paths, where sorrow sleeps," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "A warrior bold, with sword that sweeps," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "On horseback strong, they ride alone," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "A tale untold, a secret grown." });
        }
        else if (!isJoyful && usesStrength && !hasHorse && !endsWithFame)
        {
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "In lands where sorrow’s shadow lays," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "A hero rises, strength ablaze," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Beside them hops a friend so spry," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Yet in the end, the tale’s a sigh." });
        }
        else if (isJoyful && !usesStrength && hasHorse && endsWithFame)
        {
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "With laughter bright, they start their quest," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Not strength, but wit puts them to the test," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Upon a steed, they ride with grace," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Their name etched in time, no one can erase." });
        }
        else if (isJoyful && !usesStrength && !hasHorse && endsWithFame)
        {
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "With joy in their heart, they begin the quest," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Their strength lies not in the blade, but in the mind’s zest," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Beside them, a frog with boundless glee," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Their name lives on, whispered in eternity." });
        }
        else if (isJoyful && !usesStrength && hasHorse && !endsWithFame)
        {
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "With joy in their heart, they begin the quest," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Their mind sharp as a blade, their wit at its best," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Upon a steed, gallant and true, they ride," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "And when the tale ends, only whispers shall abide." });
        }
        else if (isJoyful && usesStrength && !hasHorse && endsWithFame)
        {
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "With laughter loud, they charged ahead," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "A blade in hand, where others fled," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Their frog companion croaked in rhyme," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Their name lives on, outlasting time." });
        }
        else if (!isJoyful && usesStrength && !hasHorse && endsWithFame)
        {
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "A sorrowed soul, yet fierce in fight," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "With every swing, they proved their might," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "A chattering frog leapt by their side," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Their tale lives on, a hero’s pride." });
        }
        else if (!isJoyful && !usesStrength && hasHorse && endsWithFame)
        {
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "A tale begins in shadowed hue," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Their mind a blade, their thoughts cut through," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "With loyal steed beneath the sky," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Their name shall echo, never die." });
        }
        else if (!isJoyful && !usesStrength && !hasHorse && endsWithFame)
        {
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Beneath grey skies, their tale took flight," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "They solved with wit, not brute nor blade," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "A frog that joked both day and night," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "And left a legend that will never fade." });
        }
        else if (!isJoyful && !usesStrength && hasHorse && !endsWithFame)
        {
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "From sorrow’s depth, their path began," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "With cunning mind and careful scheme," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "A noble horse did heed their plan," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Yet where they went, none dare to dream." });
        }



        else
        {
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "A tale lost, unknown and grim," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Its edges frayed and colors dim." });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "No steed nor friend to light the way," });
            poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Yet still, it echoes to this day." });
        }

        poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Thus ends thy tale, oh soul sincere—" });
        poemLines.Add(new DialogueLine { speaker = "Town Baird", line = "Now go, and live with heart and cheer!" });

        ShowLines(poemLines.ToArray());
    }



    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        typingAudioSource.Play();

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.02f);
        }

        typingAudioSource.Stop();
        isTyping = false;
    }

    void FinishTyping()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (currentLine < dialogueSequence.Length)
        {
            dialogueText.text = dialogueSequence[currentLine].line.Replace("[CHOICE]", "");
        }
        isTyping = false;
        typingAudioSource.Stop();
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        ResumePlayerMovement();
        music.Stop();
        animator.SetBool("isTalking", false);  // Stop talking animation
        animator.SetBool("isYelling", false);  // Stop yelling animation
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
