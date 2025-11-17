using UnityEngine;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;

public class MicButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public static bool pointerDown = false;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void StartRecognition();

    [DllImport("__Internal")]
    private static extern void StopRecognition();
#endif

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("[MicButton] Pressed: Start recognition");
        StartRecognition();
#else
        Debug.Log("[MicButton] Pressed (editor): would start recognition");
#endif
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!pointerDown) return;
        pointerDown = false;
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("[MicButton] Released: Stop recognition");
        StopRecognition();
#else
        Debug.Log("[MicButton] Released (editor): would stop recognition");
#endif
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!pointerDown) return;
        pointerDown = false;
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("[MicButton] Pointer exited while pressed: Stop recognition");
        StopRecognition();
#else
        Debug.Log("[MicButton] Pointer exited (editor): would stop recognition");
#endif
    }

    private void OnDisable()
    {
        if (pointerDown)
        {
            pointerDown = false;
#if UNITY_WEBGL && !UNITY_EDITOR
            Debug.Log("[MicButton] OnDisable: Stop recognition");
            StopRecognition();
#else
            Debug.Log("[MicButton] OnDisable (editor): would stop recognition");
#endif
        }
    }
}
