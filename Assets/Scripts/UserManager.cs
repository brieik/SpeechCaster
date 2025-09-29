using UnityEngine;
using System.Collections.Generic;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance;

    public string CurrentUser { get; private set; } = "";
    private string usersKey = "UsersList"; // list of all users

    // All achievement IDs
    public string[] allAchievements = {
        "HotStreak", "Unstoppable", "SilverPronouncer", "GoldenPronouncer",
        "First Word", "10 Words", "25 Words", "Word Master",
        "Survivor", "Game Over"
    };

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public List<string> GetAllUsers()
    {
        string saved = PlayerPrefs.GetString(usersKey, "");
        if (string.IsNullOrEmpty(saved)) return new List<string>();
        return new List<string>(saved.Split(','));
    }

    public bool CreateUser(string username)
    {
        username = username.Trim();
        if (string.IsNullOrEmpty(username)) return false;

        List<string> users = GetAllUsers();
        if (users.Contains(username)) return false;

        users.Add(username);
        PlayerPrefs.SetString(usersKey, string.Join(",", users));
        PlayerPrefs.Save();

        CurrentUser = username;
        InitStats(username);
        return true;
    }

    public bool SwitchUser(string username)
    {
        List<string> users = GetAllUsers();
        if (!users.Contains(username)) return false;
        CurrentUser = username;
        return true;
    }

    public bool DeleteUser(string username)
    {
        if (string.IsNullOrEmpty(username)) return false;

        List<string> users = GetAllUsers();
        if (!users.Contains(username)) return false;

        users.Remove(username);
        PlayerPrefs.SetString(usersKey, string.Join(",", users));

        // Delete stats
        PlayerPrefs.DeleteKey(username + "_WordsAttempted");
        PlayerPrefs.DeleteKey(username + "_WordsCorrect");
        PlayerPrefs.DeleteKey(username + "_BestStreak");
        PlayerPrefs.DeleteKey(username + "_HighScore");

        // Delete achievements
        foreach (var ach in allAchievements)
            PlayerPrefs.DeleteKey(username + "_Achv_" + ach);

        // Remove last user if it was this one
        if (PlayerPrefs.GetString("LastUser", "") == username)
            PlayerPrefs.DeleteKey("LastUser");

        PlayerPrefs.Save();

        if (CurrentUser == username)
            CurrentUser = "";

        return true;
    }

    private void InitStats(string username)
    {
        PlayerPrefs.SetInt(username + "_WordsAttempted", 0);
        PlayerPrefs.SetInt(username + "_WordsCorrect", 0);
        PlayerPrefs.SetInt(username + "_BestStreak", 0);
        PlayerPrefs.SetInt(username + "_HighScore", 0);

        foreach (var ach in allAchievements)
            PlayerPrefs.SetInt(username + "_Achv_" + ach, 0);

        PlayerPrefs.Save();
    }
}
