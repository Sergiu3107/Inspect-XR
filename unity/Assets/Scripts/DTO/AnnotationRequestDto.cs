using System;

[Serializable]
public class AnnotationRequestDto
{
    public long createdById;
    public long modelId;
    public long sessionId;
    public string data;
}