using System;
using System.Collections;
using System.Collections.Generic;
using _02_Scripts.Eobak;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[Serializable]
public class ConsumeCoinRequest
{
    public int amount;
}

[Serializable]
public class ConsumeCoinResponse
{
    public string message;
}
    
        
[Serializable]
public class RewardAdResponse
{
    public string message;
}

[Serializable]
public class PurchaseCoinRequest
{
    public string purchaseAmount;
}

[Serializable]
public class PurchaseCoinResponse
{
    public string message;
}

[Serializable]
public class GetCoinBalanceResponse
{
    public int coins;
}

// 랭킹

[Serializable]
public class RankingUser
{
    public string nickname;
    public int rank;
    public int win;
    public int lose;
    public string rankAchievedTime;
    public int rankingPosition;
}

[Serializable]
public class GetRankingResponse
{
    public RankingUser[] ranking;
}

[Serializable]
public class AddRankingRequest
{
    public string nickname;
    public int rank;
    public int win;
    public int lose;
}

[Serializable]
public class AddRankingResponse
{
    public string message;
}


public class NetworkManager : Singleton<NetworkManager>
{
    #region 코인
    private IEnumerator ConsumeCoinCoroutine(int amount, Action<ConsumeCoinResponse> success, Action<string> failure)
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("sid")))
        {
            failure?.Invoke("로그인이 필요합니다.");
            yield break;
        }

        ConsumeCoinRequest requestData = new ConsumeCoinRequest { amount = amount };
        string jsonString = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);

        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/coin/consume", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Cookie", PlayerPrefs.GetString("sid"));

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"코인 차감 오류: {www.error}");
                failure?.Invoke(www.error);
            }
            else
            {
                if (www.responseCode == 200)
                {
                    ConsumeCoinResponse response = JsonUtility.FromJson<ConsumeCoinResponse>(www.downloadHandler.text);
                    success?.Invoke(response);
                }
                else if (www.responseCode == 400)
                {
                    failure?.Invoke(www.downloadHandler.text); // "코인이 부족합니다."
                }
                else if (www.responseCode == 401)
                {
                    failure?.Invoke("로그인이 필요합니다.");
                }
                else
                {
                    failure?.Invoke($"서버 오류: {www.responseCode}");
                }
            }
        }
    }

    public void ConsumeCoin(int amount, Action<ConsumeCoinResponse> success, Action<string> failure)
    {
        Instance.StartCoroutine(ConsumeCoinCoroutine(amount, success, failure));
    }

    private IEnumerator RewardAdCoroutine(Action<RewardAdResponse> success, Action<string> failure)
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("sid")))
        {
            failure?.Invoke("로그인이 필요합니다.");
            yield break;
        }

        using (UnityWebRequest www = UnityWebRequest.Post(Constants.ServerURL + "/coin/rewardad", ""))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Cookie", PlayerPrefs.GetString("sid"));

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"광고 시청 보상 오류: {www.error}");
                failure?.Invoke(www.error);
            }
            else
            {
                if (www.responseCode == 200)
                {
                    RewardAdResponse response = JsonUtility.FromJson<RewardAdResponse>(www.downloadHandler.text);
                    success?.Invoke(response);
                }
                else if (www.responseCode == 401)
                {
                    failure?.Invoke("로그인이 필요합니다.");
                }
                else
                {
                    failure?.Invoke($"서버 오류: {www.responseCode}");
                }
            }
        }
    }

    public void RewardAd(Action<RewardAdResponse> success, Action<string> failure)
    {
        Instance.StartCoroutine(RewardAdCoroutine(success, failure));
    }

    private IEnumerator PurchaseCoinCoroutine(string purchaseAmount, Action<PurchaseCoinResponse> success, Action<string> failure)
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("sid")))
        {
            failure?.Invoke("로그인이 필요합니다.");
            yield break;
        }

        PurchaseCoinRequest requestData = new PurchaseCoinRequest { purchaseAmount = purchaseAmount };
        string jsonString = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);

        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/coin/purchase", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Cookie", PlayerPrefs.GetString("sid"));

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"코인 구매 오류: {www.error}");
                failure?.Invoke(www.error);
            }
            else
            {
                if (www.responseCode == 200)
                {
                    PurchaseCoinResponse response = JsonUtility.FromJson<PurchaseCoinResponse>(www.downloadHandler.text);
                    success?.Invoke(response);
                }
                else if (www.responseCode == 400)
                {
                    failure?.Invoke(www.downloadHandler.text); // "유효하지 않은 구매 금액입니다." 등
                }
                else if (www.responseCode == 401)
                {
                    failure?.Invoke("로그인이 필요합니다.");
                }
                else
                {
                    failure?.Invoke($"서버 오류: {www.responseCode}");
                }
            }
        }
    }

    public void PurchaseCoin(string purchaseAmount, Action<PurchaseCoinResponse> success, Action<string> failure)
    {
        Instance.StartCoroutine(PurchaseCoinCoroutine(purchaseAmount, success, failure));
    }

    private IEnumerator GetCoinBalanceCoroutine(Action<int> success, Action<string> failure)
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("sid")))
        {
            failure?.Invoke("로그인이 필요합니다.");
            yield break;
        }

        using (UnityWebRequest www = UnityWebRequest.Get(Constants.ServerURL + "/coin/getbalance"))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Cookie", PlayerPrefs.GetString("sid"));

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"코인 잔액 조회 오류: {www.error}");
                failure?.Invoke(www.error);
            }
            else
            {
                if (www.responseCode == 200)
                {
                    GetCoinBalanceResponse response = JsonUtility.FromJson<GetCoinBalanceResponse>(www.downloadHandler.text);
                    success?.Invoke(response.coins);
                }
                else if (www.responseCode == 401)
                {
                    failure?.Invoke("로그인이 필요합니다.");
                }
                else
                {
                    failure?.Invoke($"서버 오류: {www.responseCode}");
                }
            }
        }
    }

    public void GetCoinBalance(Action<int> success, Action<string> failure)
    {
        Instance.StartCoroutine(GetCoinBalanceCoroutine(success, failure));
    }

    #endregion

    #region 랭킹

    private IEnumerator GetRankingCoroutine(Action<RankingUser[]> success, Action<string> failure)
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("sid")))
        {
            failure?.Invoke("로그인이 필요합니다.");
            yield break;
        }

        using (UnityWebRequest www = UnityWebRequest.Get(Constants.ServerURL + "/ranking/getranking"))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Cookie", PlayerPrefs.GetString("sid"));

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"랭킹 정보 조회 오류: {www.error}");
                failure?.Invoke(www.error);
            }
            else
            {
                if (www.responseCode == 200)
                {
                    GetRankingResponse response = JsonUtility.FromJson<GetRankingResponse>(www.downloadHandler.text);
                    success?.Invoke(response.ranking);
                }
                else if (www.responseCode == 401)
                {
                    failure?.Invoke("로그인이 필요합니다.");
                }
                else
                {
                    failure?.Invoke($"서버 오류: {www.responseCode}");
                }
            }
        }
    }

    public void GetRanking(Action<RankingUser[]> success, Action<string> failure)
    {
        Instance.StartCoroutine(GetRankingCoroutine(success, failure));
    }

    private IEnumerator AddRankingCoroutine(string nickname, int rank, int win, int lose, Action<AddRankingResponse> success, Action<string> failure)
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("sid")))
        {
            failure?.Invoke("로그인이 필요합니다.");
            yield break;
        }

        AddRankingRequest requestData = new AddRankingRequest { nickname = nickname, rank = rank, win = win, lose = lose };
        string jsonString = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);

        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/ranking/addranking", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Cookie", PlayerPrefs.GetString("sid"));

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"랭킹 추가 오류: {www.error}");
                failure?.Invoke(www.error);
            }
            else
            {
                if (www.responseCode == 201)
                {
                    AddRankingResponse response = JsonUtility.FromJson<AddRankingResponse>(www.downloadHandler.text);
                    success?.Invoke(response);
                }
                else if (www.responseCode == 400)
                {
                    failure?.Invoke(www.downloadHandler.text); // "유효하지 않은 급수입니다." 등
                }
                else if (www.responseCode == 401)
                {
                    failure?.Invoke("로그인이 필요합니다.");
                }
                else
                {
                    failure?.Invoke($"서버 오류: {www.responseCode}");
                }
            }
        }
    }

    public void AddRanking(string nickname, int rank, int win, int lose, Action<AddRankingResponse> success, Action<string> failure)
    {
        Instance.StartCoroutine(AddRankingCoroutine(nickname, rank, win, lose, success, failure));
    }

    #endregion

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
}