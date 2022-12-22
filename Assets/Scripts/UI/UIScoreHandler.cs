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

    private void UpdateScore(float _newScore)
    {
        uiText.text = $"SCORE: {_newScore}";   
    }
}
