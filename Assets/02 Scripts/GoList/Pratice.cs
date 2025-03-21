using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pratice : MonoBehaviour
{
    [SerializeField] public GameObject previewStone;
    [SerializeField] public RectTransform boardTransform;
    
    private float boardSize = 45f;
    
    private GameObject _previewStoneInstance;
    private ReplayManager _replayManagerPractice;
    private List<GameObject> _previewStones = new List<GameObject>();
    
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        _replayManagerPractice = GetComponent<ReplayManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(boardTransform, mousePos, null, out localPoint);
        
        float snappedX = Mathf.Round(localPoint.x / boardSize) * boardSize; // 45로 스냅
        float snappedY = Mathf.Round(localPoint.y / boardSize) * boardSize;
        
        snappedX = Mathf.Clamp(snappedX, -315, 315);
        snappedY = Mathf.Clamp(snappedY, -315, 315);
        
        if (_previewStoneInstance == null)
        {
            _previewStoneInstance = Instantiate(previewStone, boardTransform);
        }

        // 미리보기 스톤 위치 업데이트
        RectTransform stoneRect = _previewStoneInstance.GetComponent<RectTransform>();
        stoneRect.anchoredPosition = new Vector2(snappedX, snappedY);
        
        // 돌놓기
        if (Input.GetMouseButtonDown(0) && localPoint.y > -315 && localPoint.y < 315)
        {
            OnClickedBoard(stoneRect);
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < _previewStones.Count; i++)
        {
            Destroy(_previewStones[i]);
        }
        _replayManagerPractice.practiceCounter = 1;
        _previewStones.Clear();
        Destroy(_previewStoneInstance);
    }

    private void OnClickedBoard(RectTransform mousePos)
    {
        if (_replayManagerPractice.practiceCounter % 2 == 1)
        {
            GameObject practiceStone = Instantiate(_replayManagerPractice.blackMarkerPrefab, boardTransform);
            practiceStone.transform.position = mousePos.position;
            _previewStones.Add(practiceStone);
            _replayManagerPractice.practiceCounter++;
        }
        else
        {
            GameObject practiceStone = Instantiate(_replayManagerPractice.whiteMarkerPrefab, boardTransform);
            practiceStone.transform.position = mousePos.position;
            _previewStones.Add(practiceStone);
            _replayManagerPractice.practiceCounter++;
        }
        
    }
}
