using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using common;

public class MatchResultUIManager : PanelController
{
    [SerializeField] private Image resultImage;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text matchCompleteText;
    [SerializeField] private Sprite victorySprite;
    [SerializeField] private Sprite defeatSprite;
    [SerializeField] private NamedButton[] Buttons;
    private Dictionary<string, Button> _buttonDict = new Dictionary<string, Button>();

    public delegate void OnRematchButtonClicked();
    private OnRematchButtonClicked _onRematchButtonClicked;
    private bool _isMinimized = false;

    protected override void Awake()
    {
        base.Awake();

        // 배열을 딕셔너리로 변환
        foreach (var item in Buttons)
        {
            _buttonDict[item.buttonName] = item.button;
        }

        // 이름으로 찾아 등록 (순서 무관!)
        _buttonDict["Rematch"].onClick.AddListener(OnClickRematchButton);
        _buttonDict["ShowBoard"].onClick.AddListener(Minimize);
        _buttonDict["ScreenTouchButton"].onClick.AddListener(Restore);
    }

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

    public void Minimize()
    {
        if (_isMinimized) return;
        _isMinimized = true;

        // 내용물만 슥 아래로 내리거나 작게 만듦
        panelTransform.DOScale(0.5f, 0.3f).SetEase(Ease.InBack);
        panelTransform.DOAnchorPos(new Vector2(0, -1000), 0.5f).SetEase(Ease.InBack).OnComplete(() => {
            // 보드가 잘 보이도록 내용물을 완전히 끔 (또는 투명도 조절)
            panelTransform.gameObject.SetActive(false);
            // 화면 어디든 누르면 복구되도록 터치 버튼 활성화
            _buttonDict["ScreenTouchButton"].gameObject.SetActive(true);
        });
    }

    public void Restore()
    {
        if (!_isMinimized) return;

        panelTransform.gameObject.SetActive(true);
        _buttonDict["ScreenTouchButton"].gameObject.SetActive(false);

        // 원래 위치와 크기로 복구
        panelTransform.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutBack);
        panelTransform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).OnComplete(() => {
            _isMinimized = false;
        });
    }
}
