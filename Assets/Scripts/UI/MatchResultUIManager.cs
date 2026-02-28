using System;
using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MatchResultUIManager : PanelController
{
    [SerializeField] private Image resultImage;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text matchCompleteText;
    [SerializeField] private Sprite victorySprite;
    [SerializeField] private Sprite defeatSprite;

    public delegate void OnRematchButtonClicked();
    private OnRematchButtonClicked _onRematchButtonClicked;

    public void Show(string resultStr, OnRematchButtonClicked onRematchButtonClicked = null)
    {
        _onRematchButtonClicked = onRematchButtonClicked;
        resultImage.sprite = victorySprite;
        resultText.text = resultStr;
        resultText.color = Color.white;
        matchCompleteText.text = "Match Complete";
        if (ColorUtility.TryParseHtmlString("#13EC5B", out Color victoryColor))
        {
            matchCompleteText.color = victoryColor;
        }
        Show();
    }

    public void Defeat()
    {
        resultImage.sprite = defeatSprite;
        resultText.text = "Defeat";
        if (ColorUtility.TryParseHtmlString("#ef4444d5", out Color defeatColor))
        {
            resultText.color = defeatColor;
        }
        matchCompleteText.text = "Match Complete";
        if (ColorUtility.TryParseHtmlString("#ffffff9a", out Color completeColor))
        {
            matchCompleteText.color = completeColor;
        }
    }

    public void OnClickRematchButton()
    {
        Hide(() =>
        {
            _onRematchButtonClicked?.Invoke();
        });
    }
}
