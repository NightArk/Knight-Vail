using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
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
  //public TextMeshProUGUI instructionText;
    public float typingSpeed = 0.1f;
    //public AudioClip typingSound;
    public string[] dialogs;
    public int[] characterIds;
    public UnityEvent onDialogStart;
    public UnityEvent onDialogEnd;

    public DialogData dialogData;
    public AnimatorData animatorData;

    private int currentDialogIndex = 0;
    public AudioSource audioSource;
    private bool isTyping = false;
    private bool isTriggered = false;
    private bool skipTyping = false;

    private void Start()
    {
        //audioSource = GetComponent<AudioSource>();
      //audioSource.clip = typingSound;
        dialogBox.SetActive(false);
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource not found on " + gameObject.name);
        }

    }

    private void Update()
    {
        if (isTriggered && Input.GetKeyDown(KeyCode.E) && !dialogBox.activeSelf)
        {
            StartDialog();
            isTriggered = false;
        }
        else if (dialogBox.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                skipTyping = true;
            }
            else
            {
                ProceedToNextLine();
            }
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
            isTriggered = false;
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
                anim.ResetTrigger(animTrigger);
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
        skipTyping = false;
        //instructionText.text = "Press [space] to skip";
        if (audioSource != null)
        {
            audioSource.Play();
        }


        string sentence = dialogs[currentDialogIndex];
        for (int i = 0; i < sentence.Length; i++)
        {
            if (skipTyping)
            {
                dialogText.text = sentence;
                break;
            }

            dialogText.text += sentence[i];
            yield return new WaitForSeconds(typingSpeed);
        }

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        isTyping = false;
        //instructionText.text = "Press [space] to continue";
    }

    private void ProceedToNextLine()
    {
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
