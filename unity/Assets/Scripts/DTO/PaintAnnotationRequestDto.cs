using System;

[Serializable]
public class PaintAnnotationRequestDto : AnnotationRequestDto
{
    public string imageLayer;
    public string color;
}