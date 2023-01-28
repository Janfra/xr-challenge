using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RotateCamera : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private CameraHandler.FacingDirection faceDirection;

    [Header("Dependencies")]
    private Camera mainCamera;

    private const float ROTATION_SPEED = 0.5f;

    private void Awake()
    {
        mainCamera = Camera.main;
        BoxCollider collider = GetComponent<BoxCollider>();
        collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController player) && mainCamera.TryGetComponent(out CameraHandler camHandler))
        {
            camHandler.SetFacingDirection(faceDirection);
        }
    }
}
