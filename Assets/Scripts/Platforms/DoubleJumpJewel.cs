using System.Collections;
using UnityEngine;

public class DoubleJumpJewel : MonoBehaviour
{
    private PlayerController player;
    private bool isActive;
    private int activeLayerIndex;

    [Header("Config")]
    [SerializeField]
    private int disableLayerIndex = 5;
    [SerializeField]
    private float pushPower;

    private void OnValidate()
    {
        LayerCheck.CheckLayerIndex(ref disableLayerIndex);
    }

    private void Awake()
    {
        activeLayerIndex = gameObject.layer;
    }

    private void OnTriggerEnter(Collider other)
    {
        transform.localScale *= 2;
        if (player == null)
        {
            if(other.TryGetComponent(out player))
            {
                player.JumpHandler.OnJump += Disable;
                player.JumpHandler.OnGrounded += Disable;
                StartCoroutine(StartJewelEffect());
            }
        }
        gameObject.layer = disableLayerIndex;
    }

    private void OnTriggerExit(Collider other)
    {
        transform.localScale /= 2;
    }

    private void Disable()
    {
        isActive = false;
        gameObject.layer = activeLayerIndex;
        player.JumpHandler.OnJump -= Disable;
        player.JumpHandler.OnGrounded -= Disable;
        StopAllCoroutines();
        player = null;
    }

    private IEnumerator StartJewelEffect()
    {
        isActive = true;
        while (isActive)
        {
            Debug.Log("Running jewel effect");
            player.SetToGrounded();
            yield return null;
        }
    }
}
