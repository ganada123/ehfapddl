using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider; // 배경음 볼륨 조절 슬라이더
    [SerializeField] private Slider sfxSlider; // 효과음 볼륨 조절 슬라이더
    [SerializeField] private Button confirmButton; // 설정 저장 버튼
    [SerializeField] private Button cancelButton; // 설정 취소 버튼
    [SerializeField] private GameObject settingPanel; // 설정 패널

    float prevBgmVolumn; // 변경 전 BGM 볼륨 저장
    float prevSfxVolumn; // 변경 전 SFX 볼륨 저장

    private void Start()
    {
        // 기존에 저장된 볼륨 값 가져오기 (기본값 0.5)
        prevBgmVolumn = PlayerPrefs.GetFloat("BGMVolumn", 0.5f);
        prevSfxVolumn = PlayerPrefs.GetFloat("SFXVolumn", 0.5f);

        // 슬라이더 초기값 설정
        bgmSlider.value = prevBgmVolumn;
        sfxSlider.value = prevSfxVolumn;

        // 슬라이더 값 변경 시 볼륨 즉시적용
        bgmSlider.onValueChanged.AddListener(UpdateBgmVolumn);
        sfxSlider.onValueChanged.AddListener(UpdateSfxVolumn);

        // 버튼 클릭시 설정 저장,취소
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
          
            Debug.Log($"BGM : {bgmSlider.value}, SFX : {sfxSlider.value} 저장 완료");
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

        // 취소 시 기존의 저장된 볼륨 값으로 되돌리기
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