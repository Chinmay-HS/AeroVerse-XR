// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
//
// public class AnnotationManager : MonoBehaviour
// {
//     [System.Serializable]
//     public class Annotation
//     {
//         public string partName;
//         public Transform attachPoint;
//         public string annotationText;
//         public GameObject annotationPrefab;
//     }
//
//     public List<Annotation> annotations;
//     private Dictionary<string, GameObject> activeAnnotations = new Dictionary<string, GameObject>();
//
//     public Button toggleAnnotationsButton;
//     public TextMeshProUGUI statusText; // Drag this from Canvas
//     private bool annotationsVisible = false;
//     private Camera arCamera;
//
//     void Start()
//     {
//         toggleAnnotationsButton.onClick.AddListener(ToggleAnnotations);
//         arCamera = Camera.main;
//
//         if (arCamera == null)
//         {
//             Debug.LogError("AR Camera not found! Ensure it's tagged 'MainCamera'.");
//         }
//
//         foreach (var annotation in annotations)
//         {
//             if (annotation.attachPoint == null || annotation.annotationPrefab == null)
//             {
//                 Debug.LogError($"Annotation setup error for: {annotation.partName}");
//                 continue;
//             }
//
//             GameObject newAnnotation = Instantiate(
//                 annotation.annotationPrefab,
//                 annotation.attachPoint.position + Vector3.up * 0.05f,
//                 Quaternion.identity
//             );
//
//             // Set annotation text
//             var textComponent = newAnnotation.GetComponentInChildren<TextMeshProUGUI>();
//             if (textComponent != null)
//                 textComponent.text = annotation.annotationText;
//
//             // Assign AR camera to canvas
//             Canvas canvas = newAnnotation.GetComponentInChildren<Canvas>();
//             if (canvas != null)
//                 canvas.worldCamera = arCamera;
//
//             newAnnotation.transform.SetParent(annotation.attachPoint);
//             newAnnotation.transform.localScale = Vector3.one * 0.01f;
//             newAnnotation.SetActive(false);
//
//             activeAnnotations[annotation.partName] = newAnnotation;
//
//             Debug.Log($"Annotation created for: {annotation.partName} at {annotation.attachPoint.position}");
//         }
//     }
//
//     void ToggleAnnotations()
//     {
//         annotationsVisible = !annotationsVisible;
//         Debug.Log($"Toggling annotations: {(annotationsVisible ? "ON" : "OFF")}");
//
//         // Update button text
//         var buttonText = toggleAnnotationsButton.GetComponentInChildren<TextMeshProUGUI>();
//         if (buttonText != null)
//         {
//             buttonText.text = annotationsVisible ? "Hide Annotations" : "Show Annotations";
//         }
//
//         // Change button color
//         ColorBlock colors = toggleAnnotationsButton.colors;
//         colors.normalColor = annotationsVisible ? Color.green : Color.white;
//         toggleAnnotationsButton.colors = colors;
//
//         // Set visibility of annotations
//         foreach (var pair in activeAnnotations)
//         {
//             if (pair.Value != null)
//             {
//                 pair.Value.SetActive(annotationsVisible);
//                 Debug.Log($"Annotation {pair.Key} visibility set to {annotationsVisible}");
//             }
//         }
//
//         // Show floating status message
//         StartCoroutine(ShowStatus(annotationsVisible ? "Annotations Enabled" : "Annotations Hidden"));
//     }
//
//     void Update()
//     {
//         if (!annotationsVisible || arCamera == null) return;
//
//         foreach (var annotation in activeAnnotations.Values)
//         {
//             if (annotation != null)
//             {
//                 annotation.transform.LookAt(arCamera.transform);
//                 annotation.transform.rotation = Quaternion.LookRotation(arCamera.transform.forward);
//             }
//         }
//     }
//
//     IEnumerator ShowStatus(string message, float duration = 2f)
//     {
//         if (statusText == null) yield break;
//
//         statusText.text = message;
//         statusText.gameObject.SetActive(true);
//         yield return new WaitForSeconds(duration);
//         statusText.gameObject.SetActive(false);
//     }
//
//     void OnDrawGizmos()
//     {
//         if (annotations == null) return;
//
//         Gizmos.color = Color.cyan;
//         foreach (var annotation in annotations)
//         {
//             if (annotation.attachPoint != null)
//             {
//                 Gizmos.DrawSphere(annotation.attachPoint.position + Vector3.up * 0.05f, 0.01f);
//             }
//         }
//     }
// }

// using UnityEngine;
// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
//
// public class AnnotationController : MonoBehaviour
// {
//     public GameObject[] annotationAnchors; // Assign in inspector
//     public GameObject annotationPrefab;
//
//     private List<GameObject> spawnedAnnotations = new List<GameObject>();
//     private bool annotationsVisible = false;
//
//     public void ToggleAnnotations()
//     {
//         if (annotationsVisible)
//         {
//             foreach (var ann in spawnedAnnotations)
//                 Destroy(ann);
//             spawnedAnnotations.Clear();
//         }
//         else
//         {
//             foreach (var anchor in annotationAnchors)
//             {
//                 GameObject ann = Instantiate(annotationPrefab, anchor.transform.position, Quaternion.identity);
//                 ann.transform.SetParent(anchor.transform); // Optional, keeps it stuck to the part
//                 ann.transform.LookAt(Camera.main.transform); // Face toward camera
//                 spawnedAnnotations.Add(ann);
//             }
//         }
//
//         annotationsVisible = !annotationsVisible;
//     }
// }
