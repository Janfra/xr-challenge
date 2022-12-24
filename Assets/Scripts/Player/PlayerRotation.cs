using UnityEngine;

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

    /// <summary>
    /// Initializes the class
    /// </summary>
    public void Init()
    {
        cam = Camera.main;
        if(pointerOnWorld == null)
        {
            Debug.LogWarning("No pointer assigned on player rotation");
        }
    }

    /// <summary>
    /// Shoots a ray that is used to change the players rotation
    /// </summary>
    public void GetRotation(Transform _transform)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
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

    /// <summary>
    /// Moves world pointer to hit position
    /// </summary>
    /// <param name="_rayCollisionPoint"></param>
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
    /// <param name="_hitPos">Target position to rotate to</param>
    /// <param name="_transform">Transform to rotate</param>
    private void RotateTowardsPoint(Vector3 _hitPos, Transform _transform)
    {
        // Get target
        _hitPos.y = _transform.position.y;
        Vector3 rotation = _hitPos - _transform.position;

        // Rotates toward target
        Quaternion rotationTarget = Quaternion.LookRotation(rotation);
        _transform.rotation = Quaternion.RotateTowards(_transform.rotation, rotationTarget, rotationSpeed);
    }

    /// <summary>
    /// Instantly rotates the player to face target position
    /// </summary>
    /// <param name="_hitPos"></param>
    /// <param name="_transform"></param>
    private void InstantlyRotateToPoint(Vector3 _hitPos, Transform _transform)
    {
        _hitPos.y = _transform.position.y;
        Vector3 rotation = _hitPos - _transform.position;
        _transform.forward = rotation;
    }
}
