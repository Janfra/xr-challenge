using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTouchKill : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.TryGetComponent(out PlayerRespawn respawn))
        {
            respawn.RespawnPlayer();
        }
    }
}
