using common;

public class BoardData
{
    private int _width, _height;

    private Constants.StoneColor[,] _board;
    public Constants.StoneColor[,] Board { get { return (Constants.StoneColor[,])_board.Clone();}}

    public BoardData(int width, int height)
    {
        _width = width;
        _height = height;

        _board = new Constants.StoneColor[_width, _height];
        Clear();
    }

    public void SetStone(int x, int y, Constants.StoneColor color)
    {
        if (x >= 0 && x < _width &&
            y >= 0 && y < _height &&
            _board[x, y] == Constants.StoneColor.None)
        {
            _board[x, y] = color;
        }
    }

    public void Clear()
    {
        for (int row = 0; row < _width; row++)
        {
            for (int col = 0; col < _height; col++)
            {
                _board[row, col] = Constants.StoneColor.None;
            }
        }
    }
}
