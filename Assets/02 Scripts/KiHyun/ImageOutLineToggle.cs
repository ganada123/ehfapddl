using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageOutLineToggle : MonoBehaviour
{
    public static Outline prevOutline;
    private Outline thisOutline;
    private bool isSelected = false; // 이미지 선택 여부

    private void Start()
    {
        thisOutline = GetComponent<Outline>();
        thisOutline.enabled = false;
    }

    public void ToggleOutline()
    {
        // 이전 Outline을 비활성화하고 선택 해제
        if (prevOutline != null && prevOutline != thisOutline)
        {
            prevOutline.enabled = false;
            ImageOutLineToggle prevToggle = prevOutline.GetComponent<ImageOutLineToggle>();
            if (prevToggle != null)
            {
                prevToggle.isSelected = false;
            }
        }

        // 현재 선택한 이미지의 Outline 설정
        thisOutline.enabled = !thisOutline.enabled; // 선택 상태 토글
        isSelected = thisOutline.enabled; // 선택 여부 반영

        if (isSelected)
        {
            prevOutline = thisOutline;
        }
        else
        {
            prevOutline = null;
        }
    }

    // 선택한 이미지의 sprite 반환
    public Sprite GetSelectedImage()
    {
        Image currentImage = GetComponent<Image>();

        if (isSelected && currentImage != null)
        {
            if (currentImage.sprite != null)
            {
                return currentImage.sprite;
            }
            else
            {
                // Debug.LogWarning(gameObject.name + "의 Image 컴포넌트에 Sprite가 할당되지 않음");
            }
        }
        // Debug.LogWarning(gameObject.name + " 선택되지 않음");
        return null;
    }
}