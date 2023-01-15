using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerCheck
{
    /// <summary>
    /// Checks the given layer index and finds if there is a match. Displays the result in the log and sets given value.
    /// </summary>
    /// <param name="_layerIndex">Layer index being check</param>
    public static void CheckLayerIndex(ref int _layerIndex, bool isDebugging)
    {
        if (_layerIndex > 0 && _layerIndex < 32)
        {
            string layerSelected = LayerMask.LayerToName(_layerIndex);
            if (layerSelected.Length > 0)
            {
                if (isDebugging)
                {
                    Debug.Log($"Selected layer: {layerSelected}");
                }
            }
            else
            {
                Debug.LogWarning($"Layer not found, set back to default index: 0");
                _layerIndex = 0;
            }
        }
        else
        {
            _layerIndex = 0;
        }
    }
}
