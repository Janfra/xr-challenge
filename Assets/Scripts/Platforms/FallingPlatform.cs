using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    private FallingStates currentState;
    private Timer timer;

    [Header("Dependencies")]
    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private Collider platformCollider;

    [Header("Config")]
    [SerializeField]
    private UntouchedState untouchedState;
    [SerializeField]
    private FalledState falledState;

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

        /// <summary>
        /// Normalizes 'alpha' with the total duration. Modified to work with values under 0. Havent tested with negatives.
        /// </summary>
        /// <param name="alpha">Current value to normalize</param>
        /// <param name="duration">Max value of the normalize formula</param>
        /// <returns>A value in between 0 and 1, the duration being 1</returns>
        protected float FractionNormalized(float alpha, float duration)
        {
            // 1 is added to everything to avoid dividing under 0 and getting unexpected values
            int minValue = 1;
            alpha += minValue;
            duration += minValue;
            return (alpha - minValue) / (duration - minValue);
        }
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
                    float alpha = FractionNormalized(_caller.timer.TotalTime - _caller.timer.CurrentTime, _caller.timer.TotalTime);
                    color.a = alpha;
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
    }

    [System.Serializable]
    private class FalledState : FallingStates
    {
        [SerializeField]
        private float durationTillRegen = 1f;
        [SerializeField]
        private LayerMask layerSwap;

        private int initialLayer;
        private bool isRegenerating;
        private const float TIMER_EXTENSION = 0.1f;

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
                    float alpha = FractionNormalized(_caller.timer.CurrentTime, _caller.timer.TotalTime);
                    color.a = alpha;
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