using UnityEngine;
using TMPro;

public class ControlsToggle : MonoBehaviour
{
    public GameObject controlsPanel; // Assign ControlsPanel
    public TextMeshProUGUI controlPanelText; // Assign Text (TMP) under ControlPanelText

    private bool isVisible = false;

    void Start()
    {
        controlsPanel.SetActive(false);
        controlPanelText.text = "Press [Tab] for controls";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isVisible = !isVisible;
            controlsPanel.SetActive(isVisible);
            controlPanelText.text = isVisible ? "Press [Tab] to hide" : "Press [Tab] for controls";
        }
    }
}
