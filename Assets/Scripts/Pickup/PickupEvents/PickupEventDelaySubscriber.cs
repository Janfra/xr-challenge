using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupEventDelaySubscriber : MonoBehaviour
{
    public event Action OnAfterDelaySubscribe;

    private void Start()
    {
        OnAfterDelaySubscribe?.Invoke();
    }
}
