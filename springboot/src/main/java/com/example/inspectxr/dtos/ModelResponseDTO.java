package com.example.inspectxr.dtos;

import lombok.Data;

@Data
public class ModelResponseDTO {

    private Long id;
    private String objectUrl;
    private String displayName;
    private UserResponseDTO owner;
}
