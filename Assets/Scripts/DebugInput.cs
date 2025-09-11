using UnityEngine;
using TMPro;

public class DebugInput : MonoBehaviour
{
    public TMP_InputField inputField;

    void Start()
    {
        // Make sure field is empty at start
        inputField.text = "";
        
        // Listen for submit (when player presses Enter)
        inputField.onSubmit.AddListener(OnSubmit);
    }

    void OnSubmit(string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            GameManager.Instance.CheckSpokenWord(text);
            inputField.text = ""; // clear after submit
            inputField.ActivateInputField(); // focus again
        }
    }
}
