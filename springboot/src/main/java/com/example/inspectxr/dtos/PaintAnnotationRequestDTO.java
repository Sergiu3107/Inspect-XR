package com.example.inspectxr.dtos;

import jakarta.validation.constraints.NotNull;
import lombok.Data;
import lombok.EqualsAndHashCode;

@Data
@EqualsAndHashCode(callSuper = true)
public class PaintAnnotationRequestDTO extends AnnotationRequestDTO {

    @NotNull
    private String imageLayer;

    @NotNull
    private String color;
}
