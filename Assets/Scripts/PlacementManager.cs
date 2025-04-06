using UnityEngine;
using Vuforia;

public class PlacementManager : MonoBehaviour
{
    [Header("Placement Settings")]
    public GameObject objectToPlace;              // The prefab to place in AR
    public Transform groundPlaneStage;            // Reference to the ground plane stage
    private GameObject placedObject;              // Reference to the placed instance
    
    [Header("Interaction Settings")]
    public bool allowMultipleObjects = false;     // Whether to allow placing multiple objects
    private bool isPlaced = false;                // Tracks if an object has been placed
    
    // Interaction states
    private enum InteractionState
    {
        Placing,     // Looking for a place to put the object
        Placed,      // Object is placed but not yet interacting
        Moving,      // User is moving the object
        Scaling,     // User is scaling the object
        Rotating     // User is rotating the object
    }
    private InteractionState currentState = InteractionState.Placing;
    
    // Touch handling variables
    private Vector2 touchStartPosition;
    private float initialDistance = 0;
    private Vector3 initialScale;
    private float initialRotation;

    void Update()
    {
        // Handle different interaction states
        switch (currentState)
        {
            case InteractionState.Placing:
                HandlePlacement();
                break;
                
            case InteractionState.Placed:
                HandleObjectInteraction();
                break;
                
            case InteractionState.Moving:
                HandleMovement();
                break;
                
            case InteractionState.Scaling:
                HandleScaling();
                break;
                
            case InteractionState.Rotating:
                HandleRotation();
                break;
        }
    }
    
    private void HandlePlacement()
    {
        // Skip if we already placed an object and don't want multiples
        if (isPlaced && !allowMultipleObjects)
            return;
            
        // Check for screen tap
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Only proceed if we have a valid ground plane
            if (groundPlaneStage != null && groundPlaneStage.gameObject.activeInHierarchy)
            {
                // Instantiate the object as a child of ground plane first
                placedObject = Instantiate(objectToPlace, groundPlaneStage);
                
                // Important: immediately detach from ground plane to prevent reanchoring
                placedObject.transform.SetParent(null);
                
                // Save the world position/rotation after detaching
                Vector3 worldPos = placedObject.transform.position;
                Quaternion worldRot = placedObject.transform.rotation;
                
                // Apply the saved transform to maintain position/orientation
                placedObject.transform.position = worldPos;
                placedObject.transform.rotation = worldRot;
                
                // Mark as placed and switch state
                isPlaced = true;
                currentState = InteractionState.Placed;
                
                Debug.Log("Object placed and detached from ground plane tracking");
            }
        }
    }
    
    private void HandleObjectInteraction()
    {
        // No object placed yet
        if (placedObject == null)
            return;
            
        // Check for different types of interactions
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                // Store initial touch position for possible movement
                touchStartPosition = touch.position;
                currentState = InteractionState.Moving;
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);
            
            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                // Calculate the initial distance between touches for scaling
                initialDistance = Vector2.Distance(touch0.position, touch1.position);
                initialScale = placedObject.transform.localScale;
                
                // Calculate initial rotation
                initialRotation = Mathf.Atan2(touch1.position.y - touch0.position.y, 
                                             touch1.position.x - touch0.position.x) * Mathf.Rad2Deg;
                
                currentState = InteractionState.Scaling;
            }
        }
    }
    
    private void HandleMovement()
    {
        if (Input.touchCount != 1)
        {
            currentState = InteractionState.Placed;
            return;
        }
        
        Touch touch = Input.GetTouch(0);
        
        if (touch.phase == TouchPhase.Ended)
        {
            currentState = InteractionState.Placed;
            return;
        }
        
        // Here you would implement raycasting to move the object
        // This is a simplified example - in a real implementation you'd
        // need to raycast onto the AR ground plane
        
        // Example pseudo-code for movement:
        // Ray ray = Camera.main.ScreenPointToRay(touch.position);
        // RaycastHit hit;
        // if (Physics.Raycast(ray, out hit, 100, groundPlaneLayer))
        // {
        //     placedObject.transform.position = hit.point;
        // }
    }
    
    private void HandleScaling()
    {
        if (Input.touchCount != 2)
        {
            currentState = InteractionState.Placed;
            return;
        }
        
        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);
        
        if (touch0.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Ended)
        {
            currentState = InteractionState.Placed;
            return;
        }
        
        // Calculate current distance between touches
        float currentDistance = Vector2.Distance(touch0.position, touch1.position);
        
        // Calculate scale factor
        float scaleFactor = currentDistance / initialDistance;
        
        // Apply scale to the object
        placedObject.transform.localScale = initialScale * scaleFactor;
        
        // Calculate current rotation angle
        float currentRotation = Mathf.Atan2(touch1.position.y - touch0.position.y,
                                           touch1.position.x - touch0.position.x) * Mathf.Rad2Deg;
        
        // Calculate rotation delta and apply
        float rotationDelta = currentRotation - initialRotation;
        placedObject.transform.Rotate(Vector3.up, rotationDelta);
        
        // Update initial values for smooth operation
        initialDistance = currentDistance;
        initialRotation = currentRotation;
    }
    
    private void HandleRotation()
    {
        // In this implementation, rotation is handled in the scaling method,
        // but you could split it out if needed
        currentState = InteractionState.Placed;
    }
    
    // Public method to reset placement
    public void ResetPlacement()
    {
        if (placedObject != null)
        {
            Destroy(placedObject);
        }
        
        isPlaced = false;
        currentState = InteractionState.Placing;
    }
}