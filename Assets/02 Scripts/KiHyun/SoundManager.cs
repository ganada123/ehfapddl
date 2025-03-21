using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] AudioClip[] bgms;
    [SerializeField] AudioClip[] sfxs;

    [SerializeField] AudioSource audioBgm;
    [SerializeField] AudioSource audioSfx;

    public enum Bgm
    {
        BGM_TITLE,
        BGM_INGAME
    }

    public enum Sfx
    {
        SFX_BUTTON,
        SFX_TOUCH,
        // SFX_STONE
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioBgm.volume = PlayerPrefs.GetFloat("BGMVolumn", 0.5f);
        audioSfx.volume = PlayerPrefs.GetFloat("SFXVolumn", 0.5f);
    }

    public void PlayBGM(Bgm bgm)
    {
        audioBgm.clip = bgms[(int)bgm];
        audioBgm.Play();
    }

    public void StopBGM() { audioBgm.Stop();}

    public void playSFX(Sfx sfx) { audioSfx.PlayOneShot(sfxs[(int)sfx]); }

    public void SetBgmVolumn(float volumn) { audioBgm.volume = volumn; }

    public void setSfxVolume(float volumn) { audioSfx.volume = volumn; }
}
