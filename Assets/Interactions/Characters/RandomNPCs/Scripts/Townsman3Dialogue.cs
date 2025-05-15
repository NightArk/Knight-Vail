using System.Collections;
using UnityEngine;
using TMPro;
using Invector.vCharacterController;

public class Townsman3Dialogue : MonoBehaviour
{
    [TextArea]
    public string[] repeatLines = new string[]
    {
        "Welcome to our little town!",
        "People here are the friendliest you'll meet.",
        "Take your time and enjoy the sights!",
        "Need a smile? You’ve come to the right place.",
        "The bakery just made fresh bread!",
        "Lots of stories here if you’re curious.",
        "Don't be shy, say hi!",
        "We love new faces around here.",
        "Good luck on your journey!"
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

    public static bool DialogueIsActiveGlobal = false;



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
        if (DialogueIsActiveGlobal)
            return; // Don't start if another dialogue is active

        DialogueIsActiveGlobal = true;
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
        speakerNameText.text = "Happy Townsman";
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
        DialogueIsActiveGlobal = false;
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
