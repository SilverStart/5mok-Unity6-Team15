using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class MatchResultUIManager : MonoBehaviour
{
    [SerializeField] private Transform dim;
    [SerializeField] private Image resultImage;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text matchCompleteText;
    [SerializeField] private Sprite victorySprite;
    [SerializeField] private Sprite defeatSprite;

    public event Action OnClickRematch;

    public void Victory()
    {
        dim.gameObject.SetActive(true);
        resultImage.sprite = victorySprite;
        resultText.text = "Victory";
        resultText.color = Color.white;
        matchCompleteText.text = "Match Complete";
        if (ColorUtility.TryParseHtmlString("#13EC5B", out Color victoryColor))
        {
            matchCompleteText.color = victoryColor;
        }
    }

    public void Defeat()
    {
        dim.gameObject.SetActive(true);
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
        OnClickRematch?.Invoke();
        dim.gameObject.SetActive(false);
    }
}
