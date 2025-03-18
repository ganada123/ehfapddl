using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] omokPoints;  // 바둑판 위의 접점들
    public int boardSize = 15;  // 15x15 바둑판 크기
    private int currentPlayer = 1;  // 1: 흑돌, 2: 백돌

    void Start()
    {
        // 바둑판의 모든 접점 오브젝트를 찾아 배열에 자동으로 할당
        omokPoints = new GameObject[boardSize * boardSize];  // 15x15 크기만큼 배열 크기 설정

        // "Image (1)"부터 "Image (225)"까지의 이름을 가진 오브젝트들을 배열에 할당
        omokPoints[0] = GameObject.Find("Image");
        for (int i = 1; i < omokPoints.Length; i++)
        {
            string objectName = "Image (" + (i) + ")";  // "Image (1)", "Image (2)" 형태의 이름 생성
            omokPoints[i] = GameObject.Find(objectName);  // 이름을 통해 해당 오브젝트를 찾고 배열에 할당
        }

        // 이제 omokPoints 배열에는 모든 "Image (1)"부터 "Image (225)"까지의 오브젝트들이 자동으로 들어감
    }

    public void OnPointClick(GameObject clickedPoint)
    {
        Point pointScript = clickedPoint.GetComponent<Point>();
        pointScript.OnClick(currentPlayer);

        // 턴 변경
        currentPlayer = (currentPlayer == 1) ? 2 : 1;
    }
}
