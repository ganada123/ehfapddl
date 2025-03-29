using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider; // ����� ���� ���� �����̴�
    [SerializeField] private Slider sfxSlider; // ȿ���� ���� ���� �����̴�
    [SerializeField] private Button confirmButton; // ���� ���� ��ư
    [SerializeField] private Button cancelButton; // ���� ��� ��ư
    [SerializeField] private GameObject settingPanel; // ���� �г�

    float prevBgmVolumn; // ���� �� BGM ���� ����
    float prevSfxVolumn; // ���� �� SFX ���� ����

    private void Start()
    {
        // ������ ����� ���� �� �������� (�⺻�� 0.5)
        prevBgmVolumn = PlayerPrefs.GetFloat("BGMVolumn", 0.5f);
        prevSfxVolumn = PlayerPrefs.GetFloat("SFXVolumn", 0.5f);

        // �����̴� �ʱⰪ ����
        bgmSlider.value = prevBgmVolumn;
        sfxSlider.value = prevSfxVolumn;

        // �����̴� �� ���� �� ���� �������
        bgmSlider.onValueChanged.AddListener(UpdateBgmVolumn);
        sfxSlider.onValueChanged.AddListener(UpdateSfxVolumn);

        // ��ư Ŭ���� ���� ����,���
        confirmButton.onClick.AddListener(SaveSettings);
        cancelButton.onClick.AddListener(CancelSetting);
    }

    public void SaveSettings()
    {
        if (SoundManager.Instance != null)
        {
            PlayerPrefs.SetFloat("BGMVolumn", bgmSlider.value);
            PlayerPrefs.SetFloat("SFXVolumn", sfxSlider.value);
            PlayerPrefs.Save();
          
            Debug.Log($"BGM : {bgmSlider.value}, SFX : {sfxSlider.value} ���� �Ϸ�");
            settingPanel.SetActive(false);
        }
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
    }

    public void OnClickSettingButton()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
        settingPanel.SetActive(true);
    }

    public void CancelSetting()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);

        // ��� �� ������ ����� ���� ������ �ǵ�����
        bgmSlider.value = prevBgmVolumn;
        sfxSlider.value = prevSfxVolumn;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBgmVolume(prevBgmVolumn);
            SoundManager.Instance.SetSfxVolume(prevSfxVolumn);
        }
        settingPanel.SetActive(false);
    }

    private void UpdateBgmVolumn(float volumn)
    {
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBgmVolume(volumn);
        }
    }

    private void UpdateSfxVolumn(float volumn)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetSfxVolume(volumn);
        }
    }    
}