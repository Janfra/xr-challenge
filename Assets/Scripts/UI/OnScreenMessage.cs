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
    #region Dependencies

    private static Canvas canvas;
    private static TextMeshProUGUI onScreenText;

    #endregion

    #region Components

    private static Timer timer;

    #endregion

    #region Variables & Constants

    private const float SCREENMESSAGE_DURATION = 2f;
    public static string OnScreenMessage => onScreenText.text;
    public static float TimerDuration => timer.CurrentTime;

    #endregion

    /// <summary>
    /// Sets the dependencies required for the static class to work
    /// </summary>
    /// <param name="_onScreenMessage">Class containing the dependencies</param>
    public static void SetDependencies(OnScreenMessage _onScreenMessage)
    {
        canvas = _onScreenMessage.Canvas;
        onScreenText = _onScreenMessage.OnScreenText;
        timer = new Timer();
    }

    /// <summary>
    /// Sets the screen message text and starts displaying it with a timeout.
    /// </summary>
    /// <typeparam name="T">Requires Monobehaviour</typeparam>
    /// <param name="_newText">Text to display</param>
    /// <param name="_caller">Object requesting to set the message</param>
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

    /// <summary>
    /// Sets the screen message text and starts displaying it.
    /// </summary>
    /// <param name="_newText">Text to display</param>
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

    /// <summary>
    /// Disables the text on screen
    /// </summary>
    public static void DisableScreenMessage()
    {
        canvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// Starts a timer than when done turns the message off
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_caller"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Checks if the dependencies are set
    /// </summary>
    /// <returns>Is a dependency missing</returns>
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
