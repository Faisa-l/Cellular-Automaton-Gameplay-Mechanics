using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

/// <summary>
/// Controls the behaviour of the camera.
/// </summary>

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField]
    float cameraSpeed = 1.0f;


    bool isMoving;
    Vector2 inputDirection;
    Camera MainCamera => Camera.main;

    private void Awake()
    {
        isMoving = false;
        inputDirection = new();
    }

    // Retrieve direction from vector
    public void MoveCamera(CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            inputDirection = context.ReadValue<Vector2>();
            isMoving = true;
            return;
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            inputDirection = new();
            isMoving = false;
            return;
        }
    }

    public void Update()
    {
        if (isMoving)
        {
            RepositionCamera();
        }
    }

    // Handles changing the position of the camera
    private void RepositionCamera()
    {
        var position = cameraSpeed * Time.deltaTime * inputDirection;

        var newPosition = new Vector3(
            -position.y + MainCamera.transform.position.x,
            MainCamera.transform.position.y,
            position.x + MainCamera.transform.position.z);

        MainCamera.transform.position = newPosition;
    }
}
