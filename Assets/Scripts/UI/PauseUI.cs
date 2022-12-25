using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    private GameObject pauseUI;

    private void Awake()
    {
        if(pauseUI == null)
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
        pauseUI.SetActive(_gameState == GameManager.GameStates.Pause);
    }
}
