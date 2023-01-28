using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearOnCollected : PickUpSubscriber
{
    [Header("Config")]
    [SerializeField]
    private GameObject appearingObject;

    [Header("Component")]
    private Timer timer;

    private const float TIME_TO_APPEAR = 0.3f;

    protected override void Init()
    {
        appearingObject.SetActive(false);
        methodRunOnEvent = context => OnAppear();
        base.Init();
        timer = new Timer(TIME_TO_APPEAR);
    }

    private void OnAppear()
    {
        appearingObject.SetActive(true);
        timer.StartTimer(this);
        StartCoroutine(MoveObject());
    }

    private IEnumerator MoveObject()
    {
        Vector3 initialPos = new Vector3(appearingObject.transform.position.x, appearingObject.transform.position.y - 1, appearingObject.transform.position.z);
        Vector3 targetPos = appearingObject.transform.position;

        while (!timer.IsTimerDone)
        {
            transform.position = Vector3.Lerp(initialPos, targetPos, timer.GetTimeNormalized());
            yield return null;
        }
        Debug.Log("Completed");
        yield return null;
    }
}
