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

    // ������� ����
    public enum Bgm
    {
        BGM_TITLE, // ���� ȭ�� BGM
        BGM_INGAME // �ΰ��� BGM
    }

    // ȿ���� ����
    public enum Sfx
    {
        SFX_BUTTON, // ��ư ������ ȿ����
        // SFX_STONE -> �� ���� ȿ����?
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

    public void PlaySFX(Sfx sfx) { audioSfx.PlayOneShot(sfxs[(int)sfx]); } // ȿ������ �ѹ��� ���

    public void SetBgmVolume(float volumn) { audioBgm.volume = volumn; }

    public void SetSfxVolume(float volumn) { audioSfx.volume = volumn; }
}