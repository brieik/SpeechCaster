using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
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
    public TMP_Text feedbackText;

    [Header("Lives UI")]
    public Image[] heartIcons;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Border Feedback UI")]
    public Image greenBorder;
    public Image yellowBorder;
    public Image redBorder;
    public float flashDuration = 0.3f;
    public float maxAlpha = 0.35f;

    [Header("Gameplay Settings")]
    public int lives = 3;
    public int score = 0;
    public int maxWords = 25;
    public GameObject explosionPrefab;

    private bool isPaused = false;
    public static bool isGameOver = false;

    private int correctWords = 0;
    private int wordsResolved = 0; // Exploded or fallen
    private int wordsSpawned = 0;  // Count how many words were spawned
    public int WordsSpawned => wordsSpawned;
    private int streak = 0;
    private int bestStreak = 0;

    private Coroutine feedbackRoutine;
    private List<float> wordConfidences = new List<float>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ResetGame();
    }

    void Update()
    {
        if (isGameOver || isPaused) return;
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }

    private void ResetGame()
    {
        isGameOver = false;
        isPaused = false;
        score = 0;
        correctWords = 0;
        wordsResolved = 0;
        wordsSpawned = 0;
        streak = 0;
        bestStreak = 0;
        wordConfidences.Clear();

        HidePanels();
        UpdateUI();
    }

    // Called from WordSpawner when a new word is spawned
    public void OnWordSpawned()
    {
        wordsSpawned++;
    }

    // Called when a word is exploded correctly
    public void OnWordExploded(float confidence)
    {
        wordsResolved++;
        correctWords++;
        streak++;
        bestStreak = Mathf.Max(bestStreak, streak);
        wordConfidences.Add(confidence);

        // Achievements
        if (correctWords == 1) AchievementManager.Instance.Unlock("FirstWord");
        if (streak >= 10) AchievementManager.Instance.Unlock("HotStreak");
        if (streak >= 20) AchievementManager.Instance.Unlock("Unstoppable");

        int addedScore = Mathf.Max(1, Mathf.RoundToInt(confidence * 10f));
        AddScore(addedScore);

         // Feedback
    if (confidence >= 0.9f)
    {
        StartCoroutine(FlashBorder(greenBorder));
        ShowFeedback("Excellent!", Color.green);
    }
    else if (confidence >= 0.84f)
    {
        StartCoroutine(FlashBorder(yellowBorder));
        ShowFeedback("Good! Slightly unclear.", new Color(1f, 0.85f, 0f));
    }
    else
    {
        StartCoroutine(FlashBorder(redBorder));
        ShowFeedback("Almost! Try again.", Color.red);
    }

    CheckEndOfRound();
    }

    // Called when a word falls past the bottom
    public void WordFallen()
    {
        if (isGameOver) return;

        lives--;
        wordsResolved++;
        streak = 0;

        StartCoroutine(FlashBorder(redBorder));
        ShowFeedback("Word missed!", Color.red);
        UpdateUI();

        if (lives <= 0)
        {
            lives = 0;
            EndRound();
            return;
        }

        CheckEndOfRound();
    }

    // Called when player speaks a word
    public void CheckSpokenWord(string spoken, float confidence = 1f)
    {
        if (isGameOver) return;
        spoken = CleanWord(spoken);

        foreach (var wordObj in UnityEngine.Object.FindObjectsByType<WordObject>(FindObjectsSortMode.None))
        {
            string target = CleanWord(wordObj.wordText.text);
            if (spoken == target)
            {
                wordObj.Explode();
            
                return;
            }
        }

        // Mispronounced word → just reset streak and show feedback
        streak = 0;
        StartCoroutine(FlashBorder(redBorder));
        ShowFeedback("Missed! Keep trying!", Color.red);
    }

    private string CleanWord(string w) => w.ToLower().Trim().TrimEnd('.', ',', '?', '!', ';', ':');

    private void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    private void CheckEndOfRound()
    {
        // End round only after all spawned words are resolved
        if (wordsSpawned >= maxWords && wordsResolved >= maxWords)
            EndRound();
    }

    private void EndRound()
{
    if (isGameOver) return;

    isGameOver = true;
    Time.timeScale = 0f;

    int difficulty = GameSettings.selectedDifficulty;

    // Save stats
    SaveStats(difficulty);

    float accuracy = (wordsResolved > 0) ? (float)correctWords / wordsResolved : 0f;
    float avgClarity = CalculateAverageClarity();

    if (accuracy >= 0.75f) AchievementManager.Instance.Unlock("SilverPronouncer");
    if (accuracy >= 0.90f) AchievementManager.Instance.Unlock("GoldenPronouncer");

    string feedbackMessage = GetFeedbackMessage(accuracy, avgClarity);

    // New win condition: if player still has lives left
    bool isWin = lives > 0;

    if (isWin)
    {
        winPanel?.SetActive(true);
        TMP_Text panelText = winPanel.GetComponentInChildren<TMP_Text>();
        if (panelText != null)
        {
            int savedHighScore = ProgressManager.Instance.LoadHighScore(difficulty);

            panelText.text = $"🏆 You Win!\n\n" +
                            $"Final Score: {score} (High Score: {savedHighScore})\n" +
                            $"Best Streak: {bestStreak}\n" +
                            $"Accuracy: {accuracy * 100f:F1}%\n" +
                            $"Average Clarity: {avgClarity * 100f:F1}%\n\n" +
                            $"{feedbackMessage}";
        }
    }
    else
    {
        gameOverPanel?.SetActive(true);
        TMP_Text panelText = gameOverPanel.GetComponentInChildren<TMP_Text>();
        if (panelText != null)
        {
            panelText.text = $"💀 Game Over\n\n" +
                            $"Final Score: {score}\n" +
                            $"Accuracy: {accuracy * 100f:F1}%\n" +
                            $"Average Clarity: {avgClarity * 100f:F1}%\n\n" +
                            $"{feedbackMessage}";
        }
    }
}


    private float CalculateAverageClarity()
    {
        if (wordConfidences.Count == 0) return 0f;
        float sum = 0f;
        foreach (float c in wordConfidences) sum += c;
        return sum / wordConfidences.Count;
    }

    private string GetFeedbackMessage(float accuracy, float avgClarity)
    {
        if (accuracy >= 0.75f && avgClarity >= 0.75f)
            return "Good job! Your pronunciation is clear. Keep focusing on tricky word endings and vowel sounds to improve even more.";
        else if (accuracy >= 0.50f)
            return "Not bad! Work on clarity and pronunciation of certain words to improve your score.";
        else
            return "Your pronunciation needs practice. Try speaking more slowly and distinctly. Don’t give up!";
    }

    private void UpdateUI()
    {
        scoreText.text = $"Score: {score}";

        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].sprite = (i < lives) ? fullHeart : emptyHeart;
        }
    }

    private void HidePanels()
    {
        gameOverPanel?.SetActive(false);
        winPanel?.SetActive(false);
        pauseMenuUI?.SetActive(false);
        greenBorder?.gameObject.SetActive(false);
        yellowBorder?.gameObject.SetActive(false);
        redBorder?.gameObject.SetActive(false);
    }

    private IEnumerator FlashBorder(Image border)
    {
        if (border == null) yield break;
        border.gameObject.SetActive(true);
        Color c = border.color;

        for (float t = 0; t < flashDuration; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(0, maxAlpha, t / flashDuration);
            border.color = c;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        for (float t = 0; t < flashDuration; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(maxAlpha, 0, t / flashDuration);
            border.color = c;
            yield return null;
        }

        border.gameObject.SetActive(false);
    }

    public void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null) return;

        feedbackText.text = message;
        feedbackText.color = color;
        feedbackText.gameObject.SetActive(true);

        if (feedbackRoutine != null)
            StopCoroutine(feedbackRoutine);

        feedbackRoutine = StartCoroutine(HideFeedback());
    }

    private IEnumerator HideFeedback()
    {
        CanvasGroup cg = feedbackText.GetComponent<CanvasGroup>();
        if (cg == null) cg = feedbackText.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 1f;
        yield return new WaitForSeconds(1f);
        for (float t = 0; t < 0.3f; t += Time.deltaTime)
        {
            cg.alpha = Mathf.Lerp(1f, 0f, t / 0.3f);
            yield return null;
        }
        feedbackText.gameObject.SetActive(false);
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuUI?.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuUI?.SetActive(false);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        wordConfidences.Clear();
        ResetGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ------------------ SAVE STATS ------------------
    private void SaveStats(int difficulty)
    {
        if (string.IsNullOrEmpty(UserManager.Instance.CurrentUser)) return;

        string username = UserManager.Instance.CurrentUser;
        string diffSuffix = "_" + difficulty;

        int savedBestStreak = PlayerPrefs.GetInt(username + "_BestStreak" + diffSuffix, 0);
        if (bestStreak > savedBestStreak)
            PlayerPrefs.SetInt(username + "_BestStreak" + diffSuffix, bestStreak);

        int savedHighScore = PlayerPrefs.GetInt(username + "_HighScore" + diffSuffix, 0);
        if (score > savedHighScore)
            PlayerPrefs.SetInt(username + "_HighScore" + diffSuffix, score);

        int wordsAttempted = PlayerPrefs.GetInt(username + "_WordsAttempted" + diffSuffix, 0);
        int wordsCorrect = PlayerPrefs.GetInt(username + "_WordsCorrect" + diffSuffix, 0);

        PlayerPrefs.SetInt(username + "_WordsAttempted" + diffSuffix, wordsAttempted + wordsResolved);
        PlayerPrefs.SetInt(username + "_WordsCorrect" + diffSuffix, wordsCorrect + correctWords);

        PlayerPrefs.Save();
    }
}
