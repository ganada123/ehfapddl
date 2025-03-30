using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject[] omokPoints;
    public GameObject cursorPrefab;
    public GameObject forbiddenPrefab;
    public Transform cursorParent;
    public Button placeStoneButton;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI timerText; // ⏳ 추가: 타이머 UI
    public int aiDifficultyLevel = 1; // AI 난이도 (Inspector에서 설정 가능)

    private GameObject currentCursor;
    private GameObject selectedPoint;
    private int currentPlayer = -1; // -1: 흑돌 (사용자), 1: 백돌 (AI)

    private int[,] boardStateInternal = new int[15, 15]; // 내부 보드 상태
    private Board board; // Board 클래스 인스턴스
    private AIController aiController;
    private GameObject[,] forbiddenMarkers = new GameObject[15, 15];

    private GameObject lastPlacedMarker;
    public GameObject lastPlacedMarkerPrefab;

    private Coroutine turnTimerCoroutine;
    private float turnTimeLimit = 30f; // ⏳ 한 턴 30초 제한

    public AudioSource audioSource; // 소리를 재생할 AudioSource
    public AudioClip placeStoneClip; // 돌을 놓을 때 사운드
    public AudioClip tickTockClip; // 5초 이하일 때 틱톡 사운드

    private readonly Vector2Int[] directions = {
        new Vector2Int(1, 0),   // 가로 (→)
        new Vector2Int(0, 1),   // 세로 (↓)
        new Vector2Int(1, 1),   // 대각선 ↘
        new Vector2Int(1, -1)  // 대각선 ↙
    };

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        board = new Board(boardStateInternal, this);
        aiController = new AIController(aiDifficultyLevel);

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

        // AI가 먼저 시작하는 경우 (선택 사항)
        // if (currentPlayer == 1)
        // {
        //     PlayAI();
        // }
    }

    public void SelectPoint(GameObject point)
    {
        if (currentCursor != null)
            Destroy(currentCursor);

        currentCursor = Instantiate(cursorPrefab, point.transform.position, Quaternion.identity, cursorParent);
        selectedPoint = point;
    }

    private int lastPlacedX, lastPlacedY;
    public string path;

    public void PlaceStone()
    {
        if (selectedPoint == null) return;

        Point pointScript = selectedPoint.GetComponent<Point>();
        int x = pointScript.x;
        int y = pointScript.y;
        int previousPlayer = currentPlayer; // 현재 플레이어를 저장

        if (currentPlayer == -1 && IsForbiddenForAI(x, y)) // 사용자(흑돌) 금수 체크
        {
            return;
        }

        if (pointScript != null && !pointScript.IsOccupied())
        {
            pointScript.PlaceStone(currentPlayer);
            board.PlaceMove(x, y, currentPlayer);
            boardStateInternal[x, y] = currentPlayer; // 내부 상태 업데이트 (필요한 경우)

            lastPlacedX = x;
            lastPlacedY = y;

            if (lastPlacedMarker != null)
            {
                Destroy(lastPlacedMarker);
            }
            lastPlacedMarker = Instantiate(lastPlacedMarkerPrefab, selectedPoint.transform.position, Quaternion.identity, cursorParent);

            // ✅ **돌을 놓을 때 소리 재생**
            PlaySound(placeStoneClip);

            // ✅ **오목 승리 체크**
            if (CheckWin(currentPlayer))
            {
                return; // 승리 시 턴을 넘기지 않고 종료
            }

            // 사용자가 돌을 놓았을 때만 턴을 넘김
            if (previousPlayer == -1)
            {
                SwitchTurn();
            }
            // AI가 돌을 놓았을 때는 이미 턴이 사용자에게 넘어간 상태여야 함 (PlayAITurn 코루틴 종료 시점)
        }

        Destroy(currentCursor);
        selectedPoint = null;

        CheckForbiddenPoints();
        PrintBoardState();
    }

    private void SwitchTurn()
    {
        currentPlayer = (currentPlayer == -1) ? 1 : -1;
        UpdateTurnUI();
        RestartTurnTimer(); // ⏳ 턴이 바뀌면 타이머 다시 시작
        if (currentPlayer == 1)
        {
            StartCoroutine(PlayAITurn());
        }
    }
    private void UpdateTurnUI()
    {
        if (turnText != null)
        {
            turnText.text = (currentPlayer == -1) ? "흑돌 차례입니다." : "백돌 차례입니다.";
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
        StopCoroutine(turnTimerCoroutine);
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

                // ⏳ 시간이 10초 이하이면 글씨 색을 빨간색으로 변경
                if (displayTime <= 10)
                    timerText.color = Color.red;
                else
                    timerText.color = Color.black; // 기본 색상 (검정)
                if (displayTime <= 5)
                    PlaySound(tickTockClip);
            }

            yield return new WaitForSeconds(0.1f);
            timeLeft -= 0.1f;
        }

        // ⏳ 시간이 다 되면 자동으로 턴을 넘김
        Debug.Log("⏳ 시간이 초과되었습니다. 자동으로 턴을 넘깁니다.");
        SwitchTurn();
        // AI 턴으로 넘어갔으므로 AI 플레이
        if (currentPlayer == 1)
        {
            StartCoroutine(PlayAITurn());
        }
    }
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void CheckForbiddenPoints()
    {
        // 기존 금수 마커 제거
        for (int x = 0; x < 15; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                if (forbiddenMarkers[x, y] != null)
                {
                    Destroy(forbiddenMarkers[x, y]);
                    forbiddenMarkers[x, y] = null;
                }
            }
        }

        // 백돌 차례면 검사 안 함
        if (currentPlayer != -1) return;

        // ✅ 바둑판 전체를 검사하여 금수 위치 마커 표시
        for (int x = 0; x < 15; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                if (boardStateInternal[x, y] == 0 && IsForbiddenForAI(x, y))
                {
                    GameObject marker = Instantiate(forbiddenPrefab, omokPoints[x * 15 + y].transform.position, Quaternion.identity, cursorParent);
                    forbiddenMarkers[x, y] = marker;
                }
            }
        }
    }

    // AIController에서 호출할 금수 판정 메서드 (흑돌 기준)
    public bool IsForbiddenForAI(int x, int y)
    {
        bool isDoubleThree = CheckDoubleThree(x, y);
        bool isDoubleFour = CheckDoubleFour(x, y);
        bool isOverline = CheckOverline(x, y);

        if (isDoubleThree && isDoubleFour)
        {
            return false;
        }

        return isDoubleThree || isDoubleFour || isOverline;
    }

    private bool IsForbidden(int x, int y) // 기존 PlaceStone에서 사용하던 메서드 (더 이상 사용 안 함)
    {
        return IsForbiddenForAI(x, y);
    }

    private bool CheckDoubleThree(int x, int y)
    {
        int openThreeCount = 0;
        Vector2Int[] directions = {
            Vector2Int.right, Vector2Int.up,
            new Vector2Int(1, 1), new Vector2Int(1, -1)
        };

        // **가상의 착수**
        boardStateInternal[x, y] = -1;

        foreach (Vector2Int dir in directions)
        {
            if (CountOpenThree(x, y, dir))
            {
                openThreeCount++;
            }
        }

        // **원상복구**
        boardStateInternal[x, y] = 0;

        bool isDoubleThree = openThreeCount >= 2;
        return isDoubleThree;
    }

    private bool CountOpenThree(int x, int y, Vector2Int dir)
    {
        string line = GetExtendedLine(x, y, dir, 5); // 더 긴 범위 체크
        return IsOpenThree(line);
    }

    private bool IsOpenThree(string line)
    {
        return (line.Contains("00-1-1-100") || // 일반적인 3,3 (흑돌: -1)
                line.Contains("0-1-10-10") || line.Contains("0-10-1-10") ||
                line.Contains("0-100-1-10") || line.Contains("00-1-10-10") ||
                line.Contains("00-1-1-10") || line.Contains("0-1-1-10") ||
                line.Contains("0-1-10-100") || line.Contains("00-1-10-1") ||
                line.Contains("0-100-1-1"))
               && !line.Contains("0-1-1-1-10") // 열린 사(4)를 포함하면 3,3이 아님 (삼사 방지)
               && !line.Contains("-1-1-1-10") // 단순 4도 금수 아님
               && !line.Contains("0-1-1-1-1") // 단순 4도 금수 아님
               && !line.Contains("-10-1-1-10-1"); // 특수 케이스
    }

    // 특정 방향의 돌 상태를 문자열로 변환 (5칸 길이 체크)
    private string GetExtendedLine(int x, int y, Vector2Int dir, int length)
    {
        string result = "";

        // **반대 방향 먼저 추가**
        for (int i = length; i > 0; i--)
        {
            int nx = x - dir.x * i, ny = y - dir.y * i;
            if (nx < 0 || ny < 0 || nx >= 15 || ny >= 15) break;
            result += boardStateInternal[nx, ny].ToString();
        }

        // **가상의 착수**
        result += "-1";

        // **정방향 추가**
        for (int i = 1; i <= length; i++)
        {
            int nx = x + dir.x * i, ny = y + dir.y * i;
            if (nx < 0 || ny < 0 || nx >= 15 || ny >= 15) break;
            result += boardStateInternal[nx, ny].ToString();
        }

        // **열린 형태 확인을 위해 양끝에 0 추가**
        result = "0" + result + "0";

        return result;
    }

    private bool CheckDoubleFour(int x, int y)
    {
        int openFourCount = 0;

        foreach (var dir in directions) // 4가지 방향 검사
        {
            openFourCount += CountOpenFour(x, y, dir);
        }

        return openFourCount >= 2; // 열린 사(44) 개수가 2개 이상이면 금수!
    }

    private int CountOpenFour(int x, int y, Vector2Int direction)
    {
        string line = GetExtendedLine(x, y, direction, 6); // 6칸 범위 검사
        int count = 0;

        if (IsOpenFour(line)) count++;

        return count;
    }
    private bool IsOpenFour(string line)
    {
        return line.Contains("0-1-1-1-10") || line.Contains("1-1-1-1-10") ||
               line.Contains("0-1-1-1-11") || line.Contains("0-10-1-1-10") ||
               line.Contains("0-1-1-10-10") || line.Contains("0-1-10-1-10")
               && !line.Contains("00-1-1-10"); // 3,3 방지
    }
    private int CountPattern(int x, int y, string pattern)
    {
        int count = 0;
        foreach (Vector2Int dir in new Vector2Int[] { Vector2Int.up, Vector2Int.right, new Vector2Int(1, 1), new Vector2Int(1, -1) })
        {
            string line = "0" + GetLine(x, y, dir) + "0"; // 가상의 착수 위치 추가
            if (line.Contains(pattern)) count++;
        }
        return count;
    }
    private string GetLine(int x, int y, Vector2Int dir)
    {
        string result = "0";   // 빈칸에서 시작
        for (int i = -4; i <= 4; i++)
        {
            int nx = x + dir.x * i, ny = y + dir.y * i;
            if (nx < 0 || ny < 0 || nx >= 15 || ny >= 15) break;
            result += boardStateInternal[nx, ny].ToString();
        }
        return result;
    }

    private bool CheckOverline(int x, int y)
    {
        return GetMaxLength(x, y) >= 6;
    }

    private int GetMaxLength(int x, int y)
    {
        int maxLength = 1;
        foreach (Vector2Int dir in new Vector2Int[] { Vector2Int.up, Vector2Int.right, new Vector2Int(1, 1), new Vector2Int(1, -1) })
        {
            int length = 1; // 착수 위치 포함
            for (int i = 1; i <= 5; i++)
            {
                int nx = x + dir.x * i, ny = y + dir.y * i;
                if (nx < 0 || ny < 0 || nx >= 15 || ny >= 15) break;
                if (boardStateInternal[nx, ny] == -1) length++;
                else break;
            }
            for (int i = -1; i >= -5; i--)
            {
                int nx = x + dir.x * i, ny = y + dir.y * i;
                if (nx < 0 || ny < 0 || nx >= 15 || ny >= 15) break;
                if (boardStateInternal[nx, ny] == -1) length++;
                else break;
            }
            maxLength = Mathf.Max(maxLength, length);
        }
        return maxLength;
    }

    private void PrintBoardState()
    {
        string boardString = "\n현재 오목판 상태:\n";

        for (int y = 14; y >= 0; y--) // 14부터 0까지 줄어드는 순서로 (위에서 아래로)
        {
            for (int x = 0; x < 15; x++) // 왼쪽에서 오른쪽으로
            {
                if (forbiddenMarkers[x, y] != null) boardString += "X "; // 금수 위치
                else if (boardStateInternal[x, y] == -1) boardString += "● "; // 흑돌
                else if (boardStateInternal[x, y] == 1) boardString += "○ "; // 백돌
                else boardString += ". "; // 빈칸
            }
            boardString += "\n";
        }

        Debug.Log(boardString);
    }

    // AIController에서 호출할 승리 판정 메서드 (AI 플레이어: 1)
    public bool CheckWin(int player)
    {
        Debug.Log($"CheckWin Called for player: {player} ({(player == 1 ? "백돌" : "흑돌")})"); // ◀◀◀ 추가
        int internalPlayer = (player == 1) ? 1 : -1; // AIController의 플레이어 표현에 맞춤
        for (int x = 0; x < 15; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                if (boardStateInternal[x, y] == internalPlayer)
                {
                    foreach (Vector2Int dir in directions)
                    {
                        int count = 1;
                        count += CountStonesInternal(x, y, dir, internalPlayer);
                        count += CountStonesInternal(x, y, -dir, internalPlayer);
                        if (count >= 5)
                        {
                            Debug.Log($"🎉 CheckWin: 플레이어 {player} 승리! ({(player == 1 ? "백돌" : "흑돌")}) at ({x}, {y})"); // ◀◀◀ 추가
                            EndGame(internalPlayer);
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    

    // 내부적으로 돌 개수를 세는 메서드
    private int CountStonesInternal(int x, int y, Vector2Int dir, int player)
    {
        int count = 0;
        for (int i = 1; i < 5; i++)
        {
            int nx = x + dir.x * i;
            int ny = y + dir.y * i;
            if (nx < 0 || ny < 0 || nx >= 15 || ny >= 15) break;
            if (boardStateInternal[nx, ny] == player) count++;
            else break;
        }
        return count;
    }

    private void EndGame(int winner)
    {
        Debug.Log($"🎉 EndGame Called! Winner: {winner} ({(winner == -1 ? "흑돌" : "백돌")})"); // ◀◀◀ 추가

        // UI 업데이트 (예: 승리 메시지 표시)
        if (turnText != null)
        {
            turnText.text = $"{(winner == -1 ? "흑돌" : "백돌")} 승리!";
        }

        // 💀 모든 입력 비활성화
        placeStoneButton.interactable = false;
        if (turnTimerCoroutine != null)
        {
            StopCoroutine(turnTimerCoroutine);
        }
    }

    // AI가 돌을 놓는 코루틴
    private IEnumerator PlayAITurn()
    {
        Debug.Log("AI Turn Started");
        yield return new WaitForSeconds(0.5f);

        Board currentBoard = new Board(boardStateInternal, this);

        try
        {
            Debug.Log($"AIController instance hash: {aiController.GetHashCode()}");
            (int, int) bestMove = aiController.GetBestMove(currentBoard);
            Debug.Log($"AI Best Move: ({bestMove.Item1}, {bestMove.Item2})");

            if (bestMove.Item1 != -1)
            {
                int x = bestMove.Item1;
                int y = bestMove.Item2;
                Debug.Log($"AI Selected Point: ({x}, {y})");
                Point pointScript = omokPoints[x * 15 + y].GetComponent<Point>();
                if (pointScript != null && !pointScript.IsOccupied())
                {
                    selectedPoint = omokPoints[x * 15 + y];
                    PlaceStone(); // AI가 돌을 놓음
                }
                else
                {
                    Debug.LogWarning("AI가 선택한 위치가 유효하지 않습니다.");
                    SwitchTurn(); // 이 부분은 상황에 따라 다르게 처리할 수 있습니다.
                }
            }
            else
            {
                Debug.LogWarning("AI가 둘 곳이 없습니다.");
                SwitchTurn(); // 이 부분은 상황에 따라 다르게 처리할 수 있습니다.
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"AI Turn Error: {e}");
        }

        // AI의 턴이 끝났으므로 다시 사용자에게 턴을 넘깁니다.
        if (!CheckWin(currentPlayer))  // AI가 이기지 않았으면 턴 넘긴다
        {
            SwitchTurn(); 
        }
    }
}