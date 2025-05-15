using System.Collections;
using UnityEngine;
using TMPro;
using Invector.vCharacterController;

public class Townsman2Dialogue : MonoBehaviour
{
    [TextArea]
    public string[] repeatLines = new string[]
    {
        "Hmph. What do you want? Can't you see I'm busy minding my own business?",
        "If you're looking for a friendly chat, you've come to the wrong person.",
        "People around here talk too much. I prefer silence.",
        "Keep your distance. I don't do well with company.",
        "Don't expect me to believe your tall tales.",
        "I’ve seen too many fools come and go to trust anyone.",
        "Leave me be. The world’s complicated enough without extra noise.",
        "I’m not unfriendly, just… selective with my time.",
        "Some folks call it grumpy, I call it being sensible.",
        "Go on, find someone else to bother."
    };

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerNameText;

    public vThirdPersonController player;
    private vThirdPersonInput playerInput;

    public float typingSpeed = 0.02f;
    public AudioSource typingAudioSource;

    private bool isPlayerInRange = false;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private string currentLine;


    private Animator animator;

    void Start()
    {
        dialoguePanel.SetActive(false);
        playerInput = player.GetComponent<vThirdPersonInput>();
        animator = GetComponentInParent<Animator>();
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
                EndDialogue();
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        StopPlayerMovement();

        ShowRandomLine();

    }

    void ShowRandomLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        currentLine = repeatLines[Random.Range(0, repeatLines.Length)];
        speakerNameText.text = "Skeptical Townsman";
        typingCoroutine = StartCoroutine(TypeLine(currentLine));
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
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingAudioSource.Stop();
        dialogueText.text = currentLine;
        isTyping = false;
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
