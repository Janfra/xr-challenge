using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    private Transform playerPosition;

    /// <summary>
    /// Distance kept from the cam to objects
    /// </summary>
    private const float CAM_DISTANCE = 5f;
    /// <summary>
    /// Height difference kept to cam target
    /// </summary>
    private const float CAM_HEIGHT = 4.151f;

    private void Awake()
    {
        Debug.Log(transform.position.y - playerPosition.position.y);
    }

    private void LateUpdate()
    {
        FollowPlayer();
    }

    /// <summary>
    /// Follows player position while facing it
    /// </summary>
    private void FollowPlayer()
    {
        transform.position = GetCameraPositionOnTarget(playerPosition);
        transform.LookAt(playerPosition);
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
