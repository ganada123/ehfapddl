using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] AudioClip[] bgms;
    [SerializeField] AudioClip[] sfxs;

    [SerializeField] AudioSource audioBgm;
    [SerializeField] AudioSource audioSfx;

    // 배경음악 종류
    public enum Bgm
    {
        BGM_TITLE, // 메인 화면 BGM
        BGM_INGAME // 인게임 BGM
    }

    // 효과음 종류
    public enum Sfx
    {
        SFX_BUTTON, // 버튼 누르는 효과음
        // SFX_STONE -> 돌 놓는 효과음?
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioBgm.volume = PlayerPrefs.GetFloat("BGMVolumn", 0.2f);
        audioSfx.volume = PlayerPrefs.GetFloat("SFXVolumn", 0.2f);
    }

    public void PlayBGM(Bgm bgm)
    {
        audioBgm.clip = bgms[(int)bgm];
        audioBgm.Play();
    }

    public void StopBGM() { audioBgm.Stop();}

    public void PlaySFX(Sfx sfx) { audioSfx.PlayOneShot(sfxs[(int)sfx]); } // 효과음은 한번만 재생

    public void SetBgmVolume(float volumn) { audioBgm.volume = volumn; }

    public void SetSfxVolume(float volumn) { audioSfx.volume = volumn; }
}