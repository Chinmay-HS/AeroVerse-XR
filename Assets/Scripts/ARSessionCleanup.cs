using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARSessionCleanup : MonoBehaviour
{
    private ARSession arSession;

    private void Awake()
    {
        arSession = FindObjectOfType<ARSession>();
    }

    private void OnDestroy()
    {
        if (arSession != null)
        {
            Debug.Log("Stopping AR session before destroying the scene...");
            arSession.Reset();  // Reset AR session
            arSession.enabled = false;  // Disable AR session
        }
    }
}