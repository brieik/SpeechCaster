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
    public void SaveHighScore(int score)
    {
        if (string.IsNullOrEmpty(UserManager.Instance.CurrentUser)) return;

        string user = UserManager.Instance.CurrentUser;
        string key = $"{user}_HighScore_{GameSettings.selectedDifficulty}";

        int best = PlayerPrefs.GetInt(key, 0);
        if (score > best)
        {
            PlayerPrefs.SetInt(key, score);
            PlayerPrefs.Save();
        }
    }

    public int LoadHighScore()
    {
        if (string.IsNullOrEmpty(UserManager.Instance.CurrentUser)) return 0;

        string user = UserManager.Instance.CurrentUser;
        string key = $"{user}_HighScore_{GameSettings.selectedDifficulty}";

        return PlayerPrefs.GetInt(key, 0);
    }

    // ---------------- Word Stats ----------------
    public void SaveWordStats(int attempted, int correct, int bestStreak)
    {
        if (string.IsNullOrEmpty(UserManager.Instance.CurrentUser)) return;

        string user = UserManager.Instance.CurrentUser;
        string difficulty = $"_{GameSettings.selectedDifficulty}";

        PlayerPrefs.SetInt(user + "_WordsAttempted" + difficulty, attempted);
        PlayerPrefs.SetInt(user + "_WordsCorrect" + difficulty, correct);
        PlayerPrefs.SetInt(user + "_BestStreak" + difficulty, bestStreak);
        PlayerPrefs.Save();
    }

    public (int attempted, int correct, int bestStreak) LoadWordStats()
    {
        if (string.IsNullOrEmpty(UserManager.Instance.CurrentUser)) return (0, 0, 0);

        string user = UserManager.Instance.CurrentUser;
        string difficulty = $"_{GameSettings.selectedDifficulty}";

        int attempted = PlayerPrefs.GetInt(user + "_WordsAttempted" + difficulty, 0);
        int correct = PlayerPrefs.GetInt(user + "_WordsCorrect" + difficulty, 0);
        int bestStreak = PlayerPrefs.GetInt(user + "_BestStreak" + difficulty, 0);

        return (attempted, correct, bestStreak);
    }
}
