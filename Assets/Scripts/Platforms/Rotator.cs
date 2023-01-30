using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private float rotationSpeed = 100;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.right, Time.deltaTime * rotationSpeed);
    }
}
