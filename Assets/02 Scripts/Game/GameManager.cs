using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public GameObject[] omokPoints;
    public GameObject cursorPrefab;
    public GameObject forbiddenPrefab;
    public Transform cursorParent;
    public Button placeStoneButton;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI timerText;

    private GameObject currentCursor;
    private GameObject selectedPoint;
    private int currentPlayer = 1; // 1: 흑돌, 2: 백돌
    private (int, int) aiPoint = (0, 0);

    private int[,] boardState = new int[15, 15];
    private GameObject[,] forbiddenMarkers = new GameObject[15, 15];

    private GameObject lastPlacedMarker;
    public GameObject lastPlacedMarkerPrefab;

    private Coroutine turnTimerCoroutine;
    private float turnTimeLimit = 30f;

    public AudioSource audioSource;
    public AudioClip placeStoneClip;
    public AudioClip tickTockClip;

    private bool isPlayer = true;
    private AIController aiController;
    public int aiDifficultyLevel = 2; // 기본 난이도 설정

    private readonly Vector2Int[] directions = {
        new Vector2Int(1, 0), new Vector2Int(0, 1),
        new Vector2Int(1, 1), new Vector2Int(1, -1)
    };

    void Start()
    {
        omokPoints = new GameObject[225];

        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                int index = i * 15 + j;
                omokPoints[index] = GameObject.Find("Image (" + index + ")");

                if (omokPoints[index] != null)
                {
                    Point pointScript = omokPoints[index].GetComponent<Point>();
                    if (pointScript != null)
                    {
                        pointScript.x = i;
                        pointScript.y = j;
                    }
                }
            }
        }

        placeStoneButton.onClick.AddListener(PlaceStone);
        UpdateTurnUI();
        StartTurnTimer();

        // AI 활성화 (매칭 실패 시 등 조건에 따라 활성화)
        EnableAI();
    }

    public void EnableAI()
    {
        aiController = new AIController(aiDifficultyLevel);
    }

    public void SelectPoint(int x, int y)
    {
        if (currentPlayer == 2) return; // AI 차례일 때 선택 방지

        GameObject point = omokPoints[x * 15 + y];
        if (currentCursor != null)
            Destroy(currentCursor);

        currentCursor = Instantiate(cursorPrefab, point.transform.position, Quaternion.identity, cursorParent);
        selectedPoint = point;
    }

    public void PlaceStone()
    {
        int x = -1, y = -1;

        if (currentPlayer == 1) // 플레이어 차례
        {
            if (selectedPoint == null) return;

            Point pointScript = selectedPoint.GetComponent<Point>();
            if (pointScript == null || pointScript.IsOccupied()) return;

            x = pointScript.x;
            y = pointScript.y;
        }
        else // AI 차례
        {
            (x, y) = aiController.GetBestMove(boardState);

            if (x == -1 || y == -1 || boardState[x, y] != 0) return; // AI가 유효한 수를 찾지 못했거나 이미 놓인 자리인 경우 방지
        }

        if (currentPlayer == 1 && IsForbidden(x, y)) return;

        boardState[x, y] = currentPlayer;
        omokPoints[x * 15 + y].GetComponent<Point>().PlaceStone(currentPlayer);

        if (lastPlacedMarker != null)
        {
            Destroy(lastPlacedMarker);
        }
        lastPlacedMarker = Instantiate(lastPlacedMarkerPrefab, omokPoints[x * 15 + y].transform.position, Quaternion.identity, cursorParent);

        PlaySound(placeStoneClip);

        if (CheckWin(x, y)) return;

        SwitchTurn();
        CheckForbiddenPoints();

        Destroy(currentCursor);
        selectedPoint = null;

        // AI 턴이 되면 자동으로 다음 수 놓기
        if (currentPlayer == 2 && aiController != null)
        {
            // 약간의 딜레이를 주어 AI가 생각하는 것처럼 보이게 함
            Invoke(nameof(PlaceStone), 0.8f);
        }
    }

    private void SwitchTurn()
    {
        currentPlayer = (currentPlayer == 1) ? 2 : 1;
        UpdateTurnUI();
        RestartTurnTimer();
    }

    private void UpdateTurnUI()
    {
        if (turnText != null)
        {
            turnText.text = (currentPlayer == 1) ? "흑돌 차례입니다." : "백돌 차례입니다.";
        }
    }

    private void StartTurnTimer()
    {
        if (turnTimerCoroutine != null)
        {
            StopCoroutine(turnTimerCoroutine);
        }
        turnTimerCoroutine = StartCoroutine(TurnTimerCoroutine());
    }

    private void RestartTurnTimer()
    {
        if (turnTimerCoroutine != null)
        {
            StopCoroutine(turnTimerCoroutine);
        }
        turnTimerCoroutine = StartCoroutine(TurnTimerCoroutine());
    }

    private IEnumerator TurnTimerCoroutine()
    {
        float timeLeft = turnTimeLimit;

        while (timeLeft > 0)
        {
            if (timerText != null)
            {
                int displayTime = Mathf.CeilToInt(timeLeft);
                timerText.text = $"남은 시간: {displayTime}초";
                timerText.color = (displayTime <= 10) ? Color.red : Color.black;

                if (displayTime == 5)
                {
                    PlaySound(tickTockClip);
                }
            }

            yield return new WaitForSeconds(1f);
            timeLeft -= 1f;
        }

        Debug.Log("⏳ 시간이 초과되었습니다. 자동으로 턴을 넘깁니다.");
        SwitchTurn();
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private bool CheckWin(int x, int y)
    {
        int player = boardState[x, y];

        foreach (Vector2Int dir in directions)
        {
            int count = 1 + CountStones(x, y, dir, player) + CountStones(x, y, -dir, player);

            if (count >= 5)
            {
                Debug.Log($"🎉 플레이어 {player} 승리!");
                EndGame(player);
                return true;
            }
        }
        return false;
    }

    private int CountStones(int x, int y, Vector2Int dir, int player)
    {
        int count = 0;
        for (int i = 1; i < 5; i++)
        {
            int nx = x + dir.x * i, ny = y + dir.y * i;
            if (nx < 0 || ny < 0 || nx >= 15 || ny >= 15 || boardState[nx, ny] != player) break;
            count++;
        }
        return count;
    }

    private void EndGame(int winner)
    {
        Debug.Log($"🎉 게임 종료! {(winner == 1 ? "흑돌" : "백돌")} 승리!");
        turnText.text = $"{(winner == 1 ? "흑돌" : "백돌")} 승리!";
        placeStoneButton.interactable = false;
        if (turnTimerCoroutine != null)
        {
            StopCoroutine(turnTimerCoroutine);
        }
    }
    private bool IsForbidden(int x, int y)
    {
        if (currentPlayer != 1) return false; // 흑돌만 금수 체크
        return IsThreeThree(x, y) || IsFourFour(x, y) || IsOverline(x, y);
    }
    private void CheckForbiddenPoints()
    {
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                if (forbiddenMarkers[i, j] != null)
                {
                    Destroy(forbiddenMarkers[i, j]);
                }

                if (IsForbidden(i, j))
                {
                    forbiddenMarkers[i, j] = Instantiate(forbiddenPrefab, omokPoints[i * 15 + j].transform.position, Quaternion.identity, cursorParent);
                }
            }
        }
    }
    private bool IsThreeThree(int x, int y)
    {
        int openThreeCount = 0;
        Vector2Int[] directions = {
            Vector2Int.right, Vector2Int.up,
            new Vector2Int(1, 1), new Vector2Int(1, -1)
        };

        // 가상의 흑돌 착수
        boardState[x, y] = 1;

        foreach (Vector2Int dir in directions)
        {
            if (CountOpenThree(x, y, dir)) openThreeCount++;
        }

        // 원상복구
        boardState[x, y] = 0;

        return openThreeCount >= 2; // 열린 삼(33)이 2개 이상이면 금수
    }

    private bool CountOpenThree(int x, int y, Vector2Int dir)
    {
        string line = GetExtendedLine(x, y, dir, 5); // 더 긴 범위 체크
        return IsOpenThree(line);
    }

    private bool IsOpenThree(string line)
    {
        // 열린 삼(3,3) 패턴만 인식 (삼사, 사사는 제외)
        return (line.Contains("0011100") ||
                line.Contains("011010") || line.Contains("010110") ||
                line.Contains("0100110") || line.Contains("0011010") ||
                line.Contains("001110") || line.Contains("01110") ||
                line.Contains("0110100") || line.Contains("001101") ||
                line.Contains("010011"))
               && !line.Contains("011110") // 열린 사(4)를 포함하면 3,3이 아님 (삼사 방지)
               && !line.Contains("11110")
               && !line.Contains("01111")
               && !line.Contains("1011101");
    }
    private bool IsFourFour(int x, int y)
    {
        int openFourCount = 0;
        Vector2Int[] directions = { Vector2Int.right, Vector2Int.up, new Vector2Int(1, 1), new Vector2Int(1, -1) };

        foreach (var dir in directions)
        {
            if (CountOpenFour(x, y, dir)) openFourCount++;
        }

        return openFourCount >= 2; // 열린 사(44)가 2개 이상이면 금수
    }

    private bool CountOpenFour(int x, int y, Vector2Int direction)
    {
        string line = GetExtendedLine(x, y, direction, 6); // 6칸 범위 검사
        return IsOpenFour(line);
    }

    private bool IsOpenFour(string line)
    {
        return (line.Contains("011110") || line.Contains("211110") ||
                line.Contains("011112") || line.Contains("0101110") ||
                line.Contains("0111010") || line.Contains("0110110"))
               && !line.Contains("001110"); // 삼삼 방지
    }
    private bool IsOverline(int x, int y)
    {
        return GetMaxLength(x, y) >= 6;
    }

    private int GetMaxLength(int x, int y)
    {
        int maxLength = 1;
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, new Vector2Int(1, 1), new Vector2Int(1, -1) };

        foreach (Vector2Int dir in directions)
        {
            int length = 1; // 착수 위치 포함
            for (int i = 1; i <= 5; i++)
            {
                int nx = x + dir.x * i, ny = y + dir.y * i;
                if (nx < 0 || ny < 0 || nx >= 15 || ny >= 15) break;
                if (boardState[nx, ny] == 1) length++;
                else break;
            }
            for (int i = -1; i >= -5; i--)
            {
                int nx = x + dir.x * i, ny = y + dir.y * i;
                if (nx < 0 || ny < 0 || nx >= 15 || ny >= 15) break;
                if (boardState[nx, ny] == 1) length++;
                else break;
            }
            maxLength = Mathf.Max(maxLength, length);
        }
        return maxLength;
    }
    private string GetExtendedLine(int x, int y, Vector2Int dir, int length)
    {
        string result = "";

        // 🔹 반대 방향 먼저 추가
        for (int i = length; i > 0; i--)
        {
            int nx = x - dir.x * i, ny = y - dir.y * i;
            if (nx < 0 || ny < 0 || nx >= 15 || ny >= 15) break;
            result += boardState[nx, ny].ToString();
        }

        // 🔹 가상의 착수 (현재 위치)
        result += "1";

        // 🔹 정방향 추가
        for (int i = 1; i <= length; i++)
        {
            int nx = x + dir.x * i, ny = y + dir.y * i;
            if (nx < 0 || ny < 0 || nx >= 15 || ny >= 15) break;
            result += boardState[nx, ny].ToString();
        }

        // 🔹 열린 형태 확인을 위해 양끝에 0 추가
        result = "0" + result + "0";

        return result;
    }
    public int CalculateBoardScore(int[,] board, int player)
    {
        int score = 0;
        int targetPlayer = (player == 1) ? 1 : 2; // 평가 대상 플레이어
        int opponentPlayer = (player == 1) ? 2 : 1; // 상대 플레이어

        // 4가지 방향 (→, ↓, ↘, ↗)
        Vector2Int[] directions = { Vector2Int.right, Vector2Int.up, new Vector2Int(1, 1), new Vector2Int(1, -1) };

        for (int x = 0; x < 15; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                // 평가 대상 플레이어의 돌에 대한 점수
                if (board[x, y] == targetPlayer)
                {
                    foreach (var dir in directions)
                    {
                        string line = GetLinePattern(board, x, y, dir, targetPlayer);
                        score += EvaluateLine(line);
                    }
                }
                // 상대 플레이어의 돌에 대한 방어 점수 (상대방의 좋은 수를 방해하는 것이 중요)
                else if (board[x, y] == opponentPlayer)
                {
                    foreach (var dir in directions)
                    {
                        string line = GetLinePattern(board, x, y, dir, opponentPlayer);
                        score -= (int)(EvaluateLine(line) * 0.8f); // 상대방의 좋은 수에 약간의 가중치를 두어 방어
                    }
                }
            }
        }
        return Mathf.RoundToInt(score);
    }
    private string GetLinePattern(int[,] board, int x, int y, Vector2Int dir, int player)
    {
        string result = "";

        // 역방향 탐색
        for (int i = 4; i >= 1; i--)
        {
            int nx = x - dir.x * i, ny = y - dir.y * i;
            if (nx < 0 || ny < 0 || nx >= 15 || ny >= 15)
            {
                result += "2"; // 벽으로 간주
                continue;
            }
            result += board[nx, ny] == player ? "1" : (board[nx, ny] == 0 ? "0" : "2");
        }

        // 자기 돌
        result += "1";

        // 정방향 탐색
        for (int i = 1; i <= 4; i++)
        {
            int nx = x + dir.x * i, ny = y + dir.y * i;
            if (nx < 0 || ny < 0 || nx >= 15 || ny >= 15)
            {
                result += "2"; // 벽으로 간주
                continue;
            }
            result += board[nx, ny] == player ? "1" : (board[nx, ny] == 0 ? "0" : "2");
        }

        return result;
    }
    private int EvaluateLine(string line)
    {
        if (line.Contains("11111")) return 100000; // 승리
        if (line.Contains("011110")) return 10000; // 열린 4
        if (line.Contains("011112") || line.Contains("211110")) return 5000; // 막힌 4 (한쪽)
        if (line.Contains("01110")) return 2000; // 열린 3
        if (line.Contains("011010") || line.Contains("010110")) return 1500; // 뚫린 3
        if (line.Contains("1111")) return 1000; // 막힌 4 (양쪽)
        if (line.Contains("211112")) return 800; // 꽉 막힌 4
        if (line.Contains("1110")) return 500; // 막힌 3 (한쪽)
        if (line.Contains("0111")) return 500; // 막힌 3 (다른쪽)
        if (line.Contains("001100")) return 200; // 열린 2
        if (line.Contains("110")) return 100; // 막힌 2 (한쪽)
        if (line.Contains("011")) return 100; // 막힌 2 (다른쪽)
        return 0;
    }
    public List<Vector2Int> GetValidMoves(int player)
    {
        List<Vector2Int> validMoves = new List<Vector2Int>();

        for (int x = 0; x < 15; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                if (boardState[x, y] == 0) // 빈 칸
                {
                    if (player == 1) // 흑돌일 경우 금수 여부 확인
                    {
                        if (!IsForbidden(x, y))
                        {
                            validMoves.Add(new Vector2Int(x, y));
                        }
                    }
                    else // 백돌일 경우 금수 없음
                    {
                        validMoves.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        return validMoves;
    }
    public int CheckWinCondition(int[,] board, int player)
    {
        for (int x = 0; x < 15; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                if (board[x, y] == player && CheckWin(x, y, board))
                {
                    return player; // 승리한 플레이어 반환 (1: 흑돌, 2: 백돌)
                }
            }
        }
        return 0; // 승리 없음
    }

    private bool CheckWin(int x, int y, int[,] currentBoard)
    {
        int player = currentBoard[x, y];

        foreach (Vector2Int dir in directions)
        {
            int count = 1 + CountStones(x, y, dir, player, currentBoard) + CountStones(x, y, -dir, player, currentBoard);

            if (count >= 5)
            {
                Debug.Log($"🎉 플레이어 {player} 승리!");
                EndGame(player);
                return true;
            }
        }
        return false;
    }

    private int CountStones(int x, int y, Vector2Int dir, int player, int[,] currentBoard)
    {
        int count = 0;
        for (int i = 1; i < 5; i++)
        {
            int nx = x + dir.x * i, ny = y + dir.y * i;
            if (nx < 0 || ny < 0 || nx >= 15 || ny >= 15 || currentBoard[nx, ny] != player) break;
            count++;
        }
        return count;
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        throw new System.NotImplementedException();
    }
}