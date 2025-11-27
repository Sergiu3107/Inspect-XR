package com.example.inspectxr.dtos;

import jakarta.validation.constraints.NotNull;
import lombok.Data;

import java.util.List;

@Data
public class SessionRequestDTO {

    @NotNull(message = "Owner ID is required")
    private Long ownerId;

    @NotNull(message = "Model ID is required")
    private Long modelId;

    private List<Long> participantIds;
}
