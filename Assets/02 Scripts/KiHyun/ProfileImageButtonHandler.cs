using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileImageButtonHandler : MonoBehaviour
{
    public Image targetImage, signupTargetImage; // ���õ� ������ �̹����� ǥ���� Image ������Ʈ
    public ImageOutLineToggle[] imageOutLineToggles;  // ��� ImageOutLineToggle �����ϴ� �迭
    [SerializeField] private GameObject selectImagePanel; // ������ �̹��� ���� �г�

    private int selectedImageIndex = 0; // ������ �̹��� �ε��� ��ȣ
   
    private void Awake()
    {
        targetImage = GameObject.Find("Profile Image Button").GetComponent<Image>(); // ������ �̹��� (��ư) ��ü ã�� ������Ʈ ������
        GameObject signupImageObj = GameObject.Find("Signup Profile Image Button");

        if(signupImageObj != null) signupTargetImage = signupImageObj.GetComponent<Image>(); // ȸ������ �̹��� ��ư�� ������ ������

        LoadProfileImage(); // ����� ������ �̹��� �ε�
    }

    public void OnClickConfirmButton() // �̹��� ���� �гο��� Ȯ�� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);

        for (int i = 0; i < imageOutLineToggles.Length; i++) // �̹��� ���� ��� �迭�� ���鼭 ���õ� �̹��� ã��
        {
            Sprite selectedImage = imageOutLineToggles[i].GetSelectedImage();
            if (selectedImage != null)
            {
                targetImage.sprite = selectedImage; // ���õ� ������ �̹��� ����

                if (signupTargetImage != null) // ȸ������ â�� �ִ� ��쿡�� ����
                    signupTargetImage.sprite = selectedImage;

                // ������ �̹��� �ε����� ����
                PlayerPrefs.SetInt("SelectedProfileImageIndex", i);
                PlayerPrefs.Save();

                Debug.Log("������ �̹��� �ε���: " + i);

                StartCoroutine(NetworkManage.Instance.UpdateProfileImage(i)); // ������ ������ �̹��� �ε��� ����
                break;
            }
        }

        if(selectImagePanel != null) 
        {
            Destroy(selectImagePanel);
        }
    }
        
    private void LoadProfileImage() // ����� ������ �̹��� �ε��ϴ� �޼���
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
    
    public void SetProfileImage(int index) // ������ �̹����� �����ϴ� �޼���
    {
        if (index >= 0 && index < imageOutLineToggles.Length)
        {
            targetImage.sprite = imageOutLineToggles[index].GetSelectedImage();
        }
    }

    public int GetSelectedImageIndex() // ������ �̹��� �ε����� ��ȯ�ϴ� �޼���
    {
        return selectedImageIndex;
    }

    public void OnClickProfileButton() // ������ �̹��� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
        MainController.Instance.OpenProfileImagePanel();
    }

    public void OnClickCancelButton() // �̹��� ���� �гο��� ��� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    {
        SoundManager.Instance.PlaySFX(SoundManager.Sfx.SFX_BUTTON);
        Destroy(selectImagePanel);
    }
}