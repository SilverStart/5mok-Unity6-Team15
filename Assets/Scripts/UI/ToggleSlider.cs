using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ToggleSlider : MonoBehaviour
{
    private RectTransform rectTransform;

    [SerializeField] private Image fill;
    [SerializeField] private Image handle;

    [SerializeField] private Color onColor;
    [SerializeField] private Color offColor;

    private bool isOn;

    public bool IsOn => isOn;

    private Action onClickAction;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        GetComponent<Button>().onClick.AddListener(() => {
            OnClickToggle(!isOn);
        });
    }

    public void Setup(Action onClick)
    {
        onClickAction = onClick;
    }

    public void OnClickToggle(bool isToggle)
    {
        if (isOn == isToggle)
            return;

        isOn = isToggle;

        fill.color = isOn ? onColor : offColor;

        float anchorX = isOn ? 1 : 0;

        handle.GetComponent<RectTransform>().anchorMax = new Vector2(anchorX, 0.5f);
        handle.GetComponent<RectTransform>().anchorMin = new Vector2(anchorX, 0.5f);
        handle.GetComponent<RectTransform>().pivot = new Vector2(anchorX, 0.5f);

        handle.GetComponent<RectTransform>().DOAnchorPosX(0, 0.8f);

        onClickAction?.Invoke();
    }

    public void Toggle(bool isToggle)
    {
        this.isOn = isToggle;

        fill.color = isOn ? onColor : offColor;

        float anchorX = isOn ? 1 : 0;

        handle.GetComponent<RectTransform>().anchorMax = new Vector2(anchorX, 0.5f);
        handle.GetComponent<RectTransform>().anchorMin = new Vector2(anchorX, 0.5f);
        handle.GetComponent<RectTransform>().pivot = new Vector2(anchorX, 0.5f);

        handle.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }
}
