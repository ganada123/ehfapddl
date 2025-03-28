using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Cell : ReplayManager
{
    [SerializeField] private TMP_Text buttonTitleText;
    [SerializeField] public Button cellButton;
    //[SerializeField] private TMP_Text subtitleText;

    public int Index { get; private set; }

    private void Start()
    {
        cellButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        SceneManager.LoadScene("GoListMain");
        GameManager.Instance.indexGameManager = Index;
    }

    public void SetItem(Item item, int setIndex)
    {
        buttonTitleText.text = item.title;
        Index = setIndex;
        
        ReplayFirst();
        //image.sprite = Resources.Load<Sprite>(item.imageFileName);
        //subtitleText.text = item.subtitle;

        Test(Index);
        Invoke("ReplayButtonLast", 0.05f);
    }
    
    
}