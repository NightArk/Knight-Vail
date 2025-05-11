using TMPro;
using UnityEngine;

public class TaskTracker : MonoBehaviour
{
    public static TaskTracker Instance;

    public GameObject taskPanel;
    public TextMeshProUGUI taskText;

    private string taskName;
    private int requiredAmount;
    private int currentAmount;

    // Only Bob's quest
    public bool IsBobQuestActive { get; private set; } = false;
    public bool IsBobQuestComplete { get; private set; } = false;
    public bool IsBobQuestFinished { get; private set; } = false;


    public bool HasActiveTask => IsBobQuestActive; // or IsOldManQuestActive
    public bool IsTaskInProgress => currentAmount < requiredAmount;
    public bool IsTaskComplete => currentAmount >= requiredAmount;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Start tracking the quest for Bob
    public void StartTracking(string task, int amount)
    {
        taskName = task;
        requiredAmount = amount;
        currentAmount = 0;

        IsBobQuestActive = true;
        IsBobQuestComplete = false;

        UpdateTaskText();
        taskPanel.SetActive(true);

        SharedTaskPanelManager.Instance?.UpdatePanelVisibility();
    
}

    public void Collect()
    {
        if (!IsBobQuestActive) return;

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
            taskText.color = new Color(0.2f, 0.7f, 0.3f); // #33B25C
            taskText.text = $"{taskName}: Completed! Go back to Bob";
        }
        else
        {
            taskText.color = new Color(0.95f, 0.75f, 0.2f); // #F2BF33
            taskText.text = $"{taskName}: {currentAmount}/{requiredAmount}";
        }
    }

    void CompleteTask()
    {
        Debug.Log("Bob's Task Completed!");
        IsBobQuestComplete = true;
    }

    public void MarkBobQuestAsFinished()
    {
        IsBobQuestActive = false;
        IsBobQuestComplete = false;
        IsBobQuestFinished = true; // Set finished to true
        taskName = "";
        currentAmount = 0;
        requiredAmount = 0;
        taskPanel.SetActive(false);

        SharedTaskPanelManager.Instance?.UpdatePanelVisibility();
    }



    public void ClearTask()
    {
        IsBobQuestActive = false;
        IsBobQuestComplete = false;
        taskName = "";
        currentAmount = 0;
        requiredAmount = 0;
        taskPanel.SetActive(false);
    }
}
