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

    private Transform currentFollowTarget;

    private float camHeight;

    /// <summary>
    /// Distance kept from the cam to objects
    /// </summary>
    private const float CAM_DISTANCE = 5f;
    /// <summary>
    /// Height difference kept to cam target
    /// </summary>
    private const float MAX_CAM_HEIGHT = 8.5f;
    private const float MIN_CAM_HEIGHT = 5f;

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

    private void LateUpdate()
    {
        if(currentFollowTarget != null)
        {
            FollowTarget();
            CheckForCoveringObjects();
        }
        else
        {
            Debug.LogError("No target set for camera");
        }
    }

    public void ChangeCameraHeight()
    {
        Debug.Log(camHeight == MAX_CAM_HEIGHT ? MIN_CAM_HEIGHT : MAX_CAM_HEIGHT);
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
        transform.position = GetCameraPositionOnTarget(currentFollowTarget);
        transform.LookAt(currentFollowTarget);
    }

    /// <summary>
    /// Gets the camera position based on target position
    /// </summary>
    /// <returns>Offset camera position</returns>
    private Vector3 GetCameraPositionOnTarget(Transform _target)
    {
        Vector3 cameraOffsetPosition = _target.position;
        cameraOffsetPosition.y = YClamp(cameraOffsetPosition.y + MAX_CAM_HEIGHT);
        cameraOffsetPosition.z -= CAM_DISTANCE;

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
        if(coveringObjectsLayer.TryGetValue(_gameObject, out int initialLayer))
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
            if(coveringObject.transform.position.x != targetXPos)
            {
                removedObjects.Add(coveringObject);
            }
        }
        foreach(var removedObject in removedObjects)
        {
            RemoveCoveredObject(removedObject);
        }
    }
}
