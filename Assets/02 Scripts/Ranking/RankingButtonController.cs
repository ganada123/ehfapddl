using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingButtonController : MonoBehaviour
{

    public GameObject RankingPanel;

    public void OnClickOpenRankingPanel()
    {
        RankingPanel.SetActive(true);
    }
    public void OnClickCloseRankingPanel()
    {
        RankingPanel.SetActive(false);
    }

    public void RankingPanelEsc()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (RankingPanel.activeSelf)
                RankingPanel.SetActive(false);
        }
    }

    private void Update()
    {
        RankingPanelEsc();
    }
}
