using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    Transform playerTransform;
    private const float OUT_OF_BOUNDS_Y_POS = -0.5f;

    private void Update()
    {
        Vector3 boundsPosition = playerTransform.position;
        boundsPosition.y = OUT_OF_BOUNDS_Y_POS;
        transform.position = boundsPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerRespawn respawn))
        {
            respawn.RespawnPlayer();
        }
    }
}
