using UnityEngine;
using static common.Constants;

public class GameTypePanelController : PanelController
{
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
