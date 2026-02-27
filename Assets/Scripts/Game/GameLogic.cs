using System.Threading.Tasks;
using UnityEngine;
using static common.Constants;

public class GameLogic
{
    private BoardData _board;
    private OmokAI _omokAI;

    // 플레이어 상태 변수
    private BaseState blackState;
    private BaseState whiteState;

    // 현재 상태를 나타내는 변수
    private BaseState _currentState;

    // 게임의 결과
    public enum GameResult { None, Win, Lose, Draw }

    public GameLogic(GameType gameType)
    {
        _board = new();
        _omokAI = new();

        switch(gameType)
        {
            case GameType.SinglePlayBlack:
                blackState = new PlayerState(StoneColor.Black, this);
                whiteState = new AIState(StoneColor.White, this);
                _omokAI.SetStoneColor(StoneColor.White);
                break;
            case GameType.SinglePlayWhite:
                blackState = new AIState(StoneColor.Black, this);
                whiteState = new PlayerState(StoneColor.White, this);
                _omokAI.SetStoneColor(StoneColor.Black);
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
            // TODO: 돌 렌더링 하기
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

    public GameResult CheckGameResult()
    {
        return _board.CheckGameEnd();
    }

    public void EndGame(GameResult gameResult)
    {
        string resultStr = "";
        switch (gameResult)
        {
            case GameResult.Win:
                resultStr = "Player1 승리!";
                break;
            case GameResult.Lose:
                resultStr = "Player2 승리!";
                break;
            case GameResult.Draw:
                resultStr = "무승부";
                break;
        }

        GameManager.Instance.OpenConfirmPanel(resultStr, () =>
        {
            GameManager.Instance.ChangeToMainScene();
        });
    }

    public void Resign(StoneColor color)
    {
        if (color == StoneColor.Black)
            EndGame(GameResult.Win);
        if (color == StoneColor.White)
            EndGame(GameResult.Lose);
    }

    public async Task<(int x, int y)?> GetAIMove(StoneColor color)
    {
        return await _omokAI.MakeBestMove(_board.Board);
    }
}
