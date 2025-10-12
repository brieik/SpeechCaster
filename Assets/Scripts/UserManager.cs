using UnityEngine;
using System.Collections.Generic;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance;

    public string CurrentUser;
    public List<string> allUsers = new List<string>();
    public List<string> allAchievements = new List<string>
    {
        "FirstWord",
        "HotStreak",
        "Unstoppable",
        "SilverPronouncer",
        "GoldenPronouncer"
    };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadAllUsers();
    }

    // ------------------ USER MANAGEMENT ------------------
    public bool CreateUser(string username)
    {
        if (string.IsNullOrEmpty(username) || allUsers.Contains(username))
            return false;

        allUsers.Add(username);
        SetCurrentUser(username);
        SaveAllUsers();
        return true;
    }

    public bool SwitchUser(string username)
    {
        if (!allUsers.Contains(username)) return false;
        CurrentUser = username;
        return true;
    }

    public void DeleteUser(string username)
    {
        if (allUsers.Contains(username))
            allUsers.Remove(username);

        for (int diff = 0; diff <= 2; diff++)
        {
            string diffSuffix = "_" + diff;
            PlayerPrefs.DeleteKey(username + "_WordsAttempted" + diffSuffix);
            PlayerPrefs.DeleteKey(username + "_WordsCorrect" + diffSuffix);
            PlayerPrefs.DeleteKey(username + "_BestStreak" + diffSuffix);
            PlayerPrefs.DeleteKey(username + "_HighScore" + diffSuffix);

            string prefix = diff switch { 0 => "Easy", 1 => "Medium", 2 => "Hard", _ => "Unknown" };
            foreach (var ach in allAchievements)
                PlayerPrefs.DeleteKey(username + "_Achv_" + prefix + "_" + ach);
        }

        if (CurrentUser == username) CurrentUser = "";
        SaveAllUsers();
        PlayerPrefs.Save();
    }

    public List<string> GetAllUsers()
    {
        return new List<string>(allUsers);
    }

    // ------------------ SET CURRENT & INIT STATS ------------------
    private void SetCurrentUser(string username)
    {
        CurrentUser = username;
        if (!allUsers.Contains(username)) allUsers.Add(username);
        InitStats(username);
    }

    private void InitStats(string username)
    {
        for (int diff = 0; diff <= 2; diff++)
        {
            string diffSuffix = "_" + diff;

            if (!PlayerPrefs.HasKey(username + "_WordsAttempted" + diffSuffix))
                PlayerPrefs.SetInt(username + "_WordsAttempted" + diffSuffix, 0);
            if (!PlayerPrefs.HasKey(username + "_WordsCorrect" + diffSuffix))
                PlayerPrefs.SetInt(username + "_WordsCorrect" + diffSuffix, 0);
            if (!PlayerPrefs.HasKey(username + "_BestStreak" + diffSuffix))
                PlayerPrefs.SetInt(username + "_BestStreak" + diffSuffix, 0);
            if (!PlayerPrefs.HasKey(username + "_HighScore" + diffSuffix))
                PlayerPrefs.SetInt(username + "_HighScore" + diffSuffix, 0);

            string prefix = diff switch { 0 => "Easy", 1 => "Medium", 2 => "Hard", _ => "Unknown" };
            foreach (var ach in allAchievements)
            {
                if (!PlayerPrefs.HasKey(username + "_Achv_" + prefix + "_" + ach))
                    PlayerPrefs.SetInt(username + "_Achv_" + prefix + "_" + ach, 0);
            }
        }

        PlayerPrefs.Save();
    }

    // ------------------ SAVE / LOAD USER LIST ------------------
    private void SaveAllUsers()
    {
        PlayerPrefs.SetInt("TotalUsers", allUsers.Count);
        for (int i = 0; i < allUsers.Count; i++)
            PlayerPrefs.SetString("User_" + i, allUsers[i]);
        PlayerPrefs.Save();
    }

    private void LoadAllUsers()
    {
        allUsers.Clear();
        int count = PlayerPrefs.GetInt("TotalUsers", 0);
        for (int i = 0; i < count; i++)
        {
            string user = PlayerPrefs.GetString("User_" + i, "");
            if (!string.IsNullOrEmpty(user)) allUsers.Add(user);
        }
    }
}
