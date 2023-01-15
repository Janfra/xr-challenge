using System.Collections;
using TMPro;
using UnityEngine;

public class OnScreenMessage : MonoBehaviour
{
    #region Dependencies

    [Header("Dependencies")]
    [SerializeField]
    private GameObject container;
    public GameObject Container => container;

    [SerializeField]
    private TextMeshProUGUI onScreenText;
    public TextMeshProUGUI OnScreenText => onScreenText;

    [SerializeField]
    private Animator onScreenAnimations;

    #endregion

    #region Variables & Constants

    private string displayText;
    private bool isCancelled = false;
    private bool isLoading = false;
    public bool IsLoading => isLoading;

    private bool isVisible;
    private bool isDelayed = true;
    private const float TEXT_LOADING_DELAY = 0.05f;

    #endregion

    private void Awake()
    {
        OnScreenMessagesHandler.SetDependencies(this);
    }

    /// <summary>
    /// Starts loading the text given if a text is not already loading.
    /// </summary>
    /// <param name="_text">Text to load</param>
    public void SetText(string _text)
    {
        if (!isLoading)
        {
            displayText = _text;
            StartCoroutine(LoadText());
        }
    }

    /// <summary>
    /// Loads the stored text letter by letter based on the TEXT_LOADING_DELAY.
    /// </summary>
    private IEnumerator LoadText()
    {
        isLoading = true;
        isCancelled = false;
        string tempString = "";
        onScreenText.text = "";

        for (int i = 0; i < displayText.Length && isLoading; i++)
        {
            if (isDelayed)
            {
                i--;
                yield return null;
                continue;
            }

            if (!isCancelled)
            {
                tempString += displayText[i];
                yield return new WaitForSeconds(TEXT_LOADING_DELAY);
                onScreenText.text = tempString;
            }
            else
            {
                // If cancelled instantly load text
                onScreenText.text = displayText;
                yield return new WaitForSeconds(TEXT_LOADING_DELAY);
                break;
            }
        }
        isLoading = false;
    }

    /// <summary>
    /// Cancels text loading and instantly displays text.
    /// </summary>
    public void CancelLoading()
    {
        isCancelled = true;
    }

    /// <summary>
    /// Sets the canvas visibility, which starts the animations
    /// </summary>
    /// <param name="_isVisible">Is the canvas visible</param>
    public void SetContainerVisible(bool _isVisible)
    {
        onScreenAnimations.enabled = true;
        onScreenAnimations.SetBool("IsShowing", _isVisible);
        isVisible = _isVisible;
    }

    /// <summary>
    /// Sets when to start or stop the delay set to align text with animation.
    /// </summary>
    private void AnimationSetDelay()
    {
        // Set with the visible state to avoid undesired behaviour when interrupted.
        isDelayed = !isVisible;
        if (isDelayed)
        {
            isLoading = false;
        }
    }

    /// <summary>
    /// Disables animation and checks that delay was properly set by animation.
    /// </summary>
    private void OnAnimationFinished()
    {
        onScreenAnimations.enabled = false;
        AnimationSetDelay();
    }
}

public static class OnScreenMessagesHandler 
{
    #region Dependencies

    private static OnScreenMessage onScreenMessage;

    #endregion

    #region Components

    private static Timer timer;

    #endregion

    #region Variables & Constants

    private const float SCREENMESSAGE_DURATION = 2f;
    public static string OnScreenMessage => onScreenMessage.OnScreenText.text;
    public static float TimerDuration => timer.CurrentTime;

    #endregion

    /// <summary>
    /// Sets the dependencies required for the static class to work
    /// </summary>
    /// <param name="_onScreenMessage">Class containing the dependencies</param>
    public static void SetDependencies(OnScreenMessage _onScreenMessage)
    {
        onScreenMessage = _onScreenMessage;
        timer = new Timer();
    }

    /// <summary>
    /// Sets the screen message text and starts displaying it with a timeout.
    /// </summary>
    /// <param name="_newText">Text to display</param>
    /// <param name="isTimeOut">Is timeout started</param>
    public static void SetScreenMessage(string _newText, bool isTimeOut)
    {
        if(AreDependenciesMissing())
        {
            return;
        }

        if(_newText.Length > 0 && isTimeOut)
        {
            onScreenMessage.StartCoroutine(StartOnScreenTimeOut(onScreenMessage));
            onScreenMessage.SetText(_newText);
        }
        else
        {
            Debug.Log("For setting message with no timeout just use overload with no boolean parameter!");
            SetScreenMessage(_newText);
        }
    }

    /// <summary>
    /// Sets the screen message text and starts displaying it.
    /// </summary>
    /// <param name="_newText">Text to display</param>
    public static void SetScreenMessage(string _newText)
    {
        if(AreDependenciesMissing())
        {
            return;
        }

        if(_newText.Length > 0)
        {
            onScreenMessage.SetContainerVisible(true);
            onScreenMessage.SetText(_newText);
        }
    }

    /// <summary>
    /// Disables the text on screen
    /// </summary>
    public static void DisableScreenMessage()
    {
        onScreenMessage.SetContainerVisible(false);
    }

    /// <summary>
    /// Returns if the text on screen is loading
    /// </summary>
    /// <returns>If the text finished loading return false, otherwise true</returns>
    public static bool IsTextLoading()
    {
        return onScreenMessage.IsLoading;
    }

    /// <summary>
    /// Stops the loading and instantly displays the text
    /// </summary>
    public static void CancelLoading()
    {
        onScreenMessage.CancelLoading();
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
        if(timer.IsTimerDone)
        {
            onScreenMessage.SetContainerVisible(true);
            timer.SetTimer(SCREENMESSAGE_DURATION);
            timer.StartTimer(_caller);
            while(!timer.IsTimerDone)
            {
                yield return null;
            }
            if(!AreDependenciesMissing())
            {
                onScreenMessage.SetContainerVisible(false);
            }
        }
        yield return null;
    }

    /// <summary>
    /// Checks if the dependencies are set
    /// </summary>
    /// <returns>Is a dependency missing</returns>
    private static bool AreDependenciesMissing()
    {
        if(onScreenMessage == null)
        {
            Debug.LogError("On screen message has not been set in the OnScreenMessagesHandler!");
            return true;
        }
        return false;
    }
}
