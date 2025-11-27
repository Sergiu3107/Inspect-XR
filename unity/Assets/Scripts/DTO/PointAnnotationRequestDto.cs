using System;

[Serializable]
public class PointAnnotationRequestDto : AnnotationRequestDto
{
    public float posX;
    public float posY;
    public float posZ;
}