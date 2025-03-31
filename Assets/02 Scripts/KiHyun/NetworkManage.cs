using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class ProfileImageUpdateRequest
{
    public int profileImageIndex; // 프로필 이미지 배열의 번호
}

public class NetworkManage : Singleton<NetworkManage>
{
    public IEnumerator Signup(SignupData signupData, Action success, Action failure) // 회원가입 요청을 서버로 보내는 메서드
    {
        string jsonString = JsonUtility.ToJson(signupData);
        Debug.Log("Sending Signup Data: " + jsonString);

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);

        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/signup", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error: " + www.error);

                if (www.responseCode == 409)
                {
                    // 중복 사용자는 생성 불가 팝업 표시
                    Debug.Log("중복사용자");
                    MainController.Instance.OpenConfirmPanel("이미 존재하는 사용자입니다.", () =>
                    {
                        failure?.Invoke();
                    });

                }
            }
            else
            {
                var result = www.downloadHandler.text;
                Debug.Log("Result: " + result);

                // 회원가입 성공 팝업 표시
                MainController.Instance.OpenConfirmPanel("회원 가입이 완료 되었습니다.", () =>
                {
                    success?.Invoke();
                });
            }
        }
    }

    public IEnumerator Signin(SigninData signinData, Action success, Action<int> failure) // 로그인 요청을 서버로 보내는 메서드
    {
        string jsonString = JsonUtility.ToJson(signinData);

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);

        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/signin", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();
            {
                var cookie = www.GetResponseHeader("set-cookie");
                if (!string.IsNullOrEmpty(cookie))
                {
                    int lastIndex = cookie.LastIndexOf(";");
                    string sid = cookie.Substring(0, lastIndex);
                    PlayerPrefs.SetString("sid", sid);
                }

                var resultString = www.downloadHandler.text;
                Debug.Log("Received JSON: " + resultString); // 서버 응답 확인 로그
                var result = JsonUtility.FromJson<SigninResult>(resultString);
                Debug.Log($"로그인 결과값: {result.profileImageIndex} {result.nickname} {result.result}");

                if (result.result == 0) // email이 유효하지 않음
                {
                    Debug.Log("이메일이 유효하지 않음");
                    MainController.Instance.OpenConfirmPanel("유저네임이 유효하지 않습니다.", () =>
                    {
                        failure?.Invoke(0);
                    });
                }
                else if (result.result == 1) // 패스워드가 유효하지 않음
                {
                    Debug.Log("비밀번호가 유효하지 않음");
                    MainController.Instance.OpenConfirmPanel("패스워드가 유효하지 않습니다.", () =>
                    {
                        failure?.Invoke(1);
                    });
                }
                else if (result.result == 2) // 로그인 성공
                {
                    MainController.Instance.ApplyProfileImage(result.profileImageIndex);
                    NicknameChange.Instance.SetNickName(result.nickname);
                    PlayerPrefs.SetInt("SelectedProfileImageIndex", result.profileImageIndex);
                    PlayerPrefs.Save();
                    MainController.Instance.OpenConfirmPanel("로그인에 성공하였습니다.", () =>
                    {
                        success?.Invoke();
                    });
                }

            }
        }
    }

    public IEnumerator SetNickname(NicknameData nicknameData, Action<string> success, Action<string> failure)
    {
        string jsonString = JsonUtility.ToJson(nicknameData);

        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/users/setnickname", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonString));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Cookie", PlayerPrefs.GetString("sid")); // 로그인 세션 유지

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var result = www.downloadHandler.text;
                Debug.Log("닉네임 설정 성공: " + result);
                success?.Invoke(result);
            }
            else
            {
                Debug.LogError("닉네임 설정 실패: " + www.error);
                failure?.Invoke(www.error);
            }
        }
    }


    public IEnumerator UpdateProfileImage(int imageIndex) // 프로필 이미지를 업데이트하는 메서드
    {
        ProfileImageUpdateRequest request = new ProfileImageUpdateRequest();
        request.profileImageIndex = imageIndex;

        string jsonString = JsonUtility.ToJson(request);

        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/users/updateProfileImage", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonString));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Cookie", PlayerPrefs.GetString("sid")); // 로그인 세션 유지

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("프로필 이미지 업데이트 성공!");
            }
            else
            {
                Debug.LogError("프로필 이미지 업데이트 실패: " + www.error);
            }
        }
    }

    public IEnumerator Signout(Action success, Action<string> failure) // 로그아웃 요청을 서버로 보내는 메서드
    {
        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/users/signout", UnityWebRequest.kHttpVerbPOST))
        {
            www.SetRequestHeader("Cookie", PlayerPrefs.GetString("sid"));

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("로그아웃 오류: " + www.error);
                failure?.Invoke("로그아웃 실패");
            }
            else
            {
                // 로그아웃 성공, 세션 정보 삭제
                PlayerPrefs.DeleteKey("sid");
                PlayerPrefs.Save();
                Debug.Log("로그아웃 성공");
                success?.Invoke();
            }
        }
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }
}