using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinLosePanelController : MonoBehaviour
{
    [SerializeField] private TMP_Text descriptionText1;
    [SerializeField] private TMP_Text descriptionText2;
    [SerializeField] private TMP_Text nextClassText;
    [SerializeField] private Image[] classesImage;

    private int playerScore;
    private int winStreak;
    private int loseStreak;
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
        
        if (whoWon)
        {
            Debug.Log("승리!");
            descriptionText1.text = "게임에서 승리했습니다.";
            descriptionText2.text = "10 승급 포인트를 받았습니다.";
            CalWinStreak();
        }
        else
        {
            Debug.Log("패배");
            descriptionText1.text = "게임에서 패배했습니다.";
            descriptionText2.text = "10 승급 포인트를 잃었습니다.";
            CalLoseStreak();
        }
    }

    #region 연승과 연패 계산 함수들
    //연승을 계산하는 함수
    private void CalWinStreak()
    {
        winStreak++;
        loseStreak = 0;
        if (winStreak == 1)
        {
            nextClassText.text = "2 게임 승리하면 승급합니다.";
            classesImage[0].enabled = false;
            classesImage[1].enabled = false;
            classesImage[2].enabled = false;
            classesImage[3].enabled = true;
            classesImage[4].enabled = false;
            classesImage[5].enabled = false;
            
        }
        else if (winStreak == 2)
        {
            nextClassText.text = "1 게임만 승리하면 승급합니다.";
            classesImage[0].enabled = false;
            classesImage[1].enabled = false;
            classesImage[2].enabled = false;
            classesImage[3].enabled = true;
            classesImage[4].enabled = true;
            classesImage[5].enabled = false;
        }
        else if (winStreak == 3)
        {
            nextClassText.text = "승급합니다.";
            classesImage[0].enabled = false;
            classesImage[1].enabled = false;
            classesImage[2].enabled = false;
            classesImage[3].enabled = true;
            classesImage[4].enabled = true;
            classesImage[5].enabled = true;
            
            ChangeRankClass();
        }
        
    }

    //연패를 계산하는 함수
    private void CalLoseStreak()
    {
        loseStreak--;
        winStreak = 0;
        if (loseStreak == 1)
        {
            nextClassText.text = "2 게임을 패배하면 강등됩니다.";
            classesImage[0].enabled = false;
            classesImage[1].enabled = false;
            classesImage[2].enabled = true;
            classesImage[3].enabled = false;
            classesImage[4].enabled = false;
            classesImage[5].enabled = false;
            
        }
        else if (loseStreak == 2)
        {
            nextClassText.text = "1 게임을 패배하면 강등됩니다.";
            classesImage[0].enabled = false;
            classesImage[1].enabled = true;
            classesImage[2].enabled = true;
            classesImage[3].enabled = false;
            classesImage[4].enabled = false;
            classesImage[5].enabled = false;
        }
        else if (loseStreak == 3)
        {
            nextClassText.text = "강등당합니다.";
            classesImage[0].enabled = true;
            classesImage[1].enabled = true;
            classesImage[2].enabled = true;
            classesImage[3].enabled = false;
            classesImage[4].enabled = false;
            classesImage[5].enabled = false;
            
            ChangeRankClass();
        }
    }

    //연승, 연패하여 플레이어의 랭크에 변화가 있을 때 호출되는 함수
    private void ChangeRankClass()
    {
        if (winStreak == 3)
        {
            playerRankClass++;
            Debug.Log(playerRankClass +"연승하여 승급 시 로그");
        }
        else if (loseStreak == -3)
        {
            playerRankClass--;
            Debug.Log(playerRankClass +"연패하여 강등 시 로그");
        }
        
    }
    #endregion

    public void GetAcceptButton()
    {
        SceneManager.LoadScene("Main");
    }
}
