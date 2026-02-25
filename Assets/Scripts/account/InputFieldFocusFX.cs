using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputFieldFocusFX : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("Components")]
    public Image iconImage;
    public Image backgroundImage;
    [Header("Colors")]
    public Color normalColor = new Color(0.3f, 0.3f, 0.3f);
    public Color activeColor = new Color(0.4f, 1.0f, 0.5f);
    private void Start()
    {
        SetColors(normalColor);
    }

    public void OnSelect(BaseEventData eventData)
    {
        SetColors(activeColor);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetColors(normalColor);
    }

    private void SetColors(Color color)
    {
        if (iconImage != null) iconImage.color = color;
        if (backgroundImage != null) backgroundImage.color = color;
    }
}