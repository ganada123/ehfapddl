using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public struct SignupData // ȸ�����Կ� �ʿ��� ������ ����ü
{
    public string email;
    public string nickname;
    public string password;
    public int profileImageIndex; // ������ �̹����� �ε��� ��ȣ    
}

public class SignupPanelController : MonoBehaviour
{
    [SerializeField] private TMP_InputField _emailInputField;
    [SerializeField] private TMP_InputField _nicknameInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [SerializeField] private TMP_InputField _confirmPasswordInputField;

    private bool isVisible = false; // ��й�ȣ ���ü� ���� (����/�Ⱥ���)

    public void OnClickConfirmButton() // ȸ������ Ȯ�� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);

        var email = _emailInputField.text;
        var password = _passwordInputField.text;
        var confirmPassword = _confirmPasswordInputField.text;
        var nickname = _nicknameInputField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(nickname)) // �Է� �ʵ� ���� ��
        {
            MainController.Instance.OpenConfirmPanel("�Է� ������ �����Ǿ����ϴ�.", () => { });
            return;
        }

        if (password.Equals(confirmPassword))
        {
            SignupData signupData = new SignupData();
            signupData.email = email;
            signupData.password = password;
            signupData.nickname = nickname;

            // PlayerPrefs���� ���õ� �̹��� �ε����� ������
            int selectedImageIndex = PlayerPrefs.GetInt("SelectedProfileImageIndex", 0); // �⺻�� 0
            signupData.profileImageIndex = selectedImageIndex;

            // ������ SignupData �����ϸ鼭 ȸ������ ����
            StartCoroutine(NetworkManage.Instance.Signup(signupData, () =>
            {
                Destroy(gameObject);
            }, () =>
            {
                _emailInputField.text = "";
                _passwordInputField.text = "";
                _confirmPasswordInputField.text = "";
                _nicknameInputField.text = "";
            }));
            Debug.Log("ȸ������ �Ϸ�");
        }
        else
        {
            MainController.Instance.OpenConfirmPanel("��й�ȣ�� ���� �ٸ��ϴ�.", () =>
            {
                Debug.Log("��й�ȣ�� ���� �ٸ��ϴ�.");
                _passwordInputField.text = "";
                _confirmPasswordInputField.text = "";
            });
        }
    }

    public void OnClickCancelButton() // ȸ������ ��� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
        Destroy(gameObject);
    }

    public void ToggleSignupPasswordVisible() // ��й�ȣ ǥ��/����� ��� �޼��� (Password <-> Standard)
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
        _passwordInputField.ForceLabelUpdate(); // UI ���ΰ�ħ
    }

    public void ToggleSignupConfirmPasswordVisible() // ��й�ȣ Ȯ�� ǥ��/����� ��� �޼��� (Password <-> Standard)
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);

        if (!isVisible)
        {
            _confirmPasswordInputField.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            _confirmPasswordInputField.contentType = TMP_InputField.ContentType.Password;
        }
        isVisible = !isVisible;
        _confirmPasswordInputField.ForceLabelUpdate();
    }
}