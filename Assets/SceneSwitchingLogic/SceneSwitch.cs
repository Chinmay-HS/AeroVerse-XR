using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class SceneSwitch : MonoBehaviour
{
    [SerializeField] private string mainMenuScene = "MainMenuScene";
    [SerializeField] private string arModeScene = "ARMode";
    private Dictionary<string, UnityEngine.Events.UnityAction> buttonSceneMapping;

    private static string previousScene;

    void Start()
    {
        buttonSceneMapping = new Dictionary<string, UnityEngine.Events.UnityAction>
        {
            { "Back_Button", LoadPreviousScene },
            { "ARMode", LoadARMode },
            { "AboutUS", LoadAboutScene },
            { "TechStuff", LoadTechStuff }
        };

        BindButtonsInScene(SceneManager.GetActiveScene());

        if (SceneManager.GetActiveScene().name != arModeScene)
        {
            previousScene = SceneManager.GetActiveScene().name;
        }

        if (SceneManager.GetActiveScene().name == mainMenuScene)
        {
            var xrCamera = GameObject.FindWithTag("MainCamera");
            if (xrCamera != null)
            {
                var audioListener = xrCamera.GetComponent<AudioListener>();
                if (audioListener != null)
                    audioListener.enabled = false;
            }
        }

        SceneManager.sceneLoaded += OnSceneLoaded;

        LoadAllScenes();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Additive)
        {
            Debug.Log($"Scene loaded additively: {scene.name}");
            BindButtonsInScene(scene);
        }
    }

    public void LoadAboutScene()
    {
        SceneManager.LoadScene("AboutUS", LoadSceneMode.Additive);
    }

    public void LoadTechStuff()
    {
        SceneManager.LoadScene("TechStuff", LoadSceneMode.Additive);
    }

    private void BindButtonsInScene(Scene scene)
    {
        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            Button[] buttons = rootObject.GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                if (buttonSceneMapping.ContainsKey(btn.name))
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(buttonSceneMapping[btn.name]);
                }
            }
        }
    }

    public void LoadARMode()
    {
        StartCoroutine(LoadARModeRoutine());
    }

    public void LoadPreviousScene()
    {
        StartCoroutine(LoadPreviousSceneRoutine());
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
        Debug.Log("Checking for existing XR Origin...");

        GameObject existingXROrigin = null;
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.CompareTag("XROrigin") && obj.hideFlags == HideFlags.None)
            {
                existingXROrigin = obj;
                break;
            }
        }

        if (existingXROrigin != null)
        {
            existingXROrigin.SetActive(true);
            yield return null;

            var arSession = existingXROrigin.GetComponentInChildren<ARSession>(true);
            if (arSession != null && !arSession.enabled) arSession.enabled = true;

            var cameraManager = existingXROrigin.GetComponentInChildren<ARCameraManager>(true);
            if (cameraManager != null && !cameraManager.enabled) cameraManager.enabled = true;

            var cam = existingXROrigin.GetComponentInChildren<Camera>(true);
            if (cam != null && !cam.enabled) cam.enabled = true;
        }
        else
        {
            Debug.LogWarning("XR Origin not found. Proceeding anyway.");
        }

        Debug.Log("Initializing XR loader...");
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Failed to initialize XR Loader. ARMode not supported.");
            yield break;
        }

        Debug.Log("Starting XR subsystems...");
        XRGeneralSettings.Instance.Manager.StartSubsystems();

        // ✅ Check if ARMode scene is already loaded
        bool isLoaded = false;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == arModeScene)
            {
                isLoaded = true;
                break;
            }
        }

        if (!isLoaded)
        {
            Debug.Log("Loading AR Mode scene additively...");
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(arModeScene, LoadSceneMode.Additive);
            yield return loadOp;
            Debug.Log("AR Mode scene loaded.");
        }
        else
        {
            Debug.Log("AR Mode scene already loaded.");
        }

        RemoveExtraAudioListeners();
    }

    private IEnumerator LoadPreviousSceneRoutine()
    {
        string mainMenuScene = "MainMenuScene";

        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();

        yield return new WaitForSeconds(0.2f);

        GameObject xrOrigin = GameObject.FindWithTag("XROrigin");
        if (xrOrigin != null)
        {
            xrOrigin.SetActive(false);
        }

        ARSession arSession = FindObjectOfType<ARSession>();
        if (arSession != null)
        {
            arSession.Reset();
        }

        RemoveExtraAudioListeners();

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(mainMenuScene, LoadSceneMode.Single);
        yield return loadOp;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void LoadAllScenes()
    {
        SceneManager.LoadScene("ARMode", LoadSceneMode.Additive);
        SceneManager.LoadScene("AboutUS", LoadSceneMode.Additive);
        SceneManager.LoadScene("TechStuff", LoadSceneMode.Additive);
    }
}
