using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketEndArea : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    private List<Pickup> pickupsSubscribedTo;
    
    [Header("Variables")]
    private List<Pickup> pickupsActivated;
    private int pickupsCollectedCount;

    private void Awake()
    {
        pickupsActivated = new List<Pickup>();

        List<Pickup> pickupDuplicatesCheck = new List<Pickup>();
        foreach (Pickup pickup in pickupsSubscribedTo)
        {
            if (pickupDuplicatesCheck.Contains(pickup))
            {
                pickupsSubscribedTo.Remove(pickup);
                Debug.LogWarning($"{pickup.name} was added twice to the pickups required to win!");
                return;
            }
            pickupDuplicatesCheck.Add(pickup);
        }
    }
    private void Start()
    {
        foreach (Pickup pickup in pickupsSubscribedTo)
        {
            pickup.OnPickUp += PickupCollected;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PickupSystem playerPickup) && AreAllPickupsCollected())
        {
            Debug.Log("Game completed!");
            OnScreenMessagesHandler.SetScreenMessage("Game Completed!");
        }
        else if (!AreAllPickupsCollected())
        {
            OnScreenMessagesHandler.SetScreenMessage($"You still need to collect {pickupsSubscribedTo.Count - pickupsCollectedCount} stars!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out PickupSystem player))
        {
            OnScreenMessagesHandler.DisableScreenMessage();
        }
    }

    private bool AreAllPickupsCollected()
    {
        return pickupsCollectedCount == pickupsSubscribedTo.Count;
    }

    private void PickupCollected(Pickup _pickup)
    {
        if (pickupsActivated.Contains(_pickup))
        {
            return;
        }

        pickupsActivated.Add(_pickup);
        pickupsCollectedCount++;
        pickupsCollectedCount = Mathf.Clamp(pickupsCollectedCount, 0, pickupsSubscribedTo.Count);
    }


    /// <summary>
    /// In case of instantiating pickups, be able to subscribe to them.
    /// </summary>
    /// <param name="_pickup">Pickup being subscribed to</param>
    public void AddPickupRequired(Pickup _pickup)
    {
        if (pickupsSubscribedTo.Contains(_pickup))
        {
            return;
        }

        pickupsSubscribedTo.Add(_pickup);
        _pickup.OnPickUp += PickupCollected;
    }
}