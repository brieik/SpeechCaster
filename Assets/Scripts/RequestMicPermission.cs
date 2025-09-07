using UnityEngine;
using System.Runtime.InteropServices;

public class MicPermissionRequester : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void RequestMicPermission();
#endif

    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RequestMicPermission();  // Triggers mic prompt once
#endif
    }
}
