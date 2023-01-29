using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    /// <summary>
    /// In case I include an event that happens when player dies
    /// </summary>
    public static event Action OnRespawn;

    [Header("Dependencies")]
    [SerializeField]
    private CameraHandler cam;
    [SerializeField]
    private Transform spawnPoint;
    private CameraHandler.FacingDirection directionFacedOnRespawn = CameraHandler.FacingDirection.Up;

    private void Awake()
    {
        if(cam == null)
        {
            if(Camera.main.TryGetComponent(out CameraHandler camHandler))
            {
                cam = camHandler;
            }
            Debug.LogError("Set camera handler on player respawn");
        }
    }

    public void RespawnPlayer()
    {
        AudioManager.Instance.TryPlayAudio("PlayerRespawn");
        transform.position = spawnPoint.position;
        cam.SetFacingDirection(directionFacedOnRespawn);
        OnRespawn?.Invoke();
    }

    public void SetSpawnPoint(Transform _spawnPoint, CameraHandler.FacingDirection _facingDirection)
    {
        spawnPoint = _spawnPoint;
        directionFacedOnRespawn = _facingDirection;
    }
}
