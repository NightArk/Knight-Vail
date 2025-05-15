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
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject creditsPanel;
    public GameObject scrollView;

    public GameObject gameOverPanel;
    public GameObject gameCompletePanel;

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
        if (!isMainMenu && (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)))
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

        if ((gameOverPanel != null && gameOverPanel.activeSelf ||
            gameCompletePanel != null && gameCompletePanel.activeSelf) && !isPaused)
        {
            AutoPauseForEnd();
        }
    }
    private void AutoPauseForEnd()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
        settingsPanel.SetActive(false);
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

    public void OpenSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }
    public void OpenCredits()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(true);

        CreditsAutoScroll autoScroll = scrollView.GetComponent<CreditsAutoScroll>();
        if (autoScroll != null)
        {
            autoScroll.StartScrolling();
        }
    }
    public void BackToMainMenu()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }
    public void OpenPauseSettings()
    {
        pauseScreen.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        settingsPanel.SetActive(false);
        pauseScreen.SetActive(true);
    }

    public void GameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void GameComplete()
    {
        if (gameCompletePanel != null)
        {
            gameCompletePanel.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ResumeAfterGameComplete()
    {
        if (gameCompletePanel != null)
        {
            gameCompletePanel.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
