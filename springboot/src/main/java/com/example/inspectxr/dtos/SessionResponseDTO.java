package com.example.inspectxr.dtos;

import lombok.Data;

import java.time.LocalDateTime;
import java.util.List;

@Data
public class SessionResponseDTO {

    private Long id;
    private UserResponseDTO owner;
    private ModelResponseDTO model;
    private LocalDateTime createdAt;
    private List<UserResponseDTO> participants;
}
