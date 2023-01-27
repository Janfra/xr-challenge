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
    [Range(0, 31)]
    public int layerSwapIndex = 2;

    [SerializeField]
    private UntouchedState untouchedState;
    [SerializeField]
    private DisappearedState disappearedState;

    #endregion

    private void Awake()
    {
        timer = new Timer();
        untouchedState.SetPlatformType(this);
        SetPlatformState(untouchedState);
    }

    private void Start()
    {
        GameManager.OnGameStateChanged += OnPause;
    }

    private void OnPause(GameManager.GameStates _gameState)
    {
        if (!timer.IsTimerDone)
        {
            timer.PauseTimer(_gameState == GameManager.GameStates.Pause);
        }
    }

    private void OnValidate()
    {
        LayerCheck.CheckLayerIndex(ref layerSwapIndex, false);
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

    private void SetPlatformState(DisappearingStates _newState)
    {
        currentState = _newState;
        currentState.OnSet(this);
    }

    private abstract class DisappearingStates
    {
        /// <summary>
        /// Sets off the logic for the current states
        /// </summary>
        /// <param name="_caller">Caller to run logic on</param>
        public abstract void StartStateLogic(DisappearingPlatform _caller);
        /// <summary>
        /// Runs the logic of the current state
        /// </summary>
        /// <param name="_caller"></param>
        protected abstract IEnumerator RunStateLogic(DisappearingPlatform _caller);
        /// <summary>
        /// Initializes the current state
        /// </summary>
        /// <param name="_caller"></param>
        protected abstract void SetupLogic(DisappearingPlatform _caller);
        /// <summary>
        /// Runs when set to current, resetting it for reusing
        /// </summary>
        /// <param name="_caller"></param>
        public abstract void OnSet(DisappearingPlatform _caller);
    }
    
    [System.Serializable]
    private class UntouchedState : DisappearingStates
    {
        [SerializeField]
        private PlatformTypes platformType = PlatformTypes.QuickFall;
        Action<DisappearingPlatform> OnSetting;
        private float durationTillFall = 0f;
        private bool isFalling;

        const float INSTANT_FALL_RESET_TIME = 60f;

        public override void StartStateLogic(DisappearingPlatform _caller)
        {
            _caller.StartCoroutine(RunStateLogic(_caller));
        }

        protected override void SetupLogic(DisappearingPlatform _caller)
        {
            isFalling = true;
            _caller.timer.StartTimer(_caller);
        }

        protected override IEnumerator RunStateLogic(DisappearingPlatform _caller)
        {
            if (!isFalling)
            {
                SetupLogic(_caller);

                Color color = _caller.meshRenderer.material.color;
                while (!_caller.timer.IsTimerDone)
                {
                    color.a = _caller.timer.GetReversedTimeNormalized();
                    _caller.meshRenderer.material.color = color;
                    yield return null;
                }

                _caller.SetPlatformState(_caller.disappearedState);
            }
            yield return null;
        }

        public override void OnSet(DisappearingPlatform _caller)
        {
            isFalling = false;
            _caller.timer.SetTimer(durationTillFall);
            OnSetting?.Invoke(_caller);
        }

        /// <summary>
        /// Logic setting in case of duration of fall being instant.
        /// </summary>
        /// <param name="_caller"></param>
        private void InstantFallSetting(DisappearingPlatform _caller)
        {
            _caller.gameObject.layer = _caller.layerSwapIndex;
            _caller.platformCollider.isTrigger = true;
        }

        /// <summary> 
        /// Logic setting in case of duration of fall not being instant.
        /// </summary>
        /// <param name="_caller"></param>
        private void TimedFallSetting(DisappearingPlatform _caller)
        {
            _caller.platformCollider.isTrigger = false;
        }

        /// <summary>
        /// Sets platform colour depending on duration till fall and setup type
        /// </summary>
        public void SetPlatformType(DisappearingPlatform _caller)
        {
            durationTillFall = (int)platformType;
            _caller.meshRenderer.material.color = GetPlatformTypeColour(platformType);
            if(platformType == PlatformTypes.InstantFall)
            {
                OnSetting = InstantFallSetting;
                _caller.disappearedState.SetDuration(INSTANT_FALL_RESET_TIME);
            }
            else
            {
                OnSetting = TimedFallSetting;
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
                case PlatformTypes.InstantFall:
                    return Color.gray;
                case PlatformTypes.QuickFall:
                    return Color.blue;
                case PlatformTypes.NormalFall:
                    return Color.yellow;
                case PlatformTypes.LongFall:
                    return Color.green;
                default:
                    Debug.LogError("Platform type requested doesnt exist, or has not been set in GetPlatformTypeColour function.");
                    return Color.red;
            }
        }

        [System.Serializable]
        public enum PlatformTypes 
        {
            InstantFall = 0,
            QuickFall = 1,
            NormalFall = 2,
            LongFall = 6,
        }
    }

    [System.Serializable]
    private class DisappearedState : DisappearingStates
    {
        #region Config

        [SerializeField]
        private float durationTillRegen = 1f;

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

        protected override void SetupLogic(DisappearingPlatform _caller)
        {
            _caller.gameObject.layer = _caller.layerSwapIndex;
            isRegenerating = true;
            _caller.timer.StartTimer(_caller);
        }

        protected override IEnumerator RunStateLogic(DisappearingPlatform _caller)
        {
            if (!isRegenerating)
            {
                SetupLogic(_caller);

                Color color = _caller.meshRenderer.material.color;
                while (!_caller.timer.IsTimerDone)
                {
                    // Change opacity based on time left
                    color.a = _caller.timer.GetTimeNormalized();
                    _caller.meshRenderer.material.color = color;
                    yield return null;
                }

                _caller.gameObject.layer = initialLayer;
                _caller.SetPlatformState(_caller.untouchedState);
            }
            else
            {
                // Change is trigger entered to be the opposite of what it currently is. Starting off as false.
                isTriggerEntered = !isTriggerEntered;
                _caller.timer.PauseTimer(isTriggerEntered);
            }
            yield return null;
        }

        /// <summary>
        /// Setup the state and start running logic
        /// </summary>
        /// <param name="_caller"></param>
        public override void OnSet(DisappearingPlatform _caller)
        {
            initialLayer = _caller.gameObject.layer;
            isRegenerating = false;
            _caller.timer.SetTimer(durationTillRegen);

            // In case the object is already a trigger set the isTriggerEntered to avoid instant pausing
            if (!_caller.platformCollider.isTrigger)
            {
                _caller.platformCollider.isTrigger = true;
            }
            else
            {
                isTriggerEntered = true;
            }

            StartStateLogic(_caller);
        }

        public void SetDuration(float _duration)
        {
            if(_duration > 0)
            {
                durationTillRegen = _duration;
            }
        }
    }

}