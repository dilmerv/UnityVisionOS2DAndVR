using UnityEngine;
using UnityEngine.InputSystem;
using Logger = LearnXR.Core.Logger;

public class WindowedInputHandler : MonoBehaviour
{
    [SerializeField] private GameObject cursorPrefab;
    
    private bool useCursor;
    private GameObject cursor;
    private VisionOSInputActions visionOSInputActions;
    private Camera mainCamera;

    private void Awake()
    {
        visionOSInputActions = new VisionOSInputActions();
        visionOSInputActions.Enable();
        visionOSInputActions.VisionPro2D.TouchTap.performed += OnTouchTap;
        visionOSInputActions.VisionPro2D.TouchPosition.performed += OnTouchPositionChanged;
        mainCamera = Camera.main;
        
        if (cursorPrefab != null)
        {
            useCursor = true;
            cursor = Instantiate(cursorPrefab);
            cursor.gameObject.SetActive(false);
        }
    }
    
    private void OnTouchTap(InputAction.CallbackContext context) => Logger.Instance.LogInfo($"Touch tap was executed");
    
    private void OnTouchPositionChanged(InputAction.CallbackContext context)
    {
        Vector2 touchPosition = context.ReadValue<Vector2>();
        Logger.Instance.LogInfo($"Touch position changed: {touchPosition}");
        if (useCursor)
        {
            cursor.gameObject.SetActive(true);
            cursor.transform.position = mainCamera.ScreenToWorldPoint(new Vector3(touchPosition.x,
                touchPosition.y, -mainCamera.transform.position.z));
        }
    }
    
    private void OnDestroy()
    {
        visionOSInputActions.VisionPro2D.TouchTap.performed -= OnTouchTap;
        visionOSInputActions.VisionPro2D.TouchPosition.performed -= OnTouchPositionChanged;
    }
}
