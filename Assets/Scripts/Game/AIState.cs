using System.Threading.Tasks;
using static common.Constants;

public class AIState : BaseState
{
    public AIState(StoneColor color, GameLogic gameLogic)
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
        //GameManager.Instance.SetGameTurn(_color);

        // 상태 진입 시 로직 구현
        // while (!isValidMove)
        // {
        //     if (await _gameLogic.GetAIMove(_color) is (int x, int y) targetMove)
        //     {
        //         // 블록이 클릭되었을 때 처리할 로직
        //         HandleMove(x, y);
        //     }
        //     else
        //     {
        //         _gameLogic.Resign(_color);
        //         break;
        //     }
        // }
    }

    public override void HandleMove(int x, int y)
    {
        ProcessMove(x, y);
    }

    public override void OnExit()
    {
    }
}
