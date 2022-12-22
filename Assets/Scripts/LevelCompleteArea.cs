using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteArea : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    private List<Pickup> pickupsNeeded;
    
    [Header("Variables")]
    private List<Pickup> pickupsCollectedList;
    private int pickupsCollectedCount;

    private void Awake()
    {
        pickupsCollectedList = new List<Pickup>();

        List<Pickup> pickupDuplicatesCheck = new List<Pickup>();
        foreach (Pickup pickup in pickupsNeeded)
        {
            if (pickupDuplicatesCheck.Contains(pickup))
            {
                pickupsNeeded.Remove(pickup);
                Debug.LogWarning($"{pickup.name} was added twice to the pickups required to win!");
                return;
            }
            pickupDuplicatesCheck.Add(pickup);
        }
    }
    private void Start()
    {
        foreach (Pickup pickup in pickupsNeeded)
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
            OnScreenMessagesHandler.SetScreenMessage($"You still need to collect {pickupsNeeded.Count} stars!");
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
        return pickupsCollectedCount == pickupsNeeded.Count;
    }

    private void PickupCollected(Pickup _pickup)
    {
        if (pickupsCollectedList.Contains(_pickup))
        {
            return;
        }

        pickupsCollectedList.Add(_pickup);
        pickupsCollectedCount++;
        pickupsCollectedCount = Mathf.Clamp(pickupsCollectedCount, 0, pickupsNeeded.Count);
    }

    public void AddPickupRequired(Pickup _pickup)
    {
        if (pickupsNeeded.Contains(_pickup))
        {
            return;
        }

        pickupsNeeded.Add(_pickup);
        _pickup.OnPickUp += PickupCollected;
    }
}
