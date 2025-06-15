using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;

public class SceneSwitcherTest : MonoBehaviour
{
    [SerializeField] private string mainMenuScene = "MainMenuScene";
    [SerializeField] private string arModeScene = "ARMode";
    
    
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == mainMenuScene)
        {
            // Find XR Camera and disable it in Main Menu
            var xrCamera = GameObject.FindWithTag("MainCamera"); // Or tag it separately
            if (xrCamera != null)
            {
                var audioListener = xrCamera.GetComponent<AudioListener>();
                if (audioListener != null)
                    audioListener.enabled = false;
            }
        }
    }


    // Public method to load ARMode scene
    public void LoadARMode()
    {
        StartCoroutine(LoadARModeRoutine());
    }

    // Public method to load Main Menu scene
    public void LoadMainMenu()
    {
        StartCoroutine(LoadMainMenuRoutine());
    }

    void RemoveExtraAudioListeners()
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        if (listeners.Length > 1)
        {
            Debug.LogWarning("Multiple AudioListeners found. Disabling extras.");
            for (int i = 1; i < listeners.Length; i++)
            {
                listeners[i].enabled = false;
            }
        }
    }


    private IEnumerator LoadARModeRoutine()
    {
        Debug.Log("Initializing XR loader...");
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Failed to initialize XR Loader. ARMode not supported.");
            yield break;
        }

        Debug.Log("Starting XR subsystems...");
        XRGeneralSettings.Instance.Manager.StartSubsystems();

        Debug.Log("Loading AR Mode scene...");
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(arModeScene, LoadSceneMode.Single);
        yield return loadOp;

        Debug.Log("AR Mode scene loaded.");

        // Remove extra audio listeners
        RemoveExtraAudioListeners();
    }

    private IEnumerator LoadMainMenuRoutine()
    {
        // Destroy all spawned prefabs tagged as "SpawnedObject"
        GameObject[] spawnedObjects = GameObject.FindGameObjectsWithTag("SpawnedPrefab");
        foreach (var obj in spawnedObjects)
        {
            Destroy(obj);
        }

        Debug.Log("Stopping XR subsystems...");
        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();

        Debug.Log("Loading Main Menu scene...");
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(mainMenuScene, LoadSceneMode.Single);
        yield return loadOp;

        Debug.Log("Main Menu scene loaded.");

        RemoveExtraAudioListeners();
    }
}
