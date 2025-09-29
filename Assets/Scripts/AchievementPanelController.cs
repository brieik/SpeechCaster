using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class AchievementData
{
    public string id;
    public string description;
    public Sprite icon; // optional
}

public class AchievementPanelController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject achievementPanel;       // Panel to show/hide
    public Transform contentParent;           // ScrollView Content
    public GameObject achievementEntryPrefab; // Prefab for each achievement
    public TMP_Dropdown difficultyDropdown;   // Dropdown for Easy/Medium/Hard
    public TMP_Text difficultyLabel;          // Label showing current difficulty

    [Header("Player Stats Texts")]
    public TMP_Text bestStreakText;
    public TMP_Text highScoreText;

    [Header("Achievement Info (optional)")]
    public AchievementData[] achievementInfos; // Optional descriptions/icons

    void Awake()
    {
        // Persist root GameObject
        DontDestroyOnLoad(transform.root.gameObject);
    }

    void Start()
    {
        if (difficultyDropdown != null)
        {
            difficultyDropdown.ClearOptions();
            difficultyDropdown.AddOptions(new List<string> { "Easy", "Medium", "Hard" });
            difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);

            // Set default
            difficultyDropdown.value = GameSettings.selectedDifficulty;
        }
    }

    public void TogglePanel()
    {
        bool isActive = !achievementPanel.activeSelf;
        achievementPanel.SetActive(isActive);

        if (isActive)
        {
            achievementPanel.transform.SetAsLastSibling();
            PopulateStats();
            PopulateAchievements();
        }
    }

    void OnDifficultyChanged(int value)
    {
        GameSettings.selectedDifficulty = value; // update global difficulty
        PopulateStats();                         // refresh stats for new difficulty
        PopulateAchievements();                  // refresh achievements for new difficulty
    }

    void PopulateStats()
    {
        if (string.IsNullOrEmpty(UserManager.Instance.CurrentUser)) return;

        // Load stats for current difficulty
        var stats = ProgressManager.Instance.LoadWordStats();
        int highScore = ProgressManager.Instance.LoadHighScore();

        if (bestStreakText != null)
            bestStreakText.text = $"Best Streak: {stats.bestStreak}";

        if (highScoreText != null)
            highScoreText.text = $"High Score: {highScore}";
    }

    void PopulateAchievements()
    {
        // Clear old entries
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        string difficultyPrefix = GetDifficultyPrefix();

        foreach (var achID in UserManager.Instance.allAchievements)
        {
            GameObject entry = Instantiate(achievementEntryPrefab, contentParent);
            entry.transform.localScale = Vector3.one;

            // Name
            TMP_Text nameText = entry.transform.Find("NameText")?.GetComponent<TMP_Text>();
            if (nameText != null) nameText.text = achID;

            // Status
            TMP_Text statusText = entry.transform.Find("StatusText")?.GetComponent<TMP_Text>();
            if (statusText != null)
            {
                bool unlocked = AchievementManager.Instance.IsUnlocked($"{difficultyPrefix}_{achID}");
                statusText.text = unlocked ? "Unlocked" : "Locked";
            }

            // Description
            TMP_Text descText = entry.transform.Find("DescriptionText")?.GetComponent<TMP_Text>();
            if (descText != null)
            {
                AchievementData info = GetAchievementInfo(achID);
                descText.text = info != null ? info.description : "";
            }

            // Icon
            Image iconImg = entry.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImg != null)
            {
                AchievementData info = GetAchievementInfo(achID);
                iconImg.sprite = (info != null && info.icon != null) ? info.icon : null;
                iconImg.gameObject.SetActive(info != null && info.icon != null);
            }
        }

        UpdateDifficultyLabel(difficultyPrefix);
    }

    AchievementData GetAchievementInfo(string id)
    {
        if (achievementInfos == null) return null;
        foreach (var a in achievementInfos)
            if (a.id == id) return a;
        return null;
    }

    private string GetDifficultyPrefix()
    {
        int val = difficultyDropdown != null ? difficultyDropdown.value : GameSettings.selectedDifficulty;
        switch (val)
        {
            case 0: return "Easy";
            case 1: return "Medium";
            case 2: return "Hard";
            default: return "Unknown";
        }
    }

    private void UpdateDifficultyLabel(string prefix)
    {
        if (difficultyLabel != null)
            difficultyLabel.text = $"Difficulty: {prefix}";
    }
}
