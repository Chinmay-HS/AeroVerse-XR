using System.Collections.Generic;
using Christina.UI;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnIndicator : MonoBehaviour
{
    [Header("Prefabs & UI")]
    [SerializeField] private GameObject placementIndicator;
    [SerializeField] private GameObject[] placedPrefabs;
    [SerializeField] private GameObject augmentToggle;
    [SerializeField] private TMP_Dropdown prefabDropdown;
   // [SerializeField] private GameObject leanTouchController; // Drag the LeanTouch GameObject here


    [Header("Input")]
    [SerializeField] private InputAction touchInput;

    private ARRaycastManager aRRaycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private ToggleSwitch augmentToggleSwitch;
    private GameObject spawnedObject;

    private int selectedDropdownIndex = 0;
    private int previousSelectedIndex = -1;

    private System.Action<InputAction.CallbackContext> placeObjectCallback;

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        placeObjectCallback = ctx => PlaceObject();
        touchInput.performed += placeObjectCallback;

        if (placementIndicator != null)
            placementIndicator.SetActive(true);

        if (augmentToggle != null)
            augmentToggleSwitch = augmentToggle.GetComponent<ToggleSwitch>();

        if (prefabDropdown != null)
            prefabDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        
        OnDropdownValueChanged(prefabDropdown.value);

    }

    private void Start()
    {
        ReassignReferencesAfterSceneLoad();
    }
    
    private void OnEnable()
    {
        touchInput.Enable();
    }

    private void OnDisable()
    {
        touchInput.Disable();
        touchInput.performed -= placeObjectCallback;

        if (prefabDropdown != null)
            prefabDropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
    }

    private void Update()
    {
        if (placementIndicator == null) return;

        if (aRRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            placementIndicator.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);

            if (!placementIndicator.activeInHierarchy)
                placementIndicator.SetActive(true);

            
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    
    private void SetLeanTouchInteractionEnabled(GameObject target, bool enabled)
    {
        if (target == null) return;

        // Get all MonoBehaviours that are LeanTouch scripts (you can be more specific)
        var leanTouchScripts = target.GetComponentsInChildren<MonoBehaviour>(true);

        foreach (var script in leanTouchScripts)
        {
            // Filter only Lean scripts (you can customize this list further)
            if (script.GetType().Name.StartsWith("Lean"))
            {
                script.enabled = enabled;
            }
        }

        Debug.Log($"LeanTouch interactions {(enabled ? "enabled" : "disabled")} on {target.name}");
    }


    public void OnDropdownValueChanged(int newIndex)
    {
        Debug.Log($"Dropdown changed to: {newIndex}");

        if (newIndex != previousSelectedIndex)
        {
            if (spawnedObject != null)
            {
                Debug.Log($"Destroying: {spawnedObject.name}");
                Destroy(spawnedObject);
                spawnedObject = null;
            }

            selectedDropdownIndex = newIndex;
            previousSelectedIndex = newIndex;

            GameObject selectedPrefab = GetSelectedPrefab();
            Debug.Log($"Selected prefab: {selectedPrefab?.name ?? "NULL"}");

            PlaceObject();
        }
    }

    public void PlaceObject()
{
    if (!placementIndicator.activeInHierarchy || placedPrefabs.Length == 0)
        return;

    GameObject prefabToUse = GetSelectedPrefab();

    if (prefabToUse == null)
        return;

    // First time spawn
    if (spawnedObject == null)
    {
        spawnedObject = Instantiate(prefabToUse, placementIndicator.transform.position, placementIndicator.transform.rotation, transform);
    }
    // Reposition if toggle is on
    else if (augmentToggleSwitch != null && augmentToggleSwitch.CurrentValue)
    {
        spawnedObject.transform.SetPositionAndRotation(placementIndicator.transform.position, placementIndicator.transform.rotation);
    }

    

}


    private GameObject GetSelectedPrefab()
    {
        if (selectedDropdownIndex >= 0 && selectedDropdownIndex < placedPrefabs.Length)
        {
            return placedPrefabs[selectedDropdownIndex];
        }

        Debug.LogWarning("[GetSelectedPrefab] Invalid index.");
        return null;
    }

    public void SetSelectedPrefabIndex(int index)
    {
        if (index < 0 || index >= placedPrefabs.Length)
        {
            Debug.LogWarning("Invalid prefab index!");
            return;
        }

        // Destroy existing object if present
        if (spawnedObject != null)
        {
            Debug.Log($"Destroying previous object: {spawnedObject.name}");
            Destroy(spawnedObject);
            spawnedObject = null;
        }

        // Update dropdown and selected index
        selectedDropdownIndex = index;
        previousSelectedIndex = index;
        prefabDropdown.value = index;

        Debug.Log($"Prefab index set to: {index} ({placedPrefabs[index].name})");

        // Only spawn if placement indicator is active
        if (placementIndicator != null && placementIndicator.activeInHierarchy)
        {
            GameObject prefabToUse = GetSelectedPrefab();
            if (prefabToUse != null)
            {
                spawnedObject = Instantiate(prefabToUse, placementIndicator.transform.position, placementIndicator.transform.rotation, transform);
                Debug.Log($"Spawned prefab: {prefabToUse.name}");
            }
            else
            {
                Debug.LogWarning("Selected prefab is null!");
            }
        }
        else
        {
            Debug.LogWarning("Placement indicator not ready — prefab not spawned.");
        }
    }


    public void SetAugmentToggleState(bool isOn)
    {
        Debug.Log($"[Toggle] State changed to: {isOn}");

        if (augmentToggleSwitch != null && augmentToggleSwitch.TryGetComponent(out Slider sliderComponent))
        {
            sliderComponent.value = isOn ? 1f : 0f;
        }

        if (spawnedObject != null)
        {
           // SetLeanTouchInteractionEnabled(spawnedObject, !isOn);

            if (isOn && placementIndicator != null && placementIndicator.activeInHierarchy)
            {
                spawnedObject.transform.SetPositionAndRotation(
                    placementIndicator.transform.position,
                    placementIndicator.transform.rotation
                );

                Debug.Log("[Toggle] Repositioned object.");
            }
        }
    }

    
    public void ReassignReferencesAfterSceneLoad()
    {
        Debug.Log("[Reassign] Attempting to relink broken references...");

        placementIndicator = GameObject.FindWithTag("PlacementIndicator");
        augmentToggle = GameObject.FindWithTag("AugmentToggle");
        GameObject dropdownObj = GameObject.FindWithTag("PrefabDropdown");
        if (dropdownObj != null)
            prefabDropdown = dropdownObj.GetComponent<TMP_Dropdown>();

        // leanTouchController = GameObject.FindWithTag("LeanTouchController");


        // Rehook any behaviors tied to these
        if (augmentToggle != null)
        {
        }

        if (prefabDropdown != null)
        {   
            prefabDropdown.onValueChanged.RemoveAllListeners(); 
            prefabDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            OnDropdownValueChanged(prefabDropdown.value); // Optional: refresh based on current dropdown state
        }

        if (placementIndicator != null)
        {
            placementIndicator.SetActive(true);
        }

        Debug.Log("[Reassign] References rehooked successfully.");
    }
    

}
