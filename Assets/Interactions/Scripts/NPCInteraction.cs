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

    private bool isCoin = false;

    private void Start()
    {
        if (textPrompt != null)
            textPrompt.gameObject.SetActive(false);

        originalPosition = textPrompt.transform.localPosition;

        if (CompareTag("Coin"))
            isCoin = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player is in range to interact.");

            if (textPrompt != null)
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
            HidePrompt();

                Debug.Log("Interacting with NPC...");
            
        }
        

        // Bounce animation
        if (textPrompt != null && textPrompt.gameObject.activeSelf)
        {
            float newY = originalPosition.y + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
            textPrompt.transform.localPosition = new Vector3(originalPosition.x, newY, originalPosition.z);
        }
    }

    void LateUpdate()
    {
        if (textPrompt != null && textPrompt.gameObject.activeSelf)
        {
            Vector3 cameraPosition = Camera.main.transform.position;
            Vector3 direction = (textPrompt.transform.position - cameraPosition).normalized;
            direction.y = 0f; // Locks vertical rotation to prevent flipping
            textPrompt.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void HidePrompt()
    {
        if (textPrompt != null)
            textPrompt.gameObject.SetActive(false);
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
