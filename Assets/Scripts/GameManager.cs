using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public TMP_Text correctFeedbackText;
    public TMP_Text missFeedbackText;
    public TMP_Text timerText;

    [Header("Panels")]
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject pauseMenuUI;

    [Header("Gameplay Settings")]
    public int lives = 3;
    public int score = 0;
    public float gameDuration = 120f;
    public GameObject explosionPrefab;

    private bool isGameOver = false;
    private bool isPaused = false;
    private float timer;

    // 🔹 Progress tracking
    private int correctWords = 0;
    private int totalWords = 0;
    private int streak = 0;
    private int bestStreak = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        isGameOver = false;
        isPaused = false;
        score = 0;
        timer = gameDuration;

        HidePanels();
        UpdateUI();
        UpdateTimerUI();
    }

    void Update()
    {
        if (isGameOver || isPaused) return;

        timer -= Time.deltaTime;
        if (timer < 0) timer = 0;
        UpdateTimerUI();

        if (timer <= 0) WinGame();

        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }

    // -------- Gameplay --------
    public void CheckSpokenWord(string spoken)
    {
        if (isGameOver) return;

        spoken = CleanWord(spoken);
        totalWords++;

        foreach (var wordObj in UnityEngine.Object.FindObjectsByType<WordObject>(FindObjectsSortMode.None))
        {
            string target = CleanWord(wordObj.wordText.text);

            if (spoken == target)
            {
                wordObj.Explode();
                IncreaseScore();
                correctWords++;
                streak++;
                if (streak > bestStreak) bestStreak = streak; // update best streak
                ShowCorrect();
                CheckProgress();

                // 🔹 Save progress after correct word
                ProgressManager.Instance.SaveWordStats(totalWords, correctWords, bestStreak);
                ProgressManager.Instance.SaveHighScore(score);

                return;
            }
        }

        MissWord();
    }

    private string CleanWord(string w)
    {
        return w.ToLower().Trim().TrimEnd('.', ',', '?', '!', ';', ':');
    }

    public void IncreaseScore()
    {
        if (isGameOver) return;
        score += 1;
        UpdateUI();

        // 🔹 Score-based achievements
        if (score == 1) AchievementManager.Instance.Unlock("First Word");
        if (score == 10) AchievementManager.Instance.Unlock("10 Words");
        if (score == 25) AchievementManager.Instance.Unlock("25 Words");
        if (score == 50) AchievementManager.Instance.Unlock("Word Master");
    }

    public void MissWord()
    {
        if (isGameOver) return;

        lives -= 1;
        streak = 0; // reset streak
        UpdateUI();
        ShowMiss();

        // 🔹 Save progress after missed word
        ProgressManager.Instance.SaveWordStats(totalWords, correctWords, bestStreak);
        ProgressManager.Instance.SaveHighScore(score);

        if (lives <= 0) GameOver();
    }

    // -------- Progress Tracking --------
    private void CheckProgress()
    {
        float accuracy = (totalWords > 0) ? (float)correctWords / totalWords : 0f;

        Debug.Log($"Accuracy: {accuracy:P0}, Streak: {streak}");

        // 🔹 Streak-based achievements
        if (streak >= 5) AchievementManager.Instance.Unlock("HotStreak");
        if (streak >= 10) AchievementManager.Instance.Unlock("Unstoppable");

        // 🔹 Accuracy-based achievements
        if (accuracy >= 0.75f) AchievementManager.Instance.Unlock("SilverPronouncer");
        if (accuracy >= 0.90f) AchievementManager.Instance.Unlock("GoldenPronouncer");

        // 🔹 Score-based achievements (already in IncreaseScore)
    }

    // -------- UI / Panels --------
    private void UpdateUI()
    {
        scoreText.text = $"Score: {score}";
        livesText.text = $"Lives: {lives}";
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void HidePanels()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (correctFeedbackText != null) correctFeedbackText.gameObject.SetActive(false);
        if (missFeedbackText != null) missFeedbackText.gameObject.SetActive(false);
    }

    // -------- Game State --------
    private void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        // 🔹 Save progress on game over
        ProgressManager.Instance.SaveWordStats(totalWords, correctWords, bestStreak);
        ProgressManager.Instance.SaveHighScore(score);

        AchievementManager.Instance.Unlock("Game Over");
    }

    private void WinGame()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        if (winPanel != null) winPanel.SetActive(true);

        // 🔹 Save progress on win
        ProgressManager.Instance.SaveWordStats(totalWords, correctWords, bestStreak);
        ProgressManager.Instance.SaveHighScore(score);

        AchievementManager.Instance.Unlock("Survivor");
    }

    // -------- Feedback --------
    private void ShowCorrect()
    {
        StopAllCoroutines();
        StartCoroutine(ShowFeedback(correctFeedbackText));
    }

    private void ShowMiss()
    {
        StopAllCoroutines();
        StartCoroutine(ShowFeedback(missFeedbackText));
    }

    private IEnumerator ShowFeedback(TMP_Text feedbackText)
    {
        feedbackText.gameObject.SetActive(true);
        feedbackText.alpha = 1f;
        yield return new WaitForSeconds(1.2f);
        feedbackText.gameObject.SetActive(false);
    }

    // -------- Pause --------
    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
    }

    // -------- Scene --------
    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
