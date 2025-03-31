using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinLosePanelController : MonoBehaviour
{
    [SerializeField] private TMP_Text descriptionText1;
    [SerializeField] private TMP_Text descriptionText2;
    [SerializeField] private TMP_Text nextClassText;
    [SerializeField] private Image[] classesImage;

    private int playerScore;
    private int winStreak;
    private int playerRankClass;
    
    GameManager gameManager;
    
    private void Start()
    {
        WinOrLose();
        
    }
    
    //자신이 이겼는지 졌는지에 따라 다른 UI가 나오는 함수
    private void WinOrLose()
    {
        bool whoWon = GameManager.Instance.amIwin;
        
        if (whoWon == true)
        {
            Debug.Log("승리!");
            descriptionText1.text = "게임에서 승리했습니다.";
            descriptionText2.text = "10 승급 포인트를 받았습니다.";
            nextClassText.text = "1 게임만 승리하면 승급합니다.";
        }
        else
        {
            Debug.Log("패배");
            descriptionText1.text = "게임에서 패배했습니다.";
            descriptionText2.text = "10 승급 포인트를 잃었습니다.";
            nextClassText.text = "2 게임만 승리하면 승급합니다.";
        }
    }
}
