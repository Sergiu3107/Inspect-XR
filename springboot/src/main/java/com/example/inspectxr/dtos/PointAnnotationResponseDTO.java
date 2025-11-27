package com.example.inspectxr.dtos;

import lombok.Data;
import lombok.EqualsAndHashCode;

@Data
@EqualsAndHashCode(callSuper = true)
public class PointAnnotationResponseDTO extends AnnotationResponseDTO {
    private float posX;
    private float posY;
    private float posZ;
}
