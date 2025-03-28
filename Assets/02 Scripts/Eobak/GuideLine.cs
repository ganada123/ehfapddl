using UnityEngine;

namespace _02_Scripts.Eobak
{
    public class GuideLine : MonoBehaviour
    {
        void Start()
        {
            // 코인 차감 예시
            NetworkManager.Instance.ConsumeCoin(Constants.CoinConsumption,
                (response) => { Debug.Log($"코인 차감 성공: {response.message}"); },
                (error) => { Debug.LogError($"코인 차감 실패: {error}"); });

            // 코인 잔액 조회 예시
            NetworkManager.Instance.GetCoinBalance(
                (coins) => { Debug.Log($"현재 코인 잔액: {coins}"); },
                (error) => { Debug.LogError($"코인 잔액 조회 실패: {error}"); });

            // 랭킹 정보 조회 예시
            NetworkManager.Instance.GetRanking(
                (ranking) => {
                    if (ranking != null && ranking.Length > 0)
                    {
                        Debug.Log("랭킹 정보:");
                        foreach (var user in ranking)
                        {
                            Debug.Log($"- {user.rankingPosition}위: {user.nickname} (랭크: {user.rank}, 승: {user.win}, 패: {user.lose})");
                        }
                    }
                    else
                    {
                        Debug.Log("랭킹 정보가 없습니다.");
                    }
                },
                (error) => { Debug.LogError($"랭킹 정보 조회 실패: {error}"); });

            // 랭킹 추가 예시
            NetworkManager.Instance.AddRanking("새로추가할유저이름01", 3, 5, 1,
                (response) => { Debug.Log($"랭킹 추가 성공: {response.message}"); },
                (error) => { Debug.LogError($"랭킹 추가 실패: {error}"); });
        }
    }
}