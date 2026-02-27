using static common.Constants;
using static BoardUtils;
using static BoardConfig;
using System;
using Unity.VisualScripting;

public partial class BoardData
{
    private readonly byte[] _layer1 = new byte[BoardSize];
    private readonly byte[][][] _layer2 = new byte[2][][];
    private readonly byte[][] _layer3 = new byte[2][];
    private readonly byte[] _p5 = new byte[2];

    private byte[][] _L2toL3 = new byte[2][];
    private byte[][][] _cache = new byte[2][][];

    private byte[,] _BEGIN = new byte[4, 256];
    private byte[,] _END = new byte[4, 256];


    private void InitLayers()
    {
        for (int p = 0; p < _layer1.Length; p++)
            _layer1[p] = Empty;
        for (int side = 0; side < 2; side++)
        {
            _layer2[side] = new byte[4][];
            for (int line = 0; line < 4; line++)
            {
                _layer2[side][line] = new byte[BoardSize];
                for (int i = 0; i < BoardSize; i++)
                    _layer2[side][line][i] = FSP_1;
            }

            _layer3[side] = new byte[BoardSize];
            for (int i = 0; i < BoardSize; i++)
                _layer3[side][i] = FMP_1;
        }
    }

    private void InitBeginEnd()
    {
        int x, y;

        for (byte p = 0; p < 255; p++)
        {
            x = GetX(p);
            y = GetY(p);
            _BEGIN[0, p] = (byte)x;
            _BEGIN[1, p] = (byte)(y * 16);
            _BEGIN[2, p] = (byte)(Math.Min(BOARD_SIZE - x - 1, y) * 15);
            _BEGIN[3, p] = (byte)(Math.Min(x, y) * 17);

            _END[0, p] = (byte)(BOARD_SIZE - x - 1);
            _END[1, p] = (byte)((BOARD_SIZE - y - 1) * 16);
            _END[2, p] = (byte)(Math.Min(x, BOARD_SIZE - y - 1) * 15);
            _END[3, p] = (byte)(Math.Min(BOARD_SIZE - x - 1, BOARD_SIZE - y - 1) * 17);
        }
    }

    public void InitL2toL3()
    {
        for (int side = 0; side <= 1; side++)
        {
            _L2toL3[side] = new byte[65536];

            for (int n0 = 0; n0 < 11; n0++)
            for (int n1 = 0; n1 < 11; n1++)
            for (int n2 = 0; n2 < 11; n2++)
            for (int n3 = 0; n3 < 11; n3++)
            {
                // 1. 각 점수 타입의 개수 카운트
                int[] countOfType = new int[11];

                countOfType[n0]++;
                countOfType[n1]++;
                countOfType[n2]++;
                countOfType[n3]++;

                // 2. 인덱스 생성 (4비트씩 시프트)
                int idx = n0 | (n1 << 4) | (n2 << 8) | (n3 << 12);

                byte finalStatus;

                if (side == Black) // 흑돌: 금수 규칙 적용
                {
                    if (countOfType[FSP_5] > 0) finalStatus = FMP_5;
                    else if (countOfType[FSP_L] > 0) finalStatus = FMP_L; // 금수(6목 이상 등)
                    else if (countOfType[FSP_44] > 0 || (countOfType[FSP_4] + countOfType[FSP_d4] + countOfType[FSP_d4p] >= 2))
                    {
                        finalStatus = FMP_44; // 4-4
                    }
                    else if ((countOfType[FSP_d4] > 0 || countOfType[FSP_4] > 0 || countOfType[FSP_d4p] > 0) && countOfType[FSP_3] >= 2)
                    {
                        finalStatus = FMP_433; // 4-3-3
                    }
                    else if (countOfType[FSP_3] >= 2) finalStatus = FMP_33; // 3-3
                    else if (countOfType[FSP_4] > 0) finalStatus = FMP_4;
                    else if ((countOfType[FSP_d4] > 0 || countOfType[FSP_4] > 0 || countOfType[FSP_d4p] > 0) && countOfType[FSP_3] > 0)
                    {
                        finalStatus = FMP_43; // 4-3
                    }
                    else if (countOfType[FSP_d4p] > 0 || (countOfType[FSP_d4] > 0 && (countOfType[FSP_2] > 0 || countOfType[FSP_d3] > 0)))
                    {
                        finalStatus = FMP_d4p;
                    }
                    else if (countOfType[FSP_d4] > 0) finalStatus = FMP_d4;
                    else if (countOfType[FSP_3] > 0 && (countOfType[FSP_2] > 0 || countOfType[FSP_d3] > 0)) finalStatus = FMP_3p;
                    else if (countOfType[FSP_3] > 0) finalStatus = FMP_3;
                    else if (countOfType[FSP_2] + countOfType[FSP_d3] >= 2) finalStatus = FMP_pp;
                    else if (countOfType[FSP_d3] > 0) finalStatus = FMP_d3;
                    else if (countOfType[FSP_2] > 0) finalStatus = FMP_2;
                    else if (countOfType[FSP_1] > 0) finalStatus = FMP_1;
                    else finalStatus = FMP_DEAD;
                }
                else // 백돌: 금수 없음 (FSP_L도 승리로 처리 가능)
                {
                    if (countOfType[FSP_5] > 0 || countOfType[FSP_L] > 0) finalStatus = FMP_5;
                    else if (countOfType[FSP_44] > 0 || (countOfType[FSP_4] + countOfType[FSP_d4] + countOfType[FSP_d4p] >= 2)) finalStatus = FMP_44;
                    else if (countOfType[FSP_4] > 0) finalStatus = FMP_4;
                    else if ((countOfType[FSP_d4] > 0 || countOfType[FSP_d4p] > 0) && countOfType[FSP_3] > 0) finalStatus = FMP_43;
                    else if (countOfType[FSP_d4p] > 0 || (countOfType[FSP_d4] > 0 && (countOfType[FSP_2] > 0 || countOfType[FSP_d3] > 0))) finalStatus = FMP_d4p;
                    else if (countOfType[FSP_d4] > 0) finalStatus = FMP_d4;
                    else if (countOfType[FSP_3] >= 2) finalStatus = FMP_33;
                    else if (countOfType[FSP_3] > 0 && (countOfType[FSP_2] > 0 || countOfType[FSP_d3] > 0)) finalStatus = FMP_3p;
                    else if (countOfType[FSP_3] > 0) finalStatus = FMP_3;
                    else if (countOfType[FSP_2] + countOfType[FSP_d3] >= 2) finalStatus = FMP_pp;
                    else if (countOfType[FSP_d3] > 0) finalStatus = FMP_d3;
                    else if (countOfType[FSP_2] > 0) finalStatus = FMP_2;
                    else if (countOfType[FSP_1] > 0) finalStatus = FMP_1;
                    else finalStatus = FMP_DEAD;
                }

                _L2toL3[side][idx] = finalStatus;
            }
        }
    }

    private void InitCache()
    {
        byte side, n;
        uint idx, pow_n;

        for (side = 0; side < 2; side++)
            _cache[side] = new byte[BoardSize + 1][];

        for (side = 0; side <= 1; side++)
        {
            for (n = 5; n <= BOARD_SIZE; n++)
            {
                pow_n = (uint)1 << n;
                _cache[side][n] = new byte[n * pow_n];

                for (idx = 0; idx < pow_n; idx++)
                {
                    uint startIdx = idx * n;
                    InitCacheLine(_cache[side][n], startIdx, idx, n, side, true);
                }
            }
        }
    }

    private void InitCacheLine(byte[] dstArray, uint startIdx, uint src, int len, byte side, bool reverse)
    {
        int n, i, blank, blank1dis;
        byte idx, j, m;
        uint rev;
        uint GetStone(int n) => (src >> n) & 1;

        // 0100101001001000 B
	    // ....|<->|.......
	    //     |idx|<- n ->

        for (n = 0; n + 4 < len; n++)
        {
            if (n > 0 && side != White)
                if (GetStone(n - 1) != 0) continue;
            blank = 0;
            idx = 0;
            blank1dis = 0;

            for (i = 0; i < 5; i++)
            {
                if (GetStone(n + i) != 0) idx |= (byte) (1 << (4 - i));
                else
                {
                    blank++;
                    if (blank == 1) blank1dis = i;
                }
            }

            if (n + 5 < len && GetStone(n + 5) != 0 && side != White)
            {
                if (blank == 1) dstArray[startIdx + n + blank1dis] = FSP_L;
            }
            else if (n + 5 < len && GetStone(n + 5) == 0 && side != White && blank == 0) dstArray[startIdx + n + 5] = FSP_L;
            else
            {
                if (n == 0) idx |= 64;
                else
                {
                    if (GetStone(n - 1) != 0) idx |= 64;
                    else if (n > 1 && side != White)
                        if (GetStone(n - 2) != 0) idx |= 64;
                }

                if (n + 5 >= len) idx |= 32;
                else
                {
                    if (GetStone(n + 5) != 0) idx |= 32;
                    else if (n + 6 < len)
                        if (GetStone(n + 6) != 0 && side != White) idx |= 32;
                }

                for (i = 0; i < 5; i++)
                {
                    j = _gene[idx, i];
                    uint currentIdx = (uint)(startIdx + n + i);
                    m = dstArray[currentIdx];
                    if ((j == FSP_d4 || j == FSP_d4p) && (m == FSP_d4 || m == FSP_d4p)) dstArray[currentIdx] = FSP_44;
                    else if (j == FSP_d4 && m == FSP_d3 || m == FSP_d4 && j == FSP_d3) dstArray[currentIdx] = FSP_d4p;
                    else if (j > m) dstArray[currentIdx] = j;
                }
            }
            if (blank >= 1) n += blank1dis;
        }
        if (side != White || !reverse) return;

        rev = 0;
        for (i = 0; i < len; i++)
        {
            if (GetStone(i) != 0) rev |= (uint)(1 << (len - i - 1));
        }

        byte[] temp = new byte[len];
        InitCacheLine(temp, 0, rev, len, side, false);

        for (i = 0; i < len; i++)
        {
            if (dstArray[startIdx + i] < temp[len - i - 1])
                dstArray[startIdx + i] = temp[len - i - 1];
        }
    }

    private void PutStone(byte p, byte side)
    {
        _layer1[p] = side;
        _lastMove.Push(p);
        PutStoneLine(p, 0, side);
        PutStoneLine(p, 1, side);
        PutStoneLine(p, 2, side);
        PutStoneLine(p, 3, side);

        _layer2[OppSide(side)][0][p] = FSP_DEAD;
        _layer2[OppSide(side)][1][p] = FSP_DEAD;
        _layer2[OppSide(side)][2][p] = FSP_DEAD;
        _layer2[OppSide(side)][3][p] = FSP_DEAD;

        _layer3[OppSide(side)][p] = FMP_DEAD;
    }

    private void PutStoneLine(byte p, int line, byte side)
    {
        byte    begin, end, left, right, plus;
        int     len;

        plus = (byte)_directions[line];
        begin = (byte)(p - _BEGIN[line, p]);
        end = (byte)(p + _END[line, p]);

        ///////////////////////////////////////////
	    //                v
	    //  X O  X  X     O    X    O   X
	    //           [ area1  ]
	    //     [ area2   ] [ area3 ]
	    //////////////////////////////////////////

        // area1
        for (left = p; left != begin; left -= plus)
            if (_layer1[left - plus] == OppSide(side)) break;
        for (right = p; right != end; right += plus)
            if (_layer1[right + plus] == OppSide(side)) break;
        len = (right - left) / plus + 1;
        UpdateL2Area(left, line, len, side);

        // area2
        for (left = p; left != begin; left -= plus)
            if (_layer1[left - plus] == side) break;
        len = (p - left) / plus;
        UpdateL2Area(left, line, len, OppSide(side));

        // area3
        for (right = p; right != end; right += plus)
            if (_layer1[right + plus] == side) break;
        len = (right - p) / plus;
        UpdateL2Area((byte)(p + plus), line, len, OppSide(side));
    }

    private void RemoveStone()
    {
        byte p, side;

        p = _lastMove.Pop();
        if (p == PASS) return;

        side = _layer1[p];

        _layer1[p] = Empty;

        RemoveStoneLine(p, 0, side);
	    RemoveStoneLine(p, 1, side);
	    RemoveStoneLine(p, 2, side);
	    RemoveStoneLine(p, 3, side);
    }

    private void RemoveStoneLine(byte p, int line, byte side)
    {
        byte begin, end, left, right, plus;
        int len;

        plus = (byte)_directions[line];
        begin = (byte)(p - _BEGIN[line, p]);
        end = (byte)(p + _END[line, p]);

        ///////////////////////////////////////////////
	    //                O
	    //  XO  X  X  O   ^    X  O   X
	    //          [ area1,W ]
	    //             [ area2,B ]
	    ///////////////////////////////////////////////

        // area1
        for (left = p; left != begin; left -= plus)
            if (_layer1[left - plus] == OppSide(side)) break;
        for (right = p; right != end; right += plus)
            if (_layer1[right + plus] == OppSide(side)) break;

        len = (right - left) / plus + 1;

        UpdateL2Area(left, line, len, side);

        // area2
        for (left = p; left != begin; left -= plus)
            if (_layer1[left - plus] == side) break;
        for (right = p; right != end; right += plus)
            if (_layer1[right + plus] == side) break;

        len = (right - left) / plus + 1;

        UpdateL2Area(left, line, len, OppSide(side));
    }

    private void UpdateL2Area(byte p, int line, int len, byte side)
    {
        byte p1;
        int n;
        byte[] l2 = _layer2[side][line];
        byte plus = (byte)_directions[line];

        if (len < 5)
        {
            for (n = 0; n < len; n++)
            {
                if (l2[p] > 0)
                {
                    l2[p] = 0;

                    UpdateLayer3(p, side);
                }
                p += plus;
            }
        }
        else
        {
            uint area = 0;
            p1 = p;
            for (n = 0; n < len; n++)
            {
                if (_layer1[p1] == side) area |= (uint)(1 << n);
                p1 += plus;
            }

            byte[] pC = _cache[side][len];
            int cacheStartIdx = len * (int)area;

            for (n = 0; n < len; n++)
            {
                byte targetScore = pC[cacheStartIdx + n];
                if (l2[p] != targetScore)
                {
                    l2[p] = targetScore;

                    UpdateLayer3(p, side);
                }
                p += plus;
            }
        }
    }

    void UpdateLayer3(byte p, byte side)
    {
        uint idx;

        idx = (uint) _layer2[side][0][p];
        idx |= ((uint) _layer2[side][1][p]) << 4;
        idx |= ((uint) _layer2[side][2][p]) << 8;
        idx |= ((uint) _layer2[side][3][p]) << 12;

        _layer3[side][p] = _L2toL3[side][idx];
        if (_layer3[side][p] == FMP_5) _p5[side] = 5;
    }

    private bool IsForbidden(byte p)
    {
        switch (_layer3[Black][p])
        {
            case FMP_5:
                return false;
            case FMP_L:
            case FMP_44:
                return true;
            case FMP_33:
            case FMP_433:
                return Is33Forbidden(p);
            default:
                return false;
        }
    }

    private bool Is33Forbidden(byte p)
    {
        int i, true3 = 0;

        for (i = 0; i < 4; i++)
        {
            if (IsTrue3(p, i))
            {
                true3++;
                if (true3 == 2)
                    return true;
            }
        }
        return false;
    }

    private bool IsTrue3(byte p, int line)
    {
        byte begin, end, left, right, plus;


        if (_layer2[Black][line][p] != FSP_3) return false;

        PutStone(p, Black);

        plus = (byte)_directions[line];
        begin = (byte)(p - _BEGIN[line, p]);
        end = (byte)(p + _END[line, p]);

        for (left = p; left != begin && _layer1[left] != White; left -= plus)
        {
            if (_layer1[left] == Empty)
            {
                if (_layer2[Black][line][left] == FSP_4)
                {
                    if (!IsForbidden(left))
                    {
                        RemoveStone();
                        return true;
                    }
                }
                break;
            }
        }

        for (right = p; right != end && _layer1[right] != White; right += plus)
        {
            if (_layer1[right] == Empty)
            {
                if (_layer2[Black][line][right] == FSP_4)
                {
                    if (!IsForbidden(right))
                    {
                        RemoveStone();
                        return true;
                    }
                }
                break;
            }
        }
        RemoveStone();
        return false;
    }
}