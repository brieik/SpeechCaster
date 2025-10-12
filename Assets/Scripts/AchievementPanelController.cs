using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
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
    public GameObject achievementPanel;
    public Transform contentParent;
    public GameObject achievementEntryPrefab;
    public TMP_Dropdown difficultyDropdown;
    public TMP_Text difficultyLabel;

    [Header("Player Stats Texts")]
    public TMP_Text bestStreakText;
    public TMP_Text highScoreText;

    [Header("Achievement Info")]
    public AchievementData[] achievementInfos;

    // Singleton for easy access
    public static AchievementPanelController Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (difficultyDropdown != null)
        {
            difficultyDropdown.ClearOptions();
            difficultyDropdown.AddOptions(new List<string> { "Easy", "Medium", "Hard" });
            difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);

            // Set dropdown value first
            difficultyDropdown.value = GameSettings.selectedDifficulty;

            // Populate stats & achievements to match dropdown
            PopulateStats(difficultyDropdown.value);
            PopulateAchievements(difficultyDropdown.value);
        }
    }

    public void TogglePanel()
    {
        bool isActive = !achievementPanel.activeSelf;
        achievementPanel.SetActive(isActive);

        if (isActive)
        {
            achievementPanel.transform.SetAsLastSibling();
            PopulateStats(difficultyDropdown.value);
            PopulateAchievements(difficultyDropdown.value);
        }
    }

    void OnDifficultyChanged(int value)
    {
        GameSettings.selectedDifficulty = value;
        PopulateStats(value);
        PopulateAchievements(value);
    }

    void PopulateStats(int difficulty)
    {
        if (string.IsNullOrEmpty(UserManager.Instance.CurrentUser)) return;

        var stats = ProgressManager.Instance.LoadWordStats(difficulty);
        int highScore = ProgressManager.Instance.LoadHighScore(difficulty);

        if (bestStreakText != null)
            bestStreakText.text = $"Best Streak: {stats.bestStreak}";

        if (highScoreText != null)
            highScoreText.text = $"High Score: {highScore}";
    }

    void PopulateAchievements(int difficulty)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var achID in UserManager.Instance.allAchievements)
        {
            GameObject entry = Instantiate(achievementEntryPrefab, contentParent);
            entry.transform.localScale = Vector3.one;

            TMP_Text nameText = entry.transform.Find("NameText")?.GetComponent<TMP_Text>();
            if (nameText != null) nameText.text = achID;

            TMP_Text statusText = entry.transform.Find("StatusText")?.GetComponent<TMP_Text>();
            if (statusText != null)
            {
                bool unlocked = AchievementManager.Instance.IsUnlocked(achID);
                statusText.text = unlocked ? "Unlocked" : "Locked";

                if (unlocked && !statusText.gameObject.GetComponent<AchievementFlashFlag>())
                {
                    FlashAchievement(entry);
                    statusText.gameObject.AddComponent<AchievementFlashFlag>();
                }
            }

            TMP_Text descText = entry.transform.Find("DescriptionText")?.GetComponent<TMP_Text>();
            if (descText != null)
            {
                AchievementData info = GetAchievementInfo(achID);
                descText.text = info != null ? info.description : "";
            }

            Image iconImg = entry.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImg != null)
            {
                AchievementData info = GetAchievementInfo(achID);
                iconImg.sprite = (info != null && info.icon != null) ? info.icon : null;
                iconImg.gameObject.SetActive(info != null && info.icon != null);
            }
        }

        UpdateDifficultyLabel(difficulty);
    }

    AchievementData GetAchievementInfo(string id)
    {
        if (achievementInfos == null) return null;
        foreach (var a in achievementInfos)
            if (a.id == id) return a;
        return null;
    }

    private void UpdateDifficultyLabel(int difficulty)
    {
        if (difficultyLabel != null)
        {
            string prefix = difficulty switch
            {
                0 => "Easy",
                1 => "Medium",
                2 => "Hard",
                _ => "Unknown"
            };
            difficultyLabel.text = $"Difficulty: {prefix}";
        }
    }

    // ================== FLASH + BOUNCE EFFECT ==================
    public void FlashAchievement(GameObject entry)
    {
        StartCoroutine(FlashBounceCoroutine(entry));
    }

    private IEnumerator FlashBounceCoroutine(GameObject entry)
    {
        if (entry == null) yield break;

        Image bg = entry.GetComponent<Image>();
        if (bg == null) yield break;

        Vector3 originalScale = entry.transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;
        Color originalColor = bg.color;
        Color flashColor = Color.yellow;

        float duration = 0.3f;

        float t = 0f;
        while (t < duration)
        {
            if (entry == null || bg == null) yield break;
            entry.transform.localScale = Vector3.Lerp(originalScale, targetScale, t / duration);
            bg.color = Color.Lerp(originalColor, flashColor, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;
        while (t < duration)
        {
            if (entry == null || bg == null) yield break;
            entry.transform.localScale = Vector3.Lerp(targetScale, originalScale, t / duration);
            bg.color = Color.Lerp(flashColor, originalColor, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        if (entry != null && bg != null)
        {
            entry.transform.localScale = originalScale;
            bg.color = originalColor;
        }
    }
}

// Helper component to prevent repeated flashing
public class AchievementFlashFlag : MonoBehaviour { }
