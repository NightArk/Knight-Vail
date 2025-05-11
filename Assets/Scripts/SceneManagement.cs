using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagement : MonoBehaviour
{
    public bool isMainMenu;
    public GameObject pauseScreen;
    public Button resumeButton;
    private bool isPaused = false;

    void Start()
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }

        if (pauseScreen != null)
        {
            pauseScreen.SetActive(false);
        }
    }

    void Update()
    {
        if (!isMainMenu && Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame();  
            }
            else
            {
                PauseGame(); 
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SceneChanger(int sceneno)
    {
        Time.timeScale = 1f; // Ensure time is reset when changing scenes
        SceneManager.LoadScene(sceneno);
    }

    public void UnlockScreen()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
