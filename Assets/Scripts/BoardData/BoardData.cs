using common;
using System.Collections.Generic;

public class BoardData
{
    // 보드의 가로와 세로 길이
    private int _width, _height;

    // 마지막 수 저장 (int, int 튜플 or Null)
    private Stack<(int x, int y)> _lastMove = new();

    // 보드의 상태
    private Constants.StoneColor[,] _board;

    // 보드 정보 (복사)
    public Constants.StoneColor[,] Board { get { return (Constants.StoneColor[,])_board.Clone();}}

    public BoardData(int width, int height)
    {
        // 보드의 가로와 세로 길이 정보 저장
        _width = width;
        _height = height;

        // 보드 정보 초기화
        _board = new Constants.StoneColor[_width, _height];
        Clear();
    }

    // 보드에 착수
    public void SetStone(int x, int y, Constants.StoneColor color)
    {
        // 착수 가능 범위 검사
        if (x >= 0 && x < _width &&
            y >= 0 && y < _height &&
            _board[x, y] == Constants.StoneColor.None)
        {
            _board[x, y] = color;
            _lastMove.Push((x, y));
        }
    }

    // 마지막 수 되돌리기
    public void Undo()
    {
        if (_lastMove.Count > 0)
        {
            var (x, y) = _lastMove.Pop();
            _board[x, y] = Constants.StoneColor.None;
        }
    }

    // 마지막 수 위치 반환
    public (int x, int y)? LastMove()
    {
        if (_lastMove.Count > 0)
        {
            return _lastMove.Peek();
        }

        // 마지막 수가 없으면 Null 반환
        return null;
    }

    // 보드 정보 초기화
    public void Clear()
    {
        for (int row = 0; row < _width; row++)
        {
            for (int col = 0; col < _height; col++)
            {
                _board[row, col] = Constants.StoneColor.None;
            }
        }
        _lastMove.Clear();
    }
}
