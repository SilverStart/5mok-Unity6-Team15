using UnityEngine;
using static common.Constants;

public class OmokAI : MonoBehaviour
{
    private const int MAX_DEPTH = 3;
    private const int MAX_SCORE = 100000;
    private const int DEFENSE_MULTIPLIER = 2;
    private PatternAnalyzer patternAnalyzer = new();
    private StoneColor stoneColor;

    public void SetStoneColor(StoneColor color)
    {
        stoneColor = color;
    }

    public (int row, int col)? MakeBestMove(StoneColor[,] board) // 인게임의 board를 전달받을 함수수
    {
        (int row, int col)? bestMove = null;

        float bestScore = -MAX_SCORE;

        float alpha = -MAX_SCORE;
        float beta = MAX_SCORE;

        for (int row = 0; row < board.GetLength(0); row++) // 모든 칸을 순회
        {
            for (int col = 0; col < board.GetLength(1); col++)
            {
                if (board[row, col] == StoneColor.None && IsValidPosition(row, col, board))
                {
                    board[row, col] = stoneColor;

                    var score = Minimax(board, 0, false, alpha, beta);

                    board[row, col] = StoneColor.None; // 원상 복구 (백트래킹)

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = (row, col);
                    }

                    alpha = Mathf.Max(alpha, bestScore);
                }
            }
        }

        if (bestMove == null)
            return (board.GetLength(0) / 2, board.GetLength(1) / 2);    // null이면 Center에 놓기기 (주의!)
        return bestMove;
    }

    private int Minimax(StoneColor[,] board, int depth, bool isMaximizing, float alpha, float beta)
    {
        // TODO :: 만약 Player_A(인간)이 승리했다면, -MAX_SCORE를 반환 -> AI입장에서 최악
        if (CheckGameWin(board, stoneColor == StoneColor.White ? StoneColor.Black : StoneColor.White)) return -MAX_SCORE;

        // TODO :: 만약 Player_B(AI)가 승리했다면, MAX_SCORE를 반환 -> AI입장에서 최적
        if (CheckGameWin(board, stoneColor)) return MAX_SCORE;

        // TODO :: 만약 무승부라면 0을 반환
        if (CheckGameDraw(board)) return 0;

        // depth가 MAX_DEPTH에 도달하면 평가 함수를 통해 점수를 반환
        if (depth >= MAX_DEPTH)
            return EvalBoard(board);

        if (isMaximizing)   // AI의 턴
        {
            var bestScore = -MAX_SCORE;

            for (var row = 0; row < board.GetLength(0); row++)
            {
                for (var col = 0; col < board.GetLength(1); col++)
                {
                    // 빈칸이면서 인접한 곳에 돌이 놓여져 있다면
                    if (board[row, col] == StoneColor.None && IsValidPosition(row, col, board))
                    {
                        // var boardCopy = (StoneColor[,])board.Clone();
                        board[row, col] = stoneColor;

                        var score = Minimax(board, depth + 1, false, alpha, beta);

                        board[row, col] = StoneColor.None; // 원상 복구 (백트래킹)

                        bestScore = Mathf.Max(bestScore, score);
                        alpha = Mathf.Max(alpha, bestScore);

                        if (beta <= alpha)
                            break;
                    }
                }

                if (beta <= alpha)
                    break;
            }

            return bestScore;
        }
        else    // 인간의 턴
        {
            var bestScore = MAX_SCORE;
            for (var row = 0; row < board.GetLength(0); row++)
            {
                for (var col = 0; col < board.GetLength(1); col++)
                {
                    if (board[row, col] == StoneColor.None && IsValidPosition(row, col, board))
                    {
                        board[row, col] = stoneColor == StoneColor.White ? StoneColor.Black : StoneColor.White;
                        var score = Minimax(board, depth + 1, true, alpha, beta);

                        board[row, col] = StoneColor.None; // 원상 복구 (백트래킹)

                        bestScore = Mathf.Min(bestScore, score);
                        beta = Mathf.Min(beta, bestScore);

                        if (beta <= alpha)
                            break;
                    }
                }

                if (beta <= alpha)
                    break;
            }

            return bestScore;
        }
    }

    private int EvalBoard(StoneColor[,] board)
    {
        int score = 0;

        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                // 빈 칸은 평가 필요없음
                if (board[i, j] == StoneColor.None)
                    continue;

                int multiplier = (board[i, j] == stoneColor) ? 1 : -DEFENSE_MULTIPLIER;

                score += patternAnalyzer.EvalPattern(board, i, j) * multiplier;
            }
        }

        return score;
    }

    private bool IsValidPosition(int row, int col, StoneColor[,] board)
    {
        if (row < 0 || row >= board.GetLength(0) || col < 0 || col >= board.GetLength(1))
            return false;
        
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                
                if (board[row + i, col + j] != StoneColor.None)
                    return true;
            }
        }

        return false;
    }

    private bool CheckGameDraw(StoneColor[,] board)
    {
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] == StoneColor.None)
                    return false;
            }
        }
        return true;
    }

    private bool CheckGameWin(StoneColor[,] board, StoneColor player)
    {
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] != player)
                    continue;
                
                if (CheckGaro(board, player, i, j) || CheckSero(board, player, i, j) || CheckDiagonal(board, player, i, j))
                    return true;
            }
        }

        return false;
    }

    private bool CheckGaro(StoneColor[,] board, StoneColor player, int x, int y)
    {
        for (int i = 0; i < 5; i++)
        {
            if ((board[x + i, y] != player) || x + i < 0 || x + i >= board.GetLength(0) || y < 0 || y >= board.GetLength(1))
                return false;
        }

        return true;
    }

    private bool CheckSero(StoneColor[,] board, StoneColor player, int x, int y)
    {
        for (int i = 0; i < 5; i++)
        {
            if ((board[x, y + i] != player) || x < 0 || x >= board.GetLength(0) || y + i < 0 || y + i >= board.GetLength(1))
                return false;
        }

        return true;
    }

    private bool CheckDiagonal(StoneColor[,] board, StoneColor player, int x, int y)
    {
        bool isWin = true;

        for (int i = 0; i < 5; i++)
        {
            int nx = x + i;
            int ny = y + i;

            if ((board[nx, ny] != player) || nx < 0 || nx >= board.GetLength(0) || ny < 0 || ny >= board.GetLength(1))
                isWin = false;
        }

        if (isWin)
            return isWin;

        for (int i = 0; i < 5; i++)
        {
            int nx = x + i;
            int ny = y - i;

            if ((board[nx, ny] != player) || nx < 0 || nx >= board.GetLength(0) || ny < 0 || ny >= board.GetLength(1))
                isWin = false;
        }

        return isWin;
    }
}
