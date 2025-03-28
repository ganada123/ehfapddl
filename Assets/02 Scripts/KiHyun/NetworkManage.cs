using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class ProfileImageUpdateRequest
{
    public int profileImageIndex; // ������ �̹��� �迭�� ��ȣ
}

public class NetworkManage : Singleton<NetworkManage>
{
    public IEnumerator Signup(SignupData signupData, Action success, Action failure) // ȸ������ ��û�� ������ ������ �޼���
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
                    // �ߺ� ����ڴ� ���� �Ұ� �˾� ǥ��
                    Debug.Log("�ߺ������");
                    MainController.Instance.OpenConfirmPanel("�̹� �����ϴ� ������Դϴ�.", () =>
                    {
                        failure?.Invoke();
                    });

                }
            }
            else
            {
                var result = www.downloadHandler.text;
                Debug.Log("Result: " + result);

                // ȸ������ ���� �˾� ǥ��
                MainController.Instance.OpenConfirmPanel("ȸ�� ������ �Ϸ� �Ǿ����ϴ�.", () =>
                {
                    success?.Invoke();
                });
            }
        }
    }

    public IEnumerator Signin(SigninData signinData, Action success, Action<int> failure) // �α��� ��û�� ������ ������ �޼���
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
                Debug.Log("Received JSON: " + resultString); // ���� ���� Ȯ�� �α�
                var result = JsonUtility.FromJson<SigninResult>(resultString);
                Debug.Log($"�α��� �����: {result.profileImageIndex} {result.nickname} {result.result}");

                if (result.result == 0) // email�� ��ȿ���� ����
                {
                    Debug.Log("�̸����� ��ȿ���� ����");
                    MainController.Instance.OpenConfirmPanel("���������� ��ȿ���� �ʽ��ϴ�.", () =>
                    {
                        failure?.Invoke(0);
                    });
                }
                else if (result.result == 1) // �н����尡 ��ȿ���� ����
                {
                    Debug.Log("��й�ȣ�� ��ȿ���� ����");
                    MainController.Instance.OpenConfirmPanel("�н����尡 ��ȿ���� �ʽ��ϴ�.", () =>
                    {
                        failure?.Invoke(1);
                    });
                }
                else if (result.result == 2) // �α��� ����
                {
                    MainController.Instance.ApplyProfileImage(result.profileImageIndex);
                    NicknameChange.Instance.SetNickName(result.nickname);
                    PlayerPrefs.SetInt("SelectedProfileImageIndex", result.profileImageIndex);
                    PlayerPrefs.Save();
                    MainController.Instance.OpenConfirmPanel("�α��ο� �����Ͽ����ϴ�.", () =>
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
            www.SetRequestHeader("Cookie", PlayerPrefs.GetString("sid")); // �α��� ���� ����

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var result = www.downloadHandler.text;
                Debug.Log("�г��� ���� ����: " + result);
                success?.Invoke(result);
            }
            else
            {
                Debug.LogError("�г��� ���� ����: " + www.error);
                failure?.Invoke(www.error);
            }
        }
    }


    public IEnumerator UpdateProfileImage(int imageIndex) // ������ �̹����� ������Ʈ�ϴ� �޼���
    {
        ProfileImageUpdateRequest request = new ProfileImageUpdateRequest();
        request.profileImageIndex = imageIndex;

        string jsonString = JsonUtility.ToJson(request);

        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/users/updateProfileImage", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonString));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Cookie", PlayerPrefs.GetString("sid")); // �α��� ���� ����

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("������ �̹��� ������Ʈ ����!");
            }
            else
            {
                Debug.LogError("������ �̹��� ������Ʈ ����: " + www.error);
            }
        }
    }

    public IEnumerator Signout(Action success, Action<string> failure) // �α׾ƿ� ��û�� ������ ������ �޼���
    {
        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/users/signout", UnityWebRequest.kHttpVerbPOST))
        {
            www.SetRequestHeader("Cookie", PlayerPrefs.GetString("sid"));

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("�α׾ƿ� ����: " + www.error);
                failure?.Invoke("�α׾ƿ� ����");
            }
            else
            {
                // �α׾ƿ� ����, ���� ���� ����
                PlayerPrefs.DeleteKey("sid");
                PlayerPrefs.Save();
                Debug.Log("�α׾ƿ� ����");
                success?.Invoke();
            }
        }
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }
}