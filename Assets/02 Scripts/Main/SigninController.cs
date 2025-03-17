using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SigninController : MonoBehaviour
{
    [SerializeField] private TMP_InputField passwordInputField; // TMP�� InputField
    [SerializeField] private GameObject signinPanel;
    private bool isPasswordVisible = false;

    public void TogglePasswordVisible()
    {
        if (isPasswordVisible)
        {
            passwordInputField.contentType = TMP_InputField.ContentType.Password;
        }
        else
        {
            passwordInputField.contentType = TMP_InputField.ContentType.Standard;
        }

        isPasswordVisible = !isPasswordVisible;
        passwordInputField.ForceLabelUpdate(); // ����� ���� ����
    }

    public void OnClickSigninButton()
    {
        if(signinPanel != null)
        {
            signinPanel.SetActive(false);
        }
        else
        {
            Debug.Log("signinPanel�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }
}
