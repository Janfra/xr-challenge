using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    #region Dependencies

    [Header("Dependencies")]
    [SerializeField]
    private Button button;
    [SerializeField]
    private Text text;
    [SerializeField]
    private RectTransform selectionPointer;

    #endregion

    #region Config

    [Header("Config")]
    [SerializeField]
    private Color deselectTextColour;
    [SerializeField]
    private Color selectTextColour;

    #endregion

    private const float DESELECT_OPACITY = 0.3f;

    private void Awake()
    {
        if(button == null)
        {
            button = GetComponent<Button>();
            Debug.LogError($"No button set in {gameObject.name}!");
        }
        if(text == null)
        {
            text = GetComponentInChildren<Text>();
            Debug.LogError($"No text set in {gameObject.name}!");
        }
        if(selectionPointer == null)
        {
            selectionPointer = GetComponentInChildren<RectTransform>();
            Debug.LogError($"No selection pointer set in {gameObject.name}!");
        }

        SetDeselectTextColour();
        SelectTextColourClamp();
        SetPointerMiddleLeftAnchor();
    }

    /// <summary>
    /// Select this button on cursor enter
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        button.Select();
    }

    /// <summary>
    /// Change text colour and points at button on select
    /// </summary>
    /// <param name="eventData"></param>
    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        text.color = selectTextColour;
        SetPointerPosition(true);
    }

    /// <summary>
    /// Change text colour and disable pointer on deselect
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDeselect(BaseEventData eventData)
    {
        SetDeselectTextColour();
        SetPointerPosition(false);
    }

    /// <summary>
    /// Sets the text colour to the deselected colour.
    /// </summary>
    private void SetDeselectTextColour()
    {
        Color newColour = deselectTextColour;
        newColour.a = DESELECT_OPACITY;
        text.color = newColour;
    }

    /// <summary>
    /// Clamps the selected text colour to be fully visible at start.
    /// </summary>
    private void SelectTextColourClamp()
    {
        Color opacityClamp = selectTextColour;
        opacityClamp.a = 1;
        selectTextColour = opacityClamp;
    }

    /// <summary>
    /// Sets the pointer position to be on the left side of the button.
    /// </summary>
    /// <param name="_isActive"></param>
    private void SetPointerPosition(bool _isActive)
    {
        selectionPointer.gameObject.SetActive(_isActive);

        if (_isActive)
        {
            float xPosOffset = -(selectionPointer.rect.width + selectionPointer.rect.width / 3);
            Vector3 positionNextToButton = new Vector3(xPosOffset, 0);
            selectionPointer.anchoredPosition = positionNextToButton;
        }
        else
        {
            selectionPointer.anchoredPosition = Vector3.zero;
        }
    }

    /// <summary>
    /// Sets the pointer anchor settings to be to the middle left.
    /// </summary>
    private void SetPointerMiddleLeftAnchor()
    {
        selectionPointer.anchorMin = new Vector2(0f, 0.5f);
        selectionPointer.anchorMax = new Vector2(0f, 0.5f);
        selectionPointer.pivot = new Vector2(0f, 0.5f);
        selectionPointer.anchoredPosition = Vector3.zero;
        selectionPointer.gameObject.SetActive(false);   
    }
}
