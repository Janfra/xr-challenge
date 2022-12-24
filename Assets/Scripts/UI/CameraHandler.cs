using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    private Transform currentTarget;

    [Header("Dependencies")]
    [SerializeField]
    private Transform playerPosition;


    #region Constants

    /// <summary>
    /// Distance kept from the cam to objects
    /// </summary>
    private const float CAM_DISTANCE = 5f;
    /// <summary>
    /// Height difference kept to cam target
    /// </summary>
    private const float CAM_HEIGHT = 4.151f;

    #endregion

    private void Awake()
    {
        currentTarget = playerPosition;
    }

    private void LateUpdate()
    {
        if(currentTarget != null)
        {
            FollowTarget(currentTarget);
        }
        else
        {
            Debug.LogError("No target set for camera");
        }
    }

    /// <summary>
    /// Follows player position while facing it
    /// </summary>
    private void FollowTarget(Transform _target)
    {
        transform.position = GetCameraPositionOnTarget(_target);
        transform.LookAt(_target);
    }

    /// <summary>
    /// Gets the camera position based on target position
    /// </summary>
    /// <returns>Offset camera position</returns>
    private Vector3 GetCameraPositionOnTarget(Transform _target)
    {
        Vector3 cameraOffsetPosition = _target.position;
        cameraOffsetPosition.y += CAM_HEIGHT;
        cameraOffsetPosition.z -= CAM_DISTANCE;

        return cameraOffsetPosition;
    }
}
