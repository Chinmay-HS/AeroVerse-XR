using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;

public class TestARLoader : MonoBehaviour
{
    IEnumerator Start()
    {
        Debug.Log("Starting XR initialization...");

        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Failed to initialize XR Loader.");
            yield break;
        }

        Debug.Log("XR Loader initialized successfully. Starting subsystems...");
        XRGeneralSettings.Instance.Manager.StartSubsystems();

        Debug.Log("XR Subsystems started.");
    }

    private void OnDisable()
    {
        Debug.Log("Stopping XR subsystems...");
        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        Debug.Log("XR subsystems stopped and loader deinitialized.");
    }
}