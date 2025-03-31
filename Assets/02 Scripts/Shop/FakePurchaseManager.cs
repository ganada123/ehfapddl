using System.Collections;
using TMPro;
using UnityEngine;

public class FakePurchaseManager : MonoBehaviour
{
    public static FakePurchaseManager Instance;

    [SerializeField] private GameObject purchasePanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject completePanel;

    private int currentPrice;
    private int currentReward;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OpenPurchasePanel(int price, int reward)
    {
        currentPrice = price;
        currentReward = reward;

        // 결제 텍스트 표시
        priceText.text = $"\\{currentPrice:N0} → <color=#FFD700>코인 {currentReward}개</color>";

        // UI 상태 변경
        purchasePanel.SetActive(true);
        shopPanel.SetActive(false);
        mainPanel.SetActive(false);
    }

    public void ConfirmPurchase()
    {
        // 인앱 결제 연동 시 여기에서 결제 요청
        // string productId = GetProductId(currentPrice);
        // IAPManager.Instance.BuyProduct(productId);

        // 지금은 페이크 보상 지급
        OnPurchaseSuccess(currentReward);
    }

    public void OnPurchaseSuccess(int rewardAmount)
    {
        Debug.Log($"결제 성공 → 코인 {rewardAmount} 지급");

        // GameManager.Instance.AddCoin(rewardAmount); ← 실제 지급 시
        ShowCompletePanel();
        ClosePurchasePanel();
    }

    public void OnPurchaseFailed(string productId, UnityEngine.Purchasing.PurchaseFailureReason reason)
    {
        Debug.Log($"결제 실패: {productId}, 이유: {reason}");
        ClosePurchasePanel();
    }

    private void ClosePurchasePanel()
    {
        purchasePanel.SetActive(false);
        shopPanel.SetActive(true);
        mainPanel.SetActive(true);
    }
    
    public void CancelPurchase()
    {
        // Debug.Log("결제 취소");
        ClosePurchasePanel();
    }
    void ShowCompletePanel()
    {
        completePanel.SetActive(true);
        Invoke(nameof(HideCompletePanel), 2f); // 2초 후 자동으로 꺼짐
    }

    void HideCompletePanel()
    {
        completePanel.SetActive(false);
    }

}
