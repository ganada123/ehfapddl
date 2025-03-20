using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Point : MonoBehaviour
{
    public Sprite emptySprite;
    public Sprite blackStoneSprite;
    public Sprite whiteStoneSprite;

    private Image imageComponent;
    private bool isOccupied = false;

    void Start()
    {
        imageComponent = GetComponent<Image>();
        if (imageComponent == null)
        {
            Debug.LogError($"{gameObject.name}에 Image 컴포넌트가 없습니다!");
            return;
        }
        imageComponent.sprite = emptySprite;

        // ✅ 버튼 이벤트 추가
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnClick(GameManager.Instance.GetCurrentPlayer()));
        }
    }

    public void OnClick(int playerTurn)
    {
        if (!isOccupied)
        {
            isOccupied = true;
            imageComponent.sprite = (playerTurn == 1) ? blackStoneSprite : whiteStoneSprite;
            GameManager.Instance.ChangeTurn();
        }
    }
}