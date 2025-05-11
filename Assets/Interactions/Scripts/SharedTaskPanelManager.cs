using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedTaskPanelManager : MonoBehaviour
{
    public static SharedTaskPanelManager Instance;

    public GameObject mainTaskPanel; // Drag your shared panel here in the Inspector

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdatePanelVisibility();
    }

    /// <summary>
    /// Call this whenever a task starts or ends
    /// </summary>
    public void UpdatePanelVisibility()
    {
        bool bobActive = TaskTracker.Instance != null && TaskTracker.Instance.HasActiveTask;
        bool oldmanActive = OldmanTaskTracker.Instance != null && OldmanTaskTracker.Instance.HasActiveTask;

        mainTaskPanel.SetActive(bobActive || oldmanActive);
    }
}