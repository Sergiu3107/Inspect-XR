package com.example.inspectxr.mapper;


import com.example.inspectxr.dtos.UserRequestDTO;
import com.example.inspectxr.dtos.UserResponseDTO;
import com.example.inspectxr.entity.UserEntity;
import org.mapstruct.Mapper;

@Mapper(componentModel = "spring")
public interface UserMapper {
    UserResponseDTO toDTO(UserEntity userEntity);
    UserEntity toEntity(UserRequestDTO userRequestDTO);
}
