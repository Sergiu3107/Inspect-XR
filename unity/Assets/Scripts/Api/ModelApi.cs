using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ModelApi
{
    private string _baseUrl = "http://localhost:9090/model";

    public IEnumerator AddModel(ModelRequestDto request, string modelFilePath,
        System.Action<ModelResponseDto> onSuccess, System.Action<string> onError)
    {
        string json = JsonUtility.ToJson(request);

        byte[] fileData = System.IO.File.ReadAllBytes(modelFilePath);
        string fileName = System.IO.Path.GetFileName(modelFilePath);

        var formSections = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("metadata", json, "application/json"),
            new MultipartFormFileSection("file", fileData, fileName, "application/zip")
        };


        using (UnityWebRequest www = UnityWebRequest.Post(_baseUrl, formSections))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                ModelResponseDto model = JsonUtility.FromJson<ModelResponseDto>(www.downloadHandler.text);
                onSuccess?.Invoke(model);
            }
            else
            {
                onError?.Invoke(www.downloadHandler.text);
            }
        }

    }

    public IEnumerator DeleteModel(long modelId, System.Action onSuccess, System.Action<string> onError)
    {
        using (UnityWebRequest www = UnityWebRequest.Delete(_baseUrl + "/" + modelId))
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

    public IEnumerator GetModelsByOwnerId(long ownerId, System.Action<ModelResponseDtoList> onSuccess,
        System.Action<string> onError)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(_baseUrl + "/owner/" + ownerId))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                ModelResponseDtoList models =
                    JsonUtility.FromJson<ModelResponseDtoList>("{\"models\":" + www.downloadHandler.text + "}");
                onSuccess?.Invoke(models);
            }
            else
            {
                onError?.Invoke(www.downloadHandler.text);
            }
        }
    }
}