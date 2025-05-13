using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider sfxSlider;
    public Slider musicSlider;

    private float lastSFXValue;
    private float lastMusicValue;

    private const string SFXVolumeKey = "SFXVolume";

    private MusicManager musicManager;

    public AudioMixer mixer;

    private void Start()
    {
        // Get the persistent MusicManager
        musicManager = FindObjectOfType<MusicManager>();

        // Load saved volumes
        float savedSFX = Mathf.Clamp(PlayerPrefs.GetFloat(SFXVolumeKey, 1f), 0.0001f, 1f);
        sfxSlider.value = savedSFX;
        lastSFXValue = savedSFX;
        ApplySFXVolume(savedSFX);

        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 1f);
        musicSlider.value = savedMusic;
        lastMusicValue = savedMusic;

        if (musicManager != null)
        {
            musicManager.SetMusicVolume(savedMusic);
        }
    }

    private void Update()
    {
        if (Mathf.Abs(sfxSlider.value - lastSFXValue) > 0.001f)
        {
            lastSFXValue = sfxSlider.value;
            ApplySFXVolume(lastSFXValue);
        }

        if (Mathf.Abs(musicSlider.value - lastMusicValue) > 0.001f)
        {
            lastMusicValue = musicSlider.value;

            if (musicManager != null)
            {
                musicManager.SetMusicVolume(lastMusicValue);
            }
        }
    }

    private void ApplySFXVolume(float value)
    {
        float dB = Mathf.Log10(value) * 20f;
        mixer.SetFloat("SFXVolume", dB);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

}
