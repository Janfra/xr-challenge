using System;
using System.Collections;
using UnityEngine;

public class Dialogues : MonoBehaviour
{
    public static event Action<bool> OnDialogue;

    #region Config

    [Header("Config")]
    [SerializeField]
    private string[] dialogueText;
    [SerializeField]
    private GameObject dialogueAlert;

    #endregion

    #region Variables & Constants

    private bool nextInput => Input.GetKeyDown(KeyCode.E);
    private int currentDialogue = 0;
    private const float ALERT_Y_MOVEMENT = 0.3f;

    #endregion

    private void Start()
    {
        if (dialogueAlert != null)
        {
            StartCoroutine(MoveAlert());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(currentDialogue != dialogueText.Length)
        {
            dialogueAlert.SetActive(false);
            StartCoroutine(StartDialogue());
        }
    }

    /// <summary>
    /// Starts dialogue sequence
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartDialogue()
    {
        OnDialogue?.Invoke(true);

        OnScreenMessagesHandler.SetScreenMessage(dialogueText[currentDialogue]);
        while(currentDialogue != dialogueText.Length || OnScreenMessagesHandler.IsTextLoading())
        {
            if (nextInput && !OnScreenMessagesHandler.IsTextLoading())
            {
                currentDialogue += 1;
                if (currentDialogue != dialogueText.Length)
                {
                    OnScreenMessagesHandler.SetScreenMessage(dialogueText[Mathf.Clamp(currentDialogue, 0, dialogueText.Length - 1)]);
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
        OnDialogue?.Invoke(false);
    }

    /// <summary>
    /// Moves the alert game object up and down.
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
        Debug.Log("Finished");
    }
}
