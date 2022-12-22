using System.Collections;
using TMPro;
using UnityEngine;

public class OnScreenMessage : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    private Canvas canvas;
    public Canvas Canvas => canvas;

    [SerializeField]
    private TextMeshProUGUI onScreenText;
    public TextMeshProUGUI OnScreenText => onScreenText;

    private void Awake()
    {
        OnScreenMessagesHandler.SetDependencies(this);
    }
}

public static class OnScreenMessagesHandler 
{
    private static Canvas canvas;
    private static TextMeshProUGUI onScreenText;
    private static Timer timer;
    private const float SCREENMESSAGE_DURATION = 2f;
    public static string OnScreenMessage => onScreenText.text;
    public static float TimerDuration => timer.CurrentTime;

    public static void SetDependencies(OnScreenMessage _onScreenMessage)
    {
        canvas = _onScreenMessage.Canvas;
        onScreenText = _onScreenMessage.OnScreenText;
        timer = new Timer();

        Canvas canvasCheck = _onScreenMessage.OnScreenText.GetComponentInParent<Canvas>();
        Debug.LogWarning($"{canvasCheck.name} was found on text given");
    }

    public static void SetScreenMessage<T>(string _newText, T _caller) where T : MonoBehaviour
    {
        if (AreDependenciesMissing())
        {
            return;
        }

        if(_newText.Length > 0)
        {
            _caller.StartCoroutine(StartOnScreenTimeOut(_caller));
            onScreenText.text = _newText;
        }
    }

    public static void SetScreenMessage(string _newText)
    {
        if (AreDependenciesMissing())
        {
            return;
        }

        if (_newText.Length > 0)
        {
            canvas.gameObject.SetActive(true);
            onScreenText.text = _newText;
        }
    }

    public static void DisableScreenMessage()
    {
        canvas.gameObject.SetActive(false);
    }

    private static IEnumerator StartOnScreenTimeOut<T>(T _caller) where T : MonoBehaviour
    {
        Debug.Log("Time out started");
        if (timer.IsTimerDone)
        {
            canvas.gameObject.SetActive(true);
            timer.SetTimer(SCREENMESSAGE_DURATION);
            timer.StartTimer(_caller);
            while (!timer.IsTimerDone)
            {
                yield return null;
            }
            if (!AreDependenciesMissing())
            {
                canvas.gameObject.SetActive(false);
            }
        }
        yield return null;
    }

    private static void ExtendTimer()
    {
        timer.SetTimer(SCREENMESSAGE_DURATION + timer.CurrentTime, false);
    }

    private static bool AreDependenciesMissing()
    {
        if (onScreenText == null || canvas == null)
        {
            Debug.LogError("On screen text has not been set in the OnScreenMessagesHandler!");
            return true;
        }
        return false;
    }
}
