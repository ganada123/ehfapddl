//using DG.Tweening.Core.Easing;
using System;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct SigninData // 로그인에 필요한 데이터 구조체
{
    public string email;
    public string password;
}

public struct SigninResult // 로그인 결과를 담을 구조체
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

    public void OnClickSigninButton() // 로그인 버튼 클릭 시 호출되는 메서드
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
        Debug.Log("로그인버튼 클릭");
        string email = _emailInputField.text;
        string password = _passwordInputField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) // 이메일 또는 비밀번호가 비어있을 때
        {
            MainController.Instance.OpenConfirmPanel("입력 내용이 누락되었습니다.", () => {

            });
            return;
        }

        var signinData = new SigninData(); // 로그인 데이터 생성
        signinData.email = email;
        signinData.password = password;

        StartCoroutine(NetworkManage.Instance.Signin(signinData, () => // signinData 객체를 서버로 전송하여 로그인 요청
        {
            // Destroy(gameObject); // 로그인 성공 후 현재 객체 삭제
            gameObject.SetActive(false);
            
        }, result =>
        {
            if (result == 0) // 이메일이 없을 때
            {
                _emailInputField.text = "";
                _passwordInputField.text = "";
            }
            else if (result == 1) // 비밀번호가 일치하지 않을 때
            {
                _passwordInputField.text = "";
            }
        }));
    }

    public void OnClickSignupButton() // 회원가입 버튼 클릭 시 호출되는 메서드
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
        MainController.Instance.OpenSignupPanel();
    }

    public void ToggleSigninPasswordVisible() // 비밀번호 표시/숨기기 토글 메서드
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