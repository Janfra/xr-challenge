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

    private void LateUpdate()
    {
        FollowPlayer();
    }

    /// <summary>
    /// Follows player position while facing it
    /// </summary>
    private void FollowPlayer()
    {
        transform.position = GetCameraPosition();
        transform.LookAt(playerPosition);
    }

    /// <summary>
    /// Gets the camera position based on player position
    /// </summary>
    /// <returns>Offset camera position</returns>
    private Vector3 GetCameraPosition()
    {
        Vector3 cameraOffsetPosition = playerPosition.position;
        cameraOffsetPosition.y = transform.position.y;
        cameraOffsetPosition.z -= CAM_DISTANCE;

        return cameraOffsetPosition;
    }
}
