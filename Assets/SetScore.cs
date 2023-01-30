using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetScore : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    TextMeshProUGUI text;

    /// <summary>
    /// Set the score text
    /// </summary>
    private void Start()
    {
        text.text = $"{PickupSystem.score}";
    }
}
