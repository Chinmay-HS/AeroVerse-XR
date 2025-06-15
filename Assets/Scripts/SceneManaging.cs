using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;

public class SceneManaging : MonoBehaviour
{
    [SerializeField] private Dictionary<string, string> buttonScenesMapping = new Dictionary<string, string>
    {
        { "Back_Button", "MainMenuScene" },
        { "ARMode", "ARMode" },
        { "AboutUS", "AboutUS" },
        { "TechStuff", "TechStuff" },
        { "MainMenuScene", "MainMenuScene" }
    };

    private Scene currentActiveScene;

    void Start()
    {
        Button[] btns = FindObjectsOfType<Button>();

        foreach (Button button in btns)
        {
            if (buttonScenesMapping.ContainsKey(button.name))
            {
                string sceneName = buttonScenesMapping[button.name];
                button.onClick.AddListener(() => ChangeScene(sceneName));
            }
        }

        
        
        currentActiveScene = SceneManager.GetActiveScene();
    }

    public void ChangeScene(string sceneName)
    {
        if (sceneName == "ARMode")
        {
            // Load fresh to ensure XR subsystems start cleanly
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            return;
        }

        // Original additive handling for menu scenes
        if (IsSceneLoaded(sceneName))
        {
            SetActiveScene(sceneName);
        }
        else
        {
            StartCoroutine(LoadAndActivateScene(sceneName));
        }
    }

    private bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName && scene.isLoaded)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator LoadAndActivateScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return null; // extra frame

        Scene sceneToActivate = SceneManager.GetSceneByName(sceneName);

        while (!sceneToActivate.IsValid() || !sceneToActivate.isLoaded)
        {
            yield return null;
            sceneToActivate = SceneManager.GetSceneByName(sceneName);
        }

        SetActiveScene(sceneName);
    }

    private void SetActiveScene(string sceneName)
    {
        Scene sceneToActivate = SceneManager.GetSceneByName(sceneName);
        if (!sceneToActivate.IsValid() || !sceneToActivate.isLoaded)
        {
            Debug.LogError($"Scene '{sceneName}' is not valid or not loaded. Cannot set active.");
            return;
        }

        DeactivateAllScenesExcept(sceneName);

        SceneManager.SetActiveScene(sceneToActivate);
        ActivateSceneElements(sceneToActivate);

        currentActiveScene = sceneToActivate;
        Debug.Log($"Scene '{sceneName}' is now active.");
    }

    private void DeactivateAllScenesExcept(string sceneToKeep)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != sceneToKeep)
            {
                DeactivateSceneElements(scene);
            }
        }
    }

    private void DeactivateSceneElements(Scene scene)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Camera cam in root.GetComponentsInChildren<Camera>(true))
            {
                if (cam != null) cam.enabled = false;
            }

            foreach (Canvas canvas in root.GetComponentsInChildren<Canvas>(true))
            {
                if (canvas != null) canvas.gameObject.SetActive(false);
            }

            foreach (EventSystem ev in root.GetComponentsInChildren<EventSystem>(true))
            {
                if (ev != null) ev.gameObject.SetActive(false);
            }
        }
    }

    private void ActivateSceneElements(Scene scene)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Camera cam in root.GetComponentsInChildren<Camera>(true))
            {
                if (cam != null)
                {
                    cam.enabled = true;
                    cam.tag = "MainCamera";
                }
            }

            foreach (Canvas canvas in root.GetComponentsInChildren<Canvas>(true))
            {
                if (canvas != null)
                {
                    canvas.gameObject.SetActive(true);
                    canvas.sortingOrder = 0;
                }
            }

            foreach (EventSystem ev in root.GetComponentsInChildren<EventSystem>(true))
            {
                if (ev != null)
                {
                    ev.gameObject.SetActive(true);
                }
            }
        }
    }

    private IEnumerator RestartARSession()
    {
        yield return new WaitForSeconds(0.2f);

        ARSession session = FindObjectOfType<ARSession>();
        XROrigin origin = FindObjectOfType<XROrigin>();

        if (session == null || origin == null)
        {
            Debug.LogWarning("XROrigin or ARSession not found in the ARMode scene.");
            yield break;
        }

        Debug.Log("Restarting AR Session...");

        session.Reset(); // soft reset
        yield return null;

        // Optionally, re-enable components if needed
        session.enabled = false;
        origin.gameObject.SetActive(false);

        yield return null;

        origin.gameObject.SetActive(true);
        session.enabled = true;
    }
}
