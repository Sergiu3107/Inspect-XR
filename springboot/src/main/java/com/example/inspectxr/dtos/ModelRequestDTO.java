package com.example.inspectxr.dtos;

import jakarta.validation.constraints.NotNull;
import lombok.Data;


@Data
public class ModelRequestDTO {

    private String objectUrl;

    @NotNull(message = "Display name is required")
    private String displayName;

    @NotNull(message = "Owner ID is required")
    private Long ownerId;
}
