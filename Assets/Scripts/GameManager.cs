using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public TMP_Text gameOverText;
    public TMP_Text correctFeedbackText; // NEW
    public TMP_Text missFeedbackText;    // NEW
    public GameObject startButton;

    [Header("Gameplay")]
    public int lives = 3;
    public int score = 0;
    public GameObject explosionPrefab;

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 0f;
        gameOverText.gameObject.SetActive(false);
        correctFeedbackText.gameObject.SetActive(false); // NEW
        missFeedbackText.gameObject.SetActive(false);    // NEW
        startButton.SetActive(true);
        UpdateUI();
    }

    public void StartGame()
    {
        isGameOver = false;
        score = 0;
        lives = 3;
        gameOverText.gameObject.SetActive(false);
        startButton.SetActive(false);

        // Clear words
        foreach (var word in Object.FindObjectsByType<WordObject>(FindObjectsSortMode.None))
        {
            Destroy(word.gameObject);
        }

        UpdateUI();
        Time.timeScale = 1f;
    }

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
        ShowMiss(); // NEW

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
                ShowCorrect(); // NEW
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

    private void GameOver()
    {
        isGameOver = true;
        gameOverText.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    private IEnumerator ShowStartButtonAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        startButton.SetActive(true);
    }

    // ---------- NEW Feedback Methods ----------
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
}
