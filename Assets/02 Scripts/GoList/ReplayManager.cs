using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ReplayManager : MonoBehaviour
{
    [SerializeField] public GameObject blackMarkerPrefab;
    [SerializeField] public GameObject whiteMarkerPrefab;
    [SerializeField] private GameObject canvas;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private GameObject interferencePanel;
    [SerializeField] private GameObject interferencePanel2;
    
    
    public GameRecord gameRecord;
    private Stack<GameObject> _redoStack = new Stack<GameObject>();
    public Stack<GameObject> Makers = new Stack<GameObject>();
    
    public int replayCounter = 1;
    public int practiceCounter = 1;
    public int index = -1;
    

    private void Start()
    {
        interferencePanel.SetActive(false);
        interferencePanel2.SetActive(false);
        Test(index);
    }


    public GameRecord LoadGame(int notationNumber)
    {
        string[] files = Directory.GetFiles(GameManager.Instance.path, "*omok_*.json");

        if (notationNumber < files.Length)
        {
            string data = File.ReadAllText(files[notationNumber]);
            GameRecord gameRecord = JsonUtility.FromJson<GameRecord>(data);
            Debug.Log("기보로드" + notationNumber);
            return gameRecord;
        }
        
        Debug.Log("기보없음");
        return null;
    }

    public void ReplayGame()
    {
        if (replayCounter > gameRecord.Data.Count) return;

        //Debug.Log(_gameRecord.Data[_replayCounter-1].TurnNumber);
        foreach (var data in gameRecord.Data)
        {
            GameObject marker;
            if (data.TurnNumber == replayCounter)
            {
                if (data.Player == "Black")
                {
                    marker = Instantiate(blackMarkerPrefab, canvas.transform);
                    Makers.Push(marker);
                }
                else
                {
                    marker = Instantiate(whiteMarkerPrefab, canvas.transform);
                    Makers.Push(marker);
                }
                RectTransform rectTransform = marker.GetComponent<RectTransform>();

                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = new Vector2(data.X, data.Y);
                }

                replayCounter++;
                break;
            }
        }
    }
    
    public void ReplayGameButton()
    {
        if (replayCounter > gameRecord.Data.Count) return;

        //Debug.Log(_gameRecord.Data[_replayCounter-1].TurnNumber);
        foreach (var data in gameRecord.Data)
        {
            if (data.TurnNumber == replayCounter)
            {
                GameObject marker;
                if (data.Player == "Black")
                {
                    marker = Instantiate(blackMarkerPrefab, canvas.transform);
                    Makers.Push(marker);
                }
                else
                {
                    marker = Instantiate(whiteMarkerPrefab, canvas.transform);
                    Makers.Push(marker);
                }
                RectTransform rectTransform = marker.GetComponent<RectTransform>();

                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = new Vector2(data.X / 4f, data.Y / 4f);
                }
                replayCounter++;
                break;
            }
        }
    }

    public void Undo()
    {
        if (replayCounter > 1)
        {
            GameObject marker = Makers.Pop();
            //_redoStack.Push(marker);
            Destroy(marker);
            replayCounter--;
        }
    }

    public void ReplayLast()
    {
        StartCoroutine(ReplayLastTest());
    }

    protected void ReplayButtonLast()
    {
        StartCoroutine(ReplayButtonLastTest());
    }
    
    public void ReplayFirst()
    {
        StartCoroutine(ReplayFirstTest());
    }

    IEnumerator ReplayLastTest()
    {
        for (int i = 0; i < gameRecord.Data.Count; i++)
        {
            ReplayGame();
            yield return new WaitForSeconds(0.05f);
        }
    }
    
    IEnumerator ReplayButtonLastTest()
    {
        for (int i = 0; i < gameRecord.Data.Count; i++)
        {
            ReplayGameButton();
            yield return new WaitForSeconds(0.05f);
        }
    }
    
    public IEnumerator ReplayFirstTest()
    {
        int count = Makers.Count;
        for (int i = 0; i < count; i++)
        {
            Undo();
            yield return null;
        }
    }

    public void PlusGoList()
    {
        if (index != GameManager.Instance.maxIndexGameManager-1)
        {
            int count = Makers.Count;
            for (int i = 0; i < count; i++)
            {
                Undo();
            }
            index += 1;
            Test(index);
        }
    }

    public void MinusGoList()
    {
        if (index != 0)
        {
            int count = Makers.Count;
            for (int i = 0; i < count; i++)
            {
                Undo();
            }
            index -= 1;
            Test(index);
        }
    }

    public void Test(int testIndex)
    {
        Debug.Log(testIndex);
        gameRecord = LoadGame(testIndex);
        titleText.text = $"제 {testIndex}국";
    }

    public void OnEnablePractice()
    {
        if (gameObject.GetComponent<Pratice>().enabled != true)
        {
            interferencePanel.SetActive(true);
            interferencePanel2.SetActive(true);
            practiceCounter = replayCounter;
            gameObject.GetComponent<Pratice>().enabled = true;
        }
        else
        {
            interferencePanel.SetActive(false);
            interferencePanel2.SetActive(false);
            gameObject.GetComponent<Pratice>().enabled = false;
        }
    }
}
