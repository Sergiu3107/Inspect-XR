using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class UserApi
{
    private string _baseUrl = "http://localhost:9090/user";

    public IEnumerator Login(UserRequestDto request, System.Action<UserResponseDto> onSuccess, System.Action<string> onError)
    {
        string json = JsonUtility.ToJson(request);
        using (UnityWebRequest www = new UnityWebRequest(_baseUrl + "/login", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                UserResponseDto user = JsonUtility.FromJson<UserResponseDto>(www.downloadHandler.text);
                onSuccess?.Invoke(user);
            }
            else
            {
                onError?.Invoke(www.downloadHandler.text);
            }
        }
    }
}
