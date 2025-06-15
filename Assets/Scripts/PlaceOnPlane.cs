using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlane : MonoBehaviour
{
    [SerializeField] private GameObject placedPrefab;
    [SerializeField] private GameObject placementIndicatorPrefab;

    private GameObject spawnedObject;
    private GameObject placementIndicator;
    private ARRaycastManager aRRaycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Vector3 lastValidPosition;
    private Quaternion lastValidRotation;

    void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        placementIndicator = Instantiate(placementIndicatorPrefab, Vector3.zero, Quaternion.identity);
        placementIndicator.SetActive(true); // Always show the indicator
        lastValidPosition = placementIndicator.transform.position;
        lastValidRotation = placementIndicator.transform.rotation;
    }

    void Update()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        if (aRRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            // Update position and rotation of indicator
            placementIndicator.transform.position = hitPose.position;
            Vector3 lookPos = Camera.main.transform.position - hitPose.position;
            lookPos.y = 0;
            placementIndicator.transform.rotation = Quaternion.LookRotation(lookPos);
            // Store last valid position/rotation
            lastValidPosition = hitPose.position;
            lastValidRotation = Quaternion.LookRotation(lookPos);
        }
        else
        {
            // No plane detected: keep indicator at last valid position
            placementIndicator.transform.position = lastValidPosition;
            placementIndicator.transform.rotation = lastValidRotation;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceOrMoveObject(lastValidPosition, lastValidRotation);
        }
    }

    void PlaceOrMoveObject(Vector3 position, Quaternion rotation)
    {
        if (spawnedObject == null)
        {
            spawnedObject = Instantiate(placedPrefab, position, rotation);
        }
        else
        {
            spawnedObject.transform.SetPositionAndRotation(position, rotation);
        }
    }
}
