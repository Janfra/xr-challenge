using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PickupEventDelaySubscriber))]
public abstract class PickUpSubscriber : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    private Pickup pickupEnabler;
    [SerializeField]
    private PickupEventDelaySubscriber delaySubscribe;
    protected Action<Pickup> methodRunOnEvent;

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

    private void SubscribeToPickUp()
    {
        if (pickupEnabler == null)
        {
            Debug.LogError($"No pickup assigned to {gameObject.name}! Event added");
        }
        else
        {
            pickupEnabler.OnPickUp += methodRunOnEvent;
        }
    }
}
