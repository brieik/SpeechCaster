using UnityEngine;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;

public class MicButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void StartRecognition();

    [DllImport("__Internal")]
    private static extern void StopRecognition();
#endif

    public void OnPointerDown(PointerEventData eventData)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("[MicButton] Pressed: Start recognition");
        StartRecognition();
#endif
    }

    public void OnPointerUp(PointerEventData eventData)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("[MicButton] Released: Stop recognition");
        StopRecognition();
#endif
    }
}
