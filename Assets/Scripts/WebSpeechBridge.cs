using UnityEngine;
using System.Runtime.InteropServices;

public static class WebSpeechBridge
{
    // JS function to set difficulty
    [DllImport("__Internal")]
    private static extern void SetWordDifficulty(string difficulty);

    /// <summary>
    /// Call this before starting recognition
    /// </summary>
    public static void SendDifficulty(int level)
    {
        string diff = "easy";
        switch (level)
        {
            case 0: diff = "easy"; break;
            case 1: diff = "medium"; break;
            case 2: diff = "hard"; break;
        }

#if !UNITY_EDITOR && UNITY_WEBGL
        SetWordDifficulty(diff);
#else
        Debug.Log("[WebSpeechBridge] Would send difficulty: " + diff);
#endif
    }
}
