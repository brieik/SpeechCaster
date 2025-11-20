using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SessionLogger
{
    public class WordLog
    {
        public string targetWord;
        public string spokenWord;
        public float confidence;
        public float reactionTime; // seconds since word spawned
        public string result; // Correct, Missed, Mispronounced
        public int streak;
    }

    private List<WordLog> logs = new List<WordLog>();
    private float roundStartTime;

    public void StartRound()
    {
        logs.Clear();
        roundStartTime = Time.time;
    }

    public void LogWord(string target, string spoken, float confidence, string result, int streak)
    {
        logs.Add(new WordLog
        {
            targetWord = target,
            spokenWord = spoken,
            confidence = confidence,
            reactionTime = Time.time - roundStartTime,
            result = result,
            streak = streak
        });
    }

    public List<WordLog> GetLogs() => logs;

    public void SaveCSV(string filename = "SessionLog.csv")
    {
        string path = Path.Combine(Application.persistentDataPath, filename);
        List<string> lines = new List<string>();
        lines.Add("TargetWord,SpokenWord,Confidence,ReactionTime,Result,Streak");

        foreach (var w in logs)
        {
            lines.Add($"{w.targetWord},{w.spokenWord},{w.confidence:F2},{w.reactionTime:F2},{w.result},{w.streak}");
        }

        File.WriteAllLines(path, lines.ToArray());
        Debug.Log("Session CSV saved at: " + path);
    }
}
