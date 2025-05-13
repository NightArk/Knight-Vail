using UnityEngine;

public class RingPickup : MonoBehaviour
{
    public string taskTitle = "Find the Old Man's ring";

    private bool playerInRange = false;
    public AudioSource audio;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (OldmanTaskTracker.Instance.IsOldManQuestActive && !OldmanTaskTracker.Instance.IsOldManQuestComplete)
            {
                audio.Play(); // Play sound
                OldmanTaskTracker.Instance.Collect(); // Increases progress to 1
                Destroy(gameObject); // 💥 Ring disappears
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
