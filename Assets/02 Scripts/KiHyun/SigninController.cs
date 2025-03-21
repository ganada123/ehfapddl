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
        passwordInputField.ForceLabelUpdate(); // ����� ���� ����
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
            Debug.Log("signinPanel�� �Ҵ���� �ʾҽ��ϴ�.");
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
            Debug.Log("signupPanel�� �Ҵ���� �ʾҽ��ϴ�.");
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
            Debug.Log("signupPanel�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }
}