using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class AnnotationApi
{
    private string _baseUrl = "http://localhost:9090/annotation";

    public IEnumerator GetPointAnnotationById(long id,
        System.Action<PointAnnotationResponseDto> onSuccess, System.Action<string> onError)
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{_baseUrl}/point/{id}"))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                PointAnnotationResponseDto response = JsonUtility.FromJson<PointAnnotationResponseDto>(www.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                onError?.Invoke(www.downloadHandler.text);
            }
        }
    }
    
    public IEnumerator AddPointAnnotation(PointAnnotationRequestDto dto,
        System.Action<AnnotationResponseDto> onSuccess, System.Action<string> onError)
    {
        string json = JsonUtility.ToJson(dto);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm($"{_baseUrl}/point", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AnnotationResponseDto response = JsonUtility.FromJson<AnnotationResponseDto>(www.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                onError?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public IEnumerator UpdatePointAnnotation(long id, PointAnnotationRequestDto dto,
        System.Action<AnnotationResponseDto> onSuccess, System.Action<string> onError)
    {
        string json = JsonUtility.ToJson(dto);
        string url = $"{_baseUrl}/point/{id}";

        UnityWebRequest www = UnityWebRequest.Put(url, json);
        www.method = "PUT";
        www.SetRequestHeader("Content-Type", "application/json");
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            AnnotationResponseDto response = JsonUtility.FromJson<AnnotationResponseDto>(www.downloadHandler.text);
            onSuccess?.Invoke(response);
        }
        else
        {
            onError?.Invoke(www.downloadHandler.text);
        }
    }

    public IEnumerator AddPaintAnnotation(PaintAnnotationRequestDto dto,
        System.Action<AnnotationResponseDto> onSuccess, System.Action<string> onError)
    {
        string json = JsonUtility.ToJson(dto);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm($"{_baseUrl}/paint", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AnnotationResponseDto response = JsonUtility.FromJson<AnnotationResponseDto>(www.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                onError?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public IEnumerator UpdatePaintAnnotation(long id, PaintAnnotationRequestDto dto,
        System.Action<AnnotationResponseDto> onSuccess, System.Action<string> onError)
    {
        string json = JsonUtility.ToJson(dto);
        string url = $"{_baseUrl}/paint/{id}";

        UnityWebRequest www = UnityWebRequest.Put(url, json);
        www.method = "PUT";
        www.SetRequestHeader("Content-Type", "application/json");
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            AnnotationResponseDto response = JsonUtility.FromJson<AnnotationResponseDto>(www.downloadHandler.text);
            onSuccess?.Invoke(response);
        }
        else
        {
            onError?.Invoke(www.downloadHandler.text);
        }
    }

    public IEnumerator DeleteAnnotation(long id, System.Action onSuccess, System.Action<string> onError)
    {
        using (UnityWebRequest www = UnityWebRequest.Delete($"{_baseUrl}/{id}"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success || www.responseCode == 204)
            {
                onSuccess?.Invoke();
            }
            else
            {
                onError?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public IEnumerator GetPointAnnotationsBySession(long sessionId,
        System.Action<PointAnnotationResponseList> onSuccess, System.Action<string> onError)
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{_baseUrl}/point/session/{sessionId}"))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                PointAnnotationResponseList response =
                    JsonUtility.FromJson<PointAnnotationResponseList>("{\"annotations\":" + www.downloadHandler.text + "}");
                onSuccess?.Invoke(response);
            }
            else
            {
                onError?.Invoke(www.downloadHandler.text);
            }
        }
    }

    public IEnumerator GetPaintAnnotationsBySession(long sessionId,
        System.Action<PaintAnnotationResponseList> onSuccess, System.Action<string> onError)
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{_baseUrl}/paint/session/{sessionId}"))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                PaintAnnotationResponseList response =
                    JsonUtility.FromJson<PaintAnnotationResponseList>("{\"annotations\":" + www.downloadHandler.text + "}");
                onSuccess?.Invoke(response);
            }
            else
            {
                onError?.Invoke(www.downloadHandler.text);
            }
        }
    }
}
