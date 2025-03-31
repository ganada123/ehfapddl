using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// TODO: indexGameManager 구현 | OnButtonClicked함수에서도 수정해야함
    /// path 도 구현
    /// </summary>
    public int indexGameManager = 0;
    public string path = "";
    public int maxIndexGameManager = 0;
    /// <summary>
    /// TODO: 구현후 제거해주세요!! ⏳⏳⏳⏳⏳⏳⏳⏳⏳⏳⏳⏳⏳
    /// </summary>
    /// 
    public static GameManager Instance;

    public GameObject[] omokPoints;
    public GameObject cursorPrefab;
    public GameObject forbiddenPrefab;
    public Transform cursorParent;
    public Button placeStoneButton;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI timerText; // ⏳ 추가: 타이머 UI

    private GameObject currentCursor;
    private GameObject selectedPoint;
    private int currentPlayer = 1; // 1: 흑돌, 2: 백돌
    private bool isGameOver = false; // 게임이 끝났나 확인하는 변수 ->WinLosePanelController.cs
    public bool amIwin = false; //승패 전달하기 위한 변수->WinLosePanelController.cs
    public int wonPlayer = 0;

    private int[,] boardState = new int[15, 15];
    private GameObject[,] forbiddenMarkers = new GameObject[15, 15];
    
    private GameObject lastPlacedMarker;
    public GameObject lastPlacedMarkerPrefab;
    [SerializeField] private GameObject winLosePrefab;
    private int playerID = 1;
    
    private Coroutine turnTimerCoroutine;
    private float turnTimeLimit = 30f; // ⏳ 한 턴 30초 제한
    
    public AudioSource audioSource; // 소리를 재생할 AudioSource
    public AudioClip placeStoneClip; // 돌을 놓을 때 사운드
    public AudioClip tickTockClip; // 5초 이하일 때 틱톡 사운드
    
    private readonly Vector2Int[] directions = {
        new Vector2Int(1, 0),  // 가로 (→)
        new Vector2Int(0, 1),  // 세로 (↓)
        new Vector2Int(1, 1),  // 대각선 ↘
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
    }

    public void SelectPoint(GameObject point)
    {
        if (currentCursor != null)
            Destroy(currentCursor);

        currentCursor = Instantiate(cursorPrefab, point.transform.position, Quaternion.identity, cursorParent);
        selectedPoint = point;
    }

    private int lastPlacedX, lastPlacedY;

    public void PlaceStone()
    {
        if (selectedPoint == null) return;

        Point pointScript = selectedPoint.GetComponent<Point>();
        int x = pointScript.x;
        int y = pointScript.y;

        if (currentPlayer == 1 && IsForbidden(x, y))
        {
            return;
        }

        if (pointScript != null && !pointScript.IsOccupied())
        {
            pointScript.PlaceStone(currentPlayer);
            boardState[x, y] = currentPlayer;

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
            if (CheckWin(x, y))
            {
                return; // 승리 시 턴을 넘기지 않고 종료
            }
            SwitchTurn();
        }

        Destroy(currentCursor);
        selectedPoint = null;

        CheckForbiddenPoints();
        PrintBoardState();
    }

    private void SwitchTurn()
    {
        currentPlayer = (currentPlayer == 1) ? 2 : 1;
        UpdateTurnUI();
        RestartTurnTimer(); // ⏳ 턴이 바뀌면 타이머 다시 시작
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
        if (currentPlayer != 1) return;

        // ✅ 바둑판 전체를 검사하여 금수 위치 마커 표시
        for (int x = 0; x < 15; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                if (boardState[x, y] == 0 && IsForbidden(x, y))
                {
                    GameObject marker = Instantiate(forbiddenPrefab, omokPoints[x * 15 + y].transform.position, Quaternion.identity, cursorParent);
                    forbiddenMarkers[x, y] = marker;
                }
            }
        }
    }

    private bool IsForbidden(int x, int y)
    {
        bool isDoubleThree = CheckDoubleThree(x, y);
        bool isDoubleFour = CheckDoubleFour(x, y);
        bool isOverline = CheckOverline(x, y);

        if (isDoubleThree && isDoubleFour)
        {
            /*Debug.Log($"[{x}, {y}] 삼삼 + 사사 발생 → 금수 아님");*/
            return false;
        }

        /*if (isDoubleThree) Debug.Log($"[{x}, {y}] 삼삼 금수 감지!");
        if (isDoubleFour) Debug.Log($"[{x}, {y}] 사사 금수 감지!");
        if (isOverline) Debug.Log($"[{x}, {y}] 장목 금수 감지!");*/

        return isDoubleThree || isDoubleFour || isOverline;
    }

    private bool CheckDoubleThree(int x, int y)
    {
        int openThreeCount = 0;
        Vector2Int[] directions = { 
            Vector2Int.right, Vector2Int.up, 
            new Vector2Int(1, 1), new Vector2Int(1, -1) 
        };

        // **가상의 착수**
        boardState[x, y] = 1; 

        /*Debug.Log($"[{x}, {y}]에 가상의 흑돌 착수 후 검사 시작");*/

        foreach (Vector2Int dir in directions)
        {
            if (CountOpenThree(x, y, dir))
            {
                openThreeCount++;
                /*Debug.Log($"[삼삼 감지] ({x}, {y}) 방향 {dir} → 열린 삼(33) 발견!");*/
            }
        }

        // **원상복구**
        boardState[x, y] = 0; 

        /*Debug.Log($"[{x}, {y}] 검사 후 원상복구 완료, openThreeCount = {openThreeCount}");*/

        bool isDoubleThree = openThreeCount >= 2;
        if (isDoubleThree)
        {
            /*Debug.Log($"[{x}, {y}] 금수 (삼삼) 판정됨!");*/
        }

        return isDoubleThree;
    }
    
    private bool CountOpenThree(int x, int y, Vector2Int dir)
    {
        string line = GetExtendedLine(x, y, dir, 5); // 더 긴 범위 체크
        return IsOpenThree(line);
    }

    private bool IsOpenThree(string line) 
    {
        // 열린 삼(3,3) 패턴만 인식 (삼사, 사사는 제외)
        return (line.Contains("0011100") || // 일반적인 3,3
                line.Contains("011010") || line.Contains("010110") || 
                line.Contains("0100110") || line.Contains("0011010") || 
                line.Contains("001110") || line.Contains("01110") || 
                line.Contains("0110100") || line.Contains("001101") || 
                line.Contains("010011"))
               && !line.Contains("011110") // 열린 사(4)를 포함하면 3,3이 아님 (삼사 방지)
               && !line.Contains("11110") // 단순 4도 금수 아님
               && !line.Contains("01111") // 단순 4도 금수 아님
               && !line.Contains("1011101"); // 특수 케이스
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
            result += boardState[nx, ny].ToString();
        }

        // **가상의 착수**
        result += "1";

        // **정방향 추가**
        for (int i = 1; i <= length; i++)
        {
            int nx = x + dir.x * i, ny = y + dir.y * i;
            if (nx < 0 || ny < 0 || nx >= 15 || ny >= 15) break;
            result += boardState[nx, ny].ToString();
        }

        // **열린 형태 확인을 위해 양끝에 0 추가**
        result = "0" + result + "0";

        /*Debug.Log($"[{x}, {y}] 방향 {dir} → 검사된 문자열: {result}");*/

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
        // 열린 사(4,4)만 체크 (삼사는 포함 X)
        return line.Contains("011110") || line.Contains("211110") || 
               line.Contains("011112") || line.Contains("0101110") || 
               line.Contains("0111010") || line.Contains("0110110")
               && !line.Contains("001110"); // 3,3 방지
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
        string result = "0";  // 빈칸에서 시작
        for (int i = -4; i <= 4; i++)
        {
            int nx = x + dir.x * i, ny = y + dir.y * i;
            if (nx < 0 || ny < 0 || nx >= 15 || ny >= 15) break;
            result += boardState[nx, ny].ToString();
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
    
    private void PrintBoardState()
    {
        string boardString = "\n현재 오목판 상태:\n";

        for (int y = 14; y >= 0; y--) // 14부터 0까지 줄어드는 순서로 (위에서 아래로)
        {
            for (int x = 0; x < 15; x++) // 왼쪽에서 오른쪽으로
            {
                if (forbiddenMarkers[x, y] != null) boardString += "X "; // 금수 위치
                else if (boardState[x, y] == 1) boardString += "● "; // 흑돌
                else if (boardState[x, y] == 2) boardString += "○ "; // 백돌
                else boardString += ". "; // 빈칸
            }
            boardString += "\n";
        }

        Debug.Log(boardString);
    }
    private bool CheckWin(int x, int y)
    {
        int player = boardState[x, y]; // 현재 플레이어 (흑돌 or 백돌)

        foreach (Vector2Int dir in directions)
        {
            int count = 1;

            // 🔼 정방향 탐색
            count += CountStones(x, y, dir, player);
            // 🔽 역방향 탐색
            count += CountStones(x, y, -dir, player);

            if (count >= 5) // 5개 이상이면 승리
            {
                Debug.Log($"🎉 플레이어 {player} 승리! ({(player == 1 ? "흑돌" : "백돌")})");
                EndGame(player);
                return true;
            }
        }

        return false;
    }
    // 🚀 특정 방향으로 연속된 돌 개수 세기
    private int CountStones(int x, int y, Vector2Int dir, int player)
    {
        int count = 0;

        for (int i = 1; i < 5; i++) // 최대 4개 더 체크
        {
            int nx = x + dir.x * i;
            int ny = y + dir.y * i;

            if (nx < 0 || ny < 0 || nx >= 15 || ny >= 15) break; // 범위 초과
            if (boardState[nx, ny] != player) break; // 같은 색 돌이 아니면 중단

            count++;
        }

        return count;
    }
    
    public void EndGame(int winner)
    {
        if(isGameOver) return;
        isGameOver = true;
        Debug.Log($"🎉 게임 종료! { (winner == 1 ? "흑돌" : "백돌") } 승리!");
        wonPlayer = winner;

        Debug.Log(wonPlayer +"승리한 플레이어 1 흑돌 2 백돌");
        Debug.Log(playerID + "플레이어ID 1 흑돌 2 백돌");
        if (wonPlayer == playerID)
        {
            amIwin = true;
        }
        else
        {
            amIwin = false;
        }
        // UI 업데이트 (예: 승리 메시지 표시)
        if (turnText != null)
        {
            turnText.text = $"{(winner == 1 ? "흑돌" : "백돌")} 승리!";
        }
        Instantiate(winLosePrefab);

        // 💀 모든 입력 비활성화
        placeStoneButton.interactable = false;
        if (turnTimerCoroutine != null)
        {
            StopCoroutine(turnTimerCoroutine);
        }
    }
}