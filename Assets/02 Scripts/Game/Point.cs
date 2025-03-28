/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Point : MonoBehaviour, IPointerClickHandler
{
    public Sprite emptySprite;
    public Sprite blackStoneSprite;
    public Sprite whiteStoneSprite;

    private Image imageComponent;
    private bool isOccupied = false;
    
    public int x, y; // 바둑판 좌표 저장

    void Start()
    {
        imageComponent = GetComponent<Image>();
        if (imageComponent != null)
            imageComponent.sprite = emptySprite;
    }

    public bool IsOccupied()
    {
        return isOccupied;
    }

    public void PlaceStone(int player)
    {
        if (isOccupied) return;

        isOccupied = true;
        imageComponent.sprite = (player == 1) ? blackStoneSprite : whiteStoneSprite;
        
        /*Debug.Log($"Point 위치: ({x}, {y})"); // 디버깅용 로그#1#
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.SelectPoint(gameObject);
    }
}*/