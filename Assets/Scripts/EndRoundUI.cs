using UnityEngine;
using TMPro;

public class EndRoundUI : MonoBehaviour
{
    public GameObject scrollContent; // Content GameObject in Scroll View
    public TMP_Text wordEntryPrefab;

    public void DisplayWordLogs(System.Collections.Generic.List<SessionLogger.WordLog> logs)
    {
        foreach (Transform child in scrollContent.transform)
            Destroy(child.gameObject);

        foreach (var log in logs)
        {
            TMP_Text entry = Instantiate(wordEntryPrefab, scrollContent.transform);
            entry.text = $"{log.targetWord} | Spoken: {log.spokenWord} | Confidence: {log.confidence:P0} | Result: {log.result} | Time: {log.reactionTime:F2}s | Streak: {log.streak}";
        }
    }
}
