using static common.Constants;
using static BoardConfig;

public static class BoardUtils
{
    // 1차원 좌표를 2차원으로 변환
    public static int GetX(byte p) => p & 0x0F;
    public static int GetY(byte p) => p >> 4;

    // 2차원 좌표를 1차원으로 변환
    public static byte GetP(int x, int y) => (byte)((y << 4) | x);

    // 보드 위인지 확인
    public static bool IsInBoard(int x, int y)
    {
        return x >= 0 && x < BOARD_SIZE && y >= 0 && y < BOARD_SIZE;
    }

    public static bool IsInBoard(byte p)
    {
        int x = GetX(p);
        int y = GetY(p);

        return x < BOARD_SIZE && y < BOARD_SIZE && p >= 0 && p < 240;
    }

    public static byte ColorToLayer(StoneColor color)
    {
        return color switch
        {
            StoneColor.White => White,
            StoneColor.Black => Black,
            _ => Empty,
        };
    }

    public static StoneColor LayerToColor(byte side)
    {
        return side switch
        {
            White => StoneColor.White,
            Black => StoneColor.Black,
            _ => StoneColor.None,
        };
    }

    public static byte OppSide(byte side)
    {
        return side switch
        {
            Black => White,
            White => Black,
            _ => Empty,
        };
    }
}
