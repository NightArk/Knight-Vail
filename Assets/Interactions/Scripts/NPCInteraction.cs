using System.Collections;
using UnityEngine;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    private bool isPlayerInRange = false;
    public TextMeshProUGUI textPrompt;
    public float fadeDuration = 0.5f;
    private Coroutine currentFadeCoroutine;
    private Vector3 originalPosition;
    public float bounceSpeed = 2f;
    public float bounceHeight = 0.1f;

    private bool suppressPrompt = false;

    private void Start()
    {
        if (textPrompt != null)
            textPrompt.gameObject.SetActive(false);

        originalPosition = textPrompt.transform.localPosition;

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player is in range to interact.");

            if (textPrompt != null && !suppressPrompt)
            {
                textPrompt.gameObject.SetActive(true);

                if (currentFadeCoroutine != null)
                    StopCoroutine(currentFadeCoroutine);

                currentFadeCoroutine = StartCoroutine(FadeInText());
            }
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            suppressPrompt = false; // allow prompt to reappear next time
            Debug.Log("Player left the interaction range.");

            if (currentFadeCoroutine != null)
                StopCoroutine(currentFadeCoroutine);

            currentFadeCoroutine = StartCoroutine(FadeOutText());
        }
    }


    void Update()
    {
        // Optional: coin collection (if needed later)

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SuppressAndHidePrompt());
            Debug.Log("Interacting with NPC...");
        }



        // Bounce animation
        if (!suppressPrompt && textPrompt != null && textPrompt.gameObject.activeSelf)
        {
            float newY = originalPosition.y + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
            textPrompt.transform.localPosition = new Vector3(originalPosition.x, newY, originalPosition.z);
        }
    }

    void LateUpdate()
    {
        if (!suppressPrompt && textPrompt != null && textPrompt.gameObject.activeSelf)
        {
            Vector3 cameraPosition = Camera.main.transform.position;
            Vector3 direction = (textPrompt.transform.position - cameraPosition).normalized;
            direction.y = 0f; // Locks vertical rotation to prevent flipping
            textPrompt.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
    private IEnumerator SuppressAndHidePrompt()
    {
        yield return null; // wait one frame
        suppressPrompt = true;
        HidePrompt();
    }


    public void HidePrompt()
    {
        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);

        if (textPrompt != null)
        {
            textPrompt.gameObject.SetActive(false);

            // Immediately make it fully transparent to avoid sudden reappearance
            Color c = textPrompt.color;
            c.a = 0f;
            textPrompt.color = c;
        }
    }


    public void ShowPrompt()
    {
        if (isPlayerInRange && textPrompt != null)
            textPrompt.gameObject.SetActive(true);
    }

    private IEnumerator FadeInText()
    {
        float timeElapsed = 0f;
        Color startColor = textPrompt.color;
        startColor.a = 0f;
        textPrompt.color = startColor;

        while (timeElapsed < fadeDuration)
        {
            if (suppressPrompt) yield break; // <-- abort if prompt is suppressed

            timeElapsed += Time.deltaTime;
            startColor.a = Mathf.Lerp(0f, 1f, timeElapsed / fadeDuration);
            textPrompt.color = startColor;
            yield return null;
        }

        startColor.a = 1f;
        textPrompt.color = startColor;
    }


    private IEnumerator FadeOutText()
    {
        float timeElapsed = 0f;
        Color startColor = textPrompt.color;
        startColor.a = 1f;
        textPrompt.color = startColor;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            startColor.a = Mathf.Lerp(1f, 0f, timeElapsed / fadeDuration);
            textPrompt.color = startColor;
            yield return null;
        }

        startColor.a = 0f;
        textPrompt.color = startColor;
        textPrompt.gameObject.SetActive(false);
    }
}
