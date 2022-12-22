using System;
using UnityEngine;

public class PickupSystem : MonoBehaviour
{
    static public event Action<float> OnPickUpUIUpdate;

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
                float pickupScore = pickupItem.GetPickedUp();
                OnPickUpUIUpdate?.Invoke(pickupScore);
            }
        }
    }
}
