using TMPro;
using UnityEngine;

public class ExclamationBounce : MonoBehaviour
{
    public TextMeshProUGUI textToDestroy; // For world space TextMeshPro — change to TextMeshProUGUI if UI
    public float bounceHeight = 0.1f;
    public float bounceSpeed = 2f;

    private bool isPlayerInRange = false;
    private Vector3 originalPosition;

    void Start()
    {
        if (textToDestroy != null)
            originalPosition = textToDestroy.transform.localPosition;
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (textToDestroy != null)
                Destroy(textToDestroy.gameObject);
        }

        if (textToDestroy != null)
        {
            float newY = originalPosition.y + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
            textToDestroy.transform.localPosition = new Vector3(originalPosition.x, newY, originalPosition.z);
        }
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