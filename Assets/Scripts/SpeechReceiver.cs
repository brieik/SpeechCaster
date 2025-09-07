using UnityEngine;

public class SpeechReceiver : MonoBehaviour
{
    public GameManager gameManager;

#if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void StartRecognition();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void StopRecognition();
#endif

    public void OnSpeechResult(string spokenWord)
    {
        Debug.Log("[Receiver] Heard: " + spokenWord);

        if (gameManager != null)
        {
            gameManager.CheckSpokenWord(spokenWord.ToLowerInvariant());
        }
        else
        {
            Debug.LogWarning("[Receiver] GameManager not assigned.");
        }
    }

    // Optional: Called from JS when auto-retry is enabled
    public void RetryRecognition()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("[Receiver] Retrying recognition...");
        StartRecognition();
#endif
    }

    public void StartMic()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("[Receiver] StartRecognition called");
        StartRecognition();
#endif
    }

    public void StopMic()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("[Receiver] StopRecognition called");
        StopRecognition();
#endif
    }
}
