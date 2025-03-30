using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 프리팹에서 이미지 선택창을 간단하게 Instantiate 하기 위한 스크립트
public class ButtonHandler : MonoBehaviour
{
    public void OnClickProfileButton()
    {
        if (MainController.Instance != null)
        {
            MainController.Instance.OpenProfileImagePanel();
        }
        else
        {
            Debug.LogError("MainController 인스턴스를 찾을 수 없습니다.");
        }
    }
}
