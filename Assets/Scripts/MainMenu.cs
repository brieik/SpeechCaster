using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject userPanel;

    [Header("User UI")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_Dropdown userDropdown;
    [SerializeField] private TMP_Text greetingText;          // Main menu greeting
    [SerializeField] private TMP_Text userPanelGreetingText; // User panel greeting

    private const string LastUserKey = "LastUser";

    private void Start()
    {


    
        RefreshUserDropdown();

        List<string> users = UserManager.Instance.GetAllUsers();

        if (users.Count == 0)
        {
            // No users → show user panel for creation
            mainMenuPanel.SetActive(false);
            userPanel.SetActive(true);
        }
        else
        {
            // Try to auto-load last used user
            string lastUser = PlayerPrefs.GetString(LastUserKey, "");
            if (!string.IsNullOrEmpty(lastUser) && users.Contains(lastUser))
            {
                UserManager.Instance.SwitchUser(lastUser);
                UpdateGreetings();
                mainMenuPanel.SetActive(true);
                userPanel.SetActive(false);
            }
            else
            {
                mainMenuPanel.SetActive(false);
                userPanel.SetActive(true);
            }
        }
    }

     public void PlayGame()
    {
    SceneManager.LoadScene("SelectDifficulty"); // instead of "MainGame"
    }   


    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OpenUserPanel()
    {
        mainMenuPanel.SetActive(false);
        userPanel.SetActive(true);
        RefreshUserDropdown();
    }

    public void CreateUser()
    {
        string newUser = usernameInput.text.Trim();
        if (string.IsNullOrEmpty(newUser)) return;

        if (UserManager.Instance.CreateUser(newUser))
        {
            RefreshUserDropdown();

            PlayerPrefs.SetString(LastUserKey, newUser);
            PlayerPrefs.Save();

            UpdateGreetings();

            userPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Username invalid or already exists.");
        }
    }

    public void SelectUser()
    {
        if (userDropdown.options.Count == 0) return;

        string selected = userDropdown.options[userDropdown.value].text;
        if (UserManager.Instance.SwitchUser(selected))
        {
            PlayerPrefs.SetString(LastUserKey, selected);
            PlayerPrefs.Save();

            UpdateGreetings();

            userPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
    }

    public void DeleteSelectedUser()
    {
        if (userDropdown.options.Count == 0) return;

        string selected = userDropdown.options[userDropdown.value].text;

        if (UserManager.Instance.DeleteUser(selected))
        {
            Debug.Log("Deleted user: " + selected);

            RefreshUserDropdown();
            UpdateGreetings();

            List<string> users = UserManager.Instance.GetAllUsers();
            if (users.Count == 0)
            {
                // No users left → show user panel to create new
                mainMenuPanel.SetActive(false);
                userPanel.SetActive(true);
            }
        }
    }

    public void BackToMenu()
    {
        userPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private void RefreshUserDropdown()
    {
        if (userDropdown == null) return;

        List<string> users = UserManager.Instance.GetAllUsers();
        userDropdown.ClearOptions();
        userDropdown.AddOptions(users);

        // Optional: highlight last user in dropdown
        string lastUser = PlayerPrefs.GetString(LastUserKey, "");
        int index = users.IndexOf(lastUser);
        if (index >= 0)
            userDropdown.value = index;
    }

    // Updates greeting in both main menu and user panel
    private void UpdateGreetings()
    {
        string current = UserManager.Instance.CurrentUser;
        string text = string.IsNullOrEmpty(current) ? "" : "Hi " + current + "!";

        if (greetingText != null)
            greetingText.text = text;

        if (userPanelGreetingText != null)
            userPanelGreetingText.text = text;
    }
}
