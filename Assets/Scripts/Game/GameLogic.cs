using System;
using System.Threading.Tasks;
using UnityEngine;
using static common.Constants;

public class GameLogic
{
    public Action<int, int, StoneColor> OnStonePlaced;
    public Action<int, int> OnUndo;
    public Action<int, int> OnPlaceX;

    private BoardData _board;
    private OmokAI _omokAI;

    // 플레이어 상태 변수
    private BaseState blackState;
    private BaseState whiteState;

    // 현재 상태를 나타내는 변수
    private BaseState _currentState;

    public GameLogic(GameType gameType)
    {
        _board = new();

        switch(gameType)
        {
            case GameType.SinglePlayBlack:
                blackState = new PlayerState(StoneColor.Black, this);
                whiteState = new AIState(StoneColor.White, this);
                _omokAI = new(StoneColor.White);
                break;
            case GameType.SinglePlayWhite:
                blackState = new AIState(StoneColor.Black, this);
                whiteState = new PlayerState(StoneColor.White, this);
                _omokAI = new(StoneColor.Black);
                break;
            case GameType.DualPlay:
                blackState = new PlayerState(StoneColor.Black, this);
                whiteState = new PlayerState(StoneColor.White, this);
                break;
        }
        SetState(blackState);
    }

    private async void SetState(BaseState newState)
    {
        _currentState?.OnExit();
        _currentState = newState;
        await _currentState.OnEnter();
    }

    public bool PlaceStone(int x, int y, StoneColor color)
    {
        if (_board.IsValidMove(x, y, color))
        {
            // 정상적인 위치일 때
            OnStonePlaced?.Invoke(x, y, color);
            _board.SetStone(x, y, color);
            return true;
        }
        else
        {
            // 비정상적인 위치일 때
            return false;
        }
    }

    public void ChangeGameState()
    {
        if (_currentState == blackState)
        {
            SetState(whiteState);
        }
        else
        {
            SetState(blackState);
        }
    }

    public void SetInputResult(PlayerInput input)
    {
        _currentState.SetInputResult(input);
    }

    public GameResult CheckGameResult()
    {
        return _board.CheckGameEnd();
    }

    public void EndGame(GameResult gameResult)
    {
        string resultStr = "";
        switch (gameResult)
        {
            case GameResult.BlackWin:
                resultStr = "Player1 승리!";
                break;
            case GameResult.WhiteWin:
                resultStr = "Player2 승리!";
                break;
            case GameResult.Draw:
                resultStr = "무승부";
                break;
        }

        GameManager.Instance.GameOver(resultStr);
    }

    public void TimeOver(StoneColor color)
    {
        if (color == StoneColor.Black)
            EndGame(GameResult.WhiteWin);
        if (color == StoneColor.White)
            EndGame(GameResult.BlackWin);
    }

    public void Resign(StoneColor color)
    {
        if (color == StoneColor.Black)
            EndGame(GameResult.WhiteWin);
        if (color == StoneColor.White)
            EndGame(GameResult.BlackWin);
    }

    public bool Undo()
    {
        var move = _board.LastMove();
        if (move.HasValue)
        {
            OnUndo?.Invoke(move.Value.x, move.Value.y);
            _board.Undo();
            return true;
        }

        return false;
    }

    // TODO: BoardData객체의 참조를 넘기는 방식으로 수정해야 함
    public async Task<(int x, int y)?> GetAIMove(StoneColor color)
    {
        // return await _omokAI.MakeBestMove(_board);
        return null;
    }

    public void Dispose()
    {
        
    }
}
