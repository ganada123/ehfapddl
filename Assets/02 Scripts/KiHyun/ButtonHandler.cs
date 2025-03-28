using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �����տ��� �̹��� ����â�� �����ϰ� Instantiate �ϱ� ���� ��ũ��Ʈ
public class ButtonHandler : MonoBehaviour
{
    public void OnClickProfileButton()
    {
        if (MainController.Instance != null)
        {
            MainController.Instance.OpenProfileImagePanel();
        }
        else
        {
            Debug.LogError("MainController �ν��Ͻ��� ã�� �� �����ϴ�.");
        }
    }
}
