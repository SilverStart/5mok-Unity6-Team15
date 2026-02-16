using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.Universal;
using static common.Constants;

public class OmokAI
{
    private const int MAX_DEPTH = 3;
    private const int MAX_SCORE = 10000000;
    private const int DEFENSE_MULTIPLIER = 2;
    private PatternAnalyzer patternAnalyzer = new();
    private StoneColor stoneColor;

    private int minimaxCount = 0;
    private int totalMinimaxCount = 0;
    private int caseConunt = 1;

    public void SetStoneColor(StoneColor color)
    {
        stoneColor = color;
    }

    public async Task<(int row, int col)?> MakeBestMove(StoneColor[,] board) // 인게임의 board를 전달받을 함수수
    {
        Debug.Log($"AI: 최적의 수를 찾는 중...");

        float time = Time.time;

        // 백그라운드 스레드에서 실행
        var result = await Task.Run(() => 
        {
            (int row, int col)? bestMove = null;

            float bestScore = -MAX_SCORE;

            float alpha = -MAX_SCORE;
            float beta = MAX_SCORE;

            var validPositions = GetValidPositions(board);
            Debug.Log($"AI: 유효한 위치 수: {validPositions.Count}");
            for (int i = 0; i < validPositions.Count; i++)
            {
                var (row, col, score) = validPositions[i];
                board[row, col] = stoneColor;

                var scoreValue = Minimax(board, 0, false, alpha, beta, (row, col));

                // Debug.Log($"Minimax : {minimaxCount}");
                totalMinimaxCount += minimaxCount;
                minimaxCount = 0;

                if (scoreValue == MAX_SCORE)
                {
                    bestMove = (row, col);
                    break;
                }

                board[row, col] = StoneColor.None; // 원상 복구 (백트래킹)

                if (scoreValue > bestScore)
                {
                    bestScore = scoreValue;
                    bestMove = (row, col);
                }

                alpha = Mathf.Max(alpha, bestScore);


            }

            if (bestMove == null)
                return (board.GetLength(0) / 2, board.GetLength(1) / 2);    // null이면 Center에 놓기기 (주의!)
            return bestMove;
        });

        Debug.Log($"AI: Minimax 함수를 {totalMinimaxCount}번 호출했습니다.");
        totalMinimaxCount = 0;

        Debug.Log($"AI: 최적의 수를 찾는데 걸린 시간: {(Time.time - time):F2}초, 놓은 자리: {result?.row}, {result?.col}");

        return result;
    }

    private int Minimax(StoneColor[,] board, int depth, bool isMaximizing, float alpha, float beta, 
        (int x, int y)? lastMove = null)
    {
        minimaxCount++;

        StoneColor enemyColor = stoneColor == StoneColor.White ? StoneColor.Black : StoneColor.White;

        // 만약 Player_A(인간)이 승리했다면, -MAX_SCORE를 반환 -> AI입장에서 최악
        if (OmokRules.CheckWin(lastMove.Value.x, lastMove.Value.y, enemyColor, board)) 
            return -MAX_SCORE;

        // 만약 Player_B(AI)가 승리했다면, MAX_SCORE를 반환 -> AI입장에서 최적
        if (OmokRules.CheckWin(lastMove.Value.x, lastMove.Value.y, stoneColor, board)) 
            return MAX_SCORE;

        // depth가 MAX_DEPTH에 도달하면 평가 함수를 통해 점수를 반환
        if (depth >= MAX_DEPTH)
            return EvalBoard(board);

        if (isMaximizing)   // AI의 턴
        {
            var bestScore = -MAX_SCORE;

            var validPositions = GetValidPositions(board, lastMove);

            for (int i = 0; i < validPositions.Count; i++)
            {
                var (row, col, score) = validPositions[i];
                board[row, col] = stoneColor;
                
                var scoreValue = Minimax(board, depth + 1, false, alpha, beta, (row, col));

                board[row, col] = StoneColor.None; // 원상 복구 (백트래킹)

                bestScore = Mathf.Max(bestScore, scoreValue);
                alpha = Mathf.Max(alpha, bestScore);

                if (beta <= alpha)
                    break;
            }

            return bestScore;
        }
        else    // 인간의 턴
        {
            var bestScore = MAX_SCORE;

            var validPositions = GetValidPositions(board, lastMove);
            for (int i = 0; i < validPositions.Count; i++)
            {
                var (row, col, score) = validPositions[i];
                board[row, col] = stoneColor == StoneColor.White ? StoneColor.Black : StoneColor.White;
                
                var scoreValue = Minimax(board, depth + 1, true, alpha, beta, (row, col));

                board[row, col] = StoneColor.None; // 원상 복구 (백트래킹)

                bestScore = Mathf.Min(bestScore, scoreValue);
                beta = Mathf.Min(beta, bestScore);

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

    // TODO) 외부에서 제공하는 금주 함수를 사용하여 valid의 조건을 충족하는지도 확인할 예정
    private bool IsNearStone(int row, int col, StoneColor[,] board)
    {
        if (row < 0 || row >= board.GetLength(0) || col < 0 || col >= board.GetLength(1))
            return false;
        
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                
                if (row + i < 0 || row + i >= board.GetLength(0) || col + j < 0 || col + j >= board.GetLength(1))
                    continue;

                if (board[row + i, col + j] != StoneColor.None)
                    return true;
            }
        }

        return false;
    }

    private List<(int row, int col, int score)> GetValidPositions(StoneColor[,] board, (int x, int y)? lastMove = null)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        var candidates = new List<(int row, int col, int score)>(128);

        // 1) 반경 제한 후보 수집
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (board[r, c] != StoneColor.None)
                    continue;
                
                if (!IsNearStone(r, c, board))
                    continue;

                candidates.Add((r, c, patternAnalyzer.EvalPattern(board, r, c)));
            }
        }

        if (candidates.Count == 0) 
        {
            return new List<(int row, int col, int score)> { (rows / 2, cols / 2, 0) };
        }

        var winMoves = new List<(int row, int col, int score)>();
        for (int i = 0; i < candidates.Count; i++)
        {
            var (r, c, score) = candidates[i];
            board[r, c] = stoneColor;

            if (OmokRules.CheckWin(r, c, stoneColor, board))
                winMoves.Add((r, c, score));

            board[r, c] = StoneColor.None;
        }

        if (winMoves.Count > 0)
        {
            winMoves.Sort((a, b) => b.score.CompareTo(a.score));
            return winMoves;
        }

        StoneColor enemy = stoneColor == StoneColor.White ? StoneColor.Black : StoneColor.White;

        var blockMoves = new List<(int row, int col, int score)>();
        for (int i = 0; i < candidates.Count; i++)
        {
            var (r, c, sc) = candidates[i];
            board[r, c] = enemy;

            /* if (CheckGameWin(board, enemy, r, c))
                blockMoves.Add((r, c, sc)); */

            if (OmokRules.CheckWin(r, c, enemy, board))
                blockMoves.Add((r, c, sc));

            board[r, c] = StoneColor.None;
        }

        if (blockMoves.Count > 0)
        {
            Debug.Log($"AI: 블로킹 위치 수: {blockMoves.Count}");
            blockMoves.Sort((a, b) => b.score.CompareTo(a.score));
            return blockMoves;
        }

        candidates.Sort((a, b) => b.score.CompareTo(a.score));
        int k = Mathf.Min(25, candidates.Count);

        if (candidates.Count > k)
            candidates.RemoveRange(k, candidates.Count - k);

        return candidates;
    }

    private bool CheckGameWin(StoneColor[,] board, StoneColor player, int row, int col)
    {
        if (CheckGaro(board, player, row, col) || CheckSero(board, player, row, col) || CheckDiagonal(board, player, row, col))
            return true;

        return false;
    }

    private bool CheckGaro(StoneColor[,] board, StoneColor player, int row, int col)
    {
        for (int i = 0; i < 5; i++)
        {
            if (row + i < 0 || row + i >= board.GetLength(0) || col < 0 || col >= board.GetLength(1) || (board[row + i, col] != player))
                return false;
        }

        return true;
    }

    private bool CheckSero(StoneColor[,] board, StoneColor player, int row, int col)
    {
        for (int i = 0; i < 5; i++)
        {
            if (row < 0 || row >= board.GetLength(0) || col + i < 0 || col + i >= board.GetLength(1) || (board[row, col + i] != player))
                return false;
        }

        return true;
    }

    private bool CheckDiagonal(StoneColor[,] board, StoneColor player, int row, int col)
    {
        bool isWin = true;

        for (int i = 0; i < 5; i++)
        {
            int nx = row + i;
            int ny = col + i;

            if (nx < 0 || nx >= board.GetLength(0) || ny < 0 || ny >= board.GetLength(1) || (board[nx, ny] != player))
                isWin = false;
        }

        if (isWin)
            return isWin;

        for (int i = 0; i < 5; i++)
        {
            int nx = row + i;
            int ny = col - i;

            if (nx < 0 || nx >= board.GetLength(0) || ny < 0 || ny >= board.GetLength(1) || (board[nx, ny] != player))
                isWin = false;
        }

        return isWin;
    }
}
