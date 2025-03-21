using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

// 준석님 메서드 사용
// Board                    보드 15x15 배열
// board.CheckWin           승패판정
// board.PlaceMove          돌 두기
// board.IsValidMoves       돌을 둘 수 있는 좌표 (int,int) 반환

// CountPattern,CountOpenPattern,CheckPattern의 경우 준석님 것이 있으면 그것을 사용

public class AIController
{
    private const int MAX_DEPTH_EASY = 1;
    private const int MAX_DEPTH_MEDIUM = 2;
    private const int MAX_DEPTH_HARD = 4;
    private const int MAX_DEPTH_EXPERT = 6; // 느려질 수 있음
    
    private int aiPlayer = 1;  // AI 돌
    private int humanPlayer = -1;  // 사용자 돌
    private int difficulty; // 난이도 (1: Easy, 2: Medium, 3: Hard, 4: Expert)

    public AIController(int difficultyLevel)
    {
        difficulty = difficultyLevel;
    }

    public (int, int) GetBestMove(Board board)
    {
        if (difficulty == 1) return MinimaxBestMove(board, MAX_DEPTH_EASY);
        if (difficulty == 2) return MinimaxBestMove(board, MAX_DEPTH_MEDIUM);
        if (difficulty == 3) return MinimaxBestMove(board, MAX_DEPTH_HARD);
        if (difficulty == 4) return MinimaxBestMove(board, MAX_DEPTH_EXPERT);
        
        return GetRandomMove(board);
    }

    /// <summary>
    /// 랜덤으로 돌 두는 메서드
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    private (int, int) GetRandomMove(Board board)
    {
        var moves = board.IsValidMoves();
        if (moves.Count == 0) return (-1, -1);
        Random rand = new Random();
        return moves[rand.Next(moves.Count)];
    }

    private (int, int) MinimaxBestMove(Board board, int depth)
    {
        int bestScore = int.MinValue;
        (int, int) bestMove = (-1, -1);

        foreach (var move in board.IsValidMoves())
        {
            board.PlaceMove(move.Item1, move.Item2, aiPlayer);
            int score = Minimax(board, depth, false, int.MinValue, int.MaxValue);
            board.PlaceMove(move.Item1, move.Item2, 0);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }
        return bestMove;
    }

    private int Minimax(Board board, int depth, bool isMaximizing, int alpha, int beta)
    {
        if (depth == 0 || board.CheckWin(aiPlayer) || board.CheckWin(humanPlayer))
        {
            return EvaluateBoard(board);
        }

        if (isMaximizing)
        {
            int maxEval = int.MinValue;
            foreach (var move in board.IsValidMoves())
            {
                board.PlaceMove(move.Item1, move.Item2, aiPlayer);
                int eval = Minimax(board, depth - 1, false, alpha, beta);
                board.PlaceMove(move.Item1, move.Item2, 0);

                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);
                if (beta <= alpha) break;  // 알파-베타 가지치기
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (var move in board.IsValidMoves())
            {
                board.PlaceMove(move.Item1, move.Item2, humanPlayer);
                int eval = Minimax(board, depth - 1, true, alpha, beta);
                board.PlaceMove(move.Item1, move.Item2, 0);

                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);
                if (beta <= alpha) break;
            }
            return minEval;
        }
    }

    private int EvaluateBoard(Board board)
    {
        // 수치 조정은 차후...
        int score = 0;
        score += CountPatterns(board, aiPlayer, 5) * 10000;
        score += CountPatterns(board, aiPlayer, 4) * 1000;
        score += CountPatterns(board, aiPlayer, 3) * 100;
        score += CountOpenPatterns(board, aiPlayer, 4) * 2000;
        score += CountOpenPatterns(board, aiPlayer, 3) * 500;
        
        
        
        score -= CountPatterns(board, humanPlayer, 5) * 10000;
        score -= CountPatterns(board, humanPlayer, 4) * 1000;
        score -= CountPatterns(board, humanPlayer, 3) * 100;
        score -= CountOpenPatterns(board, humanPlayer, 4) * 2000;
        score -= CountOpenPatterns(board, humanPlayer, 3) * 500;
        
        return score;
    }

    private int CountPatterns(Board board, int player, int length)
    {
        int count = 0;
        for (int x = 0; x < board.SIZE; x++)
        {
            for (int y = 0; y < board.SIZE; y++)
            {
                if (board.board[x, y] == player)
                {
                    if (CheckPattern(board, x, y, 1, 0, player, length) ||  // 가로
                        CheckPattern(board, x, y, 0, 1, player, length) ||  // 세로
                        CheckPattern(board, x, y, 1, 1, player, length) ||  // 대각선 \
                        CheckPattern(board, x, y, 1, -1, player, length))   // 대각선 /
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }

    private bool CheckPattern(Board board, int x, int y, int dx, int dy, int player, int length)
    {
        int count = 0;
        for (int i = 0; i < length; i++)
        {
            int nx = x + i * dx;
            int ny = y + i * dy;
            if (nx >= 0 && ny >= 0 && nx < board.SIZE && ny < board.SIZE && board.board[nx, ny] == player)
            {
                count++;
            }
            else break;
        }
        return count == length;
    }
    
    private int CountOpenPatterns(Board board, int player, int length)
    {
        int count = 0;
        foreach (var move in board.IsValidMoves())
        {
            (int x, int y) = move;
            board.PlaceMove(x, y, player);
            if (CountPatterns(board, player, length) > 0)
            {
                count++;
            }
            board.PlaceMove(x, y, 0);
        }
        return count;
    }
}
