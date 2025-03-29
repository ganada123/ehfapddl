using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class ConfirmPanelController : PanelController
{
    [SerializeField] private TMP_Text messageText;

    public delegate void OnConfirmButtonClick();
    private OnConfirmButtonClick onConfirmButtonClick;

    public void Show(string message, OnConfirmButtonClick onConfirmButtonClick)
    {
        messageText.text = message;
        this.onConfirmButtonClick = onConfirmButtonClick;
        base.Show();
    }

    /// <summary>
    /// Confirm ��ư Ŭ���� ȣ��Ǵ� �Լ�
    /// </summary>
    public void OnClickConfirmButton()
    {
        Hide(() => onConfirmButtonClick?.Invoke());
    }

    /// <summary>
    /// X ��ư Ŭ���� ȣ��Ǵ� �Լ�
    /// </summary>
    public void OnClickCloseButton()
    {
        Hide();
    }
}
