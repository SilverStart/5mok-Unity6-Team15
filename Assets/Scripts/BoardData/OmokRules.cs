using static common.Constants;

public static class OmokRules
{
    private static int[] dx = { 0, 1, 1, 1 };
    private static int[] dy = { 1, 1, 0, -1 };

    // 승리 여부 확인
    public static bool CheckWin(int x, int y, StoneColor color, StoneColor[,] board)
    {
        bool isTry = true;
        bool isWin = false;

        if (board[x, y] != StoneColor.None)
            isTry = false;
        else
            board[x, y] = color;

        for (int i = 0; i < 4; i++)
        {
            int count = CountLongestInLine(x, y, color, board, dx[i], dy[i]);
            if ((color == StoneColor.White && count >= 5) ||
                (color == StoneColor.Black && count == 5))
            {
                isWin = true;
                break;
            }
        }

        if (isTry)
            board[x, y] = StoneColor.None;

        if (isWin)
            return true;
        return false;
    }

    // 착수 가능한 위치인지 확인
    public static bool IsValidMove(int x, int y, StoneColor color, StoneColor[,] board)
    {
        if (IsOutOfBoundary(x, y, board))
            return false;
        if (IsAlreadyExist(x, y, board))
            return false;
        if (color == StoneColor.Black && IsForbidden(x, y, board))
            return false;

        return true;
    }

    // 보드 범위 내 여부 확인
    private static bool IsOutOfBoundary(int x, int y, StoneColor[,] board)
    {
        if (x >= 0 && x < board.GetLength(0) &&
            y >= 0 && y < board.GetLength(1))
            return false;

        return true;
    }

    // 이미 돌이 있는지 확인
    private static bool IsAlreadyExist(int x, int y, StoneColor[,] board)
    {
        if (board[x, y] == StoneColor.None)
            return false;

        return true;
    }

    // 금수인지 확인 (흑돌인 경우)
    private static bool IsForbidden(int x, int y, StoneColor[,] board)
    {
        // 5목 완성 확인
        if (CheckWin(x, y, StoneColor.Black, board))
            return false;
        // 장목 확인
        if (IsOverline(x, y, board))
            return true;
        // 44 확인
        if (IsDoubleFour(x, y, board))
            return true;
        // 33 확인
        if (IsDoubleThree(x, y, board))
            return true;
        return false;
    }

    // 흑돌 장목 확인
    private static bool IsOverline(int x, int y, StoneColor[,] board)
    {
        board[x, y] = StoneColor.Black;

        bool isOver = false;

        for (int i = 0; i < 4; i++)
        {
            int count = CountLongestInLine(x, y, StoneColor.Black, board, dx[i], dy[i]);
            if (count > 5)
            {
                isOver = true;
                break;
            }
        }

        board[x, y] = StoneColor.None;

        if (isOver)
            return true;
        return false;
    }

    private static bool IsDoubleFour(int x, int y, StoneColor[,] board)
    {
        board[x, y] = StoneColor.Black;
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            if (IsFourExist(x, y, board, dx[i], dy[i]))
            {
                count++;
                if (count >= 2)
                    break;
            }
        }
        board[x, y] = StoneColor.None;

        if (count >= 2)
            return true;
        return false;
    }

    private static bool IsDoubleThree(int x, int y, StoneColor[,] board)
    {
        board[x, y] = StoneColor.Black;
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            if (IsThreeExist(x, y, board, dx[i], dy[i]))
            {
                count++;
                if (count >= 2)
                    break;
            }
        }
        board[x, y] = StoneColor.None;

        if (count >= 2)
            return true;
        return false;
    }

    // 가장 긴 연속된 갯수 구하기
    private static int CountLongestInLine(int x, int y, StoneColor color, StoneColor[,] board, int xDirection, int yDirection)
    {
        x -= 5 * xDirection;
        y -= 5 * yDirection;

        int count = 0;
        int longest = 0;

        for (int i = 0; i < 11; i++)
        {
            if (!IsOutOfBoundary(x, y, board))
            {
                if (board[x, y] == color)
                {
                    count++;
                }
                else
                {
                    longest = count > longest ? count : longest;
                    count = 0;
                }
            }

            x += xDirection;
            y += yDirection;
        }

       return count > longest ? count : longest;
    }

    // '4' 존재여부 확인
    private static bool IsFourExist(int x, int y, StoneColor[,] board, int xDirection, int yDirection)
    {
        x -= 4 * xDirection;
        y -= 4 * yDirection;

        int countA = 0;
        int countB = 0;
        bool noneFlag = false;

        for (int i = 0; i < 9; i++)
        {
            if (!IsOutOfBoundary(x, y, board))
            {
                if (board[x, y] == StoneColor.Black)
                {
                    if (noneFlag)
                        countB++;
                    else
                        countA++;
                }
                else if (board[x, y] == StoneColor.None)
                {
                    if (noneFlag)
                    {
                        countA = countB;
                        countB = 0;
                    }
                    else
                        noneFlag = true;
                }
                else
                {
                    countA = 0;
                    countB = 0;
                    noneFlag = false;
                }
                if (noneFlag && countA + countB == 4)
                    return true;
            }

            x += xDirection;
            y += yDirection;
        }
        return false;
    }

    // '3' 존재여부 확인
    private static bool IsThreeExist(int x, int y, StoneColor[,] board, int xDirection, int yDirection)
    {
        x -= 4 * xDirection;
        y -= 4 * yDirection;

        int count = 0;
        bool start = false;
        bool noneFlag = false;

        for (int i = 0; i < 9; i++)
        {
            if (!IsOutOfBoundary(x, y, board))
            {
                if (board[x, y] == StoneColor.White)
                {
                    start = false;
                    noneFlag = false;
                    count = 0;
                }
                else if (board[x, y] == StoneColor.None)
                {
                    if (start)
                    {
                        if (noneFlag)
                        {
                            start = false;
                            noneFlag = false;
                            count = 0;
                        }
                        else
                        {
                            if (IsValidForThree(x, y, board))
                                noneFlag = true;
                        }
                    }
                }
                if (board[x, y] == StoneColor.Black)
                {
                    if (start)
                    {
                        count++;
                        if (count == 3)
                        {
                            if (IsValidForThree(x + xDirection, y + yDirection, board))
                                return true;
                            else
                            {
                                start = false;
                                noneFlag = false;
                                count = 0;
                            }
                        }
                    }
                    else
                    {
                        if (IsValidForThree(x - xDirection, y - yDirection, board))
                        {
                            start = true;
                            count++;
                        }
                    }
                }
            }

            x += xDirection;
            y += yDirection;
        }
        return false;
    }

    private static bool IsValidForThree(int x, int y, StoneColor[,] board)
    {
        if (IsOutOfBoundary(x, y, board))
            return false;
        if (board[x, y] != StoneColor.None)
            return false;
        if (CheckWin(x, y, StoneColor.Black, board))
            return true;
        if (IsOverline(x, y, board))
            return false;
        if (IsDoubleFour(x, y, board))
            return false;
        return true;
    }
}