using UnityEngine;
using System.Collections.Generic;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;
    private HashSet<string> unlocked = new HashSet<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        LoadAchievements();
    }

    // ---------------- Unlock achievement for current difficulty ----------------
    public void Unlock(string achievementID)
    {
        if (string.IsNullOrEmpty(UserManager.Instance.CurrentUser)) return;

        string difficultyPrefix = GetDifficultyPrefix();
        string key = UserManager.Instance.CurrentUser + "_Achv_" + difficultyPrefix + "_" + achievementID;

        if (!PlayerPrefs.HasKey(key) || PlayerPrefs.GetInt(key) == 0)
        {
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save();
            unlocked.Add(achievementID);
            Debug.Log($"Unlocked achievement ({difficultyPrefix}): {achievementID}");
        }
    }

    // ---------------- Check if unlocked for current difficulty ----------------
    public bool IsUnlocked(string achievementID)
    {
        if (string.IsNullOrEmpty(UserManager.Instance.CurrentUser)) return false;

        string difficultyPrefix = GetDifficultyPrefix();
        string key = UserManager.Instance.CurrentUser + "_Achv_" + difficultyPrefix + "_" + achievementID;
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    // ---------------- Difficulty prefix ----------------
    private string GetDifficultyPrefix()
    {
        switch (GameSettings.selectedDifficulty)
        {
            case 0: return "Easy";
            case 1: return "Medium";
            case 2: return "Hard";
            default: return "Unknown";
        }
    }

    // ---------------- Load unlocked achievements for current difficulty ----------------
    private void LoadAchievements()
    {
        unlocked.Clear();
        string user = UserManager.Instance.CurrentUser;
        if (string.IsNullOrEmpty(user)) return;

        foreach (var ach in UserManager.Instance.allAchievements)
        {
            if (IsUnlocked(ach)) unlocked.Add(ach);
        }
    }
}
