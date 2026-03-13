using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

/// <summary>
/// Allows this object to be moved by user input
/// </summary>
public class MovingObject : MonoBehaviour
{
    [SerializeField]
    float rotationSpeed, movementSpeed;

    Vector2 moveInput;

    private void Update()
    {
        transform.SetPositionAndRotation(DoForward(), DoRotation());
    }

    public void OnMoveInput(CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private Vector3 DoForward()
    {
        return transform.position + (transform.forward * movementSpeed * moveInput.y * Time.deltaTime);
    }

    private Quaternion DoRotation()
    {
        return transform.rotation * Quaternion.Euler(0f, 90f * rotationSpeed * moveInput.x * Time.deltaTime, 0f);
    }
}
