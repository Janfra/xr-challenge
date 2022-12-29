using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpJewel : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private int layerIndex = 5;
    [SerializeField]
    private float pushPower;

    private PlayerController player;

    private void OnValidate()
    {
        LayerCheck.CheckLayerIndex(ref layerIndex);
    }

    private void OnTriggerEnter(Collider other)
    {
        transform.localScale *= 2;
        if(other.TryGetComponent(out player))
        {
            player.JumpHandler.OnJump += Activate;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (player != null)
        {
            player.SetToGrounded();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        transform.localScale /= 2;
        if(player != null)
        {
            player.JumpHandler.OnJump -= Activate;
        }
        player = null;
    }
    private void Activate()
    {
        throw new NotImplementedException();
    }
}
