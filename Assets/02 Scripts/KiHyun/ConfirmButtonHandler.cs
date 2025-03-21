using UnityEngine;
using UnityEngine.UI;

public class ConfirmButtonHandler : MonoBehaviour
{
    public Image targetImage;
    public ImageOutLineToggle[] imageOutLineToggles;  // 모든 ImageOutLineToggle 참조
    [SerializeField] private GameObject selectImagePanel;

    // 선택된 이미지 확인후 프로필 이미지에 적용
    public void OnClickConfirmButton()
    {
        SoundManager.instance.playSFX(SoundManager.Sfx.SFX_BUTTON);

        foreach (var toggle in imageOutLineToggles)
        {
            Sprite selectedImage = toggle.GetSelectedImage();
            if (selectedImage != null)
            {
                Debug.Log(selectedImage.name + " 이미지 적용");
                targetImage.sprite = selectedImage;
                break;
            }
        }
        selectImagePanel.SetActive(false);
    }

}
