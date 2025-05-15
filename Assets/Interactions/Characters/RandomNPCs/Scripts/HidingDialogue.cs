using System.Collections;
using UnityEngine;
using TMPro;
using Invector.vCharacterController;

public class HidingDialogue : MonoBehaviour
{
    [TextArea]
    public string[] repeatLines = new string[]
    {
        "Shhh! Did you hear that? I think they're close...",
        "I just want to go back home... I miss my bed.",
        "The elves... they were everywhere... glowing eyes!",
        "Monsters took my brother. I ran. I didn’t look back.",
        "I’ve been hiding here for days. I’m too scared to move.",
        "They scream at night. I can’t sleep. I don’t want to sleep.",
        "I came here to trade... I didn’t expect *this*.",
        "Do you think it's safe now? Please tell me it's safe.",
        "I’m not a fighter… I just want to see my family again.",
        "Why did I ever leave the village? This was a mistake.",
        "They move so fast. You hear a whisper and they’re already behind you.",
        "My hands won’t stop shaking. I haven’t eaten in two days.",
        "I keep seeing their shadows in my dreams… even when I’m awake.",
        "You’re brave to be out there. I’m not like you.",
        "Please… if you find a way out, come back for me."
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
            return;

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
        speakerNameText.text = "Frightened Man";
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
