package com.example.inspectxr.mapper;

import com.example.inspectxr.dtos.SessionResponseDTO;
import com.example.inspectxr.entity.SessionEntity;
import org.mapstruct.Mapper;

@Mapper(componentModel = "spring", uses = {UserMapper.class, ModelMapper.class})
public interface SessionMapper {
    SessionResponseDTO toDTO(SessionEntity sessionEntity);
}
