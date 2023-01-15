using System.Collections;
using UnityEngine;

public class DoubleJumpJewel : MonoBehaviour
{
    #region Components

    private Timer timer;
    private PlayerController player;

    #endregion

    #region Variables & Constants

    private bool isActive;
    private int activeLayerIndex;
    private const float TRANSITION_TIME = 1f;
    private const float OPACITY_ONTAKEN = 0.3f;
    private const float OPACITY_ONACTIVE = 1f;
    private const float EFFECT_Y_OFFSET = 0.8f;

    #endregion

    #region Dependencies

    [Header("Dependencies")]
    [SerializeField]
    private Renderer meshRenderer;

    #endregion

    #region Config

    [Header("Config")]
    [SerializeField]
    private float timeForReactivation = 3f;
    [SerializeField]
    [Range(0, 31)]
    private int disableLayerIndex = 5;
    [SerializeField]
    private float pushPower;
    [SerializeField]
    private GameObject jewelEffectVisual;

    #endregion

    private void OnValidate()
    {
        LayerCheck.CheckLayerIndex(ref disableLayerIndex, false);
    }

    private void Awake()
    {
        timer = new Timer();
        activeLayerIndex = gameObject.layer;
        if(meshRenderer == null)
        {
            Debug.LogError($"Please set the jewel renderer in {gameObject.name}!");
            meshRenderer = GetComponentInChildren<Renderer>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player == null)
        {
            if(other.TryGetComponent(out player))
            {
                player.JumpHandler.OnJump += Disable;
                player.JumpHandler.OnGrounded += Disable;
                StartCoroutine(StartJewelEffect());
                OnTaken();
            }
        }
    }

    /// <summary>
    /// Disables the jewel effect 
    /// </summary>
    private void Disable()
    {
        isActive = false;
        player.JumpHandler.OnJump -= Disable;
        player.JumpHandler.OnGrounded -= Disable;
        player = null;
    }

    /// <summary>
    /// While active it lets the player jump at any point.
    /// </summary>
    private IEnumerator StartJewelEffect()
    {
        isActive = true;
        jewelEffectVisual.SetActive(true);
        while (isActive)
        {
            player.EnableJumping();
            SetJewelEffectOnPlayer();
            yield return null;
        }
        jewelEffectVisual.SetActive(false);
    }

    /// <summary>
    /// Sets the jewel effect on top of the players
    /// </summary>
    private void SetJewelEffectOnPlayer()
    {
        Vector3 jewelPosition = player.transform.position;
        jewelPosition.y += EFFECT_Y_OFFSET;

        jewelEffectVisual.transform.position = jewelPosition;
    }

    /// <summary>
    /// Disables the jewel to avoid being able to take it again, and starts reactivation timer.
    /// </summary>
    private void OnTaken()
    {
        gameObject.layer = disableLayerIndex;
        StartCoroutine(LerpOverTime(TRANSITION_TIME, OPACITY_ONTAKEN, Vector3.one / 2));
        StartCoroutine(StartReactivateTimer());
    }

    /// <summary>
    /// Starts timer until the jewel is active and can be taken again.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartReactivateTimer()
    {
        yield return new WaitForSeconds(timeForReactivation);
        gameObject.layer = activeLayerIndex;
        StartCoroutine(LerpOverTime(TRANSITION_TIME, OPACITY_ONACTIVE, Vector3.one));
    }

    /// <summary>
    /// Changes appearance of jewel over time.
    /// </summary>
    /// <param name="_duration">Duration of change</param>
    /// <param name="_opacity">New opacity of the jewel</param>
    /// <param name="_size">New size of the jewel</param>
    private IEnumerator LerpOverTime(float _duration, float _opacity, Vector3 _size)
    {
        timer.SetTimer(_duration);
        float initialOpacity = GetOpacity();
        Vector3 initialSize = transform.localScale;
        yield return null;

        timer.StartTimer(this);
        while (!timer.IsTimerDone)
        {
            transform.localScale = Vector3.Lerp(initialSize, _size, timer.GetTimeNormalized());
            SetOpacity(Mathf.Lerp(initialOpacity, _opacity, timer.GetTimeNormalized()));
            yield return null;
        }

        transform.localScale = _size;
        SetOpacity(_opacity);
    }

    /// <summary>
    /// Sets the opacity of the jewel
    /// </summary>
    /// <param name="_opacity">New opacity</param>
    private void SetOpacity(float _opacity)
    {
        Color jewelColour = meshRenderer.material.color;
        jewelColour.a = _opacity;
        meshRenderer.material.color = jewelColour;
    }

    /// <summary>
    /// Gets the current opacity of the jewel
    /// </summary>
    /// <returns>Opacity of the jewel material</returns>
    private float GetOpacity()
    {
        return meshRenderer.material.color.a;
    }

}
