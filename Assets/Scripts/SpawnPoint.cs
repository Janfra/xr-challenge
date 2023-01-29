using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SpawnPoint : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private CameraHandler.FacingDirection directionFacedOnRespawn;

    private void Awake()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerRespawn respawn))
        {
            respawn.SetSpawnPoint(transform, directionFacedOnRespawn);
        }
    }
}
