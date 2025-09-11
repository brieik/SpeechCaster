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
    public float gameDuration = 120f; // 2 minutes to survive
    public GameObject explosionPrefab;

    private bool isGameOver = false;
    private bool isPaused = false;
    private float timer;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Initialize state
        isGameOver = false;
        isPaused = false;
        score = 0;
        timer = gameDuration;

        // Hide UI panels
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);

        correctFeedbackText.gameObject.SetActive(false);
        missFeedbackText.gameObject.SetActive(false);

        UpdateUI();
        UpdateTimerUI();
    }

    void Update()
    {
        if (isGameOver || isPaused) return;

        // Countdown timer
        timer -= Time.deltaTime;
        if (timer < 0) timer = 0;

        UpdateTimerUI();

        if (timer <= 0)
        {
            WinGame();
        }

        // Pause toggle
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // -------- Gameplay Logic --------
    public void IncreaseScore()
    {
        if (isGameOver) return;

        score += 1;
        UpdateUI();
    }

    public void MissWord()
    {
        if (isGameOver) return;

        lives -= 1;
        UpdateUI();
        ShowMiss();

        if (lives <= 0)
        {
            GameOver();
        }
    }

    public void CheckSpokenWord(string spoken)
    {
        if (isGameOver) return;

        spoken = spoken.ToLower().Trim().TrimEnd('.', ',', '?', '!', ';', ':');

        foreach (var wordObj in Object.FindObjectsByType<WordObject>(FindObjectsSortMode.None))
        {
            string target = wordObj.wordText.text.ToLower().Trim();

            if (spoken == target || spoken.Contains(target) || target.Contains(spoken))
            {
                wordObj.Explode();
                IncreaseScore();
                ShowCorrect();
                return;
            }
        }

        ShowMiss(); // If no match, show miss
    }

    private void UpdateUI()
    {
        scoreText.text = "Score: " + score;
        livesText.text = "Lives: " + lives;
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    private void WinGame()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        if (winPanel != null) winPanel.SetActive(true);
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

    // -------- Pause System --------
    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
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

    // -------- Scene Management --------
    public void ReturnToMenu()
    {
        Time.timeScale = 1f; // make sure time is unpaused
        SceneManager.LoadScene("MainMenu");
    }

    public void RetryGame()
    {
        Time.timeScale = 1f; // reset time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
