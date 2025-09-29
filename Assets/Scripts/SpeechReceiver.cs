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

    /// <summary>
    /// Called from WebSpeech.jslib when a spoken result is ready.
    /// </summary>
    public void OnSpeechResult(string spokenWord)
    {
        if (string.IsNullOrEmpty(spokenWord)) return;

        Debug.Log($"[SpeechReceiver] Heard: {spokenWord}");

        if (gameManager != null)
        {
            gameManager.CheckSpokenWord(spokenWord);
        }
        else
        {
            Debug.LogWarning("[SpeechReceiver] GameManager not assigned.");
        }
    }

    public void RetryRecognition()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("[SpeechReceiver] Retrying recognition...");
        StartRecognition();
#endif
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
}
