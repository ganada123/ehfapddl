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

    public bool CheckWin(int player)
    {
        // GameManager의 CheckWin 메서드 활용 (플레이어 표현 방식에 맞춰 조정)
        return gameManager.CheckWinForAI(player == 1 ? 1 : 2);
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