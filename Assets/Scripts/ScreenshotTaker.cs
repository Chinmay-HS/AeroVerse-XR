using UnityEngine;
using System.Collections;
using System.IO;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class ScreenshotTaker : MonoBehaviour
{
    public string albumName = "JWST_Snapshots";

    public void TakeScreenshot()
    {
        StartCoroutine(CaptureScreenshot());
    }

    private IEnumerator CaptureScreenshot()
    {
        // Request permissions first
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            yield return new WaitForSeconds(0.5f);
            
            // If permission is still not granted, exit
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                Debug.LogError("Storage permission not granted!");
                yield break;
            }
        }
#endif

        // Wait for rendering to complete
        yield return new WaitForEndOfFrame();

        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filename = "JWST_" + timestamp + ".png";
        string path = "";

#if UNITY_ANDROID
        // Use a more reliable method for Android
        string directoryPath = Path.Combine(Application.persistentDataPath, albumName);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        path = Path.Combine(directoryPath, filename);
#elif UNITY_IOS
        string directoryPath = Application.persistentDataPath;
        path = Path.Combine(directoryPath, filename);
#else
        string directoryPath = Path.Combine(Application.dataPath, albumName);
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
        path = Path.Combine(directoryPath, filename);
#endif

        // Take screenshot and get the texture
        Texture2D screenImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();
        
        // Convert to bytes and save
        byte[] imageBytes = screenImage.EncodeToPNG();
        File.WriteAllBytes(path, imageBytes);
        
        // Clean up
        Destroy(screenImage);

        Debug.Log($"Screenshot saved to: {path}");

#if UNITY_ANDROID
        // Wait briefly to ensure screenshot is saved
        yield return new WaitForSeconds(0.5f);

        // Refresh gallery - make sure the scan actually works
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext"))
            {
                string[] filesToScan = new string[] { path };
                AndroidJavaClass mediaScanner = new AndroidJavaClass("android.media.MediaScannerConnection");
                mediaScanner.CallStatic("scanFile", context, filesToScan, null, null);
                Debug.Log("Media scan initiated for: " + path);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error refreshing gallery: " + e.Message);
        }
#endif

        yield return null;
    }
}