package com.example.inspectxr.dtos;

import jakarta.validation.constraints.NotNull;
import lombok.Data;
import lombok.EqualsAndHashCode;

@Data
@EqualsAndHashCode(callSuper = true)
public class PointAnnotationRequestDTO extends AnnotationRequestDTO {

    @NotNull
    private float posX;

    @NotNull
    private float posY;

    @NotNull
    private float posZ;
}
