using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NicknameData
{
    public string nickname;
}

public class NicknameChange : Singleton<NicknameChange>
{
    [SerializeField] GameObject nicknameChangePanel;
    [SerializeField] private TMP_InputField _nicknameInputField;
    [SerializeField] public TMP_Text _nicknameText;

    public void OnClickNicknameConfirm() // 닉네임 변경하는 메서드
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);

        var nickname = _nicknameInputField.text;
        Debug.Log("닉네임 :" + nickname);

        if (string.IsNullOrEmpty(nickname))
        {
            MainController.Instance.OpenConfirmPanel("닉네임을 입력해주세요", () => { });
            return;
        }

        // NicknameData 클래스를 이용하여 객체 생성
        NicknameData nicknameData = new NicknameData { nickname = nickname };
        Debug.Log("닉네임 :" + nicknameData.nickname);

        StartCoroutine(NetworkManage.Instance.SetNickname(nicknameData,
        success => {
            MainController.Instance.OpenConfirmPanel("닉네임이 설정되었습니다.", () =>
            {
                _nicknameText.text = nickname;
            });
        },
        error => {
            MainController.Instance.OpenConfirmPanel("닉네임 설정 실패: " + error, null);
        }));

        nicknameChangePanel.SetActive(false);
    }

    public void SetNickName(string nickname)
    {
        _nicknameText.text = nickname; // 닉네임 변경 후 UI 업데이트
    }

    public void OnClickNicknamePanel()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
        nicknameChangePanel.SetActive(true);
    }

    public void OnClickNicknameCancel()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
        _nicknameInputField.text = "";
        nicknameChangePanel.SetActive(false);
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
}
