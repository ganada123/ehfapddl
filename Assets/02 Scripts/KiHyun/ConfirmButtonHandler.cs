using UnityEngine;
using UnityEngine.UI;

public class ConfirmButtonHandler : MonoBehaviour
{
    public Image targetImage;
    public ImageOutLineToggle[] imageOutLineToggles;  // ��� ImageOutLineToggle ����
    [SerializeField] private GameObject selectImagePanel;

    // ���õ� �̹��� Ȯ���� ������ �̹����� ����
    public void OnClickConfirmButton()
    {
        SoundManager.instance.playSFX(SoundManager.Sfx.SFX_BUTTON);

        foreach (var toggle in imageOutLineToggles)
        {
            Sprite selectedImage = toggle.GetSelectedImage();
            if (selectedImage != null)
            {
                Debug.Log(selectedImage.name + " �̹��� ����");
                targetImage.sprite = selectedImage;
                break;
            }
        }
        selectImagePanel.SetActive(false);
    }

}
