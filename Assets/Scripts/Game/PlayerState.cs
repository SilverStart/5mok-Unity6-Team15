using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using static common.Constants;

public class PlayerState : BaseState
{
    private TaskCompletionSource<(int x, int y)> _inputTcs;

    public PlayerState(StoneColor color, GameLogic gameLogic)
    {
        _color = color;
        _gameLogic = gameLogic;
    }

    // 턴 변경
    public override void HandleNextTurn()
    {
        _gameLogic.ChangeGameState();
    }

    public override async Task OnEnter()
    {
        // OX UI 업데이트
        GameManager.Instance.SetGameTurn(_color);

        // 상태 진입 시 로직 구현
        while (!isValidMove)
        {
            _inputTcs = new TaskCompletionSource<(int x, int y)>();
            var move = await _inputTcs.Task;

            // 블록이 클릭되었을 때 처리할 로직
            HandleMove(move.x, move.y);
        }
    }

    public override void HandleMove(int x, int y)
    {
        ProcessMove(x, y);
    }

    public override void OnExit()
    {
    }
}
