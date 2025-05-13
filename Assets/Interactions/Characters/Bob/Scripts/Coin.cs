using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Coin : MonoBehaviour
{
    public float spinSpeed = 90f;
    public float bounceSpeed = 2f;
    public float bounceHeight = 0.25f;
    private Vector3 startPos;

    public AudioSource audioSource;

    void Start()
    {
        startPos = transform.position;

        // Ensure collider is set as trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Update()
    {
        // Spin
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);

        // Bounce
        float newY = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        transform.position = new Vector3(startPos.x, startPos.y + newY, startPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player collected the coin.");
            TaskTracker.Instance.Collect();
            audioSource.Play();
            Destroy(gameObject);
        }
    }
}
