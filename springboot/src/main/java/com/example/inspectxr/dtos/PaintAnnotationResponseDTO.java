package com.example.inspectxr.dtos;

import lombok.Data;
import lombok.EqualsAndHashCode;

@Data
@EqualsAndHashCode(callSuper = true)
public class PaintAnnotationResponseDTO extends AnnotationResponseDTO {
    private String imageLayer;
    private String color;
}
