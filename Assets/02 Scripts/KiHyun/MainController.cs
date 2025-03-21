using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    [SerializeField] private GameObject selectImagePanel;

    private void Start()
    {
        SoundManager.instance.PlayBGM(SoundManager.Bgm.BGM_TITLE);
    }

    public void OnClickProfileButton()
    {
        SoundManager.instance.playSFX(SoundManager.Sfx.SFX_BUTTON);

        if (selectImagePanel != null)
        {
            selectImagePanel.SetActive(true);
        }
        else
        {
            Debug.Log("selectImagePanel�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    public void OnClickProfileCancelButton()
    {
        SoundManager.instance.playSFX(SoundManager.Sfx.SFX_BUTTON);

        if (selectImagePanel != null)
        {
            selectImagePanel.SetActive(false);
        }
        else
        {
            Debug.Log("selectImagePanel�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }    
}
