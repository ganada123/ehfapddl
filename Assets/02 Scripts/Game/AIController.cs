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
// board.IsValidMoves       돌을 둘 수 있는 좌표 List<(int,int)> 반환

// PatternScore의 경우 준석님 것이 있으면 그것을 사용

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

        int index = Random.Range(0, moves.Count);
        return moves[index];
    }

    // 모든 범위를 탐색하지 않고, 기존에 놓인 돌을 기준으로 2칸 이내만 탐색
    // 탐색량을 줄여 탐색 속도를 높임
    private List<(int, int)> GetNearbyMoves(Board board)
    {
        HashSet<(int, int)> nearby = new HashSet<(int, int)>();
        for (int x = 0; x < board.SIZE; x++)
        {
            for (int y = 0; y < board.SIZE; y++)
            {
                if (board[x, y] != 0)
                {
                    for (int dx = -2; dx <= 2; dx++)
                    {
                        for (int dy = -2; dy <= 2; dy++)
                        {
                            int nx = x + dx;
                            int ny = y + dy;
                            if (nx >= 0 && ny >= 0 && nx < board.SIZE && ny < board.SIZE && board[nx, ny] == 0)
                                nearby.Add((nx, ny));
                        }
                    }
                }
            }
        }

        return new List<(int, int)>(nearby);
    }
    
    private bool GameWin(Board board, (int, int) move, int player)
    {
        board.PlaceMove(move.Item1, move.Item2, player);
        bool win = board.CheckWin(player);
        board.PlaceMove(move.Item1, move.Item2, 0);
        return win;
    }
    
    private (int, int) MinimaxBestMove(Board board, int depth)
    {
        int bestScore = int.MinValue;
        (int, int) bestMove = (-1, -1);

        List<(int, int)> moves = GetNearbyMoves(board);
        
        foreach (var move in moves)
        {
            if (GameWin(board, move, aiPlayer)) return move;  // AI 즉시 승리
            if (GameWin(board, move, humanPlayer)) return move;  // 즉시 차단
        }
        
        foreach (var move in moves)
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
        // TODO: 무승부 처리 필요
        if (board.IsValidMoves().Count == 0)
            return 0;
        
        if (board.CheckWin(aiPlayer))
            return int.MaxValue - depth;

        if (board.CheckWin(humanPlayer))
            return int.MinValue + depth;

        if (depth == 0)
            return EvaluateBoard(board);

        List<(int, int)> moves = GetNearbyMoves(board);
        // 가장 유망한 수부터 두도록 정렬하여 알파-베타 가지치기가 더 많이 작동하도록 함
        moves.Sort((a, b) =>
            EvaluateMove(board, b, isMaximizing ? aiPlayer : humanPlayer)
                .CompareTo(
                    EvaluateMove(board, a, isMaximizing ? aiPlayer : humanPlayer))
        );
        
        
        if (isMaximizing)
        {
            int maxEval = int.MinValue;
            foreach (var move in moves)
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
            foreach (var move in moves)
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

    private int EvaluateMove(Board board, (int, int) move, int player)
    {
        board.PlaceMove(move.Item1, move.Item2, player);
        int score = EvaluateBoard(board);
        board.PlaceMove(move.Item1, move.Item2, 0);
        return score;
    }
    
    private int EvaluateBoard(Board board)
    {
        return EvaluatePlayer(board, aiPlayer) - EvaluatePlayer(board, humanPlayer);
    }
    
    private int EvaluatePlayer(Board board, int player)
    {
        int score = 0;
        
        for (int x = 0; x < board.SIZE; x++)
        {
            for (int y = 0; y < board.SIZE; y++)
            {
                score += EvaluateDirection(board, x, y, 1, 0, player); // 가로
                score += EvaluateDirection(board, x, y, 0, 1, player); // 세로
                score += EvaluateDirection(board, x, y, 1, 1, player); // 대각 /
                score += EvaluateDirection(board, x, y, 1, -1, player); // 대각 \
            }
        }
        return score;
    }
    
    private int EvaluateDirection(Board board, int x, int y, int dx, int dy, int player)
    {
        int size = board.SIZE;
        int[] line = new int[7];

        for (int i = -1; i <= 5; i++)
        {
            int nx = x + i * dx;
            int ny = y + i * dy;

            if (nx < 0 || ny < 0 || nx >= size || ny >= size)
                line[i + 1] = -99;
            else
                line[i + 1] = board[nx, ny];
        }

        return PatternScore(line, player);
    }
    
    private int PatternScore(int[] line, int player)
    {
        // 필요시 패턴 추가 필요 ( 모든 패턴을 추가하는 것보다 필요한 패턴만 추가하여 가중치를 조절 하는 것이 더 효과적일 수 있다...)
        // TODO: 쌍삼의 경우 단방향 패턴 검사인 PatternScore가 아닌 모든 방향에 대해 검사하는 EvaluatePlayer에서 추가해야 할 것으로 보임
        // TODO: 가중치 수정 필요!!!
        // 가중치에 따라 엉망이 될 수도 있음...
        
        int enemy = -player;

        // 열린 4: 0 P P P P 0
        if (line[0] == 0 && line[1] == player && line[2] == player && line[3] == player && line[4] == player && line[5] == 0)
            return 10000;
        
        // 가운데 비어있는 4: P P 0 P P
        if (line[1] == player && line[2] == player && line[3] == 0 && line[4] == player && line[5] == player)
            return 9000;

        // 닫힌 4: X P P P P 0 or 0 P P P P X
        if ((line[0] == enemy && line[1] == player && line[2] == player && line[3] == player && line[4] == player && line[5] == 0) ||
            (line[0] == 0 && line[1] == player && line[2] == player && line[3] == player && line[4] == player && line[5] == enemy))
            return 1000;

        // 열린 비연속 3: 0 P P 0 P 0
        if (line[0] == 0 && line[1] == player && line[2] == player && line[3] == 0 && line[4] == player && line[5] == 0)
            return 700;
        
        // 열린 비연속 3: 0 P 0 P P 0
        if (line[0] == 0 && line[1] == player && line[2] == 0 && line[3] == player && line[4] == player && line[5] == 0)
            return 700;
        
        // 열린 3: 0 P P P 0
        if (line[1] == 0 && line[2] == player && line[3] == player && line[4] == player && line[5] == 0)
            return 500;

        // 닫힌 3: X P P P 0 or 0 P P P X
        if ((line[1] == enemy && line[2] == player && line[3] == player && line[4] == player && line[5] == 0) ||
            (line[1] == 0 && line[2] == player && line[3] == player && line[4] == player && line[5] == enemy))
            return 100;

        // 열린 2: 0 P P 0
        if (line[2] == 0 && line[3] == player && line[4] == player && line[5] == 0)
            return 50;

        return 0;
    }
}
