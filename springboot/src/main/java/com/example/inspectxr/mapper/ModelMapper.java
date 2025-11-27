package com.example.inspectxr.mapper;

import com.example.inspectxr.dtos.ModelRequestDTO;
import com.example.inspectxr.dtos.ModelResponseDTO;
import com.example.inspectxr.entity.ModelEntity;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;

@Mapper(componentModel = "spring", uses = {UserMapper.class})
public interface ModelMapper {

    @Mapping(source = "ownerId", target = "owner.id")
    ModelEntity toEntity(ModelRequestDTO modelRequestDTO);

    ModelResponseDTO toDTO(ModelEntity modelEntity);
}
