using UnityEngine;
using TMPro;

public class FakePurchaseItem : MonoBehaviour
{
    public int price;
    [SerializeField] private TextMeshProUGUI rewardAmountText;
    [SerializeField] private TextMeshProUGUI priceText; // 구매버튼 텍스트

    void Start()
    {
        // 가격에 맞는 보상 코인 수를 계산해서 텍스트로 표시
        rewardAmountText.text = GetRewardText(price);
        priceText.text = $"\u20a9{price:N0}";
    }

    public void OnClick()
    {
        // 결제창 열고 가격과 보상 전달
        FakePurchaseManager.Instance.OpenPurchasePanel(price, GetRewardAmount(price));
    }

    // 가격에 따라 코인 수 리턴
    int GetRewardAmount(int price)
    {
        switch (price)
        {
            case 3900: return 300;
            case 5900: return 600;
            case 8900: return 950;
            case 12900: return 1400;
            default: return 300;
        }
    }

    // 보상 텍스트 생성
    string GetRewardText(int price)
    {
        return $"코인 {GetRewardAmount(price)}개";
    }
}