using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class AnnotationData
{
    public Transform anchorTransform;
    [TextArea]
    public string annotationText;
}

public class AnnotationController : MonoBehaviour
{
    [Header("Annotation Settings")]
    public List<AnnotationData> annotations;  // Set this from inspector
    public GameObject annotationPrefab;       // Prefab with AnnotationBehaviour attached

    private List<GameObject> spawnedAnnotations = new List<GameObject>();
    private bool annotationsVisible = false;

    public void ToggleAnnotations()
    {
        if (annotationsVisible)
        {
            // Hide all annotations
            foreach (var ann in spawnedAnnotations)
                Destroy(ann);
            spawnedAnnotations.Clear();
        }
        else
        {
            // Show annotations at their anchors
            foreach (var data in annotations)
            {
                GameObject ann = Instantiate(annotationPrefab, data.anchorTransform.position, Quaternion.identity);
                ann.transform.SetParent(data.anchorTransform, worldPositionStays: true);

                // Pass the text to the prefab
                var behaviour = ann.GetComponent<AnnotationBehaviour>();
                if (behaviour != null)
                {
                    behaviour.SetAnnotation(data.annotationText);
                }

                spawnedAnnotations.Add(ann);
            }
        }

        annotationsVisible = !annotationsVisible;
    }
}

