using System;
using UnityEngine;

public class PickupSystem : MonoBehaviour
{
    static public event Action<float> OnPickUpUIUpdate;
    private float score = 0f;

    /// <summary>
    /// If collided with pickup, try to collect it.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Pickup pickupItem))
        {
            if (!pickupItem.IsCollected)
            {
                score += pickupItem.GetPickedUp();
                OnPickUpUIUpdate?.Invoke(score);
            }
        }
    }
}
