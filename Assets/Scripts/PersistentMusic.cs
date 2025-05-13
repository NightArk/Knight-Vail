using UnityEngine;

public class PersistentMusic : MonoBehaviour
{
    private static PersistentMusic instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Make sure the AudioSource starts playing
        AudioSource source = GetComponent<AudioSource>();
        if (!source.isPlaying)
        {
            source.Play();
        }
    }
}
