using System.Collections;
using System.Collections.Generic;
using _02_Scripts.Eobak;
using UnityEngine;
using UnityEngine.Networking;

public class RankingResponse
{
    public List<RankData> ranks;
}

public class RankData
{
    public string nickname;
    public int rank;
    public int win;
    public int lose;
}

public class NetworkManager : MonoBehaviour
{
    public void GetRankingData()
    {
        StartCoroutine(GetRankingDataCoroutine());
    }

    IEnumerator GetRankingDataCoroutine()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(Constants.ServerURL + "/ranking"))
        {
            // 헤더에 인증 정보 추가 (세션 쿠키)
            string sid = PlayerPrefs.GetString("sid", "");
            if(!string.IsNullOrEmpty(sid))
            {
                webRequest.SetRequestHeader("Cookie", sid);
            }

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string json = webRequest.downloadHandler.text;
                RankingResponse response = JsonUtility.FromJson<RankingResponse>(json);
                ProcessRankingData(response.ranks);
            }
            else
            {
                Debug.LogError("랭킹 데이터 요청 실패: " + webRequest.error);
            }
        }
    }

    public void ProcessRankingData(List<RankData> ranks)
    {
        // 받아온 랭킹 데이터 처리
        foreach (var rank in ranks)
        {
            Debug.Log($"닉네임: {rank.nickname}, 랭크: {rank.rank}, 승리: {rank.win}, 패배: {rank.lose}");
            // UI에 랭킹 정보 표시 등 필요한 작업 수행
        }
    }
}
