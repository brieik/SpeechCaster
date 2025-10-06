using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public TMP_Text scoreText;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject pauseMenuUI;

    [Header("Lives UI")]
    public Image[] heartIcons; 
    public Sprite fullHeart;   
    public Sprite emptyHeart;  

    [Header("Border Feedback UI")]
    public Image greenBorder;  // ✅ Assign green overlay image
    public Image redBorder;    // ✅ Assign red overlay image
    public float flashDuration = 0.3f; // ✅ Flash timing
    public float maxAlpha = 0.35f;     // ✅ Max glow strength

    [Header("Gameplay Settings")]
    public int lives = 3;
    public int score = 0;
    public int maxWords = 25;
    public GameObject explosionPrefab;

    public static bool isGameOver = false;
    private bool isPaused = false;

    private int correctWords = 0;
    public static int totalWords = 0;
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
        totalWords = 0;

        HidePanels();
        UpdateUI();
    }

    void Update()
    {
        if (isGameOver || isPaused) return;
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }

    // -------- Gameplay --------
    public void OnWordSpawned()
    {
        totalWords++;
        Debug.Log($"Word {totalWords}/{maxWords} spawned.");

        if (totalWords >= maxWords)
            WinGame();
    }

    public void CheckSpokenWord(string spoken)
    {
        if (isGameOver) return;

        spoken = CleanWord(spoken);

        foreach (var wordObj in UnityEngine.Object.FindObjectsByType<WordObject>(FindObjectsSortMode.None))
        {
            string target = CleanWord(wordObj.wordText.text);

            if (spoken == target)
            {
                wordObj.Explode();
                IncreaseScore();
                correctWords++;
                streak++;
                if (streak > bestStreak) bestStreak = streak;
                StartCoroutine(FlashBorder(greenBorder)); // ✅ Correct feedback
                CheckProgress();

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
        score++;
        UpdateUI();

        if (score == 1) AchievementManager.Instance.Unlock("First Word");
        if (score == 10) AchievementManager.Instance.Unlock("10 Words");
        if (score == 25) AchievementManager.Instance.Unlock("25 Words");
        if (score == 50) AchievementManager.Instance.Unlock("Word Master");
    }

    public void MissWord()
    {
        if (isGameOver) return;

        lives--;
        streak = 0;
        UpdateUI();
        StartCoroutine(FlashBorder(redBorder)); // ✅ Miss feedback

        ProgressManager.Instance.SaveWordStats(totalWords, correctWords, bestStreak);
        ProgressManager.Instance.SaveHighScore(score);

        if (lives <= 0)
            GameOver();
    }

    private void CheckProgress()
    {
        float accuracy = (totalWords > 0) ? (float)correctWords / totalWords : 0f;
        Debug.Log($"Accuracy: {accuracy:P0}, Streak: {streak}");

        if (streak >= 5) AchievementManager.Instance.Unlock("HotStreak");
        if (streak >= 10) AchievementManager.Instance.Unlock("Unstoppable");
        if (accuracy >= 0.75f) AchievementManager.Instance.Unlock("SilverPronouncer");
        if (accuracy >= 0.90f) AchievementManager.Instance.Unlock("GoldenPronouncer");
    }

    // -------- UI --------
    private void UpdateUI()
    {
        scoreText.text = $"Score: {score}";
        UpdateHearts();
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            if (i < lives)
                heartIcons[i].sprite = fullHeart;
            else
                heartIcons[i].sprite = emptyHeart;
        }
    }

    private void HidePanels()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);

        if (greenBorder != null) greenBorder.gameObject.SetActive(false);
        if (redBorder != null) redBorder.gameObject.SetActive(false);
    }

    // -------- Flash Feedback --------
    private IEnumerator FlashBorder(Image border)
    {
        if (border == null) yield break;

        border.gameObject.SetActive(true);
        Color c = border.color;

        // Fade In
        for (float t = 0; t < flashDuration; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(0, maxAlpha, t / flashDuration);
            border.color = c;
            yield return null;
        }

        // Hold briefly
        yield return new WaitForSeconds(0.1f);

        // Fade Out
        for (float t = 0; t < flashDuration; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(maxAlpha, 0, t / flashDuration);
            border.color = c;
            yield return null;
        }

        border.gameObject.SetActive(false);
    }

    // -------- Game State --------
    private void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        ProgressManager.Instance.SaveWordStats(totalWords, correctWords, bestStreak);
        ProgressManager.Instance.SaveHighScore(score);

        AchievementManager.Instance.Unlock("Game Over");
    }

    private void WinGame()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;
        if (winPanel != null) winPanel.SetActive(true);

        ProgressManager.Instance.SaveWordStats(totalWords, correctWords, bestStreak);
        ProgressManager.Instance.SaveHighScore(score);

        AchievementManager.Instance.Unlock("Survivor");
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
