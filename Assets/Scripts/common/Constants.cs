using UnityEngine.UI;

namespace common
{
    public static partial class Constants
    {
        public enum StoneColor
        {
            Black,
            White,
            None,
        }

        public enum GameType
        {
            SinglePlayBlack,
            SinglePlayWhite,
            DualPlay,
        }

        // 게임의 결과
        public enum GameResult { None, BlackWin, WhiteWin, Draw };

        public const int BOARD_SIZE = 15;
    }

    [System.Serializable]
    public class NamedButton
    {
        public string buttonName;
        public Button button;
    }
}