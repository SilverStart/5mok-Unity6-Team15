using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static common.Constants;

public class PatternAnalyzer
{
    private readonly (int dx, int dy)[] directions = 
    {
        (1, 0),
        (0, 1),
        (1, 1),
        (1, -1),
    };

    public int EvalPattern(BoardData boardState, int x, int y, StoneColor currentPlayer)
    {
        int score = 0;

        foreach (var (dx, dy) in directions)
        {
            string line = BuildLine(boardState, x, y, (dx, dy), currentPlayer);  
            PatternType pattern = MatchBestPattern(line);

            score += PatternContainer.LineTemplates.FirstOrDefault(t => t.patternType == pattern).Score;
        }

        return score;
    }

    private string BuildLine(BoardData boardState, int x, int y, (int dx, int dy) direction, StoneColor currentPlayer)
    {
        StringBuilder line = new();

        StringBuilder left = new();
        StringBuilder right = new();

        for (int i = -4; i <= 4; i++)
        {
            int nx = x + i * direction.dx;
            int ny = y + i * direction.dy;

            if (nx < 0 || nx >= boardState.GetLength(0) || ny < 0 || ny >= boardState.GetLength(1))
            {
                if (i < 0)
                    left.Append('B');
                else
                    right.Append('B');
                
                continue;
            }

            StoneColor nextState = (nx == x && ny == y) ? currentPlayer : boardState[nx, ny];
            char nextC = nextState == StoneColor.None ? 'E' : nextState == currentPlayer ? 'S' : 'B';


            if (i <= 0)
                left.Append(nextC);
            else
                right.Append(nextC);
        }

        /* for (int i = left.Length - 1; i >= 0; i--)
            line.Append(left[i]); */
        line.Append(left);
        line.Append(right);

        return line.ToString();
    }

    private PatternType MatchBestPattern(string line)
    {
        PatternType bestPattern = PatternType.None;
        int bestScore = -1;

        for (int i = 0; i < PatternContainer.LineTemplates.Length; i++)
        {
            PatternTemplate t = PatternContainer.LineTemplates[i];
            if (line.Contains(t.Template))
            {
                if (t.Score > bestScore)
                {
                    bestScore = t.Score;
                    bestPattern = t.patternType;
                }
            }
        }

        return bestPattern;
    }
}
