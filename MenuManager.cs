using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Called by UI Buttons (mouse selection)
    public void StartNewGame()
    {
        // Replace "NewGameScene" with the actual scene name
        SceneManager.LoadScene("HeroSelectScene");
    }

    public void ContinueGame()
    {
        // Continue game functionality (to be implemented)
        Debug.Log("Continue Game selected. (Functionality to be implemented.)");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game selected. Exiting application.");
        Application.Quit();
    }
}
