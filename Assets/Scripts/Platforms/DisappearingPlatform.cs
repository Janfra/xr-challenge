using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    #region Components

    private DisappearingStates currentState;
    private Timer timer;

    #endregion

    #region Dependencies

    [Header("Dependencies")]
    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private Collider platformCollider;

    #endregion

    #region Config

    [Header("Config")]
    [SerializeField]
    private UntouchedState untouchedState;
    [SerializeField]
    private DisappearedState disappearedState;

    #endregion

    private void Awake()
    {
        timer = new Timer();
        currentState = untouchedState;
        untouchedState.SetPlatformType(this);
    }

    private void Start()
    {
        GameManager.OnGameStateChanged += OnPause;
    }

    private void OnPause(GameManager.GameStates _gameState)
    {
        timer.PauseTimer(_gameState == GameManager.GameStates.Pause);
    }

    private void OnValidate()
    {
        disappearedState.OnValidate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentState.StartStateLogic(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        currentState.StartStateLogic(this);
    }

    private void OnTriggerExit(Collider other)
    {
        currentState.StartStateLogic(this);
    }

    private abstract class DisappearingStates
    {
        public abstract void StartStateLogic(DisappearingPlatform _caller);
        protected abstract IEnumerator RunStateLogic(DisappearingPlatform _caller);
        protected abstract void Init(DisappearingPlatform _caller);
        protected abstract void OnComplete(DisappearingPlatform _caller);
    }
    
    [System.Serializable]
    private class UntouchedState : DisappearingStates
    {
        [SerializeField]
        private float durationTillFall = 1f;
        private bool isFalling;

        public override void StartStateLogic(DisappearingPlatform _caller)
        {
            _caller.StartCoroutine(RunStateLogic(_caller));
        }

        protected override void Init(DisappearingPlatform _caller)
        {
            isFalling = true;
            _caller.timer.SetTimer(durationTillFall);
            _caller.timer.StartTimer(_caller);
        }

        protected override IEnumerator RunStateLogic(DisappearingPlatform _caller)
        {
            if (!isFalling)
            {
                Init(_caller);

                Color color = _caller.meshRenderer.material.color;
                while (!_caller.timer.IsTimerDone)
                {
                    color.a = _caller.timer.GetReversedTimeNormalized();
                    _caller.meshRenderer.material.color = color;
                    yield return null;
                }

                OnComplete(_caller);
            }
            yield return null;
        }

        protected override void OnComplete(DisappearingPlatform _caller)
        {
            _caller.platformCollider.isTrigger = true;
            _caller.currentState = _caller.disappearedState;
            _caller.currentState.StartStateLogic(_caller);
            isFalling = false;
        }

        /// <summary>
        /// Sets platform colour depending on duration till fall
        /// </summary>
        public void SetPlatformType(DisappearingPlatform _caller)
        {
            if(durationTillFall <= ((int)PlatformTypes.QuickFall))
            {
                _caller.meshRenderer.material.color = GetPlatformTypeColour(PlatformTypes.QuickFall);
            }
            else if(durationTillFall <= ((int)PlatformTypes.NormalFall))
            {
                _caller.meshRenderer.material.color = GetPlatformTypeColour(PlatformTypes.NormalFall);
            }
            else
            {
                _caller.meshRenderer.material.color = GetPlatformTypeColour(PlatformTypes.LongFall);
            }
        }

        /// <summary>
        /// Gets the colour set for the type requested.
        /// </summary>
        /// <param name="platformType">Platform type requested</param>
        /// <returns>Colour set for the type</returns>
        private static Color GetPlatformTypeColour(PlatformTypes platformType)
        {
            switch (platformType)
            {
                case PlatformTypes.QuickFall:
                    return Color.blue;
                case PlatformTypes.NormalFall:
                    return Color.yellow;
                case PlatformTypes.LongFall:
                    return Color.green;
                default:
                    Debug.Log("Platform type requested doesnt exist... somehow.");
                    return Color.red;
            }
        }

        [System.Serializable]
        public enum PlatformTypes 
        {
            QuickFall = 2,
            NormalFall = 4,
            LongFall = 6,
        }
    }

    [System.Serializable]
    private class DisappearedState : DisappearingStates
    {
        #region Config

        [SerializeField]
        private float durationTillRegen = 1f;
        [SerializeField]
        [Range(0, 31)]
        private int layerSwapIndex = 5;

        #endregion

        #region Variables & Constants

        private int initialLayer;
        private bool isRegenerating;
        private bool isTriggerEntered = false;
  
        #endregion

        public override void StartStateLogic(DisappearingPlatform _caller)
        {
            _caller.StartCoroutine(RunStateLogic(_caller));
        }

        protected override void Init(DisappearingPlatform _caller)
        {
            initialLayer = _caller.gameObject.layer;
            _caller.gameObject.layer = layerSwapIndex;
            isRegenerating = true;
            _caller.timer.SetTimer(durationTillRegen);
            _caller.timer.StartTimer(_caller);
        }

        protected override IEnumerator RunStateLogic(DisappearingPlatform _caller)
        {
            if (!isRegenerating)
            {
                Init(_caller);

                Color color = _caller.meshRenderer.material.color;
                while (!_caller.timer.IsTimerDone)
                {
                    color.a = _caller.timer.GetTimeNormalized();
                    _caller.meshRenderer.material.color = color;
                    yield return null;
                }

                OnComplete(_caller);
            }
            else
            {
                // Change is trigger entered to be the opposite of what it currently is. Starting off as false.
                isTriggerEntered = !isTriggerEntered;
                _caller.timer.PauseTimer(isTriggerEntered);
            }
            yield return null;
        }

        protected override void OnComplete(DisappearingPlatform _caller)
        {
            _caller.platformCollider.isTrigger = false;
            _caller.gameObject.layer = initialLayer;
            _caller.currentState = _caller.untouchedState;
            isRegenerating = false;
        }

        public void OnValidate()
        {
            LayerCheck.CheckLayerIndex(ref layerSwapIndex);
        }
    }

}