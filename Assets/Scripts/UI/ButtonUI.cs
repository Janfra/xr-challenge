using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("Dependencies")]
    [SerializeField]
    private Button button;
    [SerializeField]
    private Text text;

    private const float DESELECT_OPACITY = 0.3f;

    private void Awake()
    {
        if(button == null)
        {
            button = GetComponent<Button>();
        }
    }

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        text.color = Color.black;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Color newColour = Color.white;
        newColour.a = DESELECT_OPACITY; 
        text.color = newColour;
    }
    
}
