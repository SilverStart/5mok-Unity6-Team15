using static common.Constants;
using static BoardUtils;
using static BoardConfig;

public partial class BoardData
{
    public StoneColor this[int x, int y] { get => LayerToColor(_layer1[GetP(x, y)]); }

    // 마지막 수 저장
    private LastMove _lastMove = new();

    public BoardData()
    {
        InitLayers();
        InitBeginEnd();
        InitCache();
        InitL2toL3();
    }

    // 보드에 착수
    public void SetStone(int x, int y, StoneColor color)
    {
        // 착수 가능 범위 검사
        if (!IsInBoard(x, y)) return;

        byte p = GetP(x, y);
        PutStone(p, ColorToLayer(color));
    }

    // 마지막 수 되돌리기
    public void Undo()
    {
        RemoveStone();
    }

    public bool IsValidMove(int x, int y, StoneColor color)
    {
        if (!IsInBoard(x, y) || _layer1[GetP(x, y)] != Empty) return false;
        if (color == StoneColor.Black)
        {
            byte currentStatus = _layer3[Black][GetP(x, y)];
            return !IsForbidden(GetP(x, y));
        }

        return true;
    }

    public bool CheckWin(int x, int y, StoneColor color)
    {
        if (_layer3[ColorToLayer(color)][GetP(x, y)] == FMP_5) return true;

        return false;
    }

    public GameResult CheckGameEnd()
    {
        byte p = _lastMove.GetLastMove();
        byte side = _layer1[p];

        RemoveStone();
        if (_layer3[side][p] == FMP_5)
        {
            return side switch
            {
                Black => GameResult.BlackWin,
                _ => GameResult.WhiteWin,
            };
        }
        if (_lastMove.GetTotal() >= BOARD_SIZE * BOARD_SIZE) return GameResult.Draw;
        PutStone(p, side);
        return GameResult.None;
    }

    // 마지막 수 위치 반환
    public (int x, int y)? LastMove()
    {
        byte move = _lastMove.GetLastMove();
        return (move == PASS) ? null : (GetX(move), GetY(move));
    }

    public int GetLength(int i)
    {
        return BOARD_SIZE;
    }
}
