using UnityEngine;
using TMPro;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump;

public class ShopRewardAdButton : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    private int coin = 300;
    private RewardedAd rewardedAd;

    void Start()
    {
        LoadRewardedAd();
    }

    public void LoadRewardedAd()
    {
        string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // 테스트용
        AdRequest request = new AdRequest();

        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("리워드 광고 로드 실패: " + error);
                return;
            }

            rewardedAd = ad;
            Debug.Log("리워드 광고 로드 성공!");

            // 광고 시청 보상 처리
            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                LoadRewardedAd(); // 광고 닫히면 다음 광고 미리 로드
            };
        });
    }

    public void ShowRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Show((Reward reward) =>
            {
                coin += 300;
                coinText.text = "코인: " + coin;
                Debug.Log(" 보상 지급 현재 코인: " + coin);
            });
        }
        else
        {
            Debug.Log(" 광고가 아직 로드되지 않았어요!");
        }
    }


}