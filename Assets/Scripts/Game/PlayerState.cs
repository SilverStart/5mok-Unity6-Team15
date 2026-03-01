using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using static common.Constants;

public class PlayerState : BaseState
{
    private TaskCompletionSource<PlayerInput> _inputTcs;

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
            _inputTcs = new TaskCompletionSource<PlayerInput>();
            var input = await _inputTcs.Task;

            switch (input.Type)
            {
                case InputType.PlaceStone:
                    // 블록이 클릭되었을 때 처리할 로직
                    HandleMove(input.x, input.y);
                    break;
                case InputType.Surrender:
                    // 항복
                    _gameLogic.Resign(_color);
                    return;
                case InputType.Undo:
                    if (_gameLogic.Undo())
                    {
                        if (_gameLogic.Undo()) break;
                        else
                        {
                            HandleNextTurn();
                            return;
                        }
                    }
                    else
                    {
                        // 무르기 실패 팝업
                        break;
                    }
            }

        }
    }

    public override void SetInputResult(PlayerInput input)
    {
        _inputTcs?.TrySetResult(input);
    }

    public override void HandleMove(int x, int y)
    {
        ProcessMove(x, y);
    }

    public override void OnExit()
    {
    }
}
