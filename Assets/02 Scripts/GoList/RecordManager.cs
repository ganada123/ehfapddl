using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecordManager : MonoBehaviour
{
    private GameRecord _gameRecord = new GameRecord();
    string _timestamp = System.DateTime.Now.ToString("yy년 MM월 dd일 HH시 mm분");
    //private Stack<OmokData> _redoStack = new Stack<OmokData>();
    
    private int _turnNumber = 0;
    private int _maxSave = 20;
    private string _playerName = "Black"; // 테스트용
    //private string _playerColor = "";

    private void Start()
    {
        PlaceStone(0,0);
        PlaceStone(45,0);
        PlaceStone(45,45);
        PlaceStone(-45,-45);
        PlaceStone(45,-45);
        PlaceStone(-45,45);
        /*foreach (var data in _gameRecord.Data)
        {
            Debug.Log(data.Player);
            Debug.Log(data.X);
            Debug.Log(data.Y);
            Debug.Log(data.TurnNumber);
        }*/
        SaveGame(_gameRecord);
        //GameOver();
    }

    // 착수 메서드와 연계
    public void PlaceStone(int x, int y)
    {
        OmokData data = new OmokData()
        {
            TurnNumber = ++_turnNumber,
            X = x, // 45, // 좌표값에서 나눠줌
            Y = y, // 45,
            Player = _playerName
        };
        _gameRecord.Data.Add(data);
        
        _playerName = (_playerName == "Black") ? "White" : "Black";
        //Debug.Log(_gameRecord.Data);
    }
    
    //임시 메서드
    private void GameOver()
    {
        SaveGame(_gameRecord);
    }

    public void SaveGame(GameRecord gameRecord)
    {
        string data = JsonUtility.ToJson(gameRecord,true);
        string filePath = Path.Combine(GameManager.Instance.path, $"omok_{_timestamp}.json");
        File.WriteAllText(filePath, data);
        MaxSave();
    }

    private void MaxSave()
    {
        string[] files = Directory.GetFiles(GameManager.Instance.path, "*omok_*.json");

        if (files.Length > _maxSave)
        {
            List<string> fileList = new List<string>(files);
            fileList.Sort();
            
            int deleteCount = fileList.Count - _maxSave;
            for (int i = 0; i < deleteCount; i++)
            {
                File.Delete(fileList[i]);
            }
        }
    }

}
