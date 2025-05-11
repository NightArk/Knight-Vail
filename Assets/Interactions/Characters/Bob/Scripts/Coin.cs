using UnityEngine;

public class Coin : MonoBehaviour
{
    private bool isPlayerInRange = false;
    public AudioSource audioSource;


    private void Start()
    {
        
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed while in range.");
            TaskTracker.Instance.Collect();
            audioSource.Play();
            // Delay destruction to let sound finish playing
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player entered range.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player exited range.");
        }
    }
}
 