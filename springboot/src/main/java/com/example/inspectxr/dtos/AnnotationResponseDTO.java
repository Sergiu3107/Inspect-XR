package com.example.inspectxr.dtos;

import lombok.Data;

@Data
public abstract class AnnotationResponseDTO {
    private Long id;
    private Long createdById;
    private Long modelId;
    private Long sessionId;
    private String data;
}
