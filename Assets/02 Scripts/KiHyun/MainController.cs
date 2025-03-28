using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainController : Singleton<MainController>
{
    [SerializeField] private GameObject selectImagePanel; // 프로필 이미지 선택 패널
    [SerializeField] private GameObject confirmPanel; // 알림 패널
    [SerializeField] private GameObject signupPanel; // 회원가입 패널
    [SerializeField] private GameObject signinPanel; // 로그인 패널
    [SerializeField] private Sprite[] profileImages; // 모든 프로필 이미지 배열
    [SerializeField] private Image profileImageUI; // 적용할 프로필 이미지

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

    public void ApplyProfileImage(int index) // 프로필 이미지에 적용하는 메서드
    {
        Sprite profileSprite = GetProfileImage(index); // 프로필 이미지 인덱스로 이미지 가져오기
        if (profileSprite != null)
        {
            profileImageUI.sprite = profileSprite; // 가져온 이미지 적용
        }
        else
        {
            Debug.LogError("잘못된 프로필 이미지 인덱스입니다.");
        }
    }

    private Sprite GetProfileImage(int index) // 프로필 이미지를 가져오는 메서드
    {
        if (profileImages != null && index >= 0 && index < profileImages.Length)
        {
            return profileImages[index];
        }
        return null;
    }
 
    public void OpenProfileImagePanel() // 프로필 이미지 선택 패널을 여는 메서드
    {
        if (_canvas != null)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
            var profileImagePanelObject = Instantiate(selectImagePanel, _canvas.transform);
        }
    }

    public void OpenSignupPanel() // 회원가입 패널을 여는 메서드
    {
        if (_canvas != null)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
            var signupPanelObject = Instantiate(signupPanel, _canvas.transform);
        }
    }    
 
    public void OpenConfirmPanel(string message, ConfirmPanelController.OnConfirmButtonClick onConfirmButtonClick) // 알림 패널을 여는 메서드
    {
        if (_canvas != null)
        {
            Debug.Log("openconfirmpanel 호출됨");
            var confirmPanelObject = Instantiate(confirmPanel, _canvas.transform);
            confirmPanelObject.GetComponent<ConfirmPanelController>()
                .Show(message, onConfirmButtonClick);
        }
        else Debug.Log ("_canvas가 없음");
    }
    
    public void OnClickLogoutButton()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
        StartCoroutine(NetworkManage.Instance.Signout(() =>
        {
            // 로그아웃 성공 후 처리
            Debug.Log("로그아웃 성공");
            MainController.Instance.OpenConfirmPanel("로그아웃 되었습니다.", () =>
            {
                // 로그아웃 후 로그인 화면 활성화
                signinPanel.SetActive(true);
            });
        }, (errorMessage) =>
        {
            // 로그아웃 실패 시 처리
            Debug.LogError("로그아웃 실패: " + errorMessage);
            MainController.Instance.OpenConfirmPanel("로그아웃에 실패했습니다. 다시 시도해주세요.", () => { });
        }));
    }
    
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }    
}