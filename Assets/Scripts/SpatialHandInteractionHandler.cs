using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.VisionOS;
using UnityEngine.XR.VisionOS.InputDevices;
using Logger = LearnXR.Core.Logger;

public class SpatialHandInteractionHandler : MonoBehaviour
{
    [SerializeField] private GameObject hand;
    [SerializeField] private GameObject spatialPointerPrefab;
    [SerializeField] private float spatialPointerDistance = 1000.0f;
    [SerializeField] private LayerMask layersUsedForInteractions;

    [SerializeField] private InputActionProperty handPositionProperty;
    [SerializeField] private InputActionProperty handRotationProperty;
    [SerializeField] private InputActionProperty spatialPointerProperty;

    private GameObject spatialPointer;
    private LineRenderer spatialPointerLine;
    private Transform selectedObject;

    private void Awake()
    {
        if (spatialPointerPrefab != null)
        {
            spatialPointer = Instantiate(spatialPointerPrefab, transform);
            spatialPointer.SetActive(false);
            var pointerLine = spatialPointer.transform.GetChild(0);
            spatialPointerLine = pointerLine.GetComponent<LineRenderer>();
        }
    }

    private void Update()
    {
        var handPosition = handPositionProperty.action.ReadValue<Vector3>();
        var handRotation = handRotationProperty.action.ReadValue<Quaternion>();
        
        // sets the hand position and rotation
        hand.transform.SetLocalPositionAndRotation(handPosition, handRotation);
        
        // update pointer info
        var pointerState = spatialPointerProperty.action.ReadValue<VisionOSSpatialPointerState>();
        
        // update pointer and ray
        UpdatePointer(pointerState, spatialPointer);
    }

    private void UpdatePointer(VisionOSSpatialPointerState pointerState, GameObject pointer)
    {
        var isPointerActive = pointerState.phase == VisionOSSpatialPointerPhase.Began ||
                              pointerState.phase == VisionOSSpatialPointerPhase.Moved;

        var pointerDevicePosition = transform.InverseTransformPoint(pointerState.inputDevicePosition);
        var pointerDeviceRotation = pointerState.inputDeviceRotation;
    
        pointer.gameObject.SetActive(pointerState.isTracked);
        pointer.transform.SetLocalPositionAndRotation(pointerDevicePosition, pointerDeviceRotation);

        if (isPointerActive)
        {
            spatialPointerLine.enabled = true;
            spatialPointerLine.SetPosition(1, new Vector3(0,0, spatialPointerDistance));
            spatialPointerLine.transform.rotation = pointerState.startRayRotation;

            if (Physics.Raycast(pointerState.startRayOrigin, pointerState.startRayDirection, out RaycastHit hit, 
                    Mathf.Infinity, layersUsedForInteractions))
            {
                selectedObject = hit.transform;
                selectedObject.GetComponent<Rotator>().Activate();
                Logger.Instance.LogInfo($"(Pointer_{spatialPointerProperty.action.name}) ray collided with ({selectedObject.name})");
            }
        }
        else
        {
            spatialPointerLine.enabled = false;
        }
    }
}
