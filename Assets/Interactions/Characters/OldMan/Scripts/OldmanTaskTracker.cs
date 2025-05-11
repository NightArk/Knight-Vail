using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class OldmanTaskTracker : MonoBehaviour
{
    public static OldmanTaskTracker Instance;

    public GameObject taskPanel;
    public TextMeshProUGUI taskText;

    private string taskName;
    private int requiredAmount;
    private int currentAmount;

    public bool IsOldManQuestActive { get; private set; } = false;
    public bool IsOldManQuestComplete { get; private set; } = false;

    public bool HasActiveTask => !string.IsNullOrEmpty(taskName);
    public bool IsTaskInProgress => currentAmount < requiredAmount;
    public bool IsTaskComplete => currentAmount >= requiredAmount;
    public bool IsOldManQuestFinished { get; private set; } = false;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartTracking(string task, int amount)
    {
        taskName = task;
        requiredAmount = amount;
        currentAmount = 0;

        IsOldManQuestActive = true;
        IsOldManQuestComplete = false;

        UpdateTaskText();
        taskPanel.SetActive(true);

        SharedTaskPanelManager.Instance?.UpdatePanelVisibility();
    

}

    public void Collect()
    {
        if (!IsOldManQuestActive) return;

        if (currentAmount < requiredAmount)
        {
            currentAmount++;
            UpdateTaskText();

            if (currentAmount >= requiredAmount)
                CompleteTask();
        }
    }

    void UpdateTaskText()
    {
        if (currentAmount >= requiredAmount)
        {
            // Muted enchanted green for completed quests
            taskText.color = new Color(0.2f, 0.7f, 0.3f); // #33B25C
            taskText.text = $"{taskName}: Completed! Return to The Old Man";
        }
        else
        {
            // Warm gold for active quests
            taskText.color = new Color(0.95f, 0.75f, 0.2f); // #F2BF33
            taskText.text = $"{taskName}: {currentAmount}/{requiredAmount}";
        }
    }

    void CompleteTask()
    {
        Debug.Log("Old Man's Task Completed!");
        IsOldManQuestComplete = true;
    }

    public void MarkOldManQuestAsFinished()
    {
        IsOldManQuestActive = false;
        IsOldManQuestComplete = false;
        IsOldManQuestFinished = true;

        taskName = "";
        currentAmount = 0;
        requiredAmount = 0;
        taskPanel.SetActive(false);

        // Moved this AFTER taskName is cleared
        SharedTaskPanelManager.Instance?.UpdatePanelVisibility();
    }



    public void ClearTask()
    {
        IsOldManQuestActive = false;
        IsOldManQuestComplete = false;
        IsOldManQuestFinished = false;
        taskName = "";
        currentAmount = 0;
        requiredAmount = 0;
        taskPanel.SetActive(false);
    }

}
