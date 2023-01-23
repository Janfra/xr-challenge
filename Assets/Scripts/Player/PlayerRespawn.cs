using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    /// <summary>
    /// In case I include an event that happens when player dies
    /// </summary>
    public static event Action OnRespawn;

    [Header("Dependencies")]
    [SerializeField]
    private Transform respawnPoint;
    
    public void RespawnPlayer()
    {
        AudioManager.Instance.TryPlayAudio("PlayerRespawn");
        transform.position = respawnPoint.position;
    }
}
