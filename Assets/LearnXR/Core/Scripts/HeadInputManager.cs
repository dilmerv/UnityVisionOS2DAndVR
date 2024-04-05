using UnityEngine;
using UnityEngine.InputSystem;

public class HeadInputManager : MonoBehaviour
{
    [SerializeField] private InputActionProperty headposePositionInputAction;
    
    [SerializeField] private InputActionProperty headposeRotationInputAction;
    
    [Header("Object To Control With Head Pose")]
    [SerializeField] private GameObject objectToControl;

    [SerializeField] private Vector3 headposeOffset;

    [SerializeField] private float smoothSpeed = 0.5f;
    
    void Start()
    {
        if (headposePositionInputAction != null)
        {
            headposePositionInputAction.action.Enable();
            headposePositionInputAction.action.performed += PositionChanged;
        }
        
        if (headposeRotationInputAction != null)
        {
            headposeRotationInputAction.action.Enable();
            headposeRotationInputAction.action.performed += RotationChanged;
        }
    }

    private void PositionChanged(InputAction.CallbackContext obj)
    {
        var headposePosition = obj.ReadValue<Vector3>();
        objectToControl.transform.position = Vector3.Lerp(objectToControl.transform.position, 
            headposePosition + headposeOffset, smoothSpeed * Time.deltaTime);
    }
    
    private void RotationChanged(InputAction.CallbackContext obj)
    {
        var headposeRotation = obj.ReadValue<Quaternion>();
        headposeRotation.y *= -1;
        headposeRotation.x *= -1;
        objectToControl.transform.rotation = Quaternion.Slerp(objectToControl.transform.rotation, 
            headposeRotation, smoothSpeed * Time.deltaTime);
    }
}
