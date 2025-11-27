using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class SessionApi
{
    private string _baseUrl = "http://localhost:9090/sessions";

    public IEnumerator CreateSession(SessionRequestDto request, Action<SessionResponseDto> onSuccess, Action<string> onError)
    {
        string json = JsonUtility.ToJson(request);
        using (UnityWebRequest www = new UnityWebRequest(_baseUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                SessionResponseDto session = JsonUtility.FromJson<SessionResponseDto>(www.downloadHandler.text);
                onSuccess?.Invoke(session);
            }
            else
            {
                onError?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public IEnumerator GetSession(long id, Action<SessionResponseDto> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(_baseUrl + "/" + id))
        {
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                SessionResponseDto session = JsonUtility.FromJson<SessionResponseDto>(www.downloadHandler.text);
                onSuccess?.Invoke(session);
            }
            else
            {
                onError?.Invoke(www.downloadHandler.text);
            }
        }
    }
    
    public IEnumerator GetSessionsByOwnerId(long ownerId, System.Action<SessionResponseDtoList> onSuccess,
        System.Action<string> onError)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(_baseUrl + "/owner/" + ownerId))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                SessionResponseDtoList sessions =
                    JsonUtility.FromJson<SessionResponseDtoList>("{\"sessions\":" + www.downloadHandler.text + "}");
                onSuccess?.Invoke(sessions);
            }
            else
            {
                onError?.Invoke(www.downloadHandler.text);
            }
        }
    }
}