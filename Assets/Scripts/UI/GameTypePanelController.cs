using static common.Constants;
using common;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameTypePanelController : PanelController
{
    [SerializeField] private NamedButton[] Buttons;
    private Dictionary<string, Button> _buttonDict = new Dictionary<string, Button>();

    protected override void Awake()
    {
        base.Awake();

        // 배열을 딕셔너리로 변환
        foreach (var item in Buttons)
        {
            _buttonDict[item.buttonName] = item.button;
        }

        // 이름으로 찾아 등록 (순서 무관!)
        _buttonDict["SingleBlack"].onClick.AddListener(OnSingleBlackClick);
        _buttonDict["SingleWhite"].onClick.AddListener(OnSingleWhiteClick);
        _buttonDict["DualPlay"].onClick.AddListener(OnDualPlayClick);
    }

    public void OnSingleBlackClick()
    {
        Hide(() =>
        {
            GameManager.Instance.OnGameStartClick(GameType.SinglePlayBlack);
        });
    }

    public void OnSingleWhiteClick()
    {
        Hide(() =>
        {
            GameManager.Instance.OnGameStartClick(GameType.SinglePlayWhite);
        });
    }

    public void OnDualPlayClick()
    {
        Hide(() =>
        {
            GameManager.Instance.OnGameStartClick(GameType.DualPlay);
        });
    }
}
