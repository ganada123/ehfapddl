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

    public void OnClickNicknameConfirm() // �г��� �����ϴ� �޼���
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);

        var nickname = _nicknameInputField.text;
        Debug.Log("�г��� :" + nickname);

        if (string.IsNullOrEmpty(nickname))
        {
            MainController.Instance.OpenConfirmPanel("�г����� �Է����ּ���", () => { });
            return;
        }

        // NicknameData Ŭ������ �̿��Ͽ� ��ü ����
        NicknameData nicknameData = new NicknameData { nickname = nickname };
        Debug.Log("�г��� :" + nicknameData.nickname);

        StartCoroutine(NetworkManage.Instance.SetNickname(nicknameData,
        success => {
            MainController.Instance.OpenConfirmPanel("�г����� �����Ǿ����ϴ�.", () =>
            {
                _nicknameText.text = nickname;
            });
        },
        error => {
            MainController.Instance.OpenConfirmPanel("�г��� ���� ����: " + error, null);
        }));

        nicknameChangePanel.SetActive(false);
    }

    public void SetNickName(string nickname)
    {
        _nicknameText.text = nickname; // �г��� ���� �� UI ������Ʈ
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
