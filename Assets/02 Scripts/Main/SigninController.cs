using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SigninController : MonoBehaviour
{
    [SerializeField] private TMP_InputField passwordInputField; // TMP의 InputField
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
}
