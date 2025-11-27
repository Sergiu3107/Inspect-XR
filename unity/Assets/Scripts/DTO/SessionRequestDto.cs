using System.Collections.Generic;

[System.Serializable]
public class SessionRequestDto
{
    public long ownerId;
    public long modelId;
    public List<long> participantIds;
}