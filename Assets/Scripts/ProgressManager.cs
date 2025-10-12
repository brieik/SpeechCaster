using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // ---------------- High Score ----------------
    /// <summary>
    /// Save high score for a specific difficulty (or current difficulty by default)
    /// </summary>
    public void SaveHighScore(int score, int difficulty = -1)
    {
        if (string.IsNullOrEmpty(UserManager.Instance.CurrentUser)) return;

        if (difficulty == -1) difficulty = GameSettings.selectedDifficulty;

        string user = UserManager.Instance.CurrentUser;
        string key = $"{user}_HighScore_{difficulty}";

        int best = PlayerPrefs.GetInt(key, 0);
        if (score > best)
        {
            PlayerPrefs.SetInt(key, score);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Load high score for a specific difficulty (or current difficulty by default)
    /// </summary>
    public int LoadHighScore(int difficulty = -1)
    {
        if (string.IsNullOrEmpty(UserManager.Instance.CurrentUser)) return 0;

        if (difficulty == -1) difficulty = GameSettings.selectedDifficulty;

        string user = UserManager.Instance.CurrentUser;
        string key = $"{user}_HighScore_{difficulty}";

        return PlayerPrefs.GetInt(key, 0);
    }

    // ---------------- Word Stats ----------------
    /// <summary>
    /// Save words attempted, correct, and best streak for a specific difficulty
    /// </summary>
    public void SaveWordStats(int attempted, int correct, int bestStreak, int difficulty = -1)
    {
        if (string.IsNullOrEmpty(UserManager.Instance.CurrentUser)) return;

        if (difficulty == -1) difficulty = GameSettings.selectedDifficulty;

        string user = UserManager.Instance.CurrentUser;
        string diffSuffix = "_" + difficulty;

        PlayerPrefs.SetInt(user + "_WordsAttempted" + diffSuffix, attempted);
        PlayerPrefs.SetInt(user + "_WordsCorrect" + diffSuffix, correct);
        PlayerPrefs.SetInt(user + "_BestStreak" + diffSuffix, bestStreak);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Load words attempted, correct, and best streak for a specific difficulty
    /// </summary>
    public (int attempted, int correct, int bestStreak) LoadWordStats(int difficulty = -1)
    {
        if (string.IsNullOrEmpty(UserManager.Instance.CurrentUser)) return (0, 0, 0);

        if (difficulty == -1) difficulty = GameSettings.selectedDifficulty;

        string user = UserManager.Instance.CurrentUser;
        string diffSuffix = "_" + difficulty;

        int attempted = PlayerPrefs.GetInt(user + "_WordsAttempted" + diffSuffix, 0);
        int correct = PlayerPrefs.GetInt(user + "_WordsCorrect" + diffSuffix, 0);
        int bestStreak = PlayerPrefs.GetInt(user + "_BestStreak" + diffSuffix, 0);

        return (attempted, correct, bestStreak);
    }

    // ---------------- Reset Stats ----------------
    /// <summary>
    /// Reset all word stats and high score for a specific difficulty
    /// </summary>
    public void ResetStats(int difficulty = -1)
    {
        if (string.IsNullOrEmpty(UserManager.Instance.CurrentUser)) return;

        if (difficulty == -1) difficulty = GameSettings.selectedDifficulty;

        string user = UserManager.Instance.CurrentUser;
        string diffSuffix = "_" + difficulty;

        PlayerPrefs.DeleteKey(user + "_WordsAttempted" + diffSuffix);
        PlayerPrefs.DeleteKey(user + "_WordsCorrect" + diffSuffix);
        PlayerPrefs.DeleteKey(user + "_BestStreak" + diffSuffix);
        PlayerPrefs.DeleteKey(user + "_HighScore" + diffSuffix);
        PlayerPrefs.Save();
    }
}
