using UnityEngine;

public class SpeechReceiver : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;

#if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void StartRecognition();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void StopRecognition();
#endif

    public void OnSpeechResult(string spokenWord)
    {
        if (string.IsNullOrEmpty(spokenWord)) return;
        Debug.Log($"[SpeechReceiver] Heard: {spokenWord}");

        if (gameManager != null)
            gameManager.CheckSpokenWord(spokenWord, 1.0f);
        else
            Debug.LogWarning("[SpeechReceiver] GameManager not assigned.");
    }

    public void OnSpeechResultWithConfidence(string data)
    {
        if (string.IsNullOrEmpty(data)) return;

        string[] parts = data.Split('|');
        if (parts.Length < 2)
        {
            Debug.LogWarning("[SpeechReceiver] Invalid result format: " + data);
            return;
        }

        string spokenWord = parts[0].Trim();
        if (!float.TryParse(parts[1], out float confidence))
            confidence = 1.0f;

        Debug.Log($"[SpeechReceiver] Heard: {spokenWord} (confidence: {confidence:F2})");

        if (gameManager != null)
            gameManager.CheckSpokenWord(spokenWord, confidence);
        else
            Debug.LogWarning("[SpeechReceiver] GameManager not assigned.");
    }

    public void OnSpeechTryAgain()
    {
        Debug.Log("[SpeechReceiver] Speech aborted or no speech detected. Showing feedback...");

        if (gameManager != null)
            gameManager.ShowFeedback("Try again!", Color.yellow);
    }

    // 🟢 Updated push-to-talk retry
    public void RetryRecognition()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // Only retry if user is currently pressing the mic button
        if (MicButtonIsPressed())
        {
            Debug.Log("[SpeechReceiver] Retrying recognition (push-to-talk active)...");
            StartRecognition();
        }
        else
        {
            Debug.Log("[SpeechReceiver] RetryRecognition skipped: push-to-talk not active.");
        }
#endif
    }

    // 🟢 Helper to check if user is pressing mic (we track pointerDown in MicButton)
    private bool MicButtonIsPressed()
    {
       return MicButton.pointerDown;
    }

    public void StartMic()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("[SpeechReceiver] Starting mic...");
        StartRecognition();
#endif
    }

    public void StopMic()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("[SpeechReceiver] Stopping mic...");
        StopRecognition();
#endif
    }

    public void StartMicForCurrentLevel(int difficulty)
    {
        Debug.Log($"[SpeechReceiver] Starting push-to-talk for difficulty {difficulty}...");
#if UNITY_WEBGL && !UNITY_EDITOR
        WebSpeechBridge.SendDifficulty(difficulty); // Load words for current difficulty
        StartRecognition();
#endif
    }
}
