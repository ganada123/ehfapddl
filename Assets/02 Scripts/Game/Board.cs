using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public const int SIZE = 15;
    private int[,] boardState;
    private GameManager gameManager; // GameManager 인스턴스 참조

    public Board(int[,] state, GameManager manager)
    {
        boardState = new int[SIZE, SIZE];
        gameManager = manager;
        // 상태 복사
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                boardState[i, j] = state[i, j];
            }
        }
    }

    public int this[int x, int y]
    {
        get { return boardState[x, y]; }
        set { boardState[x, y] = value; }
    }
    
    // AIController에서 쓰이는 CheckWin
    public bool CheckWinforAI(int player)
    {
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                if (boardState[x, y] == player)
                {
                    if (CountFiveStones(x, y, 1, 0, player) >= 5) return true;
                    if (CountFiveStones(x, y, 0, 1, player) >= 5) return true;
                    if (CountFiveStones(x, y, 1, 1, player) >= 5) return true;
                    if (CountFiveStones(x, y, 1, -1, player) >= 5) return true;
                }
            }
        }
        return false;
    }

    private int CountFiveStones(int x, int y, int dx, int dy, int player)
    {
        int count = 1;
        
        int nx = x + dx;
        int ny = y + dy;
        while (IsInBounds(nx, ny) && boardState[nx, ny] == player)
        {
            count++;
            nx += dx;
            ny += dy;
        }
        
        nx = x - dx;
        ny = y - dy;
        while (IsInBounds(nx, ny) && boardState[nx, ny] == player)
        {
            count++;
            nx -= dx;
            ny -= dy;
        }

        return count;
    }

    private bool IsInBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < SIZE && y < SIZE;
    }

    public void PlaceMove(int x, int y, int player)
    {
        boardState[x, y] = player;
    }

    public List<(int, int)> IsValidMoves()
    {
        List<(int, int)> validMoves = new List<(int, int)>();
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                if (boardState[i, j] == 0)
                {
                    validMoves.Add((i, j));
                }
            }
        }
        return validMoves;
    }

    public bool IsForbiddenMove(int x, int y, int player)
    {
        // AI는 금수 규칙 적용 안 함
        if (player == 1) return false;
        // GameManager의 IsForbidden 메서드 활용 (플레이어 표현 방식에 맞춰 조정)
        return gameManager.IsForbiddenForAI(x, y);
    }

    public int[,] GetBoardState()
    {
        return boardState;
    }
}