using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAreaComplete : AreaComplete
{
    [Header("Dependencies")]
    [SerializeField]
    private GameObject stairs;


    private bool isNextAreaGenerated = false;
    private Timer timer;
    private const float TRANSITION_TIME = 2.5f;
    private const float STAIRS_Y_POS = 1.25f;
    private const float PLATFORMS_Y_POS = -13.2f;

    protected override void Init()
    {
        base.Init();
        timer = new Timer(TRANSITION_TIME);
    }

    protected override void OnAreaComplete(Pickup _pickup)
    {
        Debug.Log("Second area enabled!");
        base.OnAreaComplete(_pickup);
        GenerateNextArea();
        MoveObjects();
    }

    private void GenerateNextArea()
    {
        isNextAreaGenerated = true;
        // Spawn next area
    }

    private void MoveObjects()
    {
        timer.StartTimer(this);
        StartCoroutine(MoveObject(transform, GetYPosition(transform, PLATFORMS_Y_POS)));
        StartCoroutine(MoveObject(stairs.transform, GetYPosition(stairs.transform, STAIRS_Y_POS)));
    }

    private IEnumerator MoveObject(Transform _transform, Vector3 _targetPos)
    {
        Vector3 initialPos = _transform.position;

        while (!timer.IsTimerDone)
        {
            _transform.position = Vector3.Lerp(initialPos, _targetPos, timer.GetTimeNormalized());
            yield return null;
        }
        Debug.Log("Completed");
        yield return null;
    }

    private Vector3 GetYPosition(Transform _transform, float _yPos)
    {
        return new Vector3(_transform.position.x, _yPos, _transform.position.z);
    }
}
