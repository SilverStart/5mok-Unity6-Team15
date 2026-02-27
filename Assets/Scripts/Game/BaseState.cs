using System.Threading.Tasks;
using UnityEngine;
using static common.Constants;

public abstract class BaseState
{
    protected GameLogic _gameLogic;
    protected StoneColor _color;
    protected bool isValidMove = false;

    public abstract Task OnEnter();                      // 상태 진입 시 호출
    public abstract void HandleMove(int x, int y);        // 플레이어 이동 처리
    public abstract void OnExit();                       // 상태 종료 시 호출
    public abstract void HandleNextTurn();               // 다음 턴 처리 

    public void ProcessMove(int x, int y)
    {
        if (_gameLogic.PlaceStone(x, y, _color))
        {
            isValidMove = true;
            var gameResult = _gameLogic.CheckGameResult();
            if (gameResult == GameLogic.GameResult.None)
            {
                // 턴 전환
                HandleNextTurn();
            }
            else
            {
                // 게임 오버 처리
                _gameLogic.EndGame(gameResult);
            }
        }
        else
        {
            // 둘 수 없는 수일 경우 처리
            isValidMove = false;
        }
    }
}
