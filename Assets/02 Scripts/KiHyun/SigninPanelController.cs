using DG.Tweening.Core.Easing;
using System;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct SigninData // �α��ο� �ʿ��� ������ ����ü
{
    public string email;
    public string password;
}

public struct SigninResult // �α��� ����� ���� ����ü
{
    public int result;
    public string message;
    public int profileImageIndex;
    public string nickname;
}
/*
public struct ScoreResult
{
    public string id;
    public string username;
    public string nickname;
    public int score;
}

[Serializable]
public struct ScoreInfo
{
    public string username;
    public string nickname;
    public int score;
}

[Serializable]
public struct Scores
{
    public ScoreInfo[] scores;
}
*/
public class SigninPanelController : MonoBehaviour
{
    [SerializeField] private TMP_InputField _emailInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    private bool isVisible = false;

    public void OnClickSigninButton() // �α��� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
        Debug.Log("�α��ι�ư Ŭ��");
        string email = _emailInputField.text;
        string password = _passwordInputField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) // �̸��� �Ǵ� ��й�ȣ�� ������� ��
        {
            MainController.Instance.OpenConfirmPanel("�Է� ������ �����Ǿ����ϴ�.", () => {

            });
            return;
        }

        var signinData = new SigninData(); // �α��� ������ ����
        signinData.email = email;
        signinData.password = password;

        StartCoroutine(NetworkManage.Instance.Signin(signinData, () => // signinData ��ü�� ������ �����Ͽ� �α��� ��û
        {
            // Destroy(gameObject); // �α��� ���� �� ���� ��ü ����
            gameObject.SetActive(false);
            
        }, result =>
        {
            if (result == 0) // �̸����� ���� ��
            {
                _emailInputField.text = "";
                _passwordInputField.text = "";
            }
            else if (result == 1) // ��й�ȣ�� ��ġ���� ���� ��
            {
                _passwordInputField.text = "";
            }
        }));
    }

    public void OnClickSignupButton() // ȸ������ ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
        MainController.Instance.OpenSignupPanel();
    }

    public void ToggleSigninPasswordVisible() // ��й�ȣ ǥ��/����� ��� �޼���
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
        if (!isVisible)
        {
            _passwordInputField.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            _passwordInputField.contentType = TMP_InputField.ContentType.Password;
        }
        isVisible = !isVisible;
        _passwordInputField.ForceLabelUpdate();
    }
}