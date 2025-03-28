using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SigninController : MonoBehaviour
{
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private GameObject signinPanel;
    [SerializeField] private GameObject signupPanel;
    [SerializeField] private GameObject selectImagePanel;
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
        passwordInputField.ForceLabelUpdate(); // 변경된 내용 적용
    }

    public void OnClickSigninButton()
    {
        if(signinPanel != null)
        {
            signinPanel.SetActive(false);
        }
        else
        {
            Debug.Log("signinPanel이 할당되지 않았습니다.");
        }
    }

    public void OnClickSignupButton()
    {
        if (signupPanel != null)
        {
            signupPanel.SetActive(true);
        }
        else
        {
            Debug.Log("signupPanel�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    public void OnClickSignupConfirmButton()
    {
        if (signupPanel != null)
        {
            signupPanel.SetActive(false);
        }
        else
        {
            Debug.Log("signupPanel�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    public void OnClickProfileButton()
    {
        if(selectImagePanel != null)
        {
            selectImagePanel.SetActive(true);
        }
        else
        {
            Debug.Log("selectImagePanel�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    public void OnClickCancelButton()
    {
        if(selectImagePanel != null)
        {
            selectImagePanel.SetActive(false);
        }
        else
        {
            Debug.Log("selectImagePanel�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }
}
