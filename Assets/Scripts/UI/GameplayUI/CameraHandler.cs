using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    #region Blocking Object Disabling

    private List<GameObject> coveringObjects;
    private Dictionary<GameObject, int> coveringObjectsLayer;
    private const float BOTTOM_FLOOR_Y_POS = 0f;

    [Header("Config")]
    [SerializeField]
    private LayerMask applicableLayers;
    [SerializeField]
    [Range(0, 32)]
    private int layerSwapIndex;

    #endregion

    [Header("Dependencies")]
    [SerializeField]
    private Transform playerPosition;

    #region Variables & Constants

    [Header("Config")]
    [SerializeField]
    private FacingDirection facingDirection = FacingDirection.Up;

    private Transform currentFollowTarget;
    private bool isChangingDirection;

    private const float ROTATION_SPEED = 10f;

    /// <summary>
    /// Distance kept from the cam to objects
    /// </summary>
    private const float CAM_DISTANCE = 5f;
    /// <summary>
    /// Height difference kept to cam target
    /// </summary>
    private const float MAX_CAM_HEIGHT = 8.5f;

    #endregion

    private void OnValidate()
    {
        LayerCheck.CheckLayerIndex(ref layerSwapIndex, false);
    }

    private void Awake()
    {
        coveringObjects = new List<GameObject>();
        coveringObjectsLayer = new Dictionary<GameObject, int>();
        SetFollowTarget(playerPosition);
    }

    private void Start()
    {
        PlayerRespawn.OnRespawn += CancelTransition;
    }

    private void LateUpdate()
    {
        if(currentFollowTarget != null)
        {
            if (!isChangingDirection)
            {
                FollowTarget();
            }
            CheckForCoveringObjects();
        }
        else
        {
            Debug.LogError("No target set for camera");
        }
    }

    /// <summary>
    /// Sets the target currently being follow by the camera.
    /// </summary>
    /// <param name="_target">New target</param>
    public void SetFollowTarget(Transform _target)
    {
        currentFollowTarget = _target;
    }

    /// <summary>
    /// Follows current target position while facing it
    /// </summary>
    private void FollowTarget()
    {
        transform.position = GetCameraPositionOnTarget();
        transform.LookAt(currentFollowTarget);
    }

    /// <summary>
    /// Gets the camera position based on target position and facing direction
    /// </summary>
    /// <returns>Offset camera position</returns>
    private Vector3 GetCameraPositionOnTarget()
    {
        Vector3 cameraOffsetPosition = currentFollowTarget.position;
        cameraOffsetPosition.y = YClamp(cameraOffsetPosition.y + MAX_CAM_HEIGHT);

        switch (facingDirection)
        {
            case FacingDirection.Up:
                cameraOffsetPosition.z -= CAM_DISTANCE;
                break;

            case FacingDirection.Down:
                cameraOffsetPosition.z += CAM_DISTANCE;

                break;
            case FacingDirection.Left:
                cameraOffsetPosition.x += CAM_DISTANCE;

                break;
            case FacingDirection.Right:
                cameraOffsetPosition.x -= CAM_DISTANCE;

                break;
            default:
                cameraOffsetPosition.z -= CAM_DISTANCE;

                break;
        }

        return cameraOffsetPosition;
    }

    /// <summary>
    /// Clamps the camera Y position to match the bottom floor Y pos constants.
    /// </summary>
    /// <param name="_yPos">position being clamped</param>
    /// <returns>The clamped result</returns>
    private float YClamp(float _yPos)
    {
        if(_yPos <= BOTTOM_FLOOR_Y_POS)
        {
            return BOTTOM_FLOOR_Y_POS;
        }
        return _yPos;
    }

    /// <summary>
    /// Sets the new facing direction of the camera
    /// </summary>
    /// <param name="_direction"></param>
    public void SetFacingDirection(FacingDirection _direction)
    {
        facingDirection = _direction;
        StartCoroutine(TransitionToNewFacingDirection());
    }

    /// <summary>
    /// Moves the camera position to the new facing direction
    /// </summary>
    /// <returns></returns>
    private IEnumerator TransitionToNewFacingDirection()
    {
        if (!isChangingDirection)
        {
            isChangingDirection = true;
            Vector3 targetPosition = GetCameraPositionOnTarget();

            // Continue as long as not at target pos and still changing
            while(transform.position != targetPosition && isChangingDirection)
            {
                targetPosition = GetCameraPositionOnTarget();
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * ROTATION_SPEED);
                transform.LookAt(currentFollowTarget);
                yield return null;
            }

            isChangingDirection = false;
        }
        yield return null;
    }

    /// <summary>
    /// Cancels camera transition to new direction
    /// </summary>
    private void CancelTransition()
    {
        isChangingDirection = false;
    }

    #region Covering Handling

    /// <summary>
    /// Check if any of the selected layers are in between the player and the camera, if so, set them to the culled camera layer.
    /// </summary>
    private void CheckForCoveringObjects()
    {
        // Shoot ray from the camera to the vector pointing at target position, with the distance in between the camera and the target.
        Ray ray = new Ray(transform.position, currentFollowTarget.position - transform.position);
        float distanceToTarget = Vector3.Distance(transform.position, currentFollowTarget.position);


        if(Physics.Raycast(ray, out RaycastHit hit, distanceToTarget, applicableLayers))
        {
            if(hit.transform.position.y > BOTTOM_FLOOR_Y_POS)
            {
                SetToUnculledLayer(hit.collider.gameObject);
            }
        }

        if(!Physics.Raycast(ray, distanceToTarget, 1 << layerSwapIndex))
        {
            ClearCoveredObjects();
        }
    }

    /// <summary>
    /// Stores a game object as a covering object and sets them to an unculled layer.
    /// </summary>
    /// <param name="_gameObject"></param>
    private void SetToUnculledLayer(GameObject _gameObject)
    {
        RemoveUnalignedObjects(_gameObject);
        AddCoveredObject(_gameObject);
        _gameObject.layer = layerSwapIndex;
    }

    /// <summary>
    /// Adds an object to the covering objects list.
    /// </summary>
    /// <param name="_gameObject"></param>
    private void AddCoveredObject(GameObject _gameObject)
    {
        coveringObjects.Add(_gameObject);
        coveringObjectsLayer.Add(_gameObject, _gameObject.layer); 
    }

    /// <summary>
    /// Removes a covering object from the list and sets them back to visible.
    /// </summary>
    /// <param name="_gameObject"></param>
    private void RemoveCoveredObject(GameObject _gameObject)
    {
        
        coveringObjects.Remove(_gameObject);
        if (coveringObjectsLayer.TryGetValue(_gameObject, out int initialLayer))
        {
            _gameObject.layer = initialLayer;
            coveringObjectsLayer.Remove(_gameObject);
        }
        else
        {
            Debug.LogError($"{_gameObject.name} was not found in the camera layers dictionary! Unable to set back to initial layer.");
        }
    }

    /// <summary>
    /// Sets the past covering object to its initial layer and removes it.
    /// </summary>
    private void ClearCoveredObjects()
    {
        if(coveringObjects.Count > 0)
        {
            List<GameObject> removedObjects = new List<GameObject>();
            foreach(GameObject coveringObject in coveringObjects)
            {
                removedObjects.Add(coveringObject);
            }
            foreach(var removedObject in removedObjects)
            {
                RemoveCoveredObject(removedObject);
            }
        }
    }

    /// <summary>
    /// Checks to remove all covering objects not aligned in the X position with the current covering object.
    /// </summary>
    /// <param name="_gameObject">Object being compared</param>
    private void RemoveUnalignedObjects(GameObject _gameObject)
    {
        List<GameObject> removedObjects = new List<GameObject>();
        float targetXPos = _gameObject.transform.position.x;

        foreach(GameObject coveringObject in coveringObjects)
        {
            if(Mathf.RoundToInt(coveringObject.transform.position.x) != Mathf.RoundToInt(targetXPos))
            {
                removedObjects.Add(coveringObject);
            }
        }
        foreach(var removedObject in removedObjects)
        {
            RemoveCoveredObject(removedObject);
        }
    }

    /// <summary>
    /// Check if target position is near a corner
    /// </summary>
    /// <returns>True if target is on a corner, otherwise false</returns>
    private bool IsTargetNearCorner()
    {
        // Does not always work, wont add until fully functional
        const float PROXIMITY_RANGE = 0.2f;
        float absoluteHorizontalPosition = Mathf.Abs(currentFollowTarget.position.x) + Mathf.Abs(currentFollowTarget.position.z);
        bool isBelowWithRange = Mathf.Floor(absoluteHorizontalPosition - PROXIMITY_RANGE) < Mathf.Floor(absoluteHorizontalPosition);
        bool isOverWithRange = Mathf.Floor(absoluteHorizontalPosition + PROXIMITY_RANGE) > Mathf.Floor(absoluteHorizontalPosition);

        return isOverWithRange || isBelowWithRange;
    }

    #endregion

    /// <summary>
    /// Get opposite direction of the current direction.
    /// </summary>
    /// <returns></returns>
    private FacingDirection GetOppositeDirection()
    {
        // Would use when the cover object is going into the same object several times
        // May instead use a function that check other directions until one has a true result when raycasting to player, otherwise stay in place or maybe go closer.
        switch (facingDirection)
        {
            case FacingDirection.Up:
                return FacingDirection.Down;
            case FacingDirection.Down:
                return FacingDirection.Up;
            case FacingDirection.Left:
                return FacingDirection.Right;
            case FacingDirection.Right:
                return FacingDirection.Left;
            default:
                break;
        }
        return FacingDirection.Up;
    }

    [System.Serializable]
    public enum FacingDirection
    {
        Up,
        Down,
        Left,
        Right,
    }

    private void OnDrawGizmos()
    {
        if(currentFollowTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (currentFollowTarget.position - transform.position));
        }
    }
}
