using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Management;
using Unity.XR.CoreUtils;
using System.Collections;
using System.Threading.Tasks;

public class SwitchARtoMain : MonoBehaviour
{
    #region Singleton Pattern
    public static SwitchARtoMain Instance { get; private set; }
    #endregion

    #region Inspector Variables
    [SerializeField] private string arScene = "ARMode";
    [SerializeField] private string homeScene = "MainMenuScene";
    [SerializeField] private float subsystemRestartDelay = 0.5f;
    #endregion

    #region Runtime References
    private XROrigin _sessionOrigin;
    private Camera _arCamera;
    #endregion

    #region Events
    public System.Action OnARSceneLoaded;
    public System.Action OnARSceneUnloaded;
    #endregion

    void Awake()
    {
        InitializeSingleton();
    }

    void InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadARScene()
    {
        if (!IsSceneValid(arScene)) return;
        StartCoroutine(LoadARSceneRoutine());
    }

    public void ReturnToHome()
    {
        if (!IsSceneValid(homeScene)) return;
        StartCoroutine(UnloadARSceneRoutine());
    }

    #region AR Scene Loading
    IEnumerator LoadARSceneRoutine()
    {
        yield return CleanupExistingAR();
        yield return LoadSceneAdditive(arScene);

        // Hide the home scene
        HideSceneRootObjects(homeScene);

        // Disable duplicates
        DisableExtraAudioListeners();
        DisableExtraEventSystems();

        yield return InitializeARSubsystems();
        FinalizeARSetup();

        OnARSceneLoaded?.Invoke();
    }
    
    void ShowSceneRootObjects(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.IsValid())
        {
            Debug.LogWarning($"Scene '{sceneName}' is not valid or not loaded.");
            return;
        }

        foreach (var rootObj in scene.GetRootGameObjects())
        {
            rootObj.SetActive(true);
        }
    }
    
    void HideSceneRootObjects(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.IsValid())
        {
            Debug.LogWarning($"Scene '{sceneName}' is not valid or not loaded.");
            return;
        }

        foreach (var rootObj in scene.GetRootGameObjects())
        {
            rootObj.SetActive(false);
        }
    }

    private IEnumerator CleanupExistingAR()
    {
        var arSession = FindObjectOfType<ARSession>();
        if (arSession != null && ARSession.state != ARSessionState.None)
        {
            Debug.Log("Cleaning up existing AR session...");
            arSession.Reset();

            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();

            yield return new WaitForSeconds(subsystemRestartDelay);
        }
    }

    private IEnumerator LoadSceneAdditive(string sceneName)
    {
        Debug.Log($"Loading scene: {sceneName}");
        var loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadOp.allowSceneActivation = true;
        yield return loadOp;
    }

    IEnumerator InitializeARSubsystems()
    {
        Debug.Log("Initializing AR subsystems...");

        XRGeneralSettings.Instance.Manager.InitializeLoader();
        yield return null;

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Failed to initialize XR Loader.");
            yield break;
        }

        Debug.Log("XR Loader initialized successfully!");

        XRGeneralSettings.Instance.Manager.StartSubsystems();

        Debug.Log("AR subsystems started successfully!");
    }

    private bool FinalizeARSetup()
    {
        Debug.Log("Finalizing AR setup...");
        var arSceneRef = SceneManager.GetSceneByName(arScene);

        foreach (var root in arSceneRef.GetRootGameObjects())
        {
            _sessionOrigin = root.GetComponentInChildren<XROrigin>();
            if (_sessionOrigin != null) break;
        }

        if (_sessionOrigin == null)
        {
            Debug.LogError("ARSessionOrigin not found in AR scene!");
            return false;
        }

        _arCamera = _sessionOrigin.GetComponentInChildren<Camera>();
        if (_arCamera == null)
        {
            Debug.LogError("AR Camera missing in AR scene!");
            return false;
        }

        Debug.Log("AR session origin and camera found!");
        return true;
    }
    #endregion

    #region AR Scene Unloading
    IEnumerator UnloadARSceneRoutine()
    {
        Debug.Log("Unloading AR scene and stopping subsystems...");

        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();

        // No need to unload AR scene manually
        // AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(arScene);
        // yield return unloadOp;

        // Directly load home scene
        AsyncOperation homeLoadOp = SceneManager.LoadSceneAsync(homeScene, LoadSceneMode.Single);
        yield return homeLoadOp;

        CleanMemory();

        OnARSceneUnloaded?.Invoke();

        Debug.Log("Home scene loaded successfully!");
    }


    private void CleanMemory()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        _sessionOrigin = null;
        _arCamera = null;
    }
    #endregion

    #region Utilities
    private bool IsSceneValid(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name not set in inspector!");
            return false;
        }

        if (!SceneExists(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' not found in build settings!");
            return false;
        }

        return true;
    }

    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var path = SceneUtility.GetScenePathByBuildIndex(i);
            if (System.IO.Path.GetFileNameWithoutExtension(path) == sceneName)
                return true;
        }
        return false;
    }

    private void CleanupSceneDuplicates()
    {
        DisableExtraAudioListeners();
        DisableExtraEventSystems();
    }

    private void DisableExtraAudioListeners()
    {
        var listeners = FindObjectsOfType<AudioListener>();
        for (int i = 1; i < listeners.Length; i++)
        {
            listeners[i].enabled = false;
        }
    }

    private void DisableExtraEventSystems()
    {
        var eventSystems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
        for (int i = 1; i < eventSystems.Length; i++)
        {
            eventSystems[i].gameObject.SetActive(false);
        }
    }
    #endregion
}
