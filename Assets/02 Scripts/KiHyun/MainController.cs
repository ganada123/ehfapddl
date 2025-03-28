using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainController : Singleton<MainController>
{
    [SerializeField] private GameObject selectImagePanel; // ������ �̹��� ���� �г�
    [SerializeField] private GameObject confirmPanel; // �˸� �г�
    [SerializeField] private GameObject signupPanel; // ȸ������ �г�
    [SerializeField] private GameObject signinPanel; // �α��� �г�
    [SerializeField] private Sprite[] profileImages; // ��� ������ �̹��� �迭
    [SerializeField] private Image profileImageUI; // ������ ������ �̹���

    private Canvas _canvas;

    public static new MainController Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        SoundManager.Instance.PlayBGM(SoundManager.Bgm.BGM_TITLE);

        int savedImageIndex = PlayerPrefs.GetInt("SelectedProfileImageIndex", 0);
        ApplyProfileImage(savedImageIndex);

        _canvas = GameObject.FindObjectOfType<Canvas>();
    }

    public void ApplyProfileImage(int index) // ������ �̹����� �����ϴ� �޼���
    {
        Sprite profileSprite = GetProfileImage(index); // ������ �̹��� �ε����� �̹��� ��������
        if (profileSprite != null)
        {
            profileImageUI.sprite = profileSprite; // ������ �̹��� ����
        }
        else
        {
            Debug.LogError("�߸��� ������ �̹��� �ε����Դϴ�.");
        }
    }

    private Sprite GetProfileImage(int index) // ������ �̹����� �������� �޼���
    {
        if (profileImages != null && index >= 0 && index < profileImages.Length)
        {
            return profileImages[index];
        }
        return null;
    }
 
    public void OpenProfileImagePanel() // ������ �̹��� ���� �г��� ���� �޼���
    {
        if (_canvas != null)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
            var profileImagePanelObject = Instantiate(selectImagePanel, _canvas.transform);
        }
    }

    public void OpenSignupPanel() // ȸ������ �г��� ���� �޼���
    {
        if (_canvas != null)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
            var signupPanelObject = Instantiate(signupPanel, _canvas.transform);
        }
    }    
 
    public void OpenConfirmPanel(string message, ConfirmPanelController.OnConfirmButtonClick onConfirmButtonClick) // �˸� �г��� ���� �޼���
    {
        if (_canvas != null)
        {
            Debug.Log("openconfirmpanel ȣ���");
            var confirmPanelObject = Instantiate(confirmPanel, _canvas.transform);
            confirmPanelObject.GetComponent<ConfirmPanelController>()
                .Show(message, onConfirmButtonClick);
        }
        else Debug.Log ("_canvas�� ����");
    }
    
    public void OnClickLogoutButton()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
        StartCoroutine(NetworkManage.Instance.Signout(() =>
        {
            // �α׾ƿ� ���� �� ó��
            Debug.Log("�α׾ƿ� ����");
            MainController.Instance.OpenConfirmPanel("�α׾ƿ� �Ǿ����ϴ�.", () =>
            {
                // �α׾ƿ� �� �α��� ȭ�� Ȱ��ȭ
                signinPanel.SetActive(true);
            });
        }, (errorMessage) =>
        {
            // �α׾ƿ� ���� �� ó��
            Debug.LogError("�α׾ƿ� ����: " + errorMessage);
            MainController.Instance.OpenConfirmPanel("�α׾ƿ��� �����߽��ϴ�. �ٽ� �õ����ּ���.", () => { });
        }));
    }
    
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }    
}