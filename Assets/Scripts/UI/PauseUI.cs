using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    private GameObject pauseUIContainer;

    private void Awake()
    {
        if(pauseUIContainer == null)
        {
            Debug.LogError("No pause UI set");
        }
    }

    private void Start()
    {
        GameManager.OnGameStateChanged += OnPause;
    }

    private void OnPause(GameManager.GameStates _gameState)
    {
        pauseUIContainer.SetActive(_gameState == GameManager.GameStates.Pause);
    }
}
