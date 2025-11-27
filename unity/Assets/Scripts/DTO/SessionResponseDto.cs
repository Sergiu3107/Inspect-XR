using System.Collections.Generic;

[System.Serializable]
public class SessionResponseDto
{
    public long id;
    public UserResponseDto owner;
    public ModelResponseDto model;
    public string createdAt;
    public List<UserResponseDto> participants;
}