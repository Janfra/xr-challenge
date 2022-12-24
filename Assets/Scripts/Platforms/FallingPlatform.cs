using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    #region Components

    private FallingStates currentState;
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
    private FalledState falledState;

    #endregion

    private void Awake()
    {
        timer = new Timer();
        currentState = untouchedState;
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentState.StartStateLogic(this);
    }

    private void OnTriggerStay(Collider other)
    {
        currentState.StartStateLogic(this);
    }

    private abstract class FallingStates
    {
        public abstract void StartStateLogic(FallingPlatform _caller);
        protected abstract IEnumerator RunStateLogic(FallingPlatform _caller);
        protected abstract void Init(FallingPlatform _caller);
        protected abstract void OnComplete(FallingPlatform _caller);
    }
    
    [System.Serializable]
    private class UntouchedState : FallingStates
    {
        [SerializeField]
        private float durationTillFall = 1f;

        private bool isFalling;

        public override void StartStateLogic(FallingPlatform _caller)
        {
            _caller.StartCoroutine(RunStateLogic(_caller));
        }

        protected override void Init(FallingPlatform _caller)
        {
            isFalling = true;
            _caller.timer.SetTimer(durationTillFall);
            _caller.timer.StartTimer(_caller);
        }

        protected override IEnumerator RunStateLogic(FallingPlatform _caller)
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

        protected override void OnComplete(FallingPlatform _caller)
        {
            _caller.platformCollider.isTrigger = true;
            _caller.currentState = _caller.falledState;
            _caller.currentState.StartStateLogic(_caller);
            isFalling = false;
        }

        /// <summary>
        /// Sets platform colour depending on duration till fall
        /// </summary>
        public void SetPlatformType()
        {

        }
    }

    [System.Serializable]
    private class FalledState : FallingStates
    {
        #region Config

        [SerializeField]
        private float durationTillRegen = 1f;
        [SerializeField]
        private LayerMask layerSwap;

        #endregion

        #region Variables & Constants

        private int initialLayer;
        private bool isRegenerating;
        private const float TIMER_EXTENSION = 0.1f;
  
        #endregion

        public override void StartStateLogic(FallingPlatform _caller)
        {
            _caller.StartCoroutine(RunStateLogic(_caller));
        }

        protected override void Init(FallingPlatform _caller)
        {
            initialLayer = _caller.gameObject.layer;
            _caller.gameObject.layer = layerSwap.value;
            isRegenerating = true;
            _caller.timer.SetTimer(durationTillRegen);
            _caller.timer.StartTimer(_caller);
        }

        protected override IEnumerator RunStateLogic(FallingPlatform _caller)
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
                _caller.timer.SetTimer(TIMER_EXTENSION + _caller.timer.CurrentTime, false);
            }
            yield return null;
        }

        protected override void OnComplete(FallingPlatform _caller)
        {
            _caller.platformCollider.isTrigger = false;
            _caller.gameObject.layer = initialLayer;
            _caller.currentState = _caller.untouchedState;
            isRegenerating = false;
        }
    }

}