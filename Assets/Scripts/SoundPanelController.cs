using UnityEngine;

public class SoundPanelController : MonoBehaviour
{
    public GameObject soundSettingsPanel; // assign your panel here

    void Start()
    {
        // make sure panel is hidden at start
        soundSettingsPanel.SetActive(false);
    }

    public void TogglePanel()
    {
        // toggle visibility
        soundSettingsPanel.SetActive(!soundSettingsPanel.activeSelf);
    }

    public void ClosePanel()
    {
        soundSettingsPanel.SetActive(false);
    }
}
