using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleSceneSwitcher : MonoBehaviour
{
    // Load ARMode scene
    public void LoadARMode()
    {
        SceneManager.LoadScene("ARMode", LoadSceneMode.Single);
    }

    // Load MainMenu scene
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
    }
}