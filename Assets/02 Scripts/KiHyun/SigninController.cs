using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SigninController : MonoBehaviour
{
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private GameObject signinPanel;
    [SerializeField] private GameObject signupPanel;

    private bool isPasswordVisible = false;

    public void TogglePasswordVisible()
    {
        SoundManager.instance.playSFX(SoundManager.Sfx.SFX_BUTTON);

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
        SoundManager.instance.playSFX(SoundManager.Sfx.SFX_BUTTON);

        if (signinPanel != null)
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
        SoundManager.instance.playSFX(SoundManager.Sfx.SFX_BUTTON);

        if (signupPanel != null)
        {
            signupPanel.SetActive(true);
        }
        else
        {
            Debug.Log("signupPanel이 할당되지 않았습니다.");
        }
    }

    public void OnClickSignupConfirmButton()
    {
        SoundManager.instance.playSFX(SoundManager.Sfx.SFX_BUTTON);

        if (signupPanel != null)
        {
            signupPanel.SetActive(false);
        }
        else
        {
            Debug.Log("signupPanel이 할당되지 않았습니다.");
        }
    }
}