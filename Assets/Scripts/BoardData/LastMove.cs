using static BoardConfig;

public class LastMove
{
    private byte[] _lastMove = new byte[BoardSize];
    private int _top = 0;

    public void Push(byte p)
    {
        if (_top < 255) _lastMove[_top++] = p;
    }

    public byte Pop()
    {
        return (_top > 0) ? _lastMove[--_top] : PASS;
    }

    public byte GetValue(int index)
    {
        return (index < _top) ? _lastMove[index] : PASS;
    }

    public byte GetLastMove()
    {
        return (_top > 0) ? _lastMove[_top - 1] : PASS;
    }

    public int GetTotal()
    {
        return _top;
    }
}
