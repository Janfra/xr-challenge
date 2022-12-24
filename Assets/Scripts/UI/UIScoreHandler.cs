using System;
using TMPro;
using UnityEngine;

public class UIScoreHandler : MonoBehaviour
{
    [Header("Dependecies")]
    [SerializeField]
    private TextMeshProUGUI uiText;

    private void Awake()
    {
        if(uiText == null)
        {
            uiText = GetComponent<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
        PickupSystem.OnPickUpUIUpdate += UpdateScore;
    }

    /// <summary>
    /// Updates the score UI
    /// </summary>
    /// <param name="_newScore"></param>
    private void UpdateScore(float _newScore)
    {
        uiText.text = $"SCORE: {_newScore}";   
    }
}
