using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SharedTaskPanelManager : MonoBehaviour
{
    public static SharedTaskPanelManager Instance;

    public GameObject mainTaskPanel; // Drag your shared panel here in the Inspector
    public TextMeshProUGUI tmpText;
    public TextMeshProUGUI tmpText1;

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
        // Create a copy of the material so changes don’t affect all texts
        Material newMat = new Material(tmpText.fontMaterial);

        // Set outline width (0.2 is good, max is usually 1)
        newMat.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.1f);

        // Set outline color to black
        newMat.SetColor(ShaderUtilities.ID_OutlineColor, Color.black);

        // Assign modified material
        tmpText.fontMaterial = newMat;
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