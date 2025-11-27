package com.example.inspectxr.dtos;

import jakarta.validation.constraints.NotNull;
import lombok.Data;

@Data
public abstract class AnnotationRequestDTO {

    @NotNull
    private Long createdById;
    @NotNull
    private Long modelId;
    @NotNull
    private Long sessionId;
    @NotNull
    private String data;
}
