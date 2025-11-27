using UnityEngine;

public static class LocalUserStorage
{
    public static void SaveUser(UserResponseDto user)
    {
        string json = JsonUtility.ToJson(user);
        PlayerPrefs.SetString("user_data", json);
        PlayerPrefs.Save();
    }

    public static UserResponseDto LoadUser()
    {
        string json = PlayerPrefs.GetString("user_data", null);
        return string.IsNullOrEmpty(json) ? null : JsonUtility.FromJson<UserResponseDto>(json);
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey("user_data");
    }
}