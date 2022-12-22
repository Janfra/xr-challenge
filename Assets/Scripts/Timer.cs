using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Timer
{
    #region Variables

    [Header("Config")]
    [SerializeField] private float targetTime = 1;
    public float TotalTime { get { return targetTime; } }
    public float CurrentTime { get; private set; }
    public bool IsTimerDone { get; private set; }

    #endregion

    /// <summary>
    /// Constructor 
    /// </summary>
    public Timer()
    {
        targetTime = 1f;
        CurrentTime = 0f;
        IsTimerDone = true;
    }

    /// <summary>
    /// Constructor 
    /// </summary>
    /// <param name="_timerTargetTime">Sets the timer duration</param>
    public Timer(float _timerTargetTime)
    {
        SetTargetTime(_timerTargetTime);
        SetCurrentTime(0f);
        IsTimerDone = true;
    }

    /// <summary>
    /// Sets timer target time and cancels old timer.
    /// </summary>
    /// <param name="_timerTargetTime">New timer target time</param>
    public void SetTimer(float _timerTargetTime)
    {
        CancelTimer();
        SetTargetTime(_timerTargetTime);
        SetCurrentTime(0f);
    }

    /// <summary>
    /// Sets timer target times and decides whether to cancel old timer
    /// </summary>
    /// <param name="_timerTargetTime">New timer target time / duration</param>
    /// <param name="_cancelCurrentTimer">Cancel the old timer</param>
    public void SetTimer(float _timerTargetTime, bool _cancelCurrentTimer)
    {
        if(_cancelCurrentTimer)
        {
            CancelTimer();
            SetCurrentTime(0f);
        }
        else if(_timerTargetTime < CurrentTime)
        {
            CancelTimer();
            SetCurrentTime(_timerTargetTime);
            Debug.LogWarning("The new timer total time is lower than the current time, timer has been canceled!");
        }
        SetTargetTime(_timerTargetTime);
    }

    /// <summary>
    /// Sets time target time
    /// </summary>
    /// <param name="_TargetTime"></param>
    private void SetTargetTime(float _TargetTime)
    {
        if(_TargetTime > 0)
        {
            targetTime = _TargetTime;
        }
        else
        {
            Debug.LogWarning("Timer cannot be set under 0! Defaulted to 1");
            targetTime = 1;
        }
    }

    /// <summary>
    /// Sets timer current time
    /// </summary>
    /// <param name="_currentTime"></param>
    private void SetCurrentTime(float _currentTime)
    {
        if(_currentTime <= targetTime)
        {
            CurrentTime = _currentTime;
        }
        else
        {
            CurrentTime = targetTime;
            // Debug.LogWarning($"Tried to set current time over total time! Value: {newCurrentTime}");
        }
    }

    /// <summary>
    /// Starts a coroutine for the timer.
    /// </summary>
    /// <typeparam name="T">Generic Monobehaviour to support running the coroutine</typeparam>
    /// <param name="_caller">Timer owner</param>
    public void StartTimer<T>(T _caller) where T : MonoBehaviour 
    {
        if(IsTimerDone == true)
        {
            IsTimerDone = false;
            _caller.StartCoroutine(RunTimer());
        }
        else
        {
            Debug.Log("Timer is not done!");
        }
    }

    /// <summary>
    /// Stops timer
    /// </summary>
    public void CancelTimer()
    {
        IsTimerDone = true;
    }

    /// <summary>
    /// Updates current time until target is reached
    /// </summary>
    /// <returns></returns>
    private IEnumerator RunTimer()
    {
        while (CurrentTime < targetTime && !IsTimerDone)
        {
            SetCurrentTime(CurrentTime + Time.deltaTime);
            yield return null;
        }
        IsTimerDone = true;
        SetCurrentTime(0f);
        // Debug.Log($"Timer has finished! Can be started again. Time runned: {totalTime}");
        yield return null;
    }
}
