using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileImageButtonHandler : MonoBehaviour
{
    public Image targetImage, signupTargetImage; // 선택된 프로필 이미지를 표시할 Image 컴포넌트
    public ImageOutLineToggle[] imageOutLineToggles;  // 모든 ImageOutLineToggle 참조하는 배열
    [SerializeField] private GameObject selectImagePanel; // 프로필 이미지 선택 패널

    private int selectedImageIndex = 0; // 선택한 이미지 인덱스 번호
   
    private void Awake()
    {
        targetImage = GameObject.Find("Profile Image Button").GetComponent<Image>(); // 프로필 이미지 (버튼) 객체 찾고 컴포넌트 가져옴
        GameObject signupImageObj = GameObject.Find("Signup Profile Image Button");

        if(signupImageObj != null) signupTargetImage = signupImageObj.GetComponent<Image>(); // 회원가입 이미지 버튼이 있으면 가져옴

        LoadProfileImage(); // 저장된 프로필 이미지 로드
    }

    public void OnClickConfirmButton() // 이미지 선택 패널에서 확인 버튼 클릭 시 호출되는 메서드
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);

        for (int i = 0; i < imageOutLineToggles.Length; i++) // 이미지 선택 토글 배열을 돌면서 선택된 이미지 찾기
        {
            Sprite selectedImage = imageOutLineToggles[i].GetSelectedImage();
            if (selectedImage != null)
            {
                targetImage.sprite = selectedImage; // 선택된 프로필 이미지 적용

                if (signupTargetImage != null) // 회원가입 창이 있는 경우에만 적용
                    signupTargetImage.sprite = selectedImage;

                // 선택한 이미지 인덱스를 저장
                PlayerPrefs.SetInt("SelectedProfileImageIndex", i);
                PlayerPrefs.Save();

                Debug.Log("선택한 이미지 인덱스: " + i);

                StartCoroutine(NetworkManage.Instance.UpdateProfileImage(i)); // 서버에 선택한 이미지 인덱스 전송
                break;
            }
        }

        if(selectImagePanel != null) 
        {
            Destroy(selectImagePanel);
        }
    }
        
    private void LoadProfileImage() // 저장된 프로필 이미지 로드하는 메서드
    {
        int savedIndex = PlayerPrefs.GetInt("SelectedProfileImageIndex", 0);
        if (savedIndex < imageOutLineToggles.Length)
        {
            Sprite loadedImage = imageOutLineToggles[savedIndex].GetSelectedImage();
            if (loadedImage != null)
            {
                targetImage.sprite = loadedImage;
            }
        }
    }
    
    public void SetProfileImage(int index) // 프로필 이미지를 설정하는 메서드
    {
        if (index >= 0 && index < imageOutLineToggles.Length)
        {
            targetImage.sprite = imageOutLineToggles[index].GetSelectedImage();
        }
    }

    public int GetSelectedImageIndex() // 선택한 이미지 인덱스를 반환하는 메서드
    {
        return selectedImageIndex;
    }

    public void OnClickProfileButton() // 프로필 이미지 버튼 클릭 시 호출되는 메서드
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
        MainController.Instance.OpenProfileImagePanel();
    }

    public void OnClickCancelButton() // 이미지 선택 패널에서 취소 버튼 클릭 시 호출되는 메서드
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
        Destroy(selectImagePanel);
    }
}