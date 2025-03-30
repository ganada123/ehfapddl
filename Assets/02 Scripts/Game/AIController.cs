using System;
using System.Collections.Generic;
using UnityEngine;

public class AIController
{
    private int humanPlayer = 1;
    private int aiPlayer = 2;
    private int difficulty;

    private readonly int[] difficultyDepth = { 1, 2, 4, 6 }; // 난이도별 Minimax 깊이

    public AIController(int difficultyLevel)
    {
        difficulty = difficultyLevel;
    }

    public (int, int) GetBestMove(int[,] board)
    {
        return MinimaxBestMove(board, difficultyDepth[difficulty - 1]);
    }

    private (int, int) MinimaxBestMove(int[,] board, int depth)
    {
        int bestScore = int.MinValue;
        (int, int) bestMove = (-1, -1);

        List<Vector2Int> moves = GameManager.Instance.GetValidMoves(aiPlayer);

        foreach (var move in moves)
        {
            board[move.x, move.y] = aiPlayer;
            int score = Minimax(board, depth - 1, false, int.MinValue, int.MaxValue);
            board[move.x, move.y] = 0;

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = (move.x, move.y);
            }
        }
        return bestMove;
    }

    private int Minimax(int[,] board, int depth, bool isMaximizing, int alpha, int beta)
    {
        if (depth == 0 || GameManager.Instance.CheckWinCondition(board, isMaximizing ? aiPlayer : humanPlayer) != 0)
        {
            return EvaluateBoard(board);
        }

        List<Vector2Int> moves = GameManager.Instance.GetValidMoves(isMaximizing ? aiPlayer : humanPlayer);

        if (isMaximizing)
        {
            int maxEval = int.MinValue;
            foreach (var move in moves)
            {
                board[move.x, move.y] = aiPlayer;
                int eval = Minimax(board, depth - 1, false, alpha, beta);
                board[move.x, move.y] = 0;

                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);
                if (beta <= alpha) break;
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (var move in moves)
            {
                board[move.x, move.y] = humanPlayer;
                int eval = Minimax(board, depth - 1, true, alpha, beta);
                board[move.x, move.y] = 0;

                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);
                if (beta <= alpha) break;
            }
            return minEval;
        }
    }

    private int EvaluateBoard(int[,] board)
    {
        return (int)(GameManager.Instance.CalculateBoardScore(board, aiPlayer) - GameManager.Instance.CalculateBoardScore(board, humanPlayer));
    }
}