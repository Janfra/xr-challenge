using System;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerRotation
{
    #region Dependencies

    [Header("Dependencies")]
    /// <summary>
    /// Camera used for rotation ray casting
    /// </summary>
    private Camera cam;

    #endregion

    #region Config

    [Header("Config")]
    /// <summary>
    /// Layers that will collide with the rotation ray
    /// </summary>
    [SerializeField] 
    private LayerMask hitOnlyLayers;

    /// <summary>
    /// Sets the speed at which the player will rotate to face hit point
    /// </summary>
    [SerializeField]
    private float rotationSpeed;

    /// <summary>
    /// Shows mouse collision on world
    /// </summary>
    [SerializeField] 
    private GameObject pointerOnWorld;

    #endregion

    private Vector3 lookAtPosition;
    private bool isGamepad => Gamepad.current != null && Gamepad.current.enabled;

    /// <summary>
    /// Delegate to run current type of rotation, cursor or gamepad.
    /// </summary>
    private Action<Transform> onRotate;

    /// <summary>
    /// Initializes the class
    /// </summary>
    public void Init(PlayerInputs _input)
    {
        cam = Camera.main;
        if(pointerOnWorld == null)
        {
            Debug.LogWarning("No pointer assigned on player rotation");
        }

        if (isGamepad)
        {
            Debug.Log("Gamepad is active, set onRotate to Gamepad rotation");
            // onRotate = GetGamepadRotation;
        }
        else
        {
            onRotate = GetCursorRotation;
        }
        _input.Player.Look.performed += context => lookAtPosition = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Runs logic for current type of rotation
    /// </summary>
    /// <param name="_transform">Player to rotate</param>
    public void GetRotation(Transform _transform)
    {
        if(onRotate != null)
        {
            onRotate.Invoke(_transform);
        }
        else
        {
            Debug.LogWarning("onRotate is null in player rotation");
        }
    }

    /// <summary>
    /// Sets the player rotation using the mouse position
    /// </summary>
    private void GetCursorRotation(Transform _transform)
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, hitOnlyLayers))
        {
            Vector3 hitPos = hit.point;
            RotateTowardsPoint(hitPos, _transform);
            GenerateVisualPoint(hitPos, _transform);
        }
        else
        {
            DisableVisualPoint();
        }
    }

    private void GetGamepadRotation(Transform _transform)
    {
        // Do gamepad rotation logic
    }

    /// <summary>
    /// Moves world pointer to hit position
    /// </summary>
    /// <param name="_rayCollisionPoint">Mouse position on world</param>
    private void GenerateVisualPoint(Vector3 _rayCollisionPoint, Transform _transform)
    {
        if(pointerOnWorld != null)
        {
            pointerOnWorld.SetActive(true);
            _rayCollisionPoint.y = _transform.position.y;
            pointerOnWorld.transform.position = _rayCollisionPoint;
        }
    }

    /// <summary>
    /// Disables world pointer 
    /// </summary>
    private void DisableVisualPoint()
    {
        if (pointerOnWorld != null)
        {
            pointerOnWorld.SetActive(false);
        }
    }

    /// <summary>
    /// Rotates player to face target position at the specified speed.
    /// </summary>
    /// <param name="_lookAtPosition">Target position to rotate to</param>
    /// <param name="_transform">Transform to rotate</param>
    private void RotateTowardsPoint(Vector3 _lookAtPosition, Transform _transform)
    {
        Vector3 targetPosition = GetRotationDirection(_lookAtPosition, _transform);
        // Rotates toward target
        Quaternion rotationTarget = Quaternion.LookRotation(targetPosition);
        _transform.rotation = Quaternion.RotateTowards(_transform.rotation, rotationTarget, rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Instantly rotates the player to face target position
    /// </summary>
    /// <param name="_lookAtPosition">Target position</param>
    /// <param name="_transform">Player to rotate</param>
    private void InstantlyRotateToPoint(Vector3 _lookAtPosition, Transform _transform)
    {
        Vector3 targetDirection = GetRotationDirection(_lookAtPosition, _transform);
        _transform.forward = targetDirection;
    }

    /// <summary>
    /// Returns a Vector3 pointing to the direction of the given Vector3 parameter
    /// </summary>
    /// <param name="_lookAtPosition">Position to point to</param>
    /// <param name="_transform">Player to be rotated</param>
    /// <returns>Vector3 pointing to _lookAtPosition</returns>
    private Vector3 GetRotationDirection(Vector3 _lookAtPosition, Transform _transform)
    {
        _lookAtPosition.y = _transform.position.y;
        return _lookAtPosition - _transform.position;
    }

    /// <summary>
    /// Unsubscribed from events on destroy
    /// </summary>
    /// <param name="_input">Event owner</param>
    public void OnDestroy(PlayerInputs _input)
    {
        _input.Player.Look.performed -= context => lookAtPosition = context.ReadValue<Vector2>();
    }
}
