using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultySelector : MonoBehaviour
{
    public void SelectEasy()   { SelectDifficulty(0); }
    public void SelectMedium() { SelectDifficulty(1); }
    public void SelectHard()   { SelectDifficulty(2); }

    private void SelectDifficulty(int difficulty)
    {
        GameSettings.selectedDifficulty = difficulty;
        SceneManager.LoadScene("MainGame"); // Change to your main game scene name
    }

    // 👇 Add this function
    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Change to your actual main menu scene name
    }
}
