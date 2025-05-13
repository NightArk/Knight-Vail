using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    public AudioClip menuMusic;
    public AudioSource audioSource;  // Now public to control volume externally

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = menuMusic;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f); // Load volume

            audioSource.Play();
        }
        else
        {
            Destroy(gameObject); // prevent duplicates
        }
    }

    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
}
