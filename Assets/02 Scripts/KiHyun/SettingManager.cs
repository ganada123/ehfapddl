using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private GameObject settingPanel;

    float prevBgmVolumn;
    float prevSfxVolumn;

    private void Start()
    {
        prevBgmVolumn = PlayerPrefs.GetFloat("BGMVolumn", 0.5f);
        prevSfxVolumn = PlayerPrefs.GetFloat("SFXVolumn", 0.5f);

        bgmSlider.value = prevBgmVolumn;
        sfxSlider.value = prevSfxVolumn;

        bgmSlider.onValueChanged.AddListener(UpdateBgmVolumn);
        sfxSlider.onValueChanged.AddListener(UpdateSfxVolumn);

        confirmButton.onClick.AddListener(SaveSettings);
        cancelButton.onClick.AddListener(CancelSetting);
    }

    public void SaveSettings()
    {
        if (SoundManager.instance != null)
        {
            PlayerPrefs.SetFloat("BGMVolumn", bgmSlider.value);
            PlayerPrefs.SetFloat("SFXVolumn", sfxSlider.value);
            PlayerPrefs.Save();
          
            Debug.Log($"BGM : {bgmSlider.value}, SFX : {sfxSlider.value} 저장 완료");
            settingPanel.SetActive(false);
        }
        SoundManager.instance.playSFX(SoundManager.Sfx.SFX_BUTTON);
    }

    public void OnClickSettingButton()
    {
        SoundManager.instance.playSFX(SoundManager.Sfx.SFX_BUTTON);
        settingPanel.SetActive(true);
    }

    public void CancelSetting()
    {
        // 취소 시 기존의 저장된 볼륨 값으로 되돌리기
        bgmSlider.value = prevBgmVolumn;
        sfxSlider.value = prevSfxVolumn;

        if (SoundManager.instance != null)
        {
            SoundManager.instance.SetBgmVolumn(prevBgmVolumn);
            SoundManager.instance.setSfxVolume(prevSfxVolumn);
        }

        SoundManager.instance.playSFX(SoundManager.Sfx.SFX_BUTTON);
        settingPanel.SetActive(false);
    }

    
    private void UpdateBgmVolumn(float volumn)
    {
        if(SoundManager.instance != null)
        {
            SoundManager.instance.SetBgmVolumn(volumn);
        }
    }

    private void UpdateSfxVolumn(float volumn)
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.setSfxVolume(volumn);
        }
    }
    
}