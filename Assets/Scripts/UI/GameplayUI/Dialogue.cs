using System;
using System.Collections;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    protected AudioManager audioHandler;
    public static event Action<bool> OnDialogue;

    #region Config

    [Header("Config")]
    [SerializeField]
    protected string[] dialogueText;
    [SerializeField]
    private GameObject dialogueAlert;

    #endregion

    #region Variables & Constants

    protected bool nextInput = false;
    protected int currentDialogue = 0;
    private const float ALERT_Y_MOVEMENT = 0.3f;

    #endregion

    private void Start()
    {
        if (dialogueAlert != null && !dialogueAlert.activeSelf)
        {
            dialogueAlert.AddComponent<DialogueMoveOnEnable>();
        }
        audioHandler = AudioManager.Instance;
    }

    private void OnEnable()
    {
        if (dialogueAlert != null)
        {
            StartCoroutine(MoveAlert());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        OnColliderStartDialogue(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController input))
        {
            RemoveInputEvent(input.ActionInputs);
        }
    }

    protected void OnColliderStartDialogue(Collider _other)
    {
        if (_other.TryGetComponent(out PlayerController input))
        {
            AddInputEvent(input.ActionInputs);
            if (currentDialogue != dialogueText.Length)
            {
                if (dialogueAlert)
                {
                    dialogueAlert.SetActive(false);
                }
                StartCoroutine(StartDialogue());
            }
        }
    }

    /// <summary>
    /// Starts dialogue sequence
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator StartDialogue()
    {
        OnDialogue?.Invoke(false);

        OnScreenMessagesHandler.SetScreenMessage(dialogueText[currentDialogue]);
        while(currentDialogue != dialogueText.Length || OnScreenMessagesHandler.IsTextLoading())
        {
            if (nextInput && !OnScreenMessagesHandler.IsTextLoading())
            {
                currentDialogue += 1;
                if (currentDialogue != dialogueText.Length)
                {
                    audioHandler.TryPlayAudio("NextDialogue");
                    OnScreenMessagesHandler.SetScreenMessage(dialogueText[Mathf.Clamp(currentDialogue, 0, dialogueText.Length - 1)]);
                    
                    // Cancels next input, otherwise stops loading permanently
                    nextInput = false;
                }
            }
            else if(nextInput && OnScreenMessagesHandler.IsTextLoading())
            {
                OnScreenMessagesHandler.CancelLoading();
            }
            yield return null;
        }

        yield return null;
        OnScreenMessagesHandler.DisableScreenMessage();
        OnDialogue?.Invoke(true);
    }

    /// <summary>
    /// Moves the alert game object up and down
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveAlert()
    {
        Vector3 firstPos = dialogueAlert.transform.position;
        Vector3 secondPos = firstPos;
        secondPos.y += ALERT_Y_MOVEMENT;

        float time = 0;
        bool switchLerp = false;
        while (dialogueAlert.activeSelf)
        {
            if(!switchLerp)
            {
                if(dialogueAlert.transform.position == secondPos)
                    switchLerp = true;
                time += Time.deltaTime;
            }
            else 
            {
                if (dialogueAlert.transform.position == firstPos)
                    switchLerp = false;
                time -= Time.deltaTime;
            }
            dialogueAlert.transform.position = Vector3.Lerp(firstPos, secondPos, Mathf.Clamp01(time));
            yield return null;
        }
    }

    /// <summary>
    /// Runs the On Dialogue event
    /// </summary>
    /// <param name="_isFinished">True if dialogue is finished, otherwise false</param>
    protected void StartOnDialogueEvent(bool _isFinished)
    {
        OnDialogue?.Invoke(_isFinished);
    }

    /// <summary>
    /// Subscribes to the player input to set the next input to the interact input
    /// </summary>
    /// <param name="_input"></param>
    public void AddInputEvent(PlayerInputs _input)
    {
        Debug.Log("Subscribed to player input");
        _input.Player.Interact.started += context =>
        {
            nextInput = true;
        };

        _input.Player.Interact.canceled += context =>
        {
            nextInput = false;
        };
    }

    /// <summary>
    /// Unsubscribes to the player input setting the next input
    /// </summary>
    /// <param name="_input"></param>
    public void RemoveInputEvent(PlayerInputs _input)
    {
        Debug.Log("Unsubscribed to player input");
        _input.Player.Interact.started -= context =>
        {
            nextInput = true;
        };

        _input.Player.Interact.canceled -= context =>
        {
            nextInput = false;
        };
        nextInput = false;
    }

    private class DialogueMoveOnEnable : MonoBehaviour
    {
        [SerializeField]
        private Dialogue parentDialogue;

        private void OnEnable()
        {
            if (parentDialogue)
            {
                parentDialogue.MoveAlert();
            }
            else
            {
                Debug.LogError($"No parent set on {gameObject.name}");
                if(parentDialogue = GetComponentInParent<Dialogue>())
                {
                    parentDialogue.MoveAlert();
                }
            }
        }
    }
}
