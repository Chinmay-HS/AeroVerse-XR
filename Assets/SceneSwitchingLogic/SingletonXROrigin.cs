using UnityEngine;

public class SingletonXROrigin : MonoBehaviour
{
    private static SingletonXROrigin instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.Log("[XROrigin] Found old instance. Replacing it with the new one.");

            // Cleanup old instance completely
            Destroy(instance.gameObject);  

            // Assign the new one
            instance = this;
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }
}