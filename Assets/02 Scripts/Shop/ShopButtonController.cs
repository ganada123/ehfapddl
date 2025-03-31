using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShopButtonController : MonoBehaviour
{
    public GameObject shopPanel;
// 상점 버튼 클릭 시
public void OnClickOpenShop()
    {
        shopPanel.SetActive(true); // 상점 패널을 활성화
    }

    // 상점 닫기 버튼 클릭 시
    public void OnClickCloseShop()
    {
        shopPanel.SetActive(false);  // 상점 패널을 비활성화
       
    }

    public void shopPanelEsc()
    {
        if (Input.GetKeyDown(KeyCode.Escape)&& shopPanel.activeSelf)
        {
            if (shopPanel.activeSelf)
                shopPanel.SetActive(false);
        }
    }
    private void Update()
    {
        shopPanelEsc();
    }
}
