// using UnityEngine;
// using TMPro;
// using UnityEngine.UI;
//
// public class AnnotationBehaviour : MonoBehaviour
// {
//     [Header("Text to display")]
//     [TextArea]
//     public string annotationText;  // optional inspector default
//
//     [Header("UI Components")]
//     public Text uiText;                // for legacy UI
//     public TextMeshProUGUI tmpText;    // for TextMeshPro UI
//
//     public void SetAnnotation(string newText)
//     {
//         annotationText = newText;
//
//         // Update text on screen
//         if (uiText != null)
//             uiText.text = annotationText;
//
//         if (tmpText != null)
//             tmpText.text = annotationText;
//     }
//
//     void Update()
//     {
//         // Make the annotation face the camera
//         if (Camera.main != null)
//         {
//             Vector3 lookDirection = transform.position - Camera.main.transform.position;
//             transform.rotation = Quaternion.LookRotation(lookDirection);
//         }
//     }
// }

using UnityEngine;
using TMPro;

public class AnnotationBehaviour : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI tmpText; // Assign this in Inspector

    // Called by AnnotationController after spawning
    public void SetAnnotation(string newText)
    {
        if (tmpText != null)
            tmpText.text = newText;
    }

    void Update()
    {
        // Make sure the annotation always faces the camera
        if (Camera.main != null)
        {
            Vector3 lookDirection = transform.position - Camera.main.transform.position;
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}

