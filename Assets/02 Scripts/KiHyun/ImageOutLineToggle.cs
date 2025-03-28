using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageOutLineToggle : MonoBehaviour
{
    public static Outline prevOutline;
    private Outline thisOutline;
    private bool isSelected = false; // �̹��� ���� ����

    private void Start()
    {
        thisOutline = GetComponent<Outline>();
        thisOutline.enabled = false;
    }

    public void ToggleOutline()
    {
        // ���� Outline�� ��Ȱ��ȭ�ϰ� ���� ����
        if (prevOutline != null && prevOutline != thisOutline)
        {
            prevOutline.enabled = false;
            ImageOutLineToggle prevToggle = prevOutline.GetComponent<ImageOutLineToggle>();
            if (prevToggle != null)
            {
                prevToggle.isSelected = false;
            }
        }

        // ���� ������ �̹����� Outline ����
        thisOutline.enabled = !thisOutline.enabled; // ���� ���� ���
        isSelected = thisOutline.enabled; // ���� ���� �ݿ�

        if (isSelected)
        {
            prevOutline = thisOutline;
        }
        else
        {
            prevOutline = null;
        }
    }

    // ������ �̹����� sprite ��ȯ
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
                // Debug.LogWarning(gameObject.name + "�� Image ������Ʈ�� Sprite�� �Ҵ���� ����");
            }
        }
        // Debug.LogWarning(gameObject.name + " ���õ��� ����");
        return null;
    }
}