using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    public Sprite emptySprite;  // 빈 칸 이미지
    public Sprite blackStoneSprite;  // 흑돌 이미지
    public Sprite whiteStoneSprite;  // 백돌 이미지

    private SpriteRenderer spriteRenderer;
    private bool isOccupied = false;  // 이 칸에 돌이 놓였는지 체크

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = emptySprite;  // 초기 상태는 빈 칸
    }

    // 클릭 시 돌을 놓는 함수
    public void OnClick(int playerTurn)
    {
        if (!isOccupied)
        {
            isOccupied = true;
            if (playerTurn == 1)
            {
                spriteRenderer.sprite = blackStoneSprite;  // 흑돌
            }
            else
            {
                spriteRenderer.sprite = whiteStoneSprite;  // 백돌
            }
        }
    }
}
