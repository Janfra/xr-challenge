using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDialogue : Dialogue
{
    [Header("Config")]
    [SerializeField]
    Transform teleportPosition;

    Transform playerTransform;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController playerController))
        {
            playerTransform = playerController.transform;
        }
        OnColliderStartDialogue(other);
    }

    protected override IEnumerator StartDialogue()
    {
        StartOnDialogueEvent(false);

        OnScreenMessagesHandler.SetScreenMessage(dialogueText[currentDialogue]);
        while (currentDialogue != dialogueText.Length || OnScreenMessagesHandler.IsTextLoading())
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
            else if (nextInput && OnScreenMessagesHandler.IsTextLoading())
            {
                OnScreenMessagesHandler.CancelLoading();
            }
            yield return null;
        }

        playerTransform.position = teleportPosition.position;
        yield return null;
        OnScreenMessagesHandler.DisableScreenMessage();
        StartOnDialogueEvent(true);
    }
}
