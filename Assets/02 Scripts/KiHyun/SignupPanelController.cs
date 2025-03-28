using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public struct SignupData // 회원가입에 필요한 데이터 구조체
{
    public string email;
    public string nickname;
    public string password;
    public int profileImageIndex; // 선택한 이미지의 인덱스 번호    
}

public class SignupPanelController : MonoBehaviour
{
    [SerializeField] private TMP_InputField _emailInputField;
    [SerializeField] private TMP_InputField _nicknameInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [SerializeField] private TMP_InputField _confirmPasswordInputField;

    private bool isVisible = false; // 비밀번호 가시성 상태 (보임/안보임)

    public void OnClickConfirmButton() // 회원가입 확인 버튼 클릭 시 호출되는 메서드
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);

        var email = _emailInputField.text;
        var password = _passwordInputField.text;
        var confirmPassword = _confirmPasswordInputField.text;
        var nickname = _nicknameInputField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(nickname)) // 입력 필드 누락 시
        {
            MainController.Instance.OpenConfirmPanel("입력 내용이 누락되었습니다.", () => { });
            return;
        }

        if (password.Equals(confirmPassword))
        {
            SignupData signupData = new SignupData();
            signupData.email = email;
            signupData.password = password;
            signupData.nickname = nickname;

            // PlayerPrefs에서 선택된 이미지 인덱스를 가져옴
            int selectedImageIndex = PlayerPrefs.GetInt("SelectedProfileImageIndex", 0); // 기본값 0
            signupData.profileImageIndex = selectedImageIndex;

            // 서버로 SignupData 전달하면서 회원가입 진행
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
            Debug.Log("회원가입 완료");
        }
        else
        {
            MainController.Instance.OpenConfirmPanel("비밀번호가 서로 다릅니다.", () =>
            {
                Debug.Log("비밀번호가 서로 다릅니다.");
                _passwordInputField.text = "";
                _confirmPasswordInputField.text = "";
            });
        }
    }

    public void OnClickCancelButton() // 회원가입 취소 버튼 클릭 시 호출되는 메서드
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
        Destroy(gameObject);
    }

    public void ToggleSignupPasswordVisible() // 비밀번호 표시/숨기기 토글 메서드 (Password <-> Standard)
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
        _passwordInputField.ForceLabelUpdate(); // UI 새로고침
    }

    public void ToggleSignupConfirmPasswordVisible() // 비밀번호 확인 표시/숨기기 토글 메서드 (Password <-> Standard)
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