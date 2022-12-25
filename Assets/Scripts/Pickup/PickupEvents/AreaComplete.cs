using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PickupEventDelaySubscriber))]
public abstract class AreaComplete : MonoBehaviour
{
    public event Action OnAreaCompleted;

    [Header("Dependencies")]
    [SerializeField]
    private Pickup pickupEnabler;
    [SerializeField]
    private PickupEventDelaySubscriber delaySubscribe;

    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        if(delaySubscribe == null)
        {
            Debug.Log($"{gameObject.name} is not subscribed to the delay.");
            delaySubscribe = GetComponent<PickupEventDelaySubscriber>();
        }
        delaySubscribe.OnAfterDelaySubscribe += SubscribeToPickUp;
    }

    protected virtual void OnAreaComplete(Pickup _pickup)
    {
        OnAreaCompleted?.Invoke();
    }

    private void SubscribeToPickUp()
    {
        if (pickupEnabler == null)
        {
            Debug.LogError($"No pickup assigned to complete area on {gameObject.name}!");
        }
        pickupEnabler.OnPickUp += OnAreaComplete;
    }
}
