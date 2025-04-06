using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("URL to open")]
    public string url = "https://github.com/Chinmay-HS/AeroVerse-XR.git"; // Replace with your desired link

    /// <summary>
    /// Opens the given URL in the default browser
    /// </summary>
    public void OpenLink()
    {
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
        else
        {
            Debug.LogWarning("URL is empty or null.");
        }
    }

    /// <summary>
    /// Quits the app or stops play mode in editor
    /// </summary>
    public void QuitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}