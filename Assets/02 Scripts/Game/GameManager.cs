using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }  // 싱글턴 인스턴스
    public GameObject[] omokPoints;  // 바둑판 위의 접점들
    public int boardSize = 15;  // 15x15 바둑판 크기
    private int currentPlayer = 1;  // 1: 흑돌, 2: 백돌

    void Awake()
    {
        // 싱글턴 패턴 적용 (중복 생성 방지)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // 바둑판의 모든 접점 오브젝트를 찾아 배열에 자동 할당
        omokPoints = new GameObject[boardSize * boardSize];

        for (int i = 0; i < omokPoints.Length; i++)
        {
            string objectName = (i == 0) ? "Image" : $"Image ({i})";
            omokPoints[i] = GameObject.Find(objectName);

            if (omokPoints[i] == null)
            {
                Debug.LogError($"오브젝트 '{objectName}'을(를) 찾을 수 없습니다! 이름을 확인하세요.");
            }
        }
    }

    public int GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public void ChangeTurn()
    {
        currentPlayer = (currentPlayer == 1) ? 2 : 1;
        Debug.Log($"현재 턴: {(currentPlayer == 1 ? "흑돌" : "백돌")}");
    }
}
