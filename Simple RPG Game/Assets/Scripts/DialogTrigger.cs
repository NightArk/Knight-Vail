using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogTrigger : MonoBehaviour
{
    [System.Serializable]
    public struct AnimatorData
    {
        public Animator[] animators;
        public string[] animationNames;
    }

    public GameObject dialogBox;
    public GameObject popup;
    public Text nameText;
    public Text dialogText;
    public float typingSpeed = 0.1f;
    public AudioClip typingSound;
    public string[] dialogs;
    public int[] characterIds;
    public UnityEvent onDialogStart;
    public UnityEvent onDialogEnd;

    public DialogData dialogData;
    public AnimatorData animatorData; // ✅ Embedded Animator Data

    private int currentDialogIndex = 0;
    private AudioSource audioSource;
    private bool isTyping = false;
    private bool isTriggered = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = typingSound;
        dialogBox.SetActive(false);
    }

    private void Update()
    {
        if (isTriggered && Input.GetKeyDown(KeyCode.E) && !dialogBox.activeSelf)
        {
            StartDialog();
            isTriggered = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTriggered = true;
            popup.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            popup.SetActive(false);
        }
    }

    public void StartDialog()
    {
        popup.SetActive(false);

        if (dialogs.Length > 0 && characterIds.Length == dialogs.Length)
        {
            dialogBox.SetActive(true);
            currentDialogIndex = 0;
            StartCoroutine(DisplayNameAndTypeDialog());
            onDialogStart.Invoke();
        }
        else
        {
            Debug.LogError("Invalid dialog or characterIds array length!");
        }
    }

    IEnumerator DisplayNameAndTypeDialog()
    {
        if (animatorData.animators != null && animatorData.animationNames != null &&
    currentDialogIndex < animatorData.animators.Length &&
    currentDialogIndex < animatorData.animationNames.Length)
        {
            Animator anim = animatorData.animators[currentDialogIndex];
            string animTrigger = animatorData.animationNames[currentDialogIndex];

            if (anim != null && !string.IsNullOrWhiteSpace(animTrigger))
            {
                anim.ResetTrigger(animTrigger); // Optional: in case it was already active
                anim.SetTrigger(animTrigger);
            }
        }

        dialogText.text = "";

        int characterIndex = GetCharacterIndexForId(characterIds[currentDialogIndex]);

        if (characterIndex >= 0 && characterIndex < dialogData.characters.Length)
        {
            nameText.color = dialogData.characters[characterIndex].color;
            nameText.text = dialogData.characters[characterIndex].characterName;
        }

        isTyping = true;
        audioSource.Play();

        foreach (char letter in dialogs[currentDialogIndex].ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        audioSource.Stop();
        isTyping = false;

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        dialogText.text = "";
        currentDialogIndex++;

        if (currentDialogIndex < dialogs.Length)
        {
            StartCoroutine(DisplayNameAndTypeDialog());
        }
        else
        {
            dialogBox.SetActive(false);
            onDialogEnd.Invoke();
        }
    }

    private int GetCharacterIndexForId(int characterId)
    {
        for (int i = 0; i < dialogData.characters.Length; i++)
        {
            if (dialogData.characters[i].idNumber == characterId)
            {
                return i;
            }
        }
        return -1;
    }
}
