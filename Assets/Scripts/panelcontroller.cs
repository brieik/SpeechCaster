using UnityEngine;

public class PanelController : MonoBehaviour
{
    [Header("Target Panel")]
    public GameObject targetPanel;

    void Start()
    {
        if (targetPanel != null)
            targetPanel.SetActive(false); // hide at start
    }

    public void OpenPanel()
    {
        if (targetPanel != null)
            targetPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        if (targetPanel != null)
            targetPanel.SetActive(false);
    }

    public void TogglePanel()
    {
        if (targetPanel != null)
            targetPanel.SetActive(!targetPanel.activeSelf);
    }
}
